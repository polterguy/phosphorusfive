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
using System.Text;
using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.mime.helpers;
using p5.exp.exceptions;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto.Parameters;

namespace p5.mime
{
    /// <summary>
    ///     Class wrapping the creation of PGP private key pairs
    /// </summary>
    public static class CreatePgpKeyPair
    {
        /// <summary>
        ///     Creates and saves a private PGP keypair to GnuPG context.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.create-pgp-keypair")]
        public static void p5_crypto_create_pgp_keypair (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new ArgsRemover (e.Args, true)) {

                // Retrieving identity (normally an email address), in addition to password.
                string identity = e.Args.GetExChildValue<string> ("identity", context);
                string password = e.Args.GetExChildValue<string> ("password", context);
                if (string.IsNullOrEmpty (identity) || string.IsNullOrEmpty (password))
                    throw new LambdaException (
                        "Minimum [identity] and [password] needs to be supplied to create a PGP keypair",
                        e.Args,
                        context);

                // Retrieving other parameters to PGP keypair creation.
                DateTime expires = e.Args.GetExChildValue ("expires", context, DateTime.Now.AddYears (3));
                int strength = e.Args.GetExChildValue<int> ("strength", context, 4096);
                long publicExponent = e.Args.GetExChildValue ("public-exponent", context, 65537L);
                int certainty = e.Args.GetExChildValue ("certainty", context, 5);

                // Generate public/secret keys.
                PgpKeyRingGenerator generator = GetKeyRingGenerator (
                                                context,
                                                e.Args,
                                                identity,
                                                password,
                                                expires,
                                                strength,
                                                publicExponent,
                                                certainty);
                PgpPublicKeyRing publicRing = generator.GeneratePublicKeyRing ();
                PgpSecretKeyRing secretRing = generator.GenerateSecretKeyRing ();

                // Creating GnuPG context to let MimeKit import keys into GnuPG database
                using (var ctx = new GnuPrivacyContext (true)) {

                    // Saves public keyring
                    ctx.Import (publicRing);

                    // Saves private keyring
                    ctx.Import (secretRing);
                }

                // In case no [seed] was given, we remove the automatically generated seed ...
                e.Args ["seed"].UnTie ();
                e.Args.Add ("fingerprint", BitConverter.ToString (publicRing.GetPublicKey ().GetFingerprint ()).Replace ("-", ""));
                e.Args.Add ("key-id", ((int)publicRing.GetPublicKey ().KeyId).ToString ("X"));
            }
        }

        /*
         * Creates a key ring generator and returns to caller
         */
        public static PgpKeyRingGenerator GetKeyRingGenerator (
            ApplicationContext context,
            Node args,
            string identity,
            string password,
            DateTime expires,
            int strength,
            long publicExponent,
            int certainty)
        {
            // Creating a secure random generator to use when creating keypairs, seeding with all sorts of different unique values
            var sr = CreateNewSecureRandom (context, args);

            // Creating our generator
            IAsymmetricCipherKeyPairGenerator generator = GeneratorUtilities.GetKeyPairGenerator ("RSA");
            generator.Init (
                new RsaKeyGenerationParameters (
                    BigInteger.ValueOf (publicExponent),
                    sr,
                    strength,
                    certainty));

            // Creates the master key (signing-only key)
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
                    HashAlgorithmTag.Sha224,
                }.Select (ix => (int)ix).ToArray ());
            masterSubPacketGenerator.SetKeyExpirationTime (false, (long)(expires - DateTime.Now).TotalSeconds);

            // Creating a new secure random generator to use when creating keypairs, seeding with all sorts of different unique values
            sr = CreateNewSecureRandom (context, args);

            // Create signing and encryption key, for daily use
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

            // Creating keyring
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

            // Add encryption subkey
            keyRingGenerator.AddSubKey (encryptionKeyPair, encryptionSubPacketGenerator.Generate (), null);

            // Returning keyring to caller
            return keyRingGenerator;
        }

        /*
         * Creates and seeds a new SecureRandom to be used for keypair creation
         */
        static SecureRandom CreateNewSecureRandom (ApplicationContext context, Node args)
        {
            // Used to to hold seed for random number generator.
            List<byte> seed = new List<byte> ();

            // First we retrieve the seed provided by caller through the [seed] argument, defaulting to "foobar" if no user seed is provided.
            seed.AddRange (Encoding.UTF8.GetBytes (args.GetExChildValue<string> ("seed", context, "foobar") ?? "foobar"));

            // Then we retrieve a cryptographically secure random number of 128 bytes.
            seed.AddRange (context.RaiseEvent ("p5.crypto.create-random", new Node ("", null, new Node [] { new Node ("resolution", 128), new Node ("raw", true) })).Get<byte []> (context));

            // Then retrieving "seed generator" from BouncyCastle.
            seed.AddRange (new ThreadedSeedGenerator ().GenerateSeed (128, false));

            // Then we retrieve the server password salt.
            seed.AddRange (Encoding.UTF8.GetBytes (context.RaiseEvent (".p5.auth.get-server-salt").Get<string> (context)));

            // Then we retrieve the ticks of server.
            seed.AddRange (Encoding.UTF8.GetBytes (DateTime.Now.Ticks.ToString ()));

            // Then appending a randomly created Guid.
            seed.AddRange (Encoding.UTF8.GetBytes (Guid.NewGuid ().ToString ()));

            // Then we change the "user seed" to make sure consecutive invocations does not in any ways use the same original seed.
            args.FindOrInsert ("seed").Value = context.RaiseEvent ("p5.crypto.hash.create-sha512", new Node ("", seed)).Get<string> (context);

            // At this point, we are fairly certain that we have a pretty random and cryptographically secure seed.
            // Provided that SecureRandom from BouncyCastle is implemented correctly, we should now have a VERY, VERY, VERY unique,
            // and cryptographically secure Random Number seed!
            SecureRandom retVal = new SecureRandom ();
            retVal.SetSeed (seed.ToArray ());

            return retVal;
        }
    }
}

