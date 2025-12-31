// <copyright file="MaxEncryptionLibraryDefaultProvider.cs" company="Lakstins Family, LLC">
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
// <change date="1/16/2017" author="Brian A. Lakstins" description="Initial creation">
// <change date="4/20/2017" author="Brian A. Lakstins" description="Update VerifyDataConditional so that if the process fails false is just returned and the error is logged.">
// <change date="2/21/2018" author="Brian A. Lakstins" description="Add methods for random numbers.">
// <change date="6/4/2020" author="Brian A. Lakstins" description="Updated for change to base class.">
// <change date="7/25/2025" author="Brian A. Lakstins" description="Updated for .net core 8">
// <change date="12/31/2025" author="Brian A. Lakstins" description="Added new version that limits block size to 128 since that's what .net core 8 supports">
// <change date="12/31/2025" author="Brian A. Lakstins" description="Arrange code for different .net versions">
// </changelog>
#endregion

namespace MaxFactry.Core.Provider
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;
    using MaxFactry.Core;

    /// <summary>
    /// Library to provide encryption functionality
    /// </summary>
    public class MaxEncryptionLibraryDefaultProvider : MaxProvider, IMaxEncryptionLibraryProvider
    {
        /// <summary>
        /// Internal passphrase
        /// </summary>
        private string _sPassphrase = "This is the default Passphrase and should only be used for testing";

        /// <summary>
        /// Extra information used to encrypt data based on this computer
        /// Needed to decrypt information that was encrypted on this computer
        /// </summary>
        private string _sEntropy = "ThisIsTheEntropyStringUsedAsBytesToProtectStrings";


        /// <summary>
        /// Iterations used when generating key data
        /// </summary>
        private int _nIterations = 1000;

        /// <summary>
        /// Text used as the header
        /// </summary>
        private string _sHeaderText = "IsEncryptedMaxFactry";

        /// <summary>
        /// Index of passphrase information
        /// </summary>
        private MaxIndex _oPassphraseIndex = new MaxIndex();

        /// <summary>
        /// Version created for original code
        /// </summary>
        private int _nVersion1 = 20111212;

        /// <summary>
        /// Version created for this provider
        /// </summary>
        private int _nVersion2 = 20170116;
        
        /// <summary>
        /// Version created for this provider
        /// </summary>
        private int _nVersion3 = 20251231;

        /// <summary>
        /// Random seed
        /// </summary>
        private Random _oRandom = new Random();

        /// <summary>
        /// Initializes a new instance of the MaxEncryptionLibraryDefaultProvider class
        /// </summary>
        public MaxEncryptionLibraryDefaultProvider()
        {
        }

        /// <summary>
        /// Gets or sets the entropy
        /// </summary>
        protected virtual string Entropy
        {
            get
            {
                return this._sEntropy;
            }

            set
            {
                this._sEntropy = value;
            }
        }

        /// <summary>
        /// Gets or sets the Passphrase
        /// </summary>
        protected virtual string Passphrase
        {
            get
            {
                return this._sPassphrase;
            }

            set
            {
                this._sPassphrase = value;
            }
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="lsName">Name of the provider.</param>
        /// <param name="loConfig">Configuration information.</param>
        public override void Initialize(string lsName, MaxIndex loConfig)
        {
            base.Initialize(lsName, loConfig);
            string lsEntropy = this.GetConfigValue(loConfig, "MaxEncryptionProviderEntropy") as string;
            if (null != lsEntropy)
            {
                this._sEntropy = lsEntropy;
            }

            string lsPassphrase = this.GetConfigValue(loConfig, "MaxEncryptionProviderPassphrase") as string;
            if (null != lsPassphrase)
            {
                this._sPassphrase = lsPassphrase;
            }
        }

        /// <summary>
        /// Encrypts the text using DPAPI (Key provided by local system)
        /// Used to encrypt pass phrases for general encryption routines.
        /// </summary>
        /// <param name="lsText">The text to encrypt</param>
        /// <returns>encrypted text</returns>
        public virtual string Protect(string lsText)
        {
            if (null == lsText || string.Empty == lsText)
            {
                return lsText;
            }

            return this.ProtectConditional(lsText);
        }

        /// <summary>
        /// Decrypts text using DPAPI (Key provided by local system)
        /// Used to decrypt pass phrases for general encryption routines.
        /// </summary>
        /// <param name="lsTextProtected">The encrypted version of text</param>
        /// <returns>The unencrypted version of text</returns>
        public virtual string Unprotect(string lsTextProtected)
        {
            if (null == lsTextProtected || string.Empty == lsTextProtected)
            {
                return lsTextProtected;
            }

            return this.UnprotectConditional(lsTextProtected);
        }

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="laValue">byte array to be encrypted</param>
        /// <returns>encrypted version of the byte array</returns>
        public virtual byte[] Encrypt(byte[] laValue)
        {
            return this.Encrypt(laValue, this.Passphrase);
        }

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="lsValue">byte array to be encrypted</param>
        /// <returns>encrypted version of the byte array</returns>
        public virtual string Encrypt(string lsValue)
        {
            byte[] laValue = this.Encode(lsValue);
            byte[] laR = this.Encrypt(laValue);
            return Convert.ToBase64String(laR);
        }

        /// <summary>
        /// Encryption routine with external passphrase
        /// </summary>
        /// <param name="lsValue">byte array to be encrypted</param>
        /// <param name="lsPassphrase">Pass phrase used to encrypt the data</param>
        /// <returns>encrypted version of the byte array</returns>
        public virtual string Encrypt(string lsValue, string lsPassphrase)
        {
            byte[] laValue = this.Encode(lsValue);
            byte[] laR = this.Encrypt(laValue, lsPassphrase);
            return Convert.ToBase64String(laR);
        }

        /// <summary>
        /// Encryption routine with external passphrase
        /// </summary>
        /// <param name="laValue">byte array to be encrypted</param>
        /// <param name="lsPassphrase">Pass phrase used to encrypt the data</param>
        /// <returns>encrypted version of the byte array</returns>
        public virtual byte[] Encrypt(byte[] laValue, string lsPassphrase)
        {
            return this.EncryptConditional(laValue, lsPassphrase);
        }

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="laValue">The data to be encrypted</param>
        /// <returns>Encrypted version of the data</returns>
        public virtual byte[] Decrypt(byte[] laValue)
        {
            return this.Decrypt(laValue, this.Passphrase);
        }

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="lsValue">The data to be encrypted</param>
        /// <returns>Encrypted version of the data</returns>
        public virtual string Decrypt(string lsValue)
        {
            byte[] laValue = Convert.FromBase64String(lsValue);
            byte[] laR = this.Decrypt(laValue);
            return this.Decode(laR);
        }

        /// <summary>
        /// Encryption routine with external passphrase
        /// </summary>
        /// <param name="lsValue">The data to be encrypted</param>
        /// <param name="lsPassphrase">Pass phrase used to encrypt the data</param>
        /// <returns>Encrypted version of the data</returns>
        public virtual string Decrypt(string lsValue, string lsPassphrase)
        {
            byte[] laValue = Convert.FromBase64String(lsValue);
            byte[] laR = this.Decrypt(laValue, lsPassphrase);
            return this.Decode(laR);
        }

        /// <summary>
        /// Encryption routine with external passphrase
        /// </summary>
        /// <param name="laValue">The data to be encrypted</param>
        /// <param name="lsPassphrase">Pass phrase used to encrypt the data</param>
        /// <returns>Encrypted version of the data</returns>
        public virtual byte[] Decrypt(byte[] laValue, string lsPassphrase)
        {
            if (null == laValue || null == lsPassphrase || string.Empty == lsPassphrase || laValue.Length == 0)
            {
                //// Not enough information to decrypt anything, so return the original
                return laValue;
            }

            return this.DecryptConditional(laValue, lsPassphrase);
        }

        /// <summary>
        /// Signs data with a private key to create a unique signature that can be verified with a public key
        /// </summary>
        /// <param name="lsPrivateKey">The private key</param>
        /// <param name="lsData">The data to sign</param>
        /// <returns>Signature that can be verified</returns>
        public virtual string SignData(string lsPrivateKey, string lsData)
        {
            string lsR = this.SignDataConditional(lsPrivateKey, lsData);
            return lsR;
        }

        /// <summary>
        /// Uses a public key to verify the data matches what was signed with the private key
        /// </summary>
        /// <param name="lsPublicKey">The public key</param>
        /// <param name="lsData">The data to verify</param>
        /// <param name="lsSignature">The signature to use to verify</param>
        /// <returns>True if matching, false if not matching</returns>
        public virtual bool VerifyData(string lsPublicKey, string lsData, string lsSignature)
        {
            return this.VerifyDataConditional(lsPublicKey, lsData, lsSignature);
        }

        /// <summary>
        /// Gets a passphrase that is stored
        /// </summary>
        /// <param name="lsName">Name of the passphrase to get</param>
        /// <returns>Clear text version of the passphrase</returns>
        public virtual string GetPassphrase(string lsName)
        {
            string lsPassphraseProtected = this._oPassphraseIndex[lsName] as string;
            if (null != lsPassphraseProtected)
            {
                return this.Unprotect(lsPassphraseProtected);
            }

            if (lsName.Substring(0, "MaxPublicKey:".Length) == "MaxPublicKey:" || lsName.Substring(0, "MaxPrivateKey:".Length) == "MaxPrivateKey:")
            {
                return this.GetPassphraseConditional(lsName);
            }

            return string.Empty;
        }

        /// <summary>
        /// Stores a passphrase in memory
        /// </summary>
        /// <param name="lsName">Name to use for the passphrase</param>
        /// <param name="lsValue">The passphrase to store</param>
        public virtual void SetPassphrase(string lsName, string lsValue)
        {
            string lsPassphraseProtected = this.Protect(lsValue);
            this._oPassphraseIndex.Add(lsName, lsPassphraseProtected);
        }

        /// <summary>
        /// Computes the MD5 hash and returns it as a string
        /// </summary>
        /// <param name="lsHash">The hash method to use</param>
        /// <param name="loValue">The object to compute the hash</param>
        /// <returns>A unique string for the byte value</returns>
        public virtual string GetHash(string lsHash, object loValue)
        {
            return this.GetHashConditional(lsHash, loValue);
        }

        /// <summary>
        /// Sets the application level default passphrase and entropy
        /// </summary>
        /// <param name="lsDefaultPassphrase">Passphrase to use for default encryption when a passphrase is not specified.</param>
        /// <param name="lsDefaultEntropy">Entropy to use for DPAPI encryption (in memory only)</param>
        public virtual void SetDefault(string lsDefaultPassphrase, string lsDefaultEntropy)
        {
            if (null != lsDefaultPassphrase && lsDefaultPassphrase.Length > 0)
            {
                MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeApplication, typeof(MaxFactry.Core.MaxProvider) + "-MaxEncryptionProviderPassphrase", lsDefaultPassphrase);
                this._sPassphrase = lsDefaultPassphrase;
            }

            if (null != lsDefaultEntropy && lsDefaultEntropy.Length > 0)
            {
                MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeApplication, typeof(MaxFactry.Core.MaxProvider) + "-MaxEncryptionProviderEntropy", lsDefaultEntropy);
                this._sEntropy = lsDefaultEntropy;
            }
        }

        /// <summary>
        /// Gets a random number
        /// </summary>
        /// <param name="lnStart">Start Number</param>
        /// <param name="lnEnd">End Number</param>
        /// <returns>Random number</returns>
        public virtual double GetRandomDouble(double lnStart, double lnEnd)
        {
            double lnR = _oRandom.NextDouble() * (lnEnd - lnStart) + lnStart;
            return lnR;
        }

        /// <summary>
        /// Gets a random number
        /// </summary>
        /// <param name="lnStart">Start Number</param>
        /// <param name="lnEnd">End Number</param>
        /// <returns>Random number</returns>
        public virtual int GetRandomInt(int lnStart, int lnEnd)
        {
            int lnR = _oRandom.Next(lnStart, lnEnd + 1);
            return lnR;
        }

        /// <summary>
        /// Gets a key of the right length
        /// </summary>
        /// <param name="laKey">Original Key</param>
        /// <param name="lnLength">Length needed</param>
        /// <returns>binary key of the length provided</returns>
        protected byte[] GetKey(byte[] laKey, int lnLength)
        {
            int lnBitLength = lnLength / 8;
            byte[] laKeyNew = new byte[lnBitLength];
            for (int lnK = 0; lnK < lnBitLength; lnK++)
            {
                if (laKey.Length > lnK)
                {
                    laKeyNew[lnK] = laKey[lnK];
                }
                else
                {
                    laKeyNew[lnK] = 64;
                }
            }

            return laKeyNew;
        }

        protected SymmetricAlgorithm GetAlgorithm()
        {
            return this.GetAlgorithmConditional();
        }


