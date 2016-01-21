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
        [ActiveEvent (Name = "__p5.crypto.create-pgp-keypair", Protection = EventProtection.LambdaClosed)]
        public static void p5_crypto_create_pgp_keypair (ApplicationContext context, ActiveEventArgs e)
        {
            // Creating our keypair
            IAsymmetricCipherKeyPairGenerator kpg = new RsaKeyPairGenerator ();
            kpg.Init (
                new RsaKeyGenerationParameters (
                    BigInteger.ValueOf (e.Args.GetExChildValue ("public-exponent", context, 65537L)), 
                    new SecureRandom (), 
                    e.Args.GetExChildValue("size", context, 2048), 
                    e.Args.GetExChildValue("certainty", context, 5)));
            AsymmetricCipherKeyPair kp = kpg.GenerateKeyPair();

            // Creating secret PGP key
            PgpSecretKey secretKey = new PgpSecretKey (
                PgpSignature.DefaultCertification,
                PublicKeyAlgorithmTag.RsaGeneral,
                kp.Public,
                kp.Private,
                e.Args.GetExChildValue("date", context, DateTime.Now.AddYears (3)),
                e.Args.GetExChildValue<string>("identity", context),
                SymmetricKeyAlgorithmTag.Cast5,
                e.Args.GetExChildValue<string> ("password", context).ToCharArray (),
                null,
                null,
                new SecureRandom ());

            // Creating GnuPG context to let MimeKit import keys into GnuPG database
            using (var ctx = new GnuPrivacyContext ()) {

                // Serializing public keyring bundle to ArmoredOutputStream, before letting MimeKit do its stuff!
                using (var publicStream = new MemoryStream ()) {

                    // Putting public key into KeyRingBundle, which is actually what MimeKit's Import takes as an argument
                    var publicRing = new PgpPublicKeyRing (secretKey.PublicKey.GetEncoded ());
                    var publicBundle = new PgpPublicKeyRingBundle (new [] { publicRing });

                    // We'll need an armored stream for MimeKit here, wrapping our MemoryStream
                    using (var armoredPublic = new ArmoredOutputStream (publicStream)) {
                        publicBundle.Encode (armoredPublic);
                        armoredPublic.Flush ();
                        publicStream.Flush ();
                    }
                    publicStream.Position = 0; // Important!
                    ctx.Import (publicStream);
                }

                // Serializing private keyring bundle to ArmoredOutputStream, before letting MimeKit do its stuff!
                using (var privateStream = new MemoryStream ()) {

                    // Putting secret key into KeyRingBundle, which is actually what MimeKit's ImportSecretKeys takes as an argument
                    var secretRing = new PgpSecretKeyRing (secretKey.GetEncoded ());
                    var secretBundle = new PgpSecretKeyRingBundle (new [] { secretRing });

                    // We'll need an armored stream for MimeKit here, wrapping our MemoryStream
                    using (var armoredSecret = new ArmoredOutputStream (privateStream)) {
                        secretBundle.Encode (armoredSecret);
                        armoredSecret.Flush ();
                        privateStream.Flush ();
                    }
                    privateStream.Position = 0; // Important!
                    ctx.ImportSecretKeys (privateStream);
                }
            }
        }

        /// <summary>
        ///     Creates and saves a private PGP keypair to GnuPG context
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.crypto.create-pgp-keypair", Protection = EventProtection.LambdaClosed)]
        public static void _p5_crypto_create_pgp_keypair (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving identity (normally an email address) and password
            string identity = e.Args.GetExChildValue<string> ("identity", context);
            string password = e.Args.GetExChildValue<string> ("password", context);
            DateTime expires = e.Args.GetExChildValue ("expires", context, DateTime.Now.AddYears (3));
            int strength = e.Args.GetExChildValue<int> ("strength", context, 2048);
            long publicExponent = e.Args.GetExChildValue ("public-exponent", context, 65537L);
            int certainty = e.Args.GetExChildValue ("certainty", context, 5);

            if (string.IsNullOrEmpty (password) || string.IsNullOrEmpty (identity))
                throw new LambdaException (
                    "Minimum [identity] and [password] needs to be supplied to create a PGP keypair",
                    e.Args,
                    context);

            // Generate public/private key rings
            PgpKeyRingGenerator generator = GetKeyRingGenerator (identity, password, expires, strength, publicExponent, certainty);
            PgpPublicKeyRing publicRing = generator.GeneratePublicKeyRing ();
            PgpSecretKeyRing secretRing = generator.GenerateSecretKeyRing ();

            // Creating GnuPG context to let MimeKit import keys into GnuPG database
            using (var ctx = new GnuPrivacyContext ()) {

                // Serializing public keyring bundle to ArmoredOutputStream, before letting MimeKit do its stuff!
                using (var publicStream = new MemoryStream ()) {

                    // Putting public key into KeyRingBundle, which is actually what MimeKit's Import takes as an argument
                    var publicBundle = new PgpPublicKeyRingBundle (new [] { publicRing });

                    // We'll need an armored stream for MimeKit here, wrapping our MemoryStream
                    using (var armoredPublic = new ArmoredOutputStream (publicStream)) {
                        publicBundle.Encode (armoredPublic);
                        armoredPublic.Flush ();
                        publicStream.Flush ();
                    }
                    publicStream.Position = 0; // Important!
                    ctx.Import (publicStream);
                }

                // Serializing private keyring bundle to ArmoredOutputStream, before letting MimeKit do its stuff!
                using (var privateStream = new MemoryStream ()) {

                    // Putting secret key into KeyRingBundle, which is actually what MimeKit's ImportSecretKeys takes as an argument
                    var secretBundle = new PgpSecretKeyRingBundle (new [] { secretRing });

                    // We'll need an armored stream for MimeKit here, wrapping our MemoryStream
                    using (var armoredSecret = new ArmoredOutputStream (privateStream)) {
                        secretBundle.Encode (armoredSecret);
                        armoredSecret.Flush ();
                        privateStream.Flush ();
                    }
                    privateStream.Position = 0; // Important!
                    ctx.ImportSecretKeys (privateStream);
                }
            }
        }

        /*
         * Creates a key ring generator and returns to caller
         */
        public static PgpKeyRingGenerator GetKeyRingGenerator(
            string identity, 
            string password, 
            DateTime expires, 
            int strength,
            long publicExponent,
            int certainty)
        {

            KeyRingParams keyRingParams = new KeyRingParams (strength, publicExponent, certainty);
            keyRingParams.Password = password;
            keyRingParams.Identity = identity;
            keyRingParams.PrivateKeyEncryptionAlgorithm = SymmetricKeyAlgorithmTag.Aes256;
            keyRingParams.SymmetricAlgorithms = new SymmetricKeyAlgorithmTag[] {
                SymmetricKeyAlgorithmTag.Aes256,
                SymmetricKeyAlgorithmTag.Aes192,
                SymmetricKeyAlgorithmTag.Aes128
            };

            keyRingParams.HashAlgorithms = new HashAlgorithmTag [] {
                HashAlgorithmTag.Sha256,
                HashAlgorithmTag.Sha1,
                HashAlgorithmTag.Sha384,
                HashAlgorithmTag.Sha512,
                HashAlgorithmTag.Sha224,
            };

            IAsymmetricCipherKeyPairGenerator generator = GeneratorUtilities.GetKeyPairGenerator ("RSA");
            generator.Init (keyRingParams.RsaParams);

            /* Create the master (signing-only) key. */
            PgpKeyPair masterKeyPair = new PgpKeyPair (
                PublicKeyAlgorithmTag.RsaGeneral,
                generator.GenerateKeyPair (),
                DateTime.UtcNow);

            PgpSignatureSubpacketGenerator masterSubpckGen = new PgpSignatureSubpacketGenerator ();
            masterSubpckGen.SetKeyFlags(false, PgpKeyFlags.CanSign | PgpKeyFlags.CanCertify);
            masterSubpckGen.SetPreferredSymmetricAlgorithms(false,
                (from a in keyRingParams.SymmetricAlgorithms
                    select (int) a).ToArray());
            masterSubpckGen.SetPreferredHashAlgorithms(false,
                (from a in keyRingParams.HashAlgorithms
                    select (int) a).ToArray());
            masterSubpckGen.SetKeyExpirationTime (false, (long)(expires - DateTime.Now).TotalSeconds);

            /* Create a signing and encryption key for daily use. */
            PgpKeyPair encKeyPair = new PgpKeyPair(
                PublicKeyAlgorithmTag.RsaGeneral,
                generator.GenerateKeyPair (),
                DateTime.UtcNow);

            PgpSignatureSubpacketGenerator encSubpckGen = new PgpSignatureSubpacketGenerator ();
            encSubpckGen.SetKeyFlags(false, 
                PgpKeyFlags.CanEncryptCommunications | 
                PgpKeyFlags.CanEncryptStorage | 
                PgpKeyFlags.CanSign);

            masterSubpckGen.SetPreferredSymmetricAlgorithms(false,
                (from a in keyRingParams.SymmetricAlgorithms
                    select (int) a).ToArray());
            masterSubpckGen.SetPreferredHashAlgorithms(false,
                (from a in keyRingParams.HashAlgorithms
                    select (int) a).ToArray());

            /* Create the key ring. */
            PgpKeyRingGenerator keyRingGen = new PgpKeyRingGenerator(
                PgpSignature.DefaultCertification,
                masterKeyPair,
                keyRingParams.Identity,
                keyRingParams.PrivateKeyEncryptionAlgorithm.Value,
                keyRingParams.GetPassword (),
                true,
                masterSubpckGen.Generate (),
                null,
                new SecureRandom ());

            /* Add encryption subkey. */
            keyRingGen.AddSubKey (encKeyPair, encSubpckGen.Generate(), null);

            return keyRingGen;

        }

        /*
         * Helper parameters class for creating PGP key
         */
        class KeyRingParams
        {
            public SymmetricKeyAlgorithmTag? PrivateKeyEncryptionAlgorithm { get; set; }
            public SymmetricKeyAlgorithmTag[] SymmetricAlgorithms { get; set; }
            public HashAlgorithmTag[] HashAlgorithms { get; set; }
            public RsaKeyGenerationParameters RsaParams { get; set; }
            public string Identity { get; set; }
            public string Password { get; set; }

            public char[] GetPassword() {
                return Password.ToCharArray ();
            }

            public KeyRingParams(int strength, long publicExponent, int certainty) {
                RsaParams = new RsaKeyGenerationParameters(BigInteger.ValueOf(publicExponent), new SecureRandom(), strength, certainty);
            }

        }
    }
}

