/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
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
        ///     Creates and saves a private PGP keypair to GnuPG context
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.create-pgp-keypair")]
        public static void p5_crypto_create_pgp_keypair (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Retrieving identity (normally an email address) and password
                string identity = e.Args.GetExChildValue<string> ("identity", context);
                string password = e.Args.GetExChildValue<string> ("password", context);
                if (string.IsNullOrEmpty (identity) || string.IsNullOrEmpty (password))
                    throw new LambdaException (
                        "Minimum [identity] and [password] needs to be supplied to create a PGP keypair",
                        e.Args,
                        context);

                // Retrieving other parameters to PGP keypair creation
                DateTime expires = e.Args.GetExChildValue ("expires", context, DateTime.Now.AddYears (3));
                int strength = e.Args.GetExChildValue<int> ("strength", context, 4096);
                long publicExponent = e.Args.GetExChildValue ("public-exponent", context, 65537L);
                int certainty = e.Args.GetExChildValue ("certainty", context, 5);

                // Generate public/secret keys
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
                using (var ctx = new GnuPrivacyContext ()) {

                    // Saves public keyring
                    ctx.Import (publicRing);

                    // Saves private keyring
                    ctx.Import (secretRing);
                }

                // In case no [seed] was given, we remove the automatically generated seed ...
                e.Args ["seed"].UnTie ();
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
                new RsaKeyGenerationParameters(
                    BigInteger.ValueOf(publicExponent), 
                    sr, 
                    strength, 
                    certainty));

            // Creates the master key (signing-only key)
            PgpKeyPair masterKeyPair = new PgpKeyPair (
                PublicKeyAlgorithmTag.RsaGeneral,
                generator.GenerateKeyPair (),
                DateTime.UtcNow);

            PgpSignatureSubpacketGenerator masterSubPacketGenerator = new PgpSignatureSubpacketGenerator ();
            masterSubPacketGenerator.SetKeyFlags(false, PgpKeyFlags.CanSign | PgpKeyFlags.CanCertify);
            masterSubPacketGenerator.SetPreferredSymmetricAlgorithms(false,
                new SymmetricKeyAlgorithmTag[] {
                    SymmetricKeyAlgorithmTag.Aes256,
                    SymmetricKeyAlgorithmTag.Aes192,
                    SymmetricKeyAlgorithmTag.Aes128
                }.Select (ix => (int)ix).ToArray ());
            masterSubPacketGenerator.SetPreferredHashAlgorithms(false,
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
            PgpKeyPair encryptionKeyPair = new PgpKeyPair(
                PublicKeyAlgorithmTag.RsaGeneral,
                generator.GenerateKeyPair (),
                DateTime.UtcNow);

            PgpSignatureSubpacketGenerator encryptionSubPacketGenerator = new PgpSignatureSubpacketGenerator ();
            encryptionSubPacketGenerator.SetKeyFlags(false, 
                PgpKeyFlags.CanEncryptCommunications | 
                PgpKeyFlags.CanEncryptStorage | 
                PgpKeyFlags.CanSign);
            encryptionSubPacketGenerator.SetKeyExpirationTime (false, (long)(expires - DateTime.Now).TotalSeconds);

            // Creating keyring
            PgpKeyRingGenerator keyRingGenerator = new PgpKeyRingGenerator(
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
            keyRingGenerator.AddSubKey (encryptionKeyPair, encryptionSubPacketGenerator.Generate(), null);

            // Returning keyring to caller
            return keyRingGenerator;
        }

        /*
         * Creates and seeds a new SecureRandom to be used for keypair creation
         */
        private static SecureRandom CreateNewSecureRandom (ApplicationContext context, Node args)
        {
            // First we retrieve the seed provided by caller through the [seed] argument, defaulting to "foobar" if no user seed is provided
            string userSeed = args.GetExChildValue<string> ("seed", context, "foobar");

            // Then we change the given seed by hashing it, such that each pass through this method creates a different user provided seed
            args.FindOrCreate ("seed").Value = context.Raise ("sha512-hash", new Node ("", userSeed)).Get<string> (context);

            // Then we retrieve a cryptographically secure random number of 128 bytes
            var rndBytes = context.Raise (
                "create-cs-random", 
                new Node ("", null, new Node[] {
                    new Node ("resolution", 128),
                    new Node ("raw", true)})).Get<byte[]> (context);

            // Then retrieving "seed generator" from BouncyCastle
            var bcSeed = new ThreadedSeedGenerator ().GenerateSeed (128, false);

            // Then we retrieve the server password salt
            string serverPasswordSalt = context.Raise (".p5.security.get-server-salt").Get<string> (context);

            // Then we retrieve the ticks of server
            string serverSeed = DateTime.Now.Ticks.ToString ();

            // Then we append the Hyperlambda for the entire code tree
            // Notice, this will even include the GnuPG password in our seed, in sha2 hashed form!
            // In addition, every time the Hyperlambda active Event calling this method changes, the seed will change
            var code = Utilities.Convert<string> (context, args.Root);
            serverSeed += context.Raise ("sha256-hash", new Node ("", code)).Get<string> (context);;

            // Then adding current thread ID
            serverSeed += System.Threading.Thread.CurrentThread.ManagedThreadId.ToString ();

            // Then appending a randomly created Guid
            serverSeed += Guid.NewGuid ().ToString ();

            // Then we hash the user seed, multiple times, depending upon the length of the supplied user seed
            // This is done this way, to avoid reducing the resolution of the user-provided seed, such that the longer seed the user
            // provides, the better the strength of the key becomes, and the more difficult a brute force of a user seed guess becomes
            // Basically, we create a new hash, for each 100 characters in user provided seed. This makes a brute force significantly more
            // difficult, since the resolution of the user-provided seed is kept, while also making a brute force more expensive, due
            // to multiple hashes having to be done
            List<byte> userSeedByteList = new List<byte>();
            for (int idx = 0; idx < userSeed.Length; idx += 100) {
                var subStr = userSeed.Substring (idx, Math.Min (100, userSeed.Length - idx));
                byte[] buffer = context.Raise ("sha512-hash", new Node ("", subStr, new Node[] {new Node ("raw", true)})).Get<byte[]> (context);
                userSeedByteList.AddRange (buffer);
            }
            byte[] userSeedBytes = userSeedByteList.ToArray ();
            args ["seed"].Value = userSeedBytes;

            // Then we hash the server seed and the user seed with sha512, to create maximum size, and spread bytes evenly around [0-255] value range
            byte[] serverSeedBytes = context.Raise ("sha512-hash", new Node ("", serverSeed, new Node[] {new Node ("raw", true)})).Get<byte[]> (context);
            byte[] serverPasswordSaltBytes = context.Raise ("sha512-hash", new Node ("", serverPasswordSalt, new Node[] {new Node ("raw", true)})).Get<byte[]> (context);

            // Then we "braid" all the different parts together, to make sure no single parts of our seed becomes predictable due to weaknesses in one or more of
            // our seed generators. Meaning, if at least ONE of our "seed generators" are well functioning, then the entire result will be difficult to predict
            List<byte> seedBytesList = new List<byte>();
            for (int idx = 0; idx < Math.Max (serverSeedBytes.Length, Math.Max (userSeedBytes.Length, Math.Max (rndBytes.Length, Math.Max (bcSeed.Length, serverPasswordSalt.Length)))); idx++) {
                seedBytesList.Add (userSeedBytes [idx % userSeedBytes.Length]);
                seedBytesList.Add (serverSeedBytes [idx % serverSeedBytes.Length]);
                seedBytesList.Add (rndBytes [idx % rndBytes.Length]);
                seedBytesList.Add (bcSeed [idx % bcSeed.Length]);
                seedBytesList.Add (serverPasswordSaltBytes [idx % serverPasswordSaltBytes.Length]);
            }

            // At this point, we are fairly certain that we have a pretty random and cryptographically secure seed
            // Provided that SecureRandom from BouncyCastle is implemented correctly, we should now have a VERY, VERY, VERY unique,
            // and cryptographically secure Random Number seed!!
            // And since the seed is not "setting the seed", but rather "stirring up with additional entropy", this logic should
            // with extremely high certainty make sure we now have a very, very, very random seed for our Random number generator!
            // In addition, there are multiple hash invocations running, either directly or indirectly, meaning it becomes very
            // expensive to do a brute force attack on random number generator, leaving us with something that is "close to guaranteed"
            // being a good random number generator, assuming SecureRandom does its job!
            // In addition, our seed is at this point 640 bytes long, which translates into 5120 bits, meaning in no ways we have unintentionally
            // reduced the resolution of SecureRandom by applying "low resolution seeds" ...
            // In addition, our seed should be evenly distributed in the [0,255] range, and also no single parts of our seed should be predictable,
            // unless every single method above fails, due to seed being "braided together".
            SecureRandom retVal = new SecureRandom ();
            retVal.SetSeed (seedBytesList.ToArray ());

            return retVal;
        }
    }
}