#if net4_52 || netcore1
        protected SymmetricAlgorithm GetAlgorithmConditional()
        {
            return Aes.Create();
        }
#elif net2
        protected SymmetricAlgorithm GetAlgorithmConditional()
        {
            return new RijndaelManaged();
        }
#endif

#if net2 || netcore_8
        /// <summary>
        /// Encrypts the text using DPAPI (Key provided by local system)
        /// Used to encrypt pass phrases for general encryption routines.
        /// </summary>
        /// <param name="lsText">The text to encrypt</param>
        /// <returns>encrypted text</returns>
        protected virtual string ProtectConditional(string lsText)
        {
            byte[] loText = Encoding.UTF8.GetBytes(lsText);
            byte[] loTextProtected = ProtectedData.Protect(loText, Encoding.UTF8.GetBytes(this.Entropy), DataProtectionScope.LocalMachine);
            return Convert.ToBase64String(loTextProtected);
        }

        /// <summary>
        /// Decrypts text using DPAPI (Key provided by local system)
        /// Used to decrypt pass phrases for general encryption routines.
        /// </summary>
        /// <param name="lsTextProtected">The encrypted version of text</param>
        /// <returns>The unencrypted version of text</returns>
        protected virtual string UnprotectConditional(string lsTextProtected)
        {
            if (string.IsNullOrEmpty(lsTextProtected))
            {
                return lsTextProtected;
            }

            try
            {
                byte[] loTextProtected = Convert.FromBase64String(lsTextProtected);
                byte[] loText = ProtectedData.Unprotect(loTextProtected, Encoding.UTF8.GetBytes(this.Entropy), DataProtectionScope.LocalMachine);
                return Encoding.UTF8.GetString(loText);
            }
            catch
            {
                return lsTextProtected;
            }
        }

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="laValue">byte array to be encrypted</param>
        /// <param name="lsPassPhrase">Passphrase to use to encrypt</param>
        /// <returns>encrypted version of the byte array</returns>
        protected virtual byte[] EncryptConditional(byte[] laValue, string lsPassPhrase)
        {
            if (lsPassPhrase.StartsWith("MaxKey:") || lsPassPhrase.StartsWith("MaxKeyName:"))
            {
                return this.EncryptAsymmetric(laValue, lsPassPhrase);
            }

            if (null == laValue || null == lsPassPhrase || string.Empty == lsPassPhrase)
            {
                //// Not enough information to encrypt anything, so return the original
                return laValue;
            }

            //// RijndaelManaged Key size and Block size max are 256 as of 1/16/2017 - should use that as the size in case the max changes
            //// Block size is reduced to 128 with _nVersion3 to be compatible with AES standard
            byte[] loR = this.EncryptWithHeaderAndSalt(laValue, lsPassPhrase, this._nVersion3);
            return loR;
        }

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="laValue">byte array to be encrypted</param>
        /// <param name="lsPassPhrase">Passphrase to use to encrypt</param>
        /// <returns>encrypted version of the byte array</returns>
        protected virtual byte[] DecryptConditional(byte[] laValue, string lsPassPhrase)
        {
            if (lsPassPhrase.StartsWith("MaxKey:") || lsPassPhrase.StartsWith("MaxKeyName:"))
            {
                return this.DecryptAsymmetric(laValue, lsPassPhrase);
            }

            byte[] loR = this.DecryptWithHeaderAndSalt(laValue, lsPassPhrase);
            return loR;
        }

        protected virtual byte[] GetSaltConditional(int lnVersion)
        {
            int lnSize = this.GetSaltSize(lnVersion);
            byte[] loSalt = new byte[lnSize];
            //// Generate a generic Salt to use in the encryption process and store with the encrypted data
            RNGCryptoServiceProvider loRNG = new RNGCryptoServiceProvider();
            loRNG.GetBytes(loSalt);
            return loSalt;
        }

        protected virtual byte[] GetEncryptedContentConditional(byte[] laContent, string lsPassPhrase, byte[] loSalt, string lsAlgorithmName, int lnKeySize, int lnBlockSize)
        {
            if (lsAlgorithmName == "AES" || lsAlgorithmName == "RijndaelManaged")
            {
                SymmetricAlgorithm loAlgorithm = this.GetAlgorithm();
                loAlgorithm.KeySize = lnKeySize;
                loAlgorithm.BlockSize = lnBlockSize;

                //// Generate key informatin based on the Pass Phrase and Salt.
                Rfc2898DeriveBytes loGenerator = new Rfc2898DeriveBytes(lsPassPhrase, loSalt, this._nIterations);

                //// Generate a Key and an Initilization Vector.  Having the Pass Phrase and Salt
                //// during the decryption process should create the same Key and Initilization Vector.
                byte[] loKey = loGenerator.GetBytes(loAlgorithm.KeySize / 8);
                byte[] loIV = loGenerator.GetBytes(loAlgorithm.BlockSize / 8);

                loAlgorithm.Key = this.GetKey(loKey, loAlgorithm.KeySize);
                loAlgorithm.IV = this.GetKey(loIV, loAlgorithm.BlockSize);
                loAlgorithm.Mode = CipherMode.CBC;

                return this.GetEncryptedContent(laContent, loAlgorithm);
            }

            return null;
        }

        protected virtual byte[] GetEncryptedContent(byte[] laContent, SymmetricAlgorithm loAlgorithm)
        {
            ICryptoTransform loEncryptor = loAlgorithm.CreateEncryptor();
            byte[] laEncryptedContent = null;
            MemoryStream loMemory = new MemoryStream();
            try
            {
                CryptoStream loCrypto = new CryptoStream(loMemory, loEncryptor, CryptoStreamMode.Write);
                try
                {
                    StreamWriter loWriter = new StreamWriter(loCrypto);
                    loCrypto.Write(laContent, 0, laContent.Length);
                    loCrypto.FlushFinalBlock();
                    laEncryptedContent = loMemory.ToArray();
                }
                finally
                {
                    loCrypto.Dispose();
                    loCrypto = null;
                }
            }
            finally
            {
                loMemory = null;
            }

            return laEncryptedContent;
        }

        protected byte[] GetDecryptedContentConditional(byte[] laValueEncrypted, string lsPassPhrase, byte[] loSalt, string lsAlgorithmName, int lnKeySize, int lnBlockSize)
        {
            if (lsAlgorithmName == "AES" || lsAlgorithmName == "RijndaelManaged")
            {
                SymmetricAlgorithm loAlgorithm = this.GetAlgorithm();
                loAlgorithm.KeySize = lnKeySize;
                loAlgorithm.BlockSize = lnBlockSize;

                //// Generate key informatin based on the Pass Phrase and Salt.
                Rfc2898DeriveBytes loGenerator = new Rfc2898DeriveBytes(lsPassPhrase, loSalt, this._nIterations);

                //// Generate a Key and an Initilization Vector.  Having the Pass Phrase and Salt
                //// during the decryption process should create the same Key and Initilization Vector.
                byte[] loKey = loGenerator.GetBytes(loAlgorithm.KeySize / 8);
                byte[] loIV = loGenerator.GetBytes(loAlgorithm.BlockSize / 8);

                loAlgorithm.Key = this.GetKey(loKey, loAlgorithm.KeySize);
                loAlgorithm.IV = this.GetKey(loIV, loAlgorithm.BlockSize);
                loAlgorithm.Mode = CipherMode.CBC;

                return this.GetDecryptedContent(laValueEncrypted, loAlgorithm);
            }

            return laValueEncrypted;
        }

        protected byte[] GetDecryptedContent(byte[] laValueEncrypted, SymmetricAlgorithm loAlgorithm)
        {
            byte[] laValue = null;
            ICryptoTransform loDecryptor = loAlgorithm.CreateDecryptor();
            MemoryStream loMemoryEncrypted = new MemoryStream(laValueEncrypted);
            try
            {
                CryptoStream loCrypto = new CryptoStream(loMemoryEncrypted, loDecryptor, CryptoStreamMode.Read);
                try
                {
                    MemoryStream loMemory = new MemoryStream();
                    try
                    {
                        byte[] loBuffer = new byte[8192];
                        int lnRead = loCrypto.Read(loBuffer, 0, loBuffer.Length);
                        while (lnRead > 0)
                        {
                            loMemory.Write(loBuffer, 0, lnRead);
                            lnRead = loCrypto.Read(loBuffer, 0, loBuffer.Length);
                        }

                        laValue = loMemory.ToArray();
                    }
                    finally
                    {
                        loMemory.Dispose();
                        loMemory = null;
                    }
                }
                finally
                {
                    loCrypto.Dispose();
                    loCrypto = null;
                }
            }
            finally
            {
                loMemoryEncrypted = null;
            }

            return laValue;
        }

        protected RSAParameters GetKey(MaxIndex loKeyIndex)
        {
            RSAParameters loKey = new RSAParameters();
            loKey.Exponent = Convert.FromBase64String(loKeyIndex["Exponent"] as string);
            loKey.Modulus = Convert.FromBase64String(loKeyIndex["Modulus"] as string);
            if (loKeyIndex.Contains("D"))
            {
                loKey.D = Convert.FromBase64String(loKeyIndex["D"] as string);
                loKey.DP = Convert.FromBase64String(loKeyIndex["DP"] as string);
                loKey.DQ = Convert.FromBase64String(loKeyIndex["DQ"] as string);
                loKey.InverseQ = Convert.FromBase64String(loKeyIndex["InverseQ"] as string);
                loKey.P = Convert.FromBase64String(loKeyIndex["P"] as string);
                loKey.Q = Convert.FromBase64String(loKeyIndex["Q"] as string);
            }

            return loKey;
        }

        protected RSACryptoServiceProvider GetAsymetricProvider(string lsPassphrase)
        {
            RSACryptoServiceProvider loRSA = new RSACryptoServiceProvider();
            if (lsPassphrase.StartsWith("MaxKeyName:"))
            {
                CspParameters loParameters = new CspParameters();
                loParameters.KeyContainerName = lsPassphrase.Substring("MaxKeyName:".Length);
                loRSA = new RSACryptoServiceProvider(loParameters);
            }
            else if (lsPassphrase.StartsWith("MaxKey:"))
            {
                MaxIndex loKeyIndex = MaxConvertLibrary.DeserializeObject(lsPassphrase.Substring("MaxKey:".Length), typeof(MaxIndex)) as MaxIndex;
                RSAParameters loKey = this.GetKey(loKeyIndex);
                loRSA.ImportParameters(loKey);
            }
            else
            {
                MaxIndex loKeyIndex = MaxConvertLibrary.DeserializeObject(lsPassphrase, typeof(MaxIndex)) as MaxIndex;
                RSAParameters loKey = this.GetKey(loKeyIndex);
                loRSA.ImportParameters(loKey);
            }

            return loRSA;
        }

        /// <summary>
        /// Encryption routine with external passphrase
        /// </summary>
        /// <param name="laValue">byte array to be encrypted</param>
        /// <param name="lsPassphrase">Pass phrase used to encrypt the data</param>
        /// <returns>encrypted version of the byte array</returns>
        protected virtual byte[] EncryptAsymmetric(byte[] laValue, string lsPassphrase)
        {
            RSACryptoServiceProvider loRSA = this.GetAsymetricProvider(lsPassphrase);
            return loRSA.Encrypt(laValue, true);
        }

        /// <summary>
        /// Encryption routine with external passphrase
        /// </summary>
        /// <param name="laValue">byte array to be encrypted</param>
        /// <param name="lsPassphrase">Pass phrase used to encrypt the data</param>
        /// <returns>encrypted version of the byte array</returns>
        protected virtual byte[] DecryptAsymmetric(byte[] laValue, string lsPassphrase)
        {
            RSACryptoServiceProvider loRSA = this.GetAsymetricProvider(lsPassphrase);
            return loRSA.Decrypt(laValue, true);
        }

        /// <summary>
        /// Signs data with a private key to create a unique signature that can be verified with a public key
        /// </summary>
        /// <param name="lsPrivateKey">The private key</param>
        /// <param name="lsData">The data to sign</param>
        /// <returns>Signature that can be verified</returns>
        protected virtual string SignDataConditional(string lsPrivateKey, string lsData)
        {
            object loHashAlgorithm = new SHA1CryptoServiceProvider();
            RSACryptoServiceProvider loRSA = new RSACryptoServiceProvider();

            MaxIndex loPrivateKeyIndex = MaxConvertLibrary.DeserializeObject(lsPrivateKey, typeof(MaxIndex)) as MaxIndex;
            RSAParameters loPrivateKey = this.GetKey(loPrivateKeyIndex);
            loRSA.ImportParameters(loPrivateKey);

            byte[] laData = System.Text.Encoding.Unicode.GetBytes(lsData);
            byte[] laSignature = loRSA.SignData(laData, loHashAlgorithm);
            return "SHA1:" + Convert.ToBase64String(laSignature);
        }

        /// <summary>
        /// Uses a public key to verify the data matches what was signed with the private key
        /// </summary>
        /// <param name="lsPublicKey">The public key</param>
        /// <param name="lsData">The data to verify</param>
        /// <param name="lsSignature">The signature to use to verify</param>
        /// <returns>True if matching, false if not matching</returns>
        protected virtual bool VerifyDataConditional(string lsPublicKey, string lsData, string lsSignature)
        {
            bool lbR = false;
            object loHashAlgorithm = new SHA1CryptoServiceProvider();
            string lsSig = lsSignature;
            string[] laSigType = lsSignature.Split(':');
            if (laSigType.Length > 1)
            {
                if (laSigType[0] != "SHA1")
                {
                    throw new MaxException("MaxEncryptionLibraryNet20Provider only supports SHA1");
                }

                lsSig = laSigType[1];
            }

            try
            {
                MaxIndex loPublicKeyIndex = MaxConvertLibrary.DeserializeObject(lsPublicKey, typeof(MaxIndex)) as MaxIndex;
                RSAParameters loPublicKey = this.GetKey(loPublicKeyIndex);
                RSACryptoServiceProvider loRSA = new RSACryptoServiceProvider();
                loRSA.ImportParameters(loPublicKey);

                byte[] laSignature = Convert.FromBase64String(lsSig);
                byte[] laData = System.Text.Encoding.Unicode.GetBytes(lsData);
                lbR = loRSA.VerifyData(laData, loHashAlgorithm, laSignature);
            }
            catch (Exception loE)
            {
                MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error in VerifyDataConditional.", loE));
            }

            return lbR;
        }

        /// <summary>
        /// Gets a passphrase that is stored
        /// </summary>
        /// <param name="lsName">Name of the passphrase to get</param>
        /// <returns>Clear text version of the passphrase</returns>
        protected virtual string GetPassphraseConditional(string lsName)
        {
            string lsPassphraseProtected = this._oPassphraseIndex[lsName] as string;
            if (null != lsPassphraseProtected)
            {
                return this.Unprotect(lsPassphraseProtected);
            }

            if (lsName.StartsWith("MaxPublicKey:") || lsName.StartsWith("MaxPrivateKey:"))
            {
                string lsKeyName = lsName.Substring("MaxPublicKey:".Length);
                bool lbIncludePrivate = false;
                if (lsName.StartsWith("MaxPrivateKey:"))
                {
                    lsKeyName = lsName.Substring("MaxPrivateKey:".Length);
                    lbIncludePrivate = true;
                }

                CspParameters loParameters = new CspParameters();
                loParameters.KeyContainerName = lsKeyName;
                int lnKeyLength = 2048;
                string lsKeyLength = string.Empty;
                int lnK = lsKeyName.Length - 1;
                while (char.IsNumber(lsKeyName[lnK]) && lnK >= 0)
                {
                    lsKeyLength = lsKeyName[lnK] + lsKeyLength;
                    lnK--;
                }

                if (!string.IsNullOrEmpty(lsKeyLength))
                {
                    lnKeyLength = MaxConvertLibrary.ConvertToInt(typeof(object), lsKeyLength);
                }

                RSACryptoServiceProvider loRSA = new RSACryptoServiceProvider(lnKeyLength, loParameters);
                RSAParameters loKey = loRSA.ExportParameters(lbIncludePrivate);
                MaxIndex loKeyIndex = new MaxIndex();
                loKeyIndex.Add("Exponent", Convert.ToBase64String(loKey.Exponent));
                loKeyIndex.Add("Modulus", Convert.ToBase64String(loKey.Modulus));
                if (lbIncludePrivate)
                {
                    loKeyIndex.Add("D", Convert.ToBase64String(loKey.D));
                    loKeyIndex.Add("DP", Convert.ToBase64String(loKey.DP));
                    loKeyIndex.Add("DQ", Convert.ToBase64String(loKey.DQ));
                    loKeyIndex.Add("InverseQ", Convert.ToBase64String(loKey.InverseQ));
                    loKeyIndex.Add("P", Convert.ToBase64String(loKey.P));
                    loKeyIndex.Add("Q", Convert.ToBase64String(loKey.Q));
                }

                string lsKey = MaxConvertLibrary.SerializeObjectToString(loKeyIndex);
                this._oPassphraseIndex.Add(lsName, this.Protect(lsKey));
                return lsKey;
            }

            return string.Empty;
        }

        /// <summary>
        /// Computes the hash and returns it as a string
        /// </summary>
        /// <param name="lsHash">The hash method to use</param>
        /// <param name="loValue">The object to compute the hash</param>
        /// <returns>A unique string for the byte value</returns>
        protected virtual string GetHashConditional(string lsHash, object loValue)
        {
            byte[] laHash = null;
            HashAlgorithm loAlgorithm = null;
            if (lsHash == MaxEncryptionLibrary.MD5Hash)
            {
                loAlgorithm = new MD5CryptoServiceProvider();
            }
            else if (lsHash == MaxEncryptionLibrary.SHA256Hash)
            {
                loAlgorithm = new SHA256Managed();
            }

            if (null != loAlgorithm)
            {

                if (loValue is byte[])
                {
                    laHash = loAlgorithm.ComputeHash((byte[])loValue);
                }
                else if (loValue is Stream)
                {
                    laHash = loAlgorithm.ComputeHash((Stream)loValue);
                }
                else if (loValue is string)
                {
                    laHash = loAlgorithm.ComputeHash(this.Encode((string)loValue));
                }

                if (null != laHash)
                {
                    return this.ConvertToString(laHash);
                }
            }
            else
            {
                throw new MaxException("Hash type of [" + lsHash + "] is not supported");
            }

            throw new MaxException("Object cannot compute a hash on type " + loValue.GetType().ToString());
        }

        protected string DecodeConditional(byte[] laText)
        {
            string lsR = string.Empty;
            Encoding loEncoding = detectTextEncoding(laText, out lsR);
            return lsR;
        }

        // Function to detect the encoding for UTF-7, UTF-8/16/32 (bom, no bom, little
        // & big endian), and local default codepage, and potentially other codepages.
        // 'taster' = number of bytes to check of the file (to save processing). Higher
        // value is slower, but more reliable (especially UTF-8 with special characters
        // later on may appear to be ASCII initially). If taster = 0, then taster
        // becomes the length of the file (for maximum reliability). 'text' is simply
        // the string with the discovered encoding applied to the file.
        public Encoding detectTextEncoding(byte[] b, out String text, int taster = 1000)
        {
            //////////////// First check the low hanging fruit by checking if a
            //////////////// BOM/signature exists (sourced from http://www.unicode.org/faq/utf_bom.html#bom4)
            if (b.Length >= 4 && b[0] == 0x00 && b[1] == 0x00 && b[2] == 0xFE && b[3] == 0xFF) { text = Encoding.GetEncoding("utf-32BE").GetString(b, 4, b.Length - 4); return Encoding.GetEncoding("utf-32BE"); }  // UTF-32, big-endian 
            else if (b.Length >= 4 && b[0] == 0xFF && b[1] == 0xFE && b[2] == 0x00 && b[3] == 0x00) { text = Encoding.UTF32.GetString(b, 4, b.Length - 4); return Encoding.UTF32; }    // UTF-32, little-endian
            else if (b.Length >= 2 && b[0] == 0xFE && b[1] == 0xFF) { text = Encoding.BigEndianUnicode.GetString(b, 2, b.Length - 2); return Encoding.BigEndianUnicode; }     // UTF-16, big-endian
            else if (b.Length >= 2 && b[0] == 0xFF && b[1] == 0xFE) { text = Encoding.Unicode.GetString(b, 2, b.Length - 2); return Encoding.Unicode; }              // UTF-16, little-endian
            else if (b.Length >= 3 && b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF) { text = Encoding.UTF8.GetString(b, 3, b.Length - 3); return Encoding.UTF8; } // UTF-8
            else if (b.Length >= 3 && b[0] == 0x2b && b[1] == 0x2f && b[2] == 0x76) { text = Encoding.UTF7.GetString(b, 3, b.Length - 3); return Encoding.UTF7; } // UTF-7


            //////////// If the code reaches here, no BOM/signature was found, so now
            //////////// we need to 'taste' the file to see if can manually discover
            //////////// the encoding. A high taster value is desired for UTF-8
            if (taster == 0 || taster > b.Length) taster = b.Length;    // Taster size can't be bigger than the filesize obviously.


            // Some text files are encoded in UTF8, but have no BOM/signature. Hence
            // the below manually checks for a UTF8 pattern. This code is based off
            // the top answer at: http://stackoverflow.com/questions/6555015/check-for-invalid-utf8
            // For our purposes, an unnecessarily strict (and terser/slower)
            // implementation is shown at: http://stackoverflow.com/questions/1031645/how-to-detect-utf-8-in-plain-c
            // For the below, false positives should be exceedingly rare (and would
            // be either slightly malformed UTF-8 (which would suit our purposes
            // anyway) or 8-bit extended ASCII/UTF-16/32 at a vanishingly long shot).
            int i = 0;
            bool utf8 = false;
            while (i < taster - 4)
            {
                if (b[i] <= 0x7F) { i += 1; continue; }     // If all characters are below 0x80, then it is valid UTF8, but UTF8 is not 'required' (and therefore the text is more desirable to be treated as the default codepage of the computer). Hence, there's no "utf8 = true;" code unlike the next three checks.
                if (b[i] >= 0xC2 && b[i] <= 0xDF && b[i + 1] >= 0x80 && b[i + 1] < 0xC0) { i += 2; utf8 = true; continue; }
                if (b[i] >= 0xE0 && b[i] <= 0xF0 && b[i + 1] >= 0x80 && b[i + 1] < 0xC0 && b[i + 2] >= 0x80 && b[i + 2] < 0xC0) { i += 3; utf8 = true; continue; }
                if (b[i] >= 0xF0 && b[i] <= 0xF4 && b[i + 1] >= 0x80 && b[i + 1] < 0xC0 && b[i + 2] >= 0x80 && b[i + 2] < 0xC0 && b[i + 3] >= 0x80 && b[i + 3] < 0xC0) { i += 4; utf8 = true; continue; }
                utf8 = false; break;
            }
            if (utf8 == true)
            {
                text = Encoding.UTF8.GetString(b);
                return Encoding.UTF8;
            }


            // The next check is a heuristic attempt to detect UTF-16 without a BOM.
            // We simply look for zeroes in odd or even byte places, and if a certain
            // threshold is reached, the code is 'probably' UF-16.          
            double threshold = 0.1; // proportion of chars step 2 which must be zeroed to be diagnosed as utf-16. 0.1 = 10%
            int count = 0;
            for (int n = 0; n < taster; n += 2) if (b[n] == 0) count++;
            if (((double)count) / taster > threshold) { text = Encoding.BigEndianUnicode.GetString(b); return Encoding.BigEndianUnicode; }
            count = 0;
            for (int n = 1; n < taster; n += 2) if (b[n] == 0) count++;
            if (((double)count) / taster > threshold) { text = Encoding.Unicode.GetString(b); return Encoding.Unicode; } // (little-endian)


            // Finally, a long shot - let's see if we can find "charset=xyz" or
            // "encoding=xyz" to identify the encoding:
            for (int n = 0; n < taster - 9; n++)
            {
                if (
                    ((b[n + 0] == 'c' || b[n + 0] == 'C') && (b[n + 1] == 'h' || b[n + 1] == 'H') && (b[n + 2] == 'a' || b[n + 2] == 'A') && (b[n + 3] == 'r' || b[n + 3] == 'R') && (b[n + 4] == 's' || b[n + 4] == 'S') && (b[n + 5] == 'e' || b[n + 5] == 'E') && (b[n + 6] == 't' || b[n + 6] == 'T') && (b[n + 7] == '=')) ||
                    ((b[n + 0] == 'e' || b[n + 0] == 'E') && (b[n + 1] == 'n' || b[n + 1] == 'N') && (b[n + 2] == 'c' || b[n + 2] == 'C') && (b[n + 3] == 'o' || b[n + 3] == 'O') && (b[n + 4] == 'd' || b[n + 4] == 'D') && (b[n + 5] == 'i' || b[n + 5] == 'I') && (b[n + 6] == 'n' || b[n + 6] == 'N') && (b[n + 7] == 'g' || b[n + 7] == 'G') && (b[n + 8] == '='))
                    )
                {
                    if (b[n + 0] == 'c' || b[n + 0] == 'C') n += 8; else n += 9;
                    if (b[n] == '"' || b[n] == '\'') n++;
                    int oldn = n;
                    while (n < taster && (b[n] == '_' || b[n] == '-' || (b[n] >= '0' && b[n] <= '9') || (b[n] >= 'a' && b[n] <= 'z') || (b[n] >= 'A' && b[n] <= 'Z')))
                    { n++; }
                    byte[] nb = new byte[n - oldn];
                    Array.Copy(b, oldn, nb, 0, n - oldn);
                    try
                    {
                        string internalEnc = Encoding.ASCII.GetString(nb);
                        text = Encoding.GetEncoding(internalEnc).GetString(b);
                        return Encoding.GetEncoding(internalEnc);
                    }
                    catch { break; }    // If C# doesn't recognize the name of the encoding, break.
                }
            }


            // If all else fails, the encoding is probably (though certainly not
            // definitely) the user's local codepage! One might present to the user a
            // list of alternative encodings as shown here: http://stackoverflow.com/questions/8509339/what-is-the-most-common-encoding-of-each-language
            // A full list can be found using Encoding.GetEncodings();
            text = Encoding.Default.GetString(b);
            return Encoding.Default;
        }

        protected virtual byte[] GetHeaderConditional(int lnVersion)
        {
            if (lnVersion >= this._nVersion2)
            {
                return System.Text.Encoding.UTF8.GetBytes(this._sHeaderText);
            }
            else if (lnVersion == this._nVersion1)
            {
                return System.Text.Encoding.Unicode.GetBytes(this._sHeaderText);
            }

            return System.Text.Encoding.ASCII.GetBytes(this._sHeaderText);
        }

