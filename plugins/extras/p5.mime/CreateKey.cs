/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.mime.helpers;
using helpers = p5.mime.helpers;
using p5.exp.exceptions;
using MimeKit;
using MimeKit.Cryptography;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;

namespace p5.mime
{
    /// <summary>
    ///     Class wrapping the creation of PGP private key pairs
    /// </summary>
    public static class CreateKey
    {
        /// <summary>
        ///     Creates and saves a private PGP keypair to GnuPG context
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.create-pgp-keypair", Protection = EventProtection.LambdaClosed)]
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

                // Generate public/secret keyrings
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
                    SavePublicKeyRing (ctx, publicRing);

                    // Saves private keyring
                    SaveSecretKeyRing (ctx, secretRing);
                }
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
         * Saves public keyring
         */
        private static void SavePublicKeyRing (GnuPrivacyContext ctx, PgpPublicKeyRing publicRing)
        {
            // Serializing public keyring bundle to ArmoredOutputStream, before letting MimeKit do its stuff!
            using (var memStream = new MemoryStream ()) {

                // Putting public key into KeyRingBundle, which is actually what MimeKit's Import takes as an argument
                var publicBundle = new PgpPublicKeyRingBundle (new [] { publicRing });

                // We'll need an armored stream for MimeKit here, wrapping our MemoryStream
                using (var armoredStream = new ArmoredOutputStream (memStream)) {
                    publicBundle.Encode (armoredStream);
                    armoredStream.Flush ();
                    memStream.Flush ();
                }
                memStream.Position = 0; // Important!
                ctx.Import (memStream);
            }
        }

        /*
         * Saves private keyring
         */
        private static void SaveSecretKeyRing (GnuPrivacyContext ctx, PgpSecretKeyRing secretRing)
        {
            // Serializing private keyring bundle to ArmoredOutputStream, before letting MimeKit do its stuff!
            using (var memStream = new MemoryStream ()) {

                // Putting secret key into KeyRingBundle, which is actually what MimeKit's ImportSecretKeys takes as an argument
                var secretBundle = new PgpSecretKeyRingBundle (new [] { secretRing });

                // We'll need an armored stream for MimeKit here, wrapping our MemoryStream
                using (var armoredStream = new ArmoredOutputStream (memStream)) {
                    secretBundle.Encode (armoredStream);
                    armoredStream.Flush ();
                    memStream.Flush ();
                }
                memStream.Position = 0; // Important!
                ctx.ImportSecretKeys (memStream);
            }
        }

        /*
         * Creates and seeds a new SecureRandom to be used for keypair creation
         */
        private static SecureRandom CreateNewSecureRandom (ApplicationContext context, Node args)
        {
            // First we use the seed provided by caller through the [seed] argument
            // If no [seed] is provided, we default to a Cryptographically Secure random number
            // [seed] is intended to be physically typed in by user, before keypair is created!
            string seed = args.GetExChildValue<string> ("seed", context, context.RaiseNative (
                "create-cs-random", 
                new Node ("", null)).Get<string> (context));

            // Then we change the given seed with the hashed seed, in case we get another run through this method,
            // such that the seed changes from each pass
            args.FindOrCreate ("seed").Value = context.RaiseNative ("sha256-hash", new Node ("", seed)).Get<string> (context);

            // Then we append the server salt to the seed
            seed += context.RaiseNative ("p5.security.get-password-salt").Get<string> (context);

            // Then we append the ticks of server
            seed += DateTime.Now.Ticks.ToString ();

            // Then we append a cryptographically secure random number of 4096 bytes, encoded as base 64
            seed += context.RaiseNative (
                "create-cs-random", 
                new Node ("", null, new Node[] {
                    new Node ("resolution", 4096)})).Get<string> (context);

            // Then we append the Hyperlisp for the entire code tree
            var code = Utilities.Convert<string> (context, args.Root);
            seed += code;

            // Then appending "seed generator" from BouncyCastle
            seed += Utilities.Convert<string> (context, new ThreadedSeedGenerator ().GenerateSeed (64, false));

            // Then adding current thread ID
            seed += System.Threading.Thread.CurrentThread.ManagedThreadId;

            // Then appending a Guid
            seed += Guid.NewGuid ().ToString ();

            // Then adding random seeds generated from "around our application" (Global.asax adds up session, cookies, browser, IP, etc)
            // p5.lambda adds up [vocabulary], etc, etc, etc
            seed += context.RaiseNative ("p5.security.get-pseudo-random-seed").Get<string> (context);

            // At this point, I am fairly certain that we have a pretty random and cryptographically secure seed
            // Provided that SecureRandom from BouncyCastle is implemented correctly, we should now have a VERY, VERY, VERY unique,
            // and cryptographically secure Random Number Generator!!
            // And since the seed is not "setting the seed", but rather "stirring up with additional entropy", this logic should
            // with extremely high certainty make sure we now have a very, very, very random seed for our Random number generator!
            // In addition, there are multiple hash invocations being runned, either directly or indirectly, meaning it becomes very
            // expensive to do a brute force attack on random number generator, leaving us with something that is "close to guaranteed"
            // being a good random number generator, assuming SecureRandom does its job!
            // Meaning, guessing the random number generators output, is literally IMPOSSIBLE!
            byte[] rawSeed = Utilities.Convert<byte[]>(context, seed);
            SecureRandom retVal = new SecureRandom ();

            // Applying seed, and returning SecureRandom to caller!
            retVal.SetSeed (rawSeed);

            return retVal;
        }
    }
}

