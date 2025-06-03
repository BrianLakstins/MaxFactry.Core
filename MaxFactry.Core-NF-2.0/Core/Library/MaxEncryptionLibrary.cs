// <copyright file="MaxEncryptionLibrary.cs" company="Lakstins Family, LLC">
// Copyright (c) Brian A. Lakstins (http://www.lakstins.com/brian/)
// </copyright>

#region License
// <license>
// This software is provided 'as-is', without any express or implied warranty. In no 
// event will the author be held liable for any damages arising from the use of this 
// software.
//  
// Permission is granted to anyone to use this software for any purpose, including 
// commercial applications, and to alter it and redistribute it freely, subject to the 
// following restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not claim that 
// you wrote the original software. If you use this software in a product, an 
// acknowledgment (see the following) in the product documentation is required.
// 
// Portions Copyright (c) Brian A. Lakstins (http://www.lakstins.com/brian/)
// 
// 2. Altered source versions must be plainly marked as such, and must not be 
// misrepresented as being the original software.
// 
// 3. This notice may not be removed or altered from any source distribution.
// </license>
#endregion

#region Change Log
// <changelog>
// <change date="1/22/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="5/21/2014" author="Brian A. Lakstins" description="Update getting provider to allow a object provider to be universal.">
// <change date="8/24/2014" author="Brian A. Lakstins" description="Update to inherit from MaxBaseByType.">
// <change date="7/23/2016" author="Brian A. Lakstins" description="Add method to override defaults.">
// <change date="7/26/2016" author="Brian A. Lakstins" description="Add methods for asymmetric Public Key encryption and signing.">
// <change date="2/21/2018" author="Brian A. Lakstins" description="Add methods for random numbers.">
// <change date="7/20/2023" author="Brian A. Lakstins" description="Add constants for configuration names.">
// <change date="6/3/2025" author="Brian A. Lakstins" description="Add default provider type">
// </changelog>
#endregion