#elif netcore1 || netstandard1_2
        /// <summary>
        /// Encrypts the text using DPAPI (Key provided by local system)
        /// Used to encrypt pass phrases for general encryption routines.
        /// </summary>
        /// <param name="lsText">The text to encrypt</param>
        /// <returns>encrypted text</returns>
        protected virtual string ProtectConditional(string lsText)
        {
            return lsText;
        }

        /// <summary>
        /// Decrypts text using DPAPI (Key provided by local system)
        /// Used to decrypt pass phrases for general encryption routines.
        /// </summary>
        /// <param name="lsTextProtected">The encrypted version of text</param>
        /// <returns>The unencrypted version of text</returns>
        protected virtual string UnprotectConditional(string lsTextProtected)
        {
            return lsTextProtected;
        }

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="laValue">byte array to be encrypted</param>
        /// <returns>encrypted version of the byte array</returns>
        protected virtual byte[] EncryptConditional(byte[] laValue, string lsPassPhrase)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="laValue">byte array to be encrypted</param>
        /// <param name="lsPassPhrase">Passphrase to use to encrypt</param>
        /// <returns>encrypted version of the byte array</returns>
        protected virtual byte[] DecryptConditional(byte[] laValue, string lsPassPhrase)
        {
            throw new NotImplementedException();
        }

        protected virtual byte[] GetSaltConditional(int lnVersion)
        {
            throw new NotImplementedException();
        }

        protected virtual byte[] GetEncryptedContentConditional(byte[] laContent, string lsPassPhrase, byte[] loSalt, string lsAlgorithmName, int lnKeySize, int lnBlockSize)
        {
            throw new NotImplementedException();
        }

        protected byte[] GetDecryptedContentConditional(byte[] laValueEncrypted, string lsPassPhrase, byte[] loSalt, string lsAlgorithmName, int lnKeySize, int lnBlockSize)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Signs data with a private key to create a unique signature that can be verified with a public key
        /// </summary>
        /// <param name="lsPrivateKey">The private key</param>
        /// <param name="lsData">The data to sign</param>
        /// <returns>Signature that can be verified</returns>
        protected virtual string SignDataConditional(string lsPrivateKey, string lsData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Uses a public key to verify the data matches what was signed with the private key
        /// </summary>
        /// <param name="lsPublicKey">The public key</param>
        /// <param name="lsData">The data to verify</param>
        /// <param name="lsSignature">The signature to use to verify</param>
        /// <returns>True if matching, false if not matching</returns>
        protected virtual bool VerifyDataConditional(string lsPublicKey, string lsData, string lsSignature)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a passphrase that is stored
        /// </summary>
        /// <param name="lsName">Name of the passphrase to get</param>
        /// <returns>Clear text version of the passphrase</returns>
        protected virtual string GetPassphraseConditional(string lsName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Computes the hash and returns it as a string
        /// </summary>
        /// <param name="lsHash">The hash method to use</param>
        /// <param name="loValue">The object to compute the hash</param>
        /// <returns>A unique string for the byte value</returns>
        protected virtual string GetHashConditional(string lsHash, object loValue)
        {
            throw new NotImplementedException();
        }

        protected string DecodeConditional(byte[] laText)
        {
            throw new NotImplementedException();
        }

        protected virtual byte[] GetHeaderConditional(int lnVersion)
        {
            return System.Text.Encoding.UTF8.GetBytes("IsEncryptedMaxFactry");
        }
#endif

        /// <summary>
        /// Converts a binary hash array to a string
        /// </summary>
        /// <param name="laHash">binary array</param>
        /// <returns>string version of the hash</returns>
        private string ConvertToString(byte[] laHash)
        {
            StringBuilder loHashText = new StringBuilder(laHash.Length);
            for (int lnH = 0; lnH < laHash.Length; lnH++)
            {
                loHashText.Append(laHash[lnH].ToString("X2"));
            }

            return loHashText.ToString();
        }

        protected virtual byte[] EncryptWithHeaderAndSalt(byte[] laValue, string lsPassphrase, int lnVersion)
        {
            byte[] laHeader = this.GetHeader(lnVersion);
            byte[] laVersion = BitConverter.GetBytes(lnVersion);
            byte[] laSalt = this.GetSaltConditional(lnVersion);
            byte[] laEncrypted = this.GetEncryptedContent(laValue, lsPassphrase, laSalt, lnVersion);
            //// Create the return data.
            //// Add the original length to the end of the returned data.
            byte[] laReturn = new byte[laHeader.Length + laSalt.Length + laEncrypted.Length + laVersion.Length];
            //// Add the Header to the beginning of the returned data.
            Buffer.BlockCopy(laHeader, 0, laReturn, 0, laHeader.Length);
            //// Add the Salt right after that
            Buffer.BlockCopy(laSalt, 0, laReturn, laHeader.Length, laSalt.Length);
            //// Add the actual encrypted data
            Buffer.BlockCopy(laEncrypted, 0, laReturn, laHeader.Length + laSalt.Length, laEncrypted.Length);
            //// Add the version infomation to the end.  The version is a fixed length.  Based on the version, we know how long the Header and Salt are.
            Buffer.BlockCopy(laVersion, 0, laReturn, laHeader.Length + laSalt.Length + laEncrypted.Length, laVersion.Length);

            //// Return the encrypted buffer.
            return laReturn;
        }

        protected virtual byte[] GetHeader(int lnVersion)
        {
            return this.GetHeaderConditional(lnVersion);
        }

        protected virtual int GetSaltSize(int lnVersion)
        {
            return 256;
        }

        protected virtual byte[] GetEncryptedContent(byte[] laContent, string lsPassPhrase, byte[] loSalt, int lnVersion)
        {
            int lnKeySize = 256;
            int lnBlockSize = 256;
            if (lnVersion >= this._nVersion3)
            {
                lnBlockSize = 128;
            }

            return this.GetEncryptedContentConditional(laContent, lsPassPhrase, loSalt, "AES", lnKeySize, lnBlockSize);
        }

        /// <summary>
        /// Decrypts data using the Pass Phrase and the SymmetricAlgorithm
        /// </summary>
        /// <param name="laValueEncryptedWithSalt">The data to be encrypted</param>
        /// <param name="lsPassPhrase">Pass phrase used to encrypt the data</param>
        /// <param name="loAlgorithm">Type of SymmetricAlgorithm to use</param>
        /// <returns>Encrypted version of the data</returns>
        protected byte[] DecryptWithHeaderAndSalt(byte[] laValueEncryptedWithSalt, string lsPassPhrase)
        {
            byte[] laVersion = new byte[BitConverter.GetBytes(int.MinValue).Length];
            //// Get the version data if available
            Buffer.BlockCopy(laValueEncryptedWithSalt, laValueEncryptedWithSalt.Length - laVersion.Length, laVersion, 0, laVersion.Length);
            int lnVersion = BitConverter.ToInt32(laVersion, 0);
            byte[] laHeader = this.GetHeader(lnVersion);

            //// Check to make sure the header matches
            if (laValueEncryptedWithSalt.Length < laHeader.Length)
            {
                return laValueEncryptedWithSalt;
            }

            for (int lnC = 0; lnC < laHeader.Length; lnC++)
            {
                if (laHeader[lnC] != laValueEncryptedWithSalt[lnC])
                {
                    return laValueEncryptedWithSalt;
                }
            }

            byte[] laSalt = new byte[this.GetSaltSize(lnVersion)];
            byte[] laValueEncrypted = new byte[laValueEncryptedWithSalt.Length - laSalt.Length - laVersion.Length - laHeader.Length];
            //// Get the original Salt from the encrypted data.
            Buffer.BlockCopy(laValueEncryptedWithSalt, laHeader.Length, laSalt, 0, laSalt.Length);
            //// Get the encrypted data only
            Buffer.BlockCopy(laValueEncryptedWithSalt, laHeader.Length + laSalt.Length, laValueEncrypted, 0, laValueEncryptedWithSalt.Length - laSalt.Length - laHeader.Length - laVersion.Length);

            //// Decrypt the original data
            byte[] laValue = this.GetDecryptedContent(laValueEncrypted, lsPassPhrase, laSalt, lnVersion);
            return laValue;
        }

        protected virtual byte[] GetDecryptedContent(byte[] laValueEncrypted, string lsPassPhrase, byte[] loSalt, int lnVersion)
        {
            int lnKeySize = 256;
            int lnBlockSize = 256;
            if (lnVersion >= this._nVersion3)
            {
                lnBlockSize = 128;
            }

            return this.GetDecryptedContentConditional(laValueEncrypted, lsPassPhrase, loSalt, "AES", lnKeySize, lnBlockSize);
        }

        protected byte[] Encode(string lsText)
        {
            return Encoding.UTF8.GetBytes(lsText);
        }


        protected string Decode(byte[] laText)
        {
            return this.DecodeConditional(laText);
        }
    }
}
