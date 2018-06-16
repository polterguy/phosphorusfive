/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.Linq;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using MimeKit.Cryptography;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto.Parameters;

namespace p5.crypto
{
    /// <summary>
    ///     Class wrapping the creation of PGP key pairs.
    /// </summary>
    public static class CreatePgpKeyPair
    {
        /// <summary>
        ///     Creates and saves a private PGP keypair to GnuPG context.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.pgp-keys.create")]
        public static void p5_crypto_pgp_keys_create (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves.
            using (new ArgsRemover (e.Args, true)) {

                // Retrieving identity (normally a name + email address), in addition to password.
                string identity = e.Args.GetExChildValue<string> ("identity", context);
                string password = e.Args.GetExChildValue<string> ("password", context);
                if (string.IsNullOrEmpty (identity) || string.IsNullOrEmpty (password))
                    throw new LambdaException (
                        "Minimum [identity] and [password] needs to be supplied to create a PGP keypair",
                        e.Args,
                        context);

                // Retrieving other parameters to PGP keypair creation, giving them sane defaults if not supplied.
                DateTime expires = e.Args.GetExChildValue ("expires", context, DateTime.Now.AddYears (3));
                int strength = e.Args.GetExChildValue<int> ("strength", context, 4096);
                long publicExponent = e.Args.GetExChildValue ("public-exponent", context, 65537L);
                int certainty = e.Args.GetExChildValue ("certainty", context, 5);

                // Creating our key generator.
                PgpKeyRingGenerator generator = GetKeyRingGenerator (
                                                context,
                                                identity,
                                                password,
                                                expires,
                                                strength,
                                                publicExponent,
                                                certainty);

                // Generating public keyrign.
                PgpPublicKeyRing publicRing = generator.GeneratePublicKeyRing ();

                // Generating secret keyring. "Secret key" is BC's name for private key.
                PgpSecretKeyRing secretRing = generator.GenerateSecretKeyRing ();

                /* 
                 * Retrieving GnuPG context to let MimeKit import keys into GnuPG database.
                 * Making sure we retrieve it in "write mode".
                 */
                using (var ctx = context.RaiseEvent (".p5.crypto.pgp-keys.context.create", new Node ("", true)).Get<OpenPgpContext> (context)) {

                    // Saves public keyring.
                    ctx.Import (publicRing);

                    // Saves secret keyring (private key, BC uses different naming convention).
                    ctx.Import (secretRing);
                }

                // Returning fingerprint and key-id to caller.
                e.Args.Add ("fingerprint", BitConverter.ToString (publicRing.GetPublicKey ().GetFingerprint ()).Replace ("-", ""));
                e.Args.Add ("key-id", ((int)publicRing.GetPublicKey ().KeyId).ToString ("X"));
            }
        }

        /*
         * Creates a key ring generator and returns to caller.
         */
        public static PgpKeyRingGenerator GetKeyRingGenerator (
            ApplicationContext context,
            string identity,
            string password,
            DateTime expires,
            int strength,
            long publicExponent,
            int certainty)
        {
            /*
             * Retrieving our SecureRandom instance to be able to create cryptographically secure random bytes
             * necessary during creation of PGP keypair.
             */
            var sr = GetSecureRandom (context);

            // Creating our generator.
            IAsymmetricCipherKeyPairGenerator generator = GeneratorUtilities.GetKeyPairGenerator ("RSA");
            generator.Init (
                new RsaKeyGenerationParameters (
                    BigInteger.ValueOf (publicExponent),
                    sr,
                    strength,
                    certainty));

            // Creates the master key (signing-only key).
            PgpKeyPair masterKeyPair = new PgpKeyPair (
                PublicKeyAlgorithmTag.RsaGeneral,
                generator.GenerateKeyPair (),
                DateTime.UtcNow);

            PgpSignatureSubpacketGenerator masterSubPacketGenerator = new PgpSignatureSubpacketGenerator ();
            masterSubPacketGenerator.SetKeyFlags (false, PgpKeyFlags.CanSign | PgpKeyFlags.CanCertify);
            masterSubPacketGenerator.SetPreferredSymmetricAlgorithms (false,
                new SymmetricKeyAlgorithmTag [] {
                    SymmetricKeyAlgorithmTag.Aes256,
                    SymmetricKeyAlgorithmTag.Aes192,
                    SymmetricKeyAlgorithmTag.Aes128
                }.Select (ix => (int)ix).ToArray ());
            masterSubPacketGenerator.SetPreferredHashAlgorithms (false,
                new HashAlgorithmTag [] {
                    HashAlgorithmTag.Sha256,
                    HashAlgorithmTag.Sha1,
                    HashAlgorithmTag.Sha384,
                    HashAlgorithmTag.Sha512,
                    HashAlgorithmTag.Sha224
                }.Select (ix => (int)ix).ToArray ());
            masterSubPacketGenerator.SetKeyExpirationTime (false, (long)(expires - DateTime.Now).TotalSeconds);

            // Create signing and encryption key, for daily use.
            PgpKeyPair encryptionKeyPair = new PgpKeyPair (
                PublicKeyAlgorithmTag.RsaGeneral,
                generator.GenerateKeyPair (),
                DateTime.UtcNow);

            PgpSignatureSubpacketGenerator encryptionSubPacketGenerator = new PgpSignatureSubpacketGenerator ();
            encryptionSubPacketGenerator.SetKeyFlags (false,
                PgpKeyFlags.CanEncryptCommunications |
                PgpKeyFlags.CanEncryptStorage |
                PgpKeyFlags.CanSign);
            encryptionSubPacketGenerator.SetKeyExpirationTime (false, (long)(expires - DateTime.Now).TotalSeconds);

            // Creating keyring.
            PgpKeyRingGenerator keyRingGenerator = new PgpKeyRingGenerator (
                PgpSignature.DefaultCertification,
                masterKeyPair,
                identity,
                SymmetricKeyAlgorithmTag.Aes256,
                password.ToCharArray (),
                true,
                masterSubPacketGenerator.Generate (),
                null,
                sr);

            // Adding encryption subkey.
            keyRingGenerator.AddSubKey (encryptionKeyPair, encryptionSubPacketGenerator.Generate (), null);

            // Returning keyring generator to caller.
            return keyRingGenerator;
        }

        /*
         * Retrieves the SecureRandom instance to be used for keypair creation.
         */
        static SecureRandom GetSecureRandom (ApplicationContext context)
        {
            return context.RaiseEvent (".p5.crypto.rng.secure-random.get").Get<SecureRandom> (context);
        }
    }
}