namespace MaxFactry.Core
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Library to provide encryption functionality
    /// </summary>
    public class MaxEncryptionLibrary : MaxByMethodFactory
    {
        /// <summary>
        /// Internal storage of single object
        /// </summary>
        private static MaxEncryptionLibrary _oInstance = null;

        /// <summary>
        /// Lock object for multi-threaded access.
        /// </summary>
        private static object _oLock = new object();

        /// <summary>
        /// Constant to reference MD5
        /// </summary>
        public const string MD5Hash = "MD5";

        /// <summary>
        /// Constant to reference Sha256
        /// </summary>
        public const string SHA256Hash = "SHA256";

        /// <summary>
        /// Constant to use for pass phrase name in configuration
        /// </summary>
        public const string PassphraseConfigName = "MaxEncryptionDefaultPassphrase";

        /// <summary>
        /// Constant to use for entropy name in configuration
        /// </summary>
        public const string EntropyConfigName = "MaxEncryptionDefaultEntropy";

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static MaxEncryptionLibrary Instance
        {
            get
            {
                if (null == _oInstance)
                {
                    lock (_oLock)
                    {
                        if (null == _oInstance)
                        {
                            _oInstance = new MaxEncryptionLibrary();
                            _oInstance.DefaultProviderType = typeof(MaxFactry.Core.Provider.MaxEncryptionLibraryDefaultProvider);
                        }
                    }
                }

                return _oInstance;
            }
        }

        /// <summary>
        /// Encrypts the text using DPAPI (Key provided by local system)
        /// Used to encrypt pass phrases for general encryption routines.
        /// </summary>
        /// <param name="loType">Type of object to protect, or type of provider to use to protect</param>
        /// <param name="lsText">The text to encrypt</param>
        /// <returns>encrypted text</returns>
        public static string Protect(Type loType, string lsText)
        {
            IMaxProvider loProvider = Instance.GetProvider(loType);
            if (loProvider is IMaxEncryptionLibraryProvider)
            {
                return ((IMaxEncryptionLibraryProvider)loProvider).Protect(lsText);
            }

            throw new MaxException("Provider for MaxEncryptionLibrary needs to implement IMaxEncryptionLibraryProvider");
        }

        /// <summary>
        /// Decrypts text using DPAPI (Key provided by local system)
        /// Used to decrypt pass phrases for general encryption routines.
        /// </summary>
        /// <param name="loType">Type of object to protect, or type of provider to use to protect</param>
        /// <param name="lsTextProtected">The encrypted version of text</param>
        /// <returns>The unencrypted version of text</returns>
        public static string Unprotect(Type loType, string lsTextProtected)
        {
            IMaxProvider loProvider = Instance.GetProvider(loType);
            if (loProvider is IMaxEncryptionLibraryProvider)
            {
                return ((IMaxEncryptionLibraryProvider)loProvider).Unprotect(lsTextProtected);
            }

            throw new MaxException("Provider for MaxEncryptionLibrary needs to implement IMaxEncryptionLibraryProvider");
        }

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="loType">Type of object to protect, or type of provider to use to protect</param>
        /// <param name="laValue">byte array to be encrypted</param>
        /// <returns>encrypted version of the byte array</returns>
        public static byte[] Encrypt(Type loType, byte[] laValue)
        {
            IMaxProvider loProvider = Instance.GetProvider(loType);
            if (loProvider is IMaxEncryptionLibraryProvider)
            {
                return ((IMaxEncryptionLibraryProvider)loProvider).Encrypt(laValue);
            }

            throw new MaxException("Provider for MaxEncryptionLibrary needs to implement IMaxEncryptionLibraryProvider");
        }

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="loType">Type of object to protect, or type of provider to use to protect</param>
        /// <param name="lsValue">string to be encrypted</param>
        /// <returns>encrypted version of the string</returns>
        public static string Encrypt(Type loType, string lsValue)
        {
            IMaxProvider loProvider = Instance.GetProvider(loType);
            if (loProvider is IMaxEncryptionLibraryProvider)
            {
                return ((IMaxEncryptionLibraryProvider)loProvider).Encrypt(lsValue);
            }

            throw new MaxException("Provider for MaxEncryptionLibrary needs to implement IMaxEncryptionLibraryProvider");
        }

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="loType">Type of object to protect, or type of provider to use to protect</param>
        /// <param name="laValue">byte array to be encrypted</param>
        /// <param name="lsPassphrase">Pass phrase used to encrypt the data</param>
        /// <returns>encrypted version of the byte array</returns>
        public static byte[] Encrypt(Type loType, byte[] laValue, string lsPassphrase)
        {
            IMaxProvider loProvider = Instance.GetProvider(loType);
            if (loProvider is IMaxEncryptionLibraryProvider)
            {
                return ((IMaxEncryptionLibraryProvider)loProvider).Encrypt(laValue, lsPassphrase);
            }

            throw new MaxException("Provider for MaxEncryptionLibrary needs to implement IMaxEncryptionLibraryProvider");
        }

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="loType">Type of object to protect, or type of provider to use to protect</param>
        /// <param name="lsValue">string to be encrypted</param>
        /// <param name="lsPassphrase">Pass phrase used to encrypt the data</param>
        /// <returns>encrypted version of the string</returns>
        public static string Encrypt(Type loType, string lsValue, string lsPassphrase)
        {
            IMaxProvider loProvider = Instance.GetProvider(loType);
            if (loProvider is IMaxEncryptionLibraryProvider)
            {
                return ((IMaxEncryptionLibraryProvider)loProvider).Encrypt(lsValue, lsPassphrase);
            }

            throw new MaxException("Provider for MaxEncryptionLibrary needs to implement IMaxEncryptionLibraryProvider");
        }

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="loType">Type of object to protect, or type of provider to use to protect</param>
        /// <param name="laValue">The data to be encrypted</param>
        /// <returns>Encrypted version of the data</returns>
        public static byte[] Decrypt(Type loType, byte[] laValue)
        {
            IMaxProvider loProvider = Instance.GetProvider(loType);
            if (loProvider is IMaxEncryptionLibraryProvider)
            {
                return ((IMaxEncryptionLibraryProvider)loProvider).Decrypt(laValue);
            }

            throw new MaxException("Provider for MaxEncryptionLibrary needs to implement IMaxEncryptionLibraryProvider");
        }

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="loType">Type of object to protect, or type of provider to use to protect</param>
        /// <param name="lsValue">The data to be encrypted</param>
        /// <returns>Encrypted version of the data</returns>
        public static string Decrypt(Type loType, string lsValue)
        {
            IMaxProvider loProvider = Instance.GetProvider(loType);
            if (loProvider is IMaxEncryptionLibraryProvider)
            {
                return ((IMaxEncryptionLibraryProvider)loProvider).Decrypt(lsValue);
            }

            throw new MaxException("Provider for MaxEncryptionLibrary needs to implement IMaxEncryptionLibraryProvider");
        }

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="loType">Type of object to protect, or type of provider to use to protect</param>
        /// <param name="laValue">The data to be encrypted</param>
        /// <param name="lsPassphrase">Pass phrase used to encrypt the data</param>
        /// <returns>Encrypted version of the data</returns>
        public static byte[] Decrypt(Type loType, byte[] laValue, string lsPassphrase)
        {
            IMaxProvider loProvider = Instance.GetProvider(loType);
            if (loProvider is IMaxEncryptionLibraryProvider)
            {
                return ((IMaxEncryptionLibraryProvider)loProvider).Decrypt(laValue, lsPassphrase);
            }

            throw new MaxException("Provider for MaxEncryptionLibrary needs to implement IMaxEncryptionLibraryProvider");
        }

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="loType">Type of object to protect, or type of provider to use to protect</param>
        /// <param name="lsValue">The data to be encrypted</param>
        /// <param name="lsPassphrase">Pass phrase used to encrypt the data</param>
        /// <returns>Encrypted version of the data</returns>
        public static string Decrypt(Type loType, string lsValue, string lsPassphrase)
        {
            IMaxProvider loProvider = Instance.GetProvider(loType);
            if (loProvider is IMaxEncryptionLibraryProvider)
            {
                return ((IMaxEncryptionLibraryProvider)loProvider).Decrypt(lsValue, lsPassphrase);
            }

            throw new MaxException("Provider for MaxEncryptionLibrary needs to implement IMaxEncryptionLibraryProvider");
        }

        /// <summary>
        /// Signs data with a private key to create a unique signature that can be verified with a public key
        /// </summary>
        /// <param name="loType">Type of object to protect, or type of provider to use to protect</param>
        /// <param name="lsPrivateKey">The private key</param>
        /// <param name="lsData">The data to sign</param>
        /// <returns>Signature that can be verified</returns>
        public static string SignData(Type loType, string lsPrivateKey, string lsData)
        {
            IMaxProvider loProvider = Instance.GetProvider(loType);
            if (loProvider is IMaxEncryptionLibraryProvider)
            {
                return ((IMaxEncryptionLibraryProvider)loProvider).SignData(lsPrivateKey, lsData);
            }

            throw new MaxException("Provider for MaxEncryptionLibrary needs to implement IMaxEncryptionLibraryProvider");
        }

        /// <summary>
        /// Uses a public key to verify the data matches what was signed with the private key
        /// </summary>
        /// <param name="loType">Type of object to protect, or type of provider to use to protect</param>
        /// <param name="lsPublicKey">The public key</param>
        /// <param name="lsData">The data to verify</param>
        /// <param name="lsSignature">The signature to use to verify</param>
        /// <returns>True if matching, false if not matching</returns>
        public static bool VerifyData(Type loType, string lsPublicKey, string lsData, string lsSignature)
        {
            IMaxProvider loProvider = Instance.GetProvider(loType);
            if (loProvider is IMaxEncryptionLibraryProvider)
            {
                return ((IMaxEncryptionLibraryProvider)loProvider).VerifyData(lsPublicKey, lsData, lsSignature);
            }

            throw new MaxException("Provider for MaxEncryptionLibrary needs to implement IMaxEncryptionLibraryProvider");
        }

        /// <summary>
        /// Gets a passphrase that is stored
        /// </summary>
        /// <param name="loType">Type of object to protect, or type of provider to use to protect</param>
        /// <param name="lsName">Name of the passphrase to get</param>
        /// <returns>Clear text version of the passphrase</returns>
        public static string GetPassphrase(Type loType, string lsName)
        {
            IMaxProvider loProvider = Instance.GetProvider(loType);
            if (loProvider is IMaxEncryptionLibraryProvider)
            {
                return ((IMaxEncryptionLibraryProvider)loProvider).GetPassphrase(lsName);
            }

            throw new MaxException("Provider for MaxEncryptionLibrary needs to implement IMaxEncryptionLibraryProvider");
        }

        /// <summary>
        /// Stores a passphrase in memory
        /// </summary>
        /// <param name="loType">Type of object to protect, or type of provider to use to protect</param>
        /// <param name="lsName">Name to use for the passphrase</param>
        /// <param name="lsValue">The passphrase to store</param>
        public static void SetPassphrase(Type loType, string lsName, string lsValue)
        {
            IMaxProvider loProvider = Instance.GetProvider(loType);
            if (loProvider is IMaxEncryptionLibraryProvider)
            {
                ((IMaxEncryptionLibraryProvider)loProvider).SetPassphrase(lsName, lsValue);
            }

            throw new MaxException("Provider for MaxEncryptionLibrary needs to implement IMaxEncryptionLibraryProvider");
        }

        /// <summary>
        /// Computes the hash and returns it as a string
        /// </summary>
        /// <param name="loType">Type of object to protect, or type of provider to use to protect</param>
        /// <param name="lsHash">The Hash type to use</param>
        /// <param name="loValue">The value used to compute the hash</param>
        /// <returns>A unique string for the byte value</returns>		
        public static string GetHash(Type loType, string lsHash, object loValue)
        {
            IMaxProvider loProvider = Instance.GetProvider(loType);
            if (loProvider is IMaxEncryptionLibraryProvider)
            {
                return ((IMaxEncryptionLibraryProvider)loProvider).GetHash(lsHash, loValue);
            }

            throw new MaxException("Provider for MaxEncryptionLibrary needs to implement IMaxEncryptionLibraryProvider");
        }

        /// <summary>
        /// Sets the application level default passphrase and entropy
        /// </summary>
        /// <param name="loType">Type of object to protect, or type of provider to use</param>
        /// <param name="lsDefaultPassphrase">Passphrase to use for default encryption when a passphrase is not specified.</param>
        /// <param name="lsDefaultEntropy">Entropy to use for DPAPI encryption (in memory only)</param>
        public static void SetDefault(Type loType, string lsDefaultPassphrase, string lsDefaultEntropy)
        {
            IMaxProvider loProvider = Instance.GetProvider(loType);
            if (loProvider is IMaxEncryptionLibraryProvider)
            {
                ((IMaxEncryptionLibraryProvider)loProvider).SetDefault(lsDefaultPassphrase, lsDefaultEntropy);
            }
        }

        public static double GetRandomDouble(Type loType, double lnStart, double lnEnd)
        {
            IMaxProvider loProvider = Instance.GetProvider(loType);
            if (loProvider is IMaxEncryptionLibraryProvider)
            {
                return ((IMaxEncryptionLibraryProvider)loProvider).GetRandomDouble(lnStart, lnEnd);
            }

            throw new MaxException("Provider for MaxEncryptionLibrary needs to implement IMaxEncryptionLibraryProvider");
        }

        /// <summary>
        /// Retruns A 32-bit signed integer greater than or equal to lnStart and less than or equal to lnEnd;
        /// </summary>
        /// <param name="loType"></param>
        /// <param name="lnStart"></param>
        /// <param name="lnEnd"></param>
        /// <returns></returns>
        public static int GetRandomInt(Type loType, int lnStart, int lnEnd)
        {
            IMaxProvider loProvider = Instance.GetProvider(loType);
            if (loProvider is IMaxEncryptionLibraryProvider)
            {
                return ((IMaxEncryptionLibraryProvider)loProvider).GetRandomInt(lnStart, lnEnd);
            }

            throw new MaxException("Provider for MaxEncryptionLibrary needs to implement IMaxEncryptionLibraryProvider");
        }
    }
}