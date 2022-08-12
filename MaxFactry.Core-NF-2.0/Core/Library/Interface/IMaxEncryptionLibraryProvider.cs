// <copyright file="IMaxEncryptionLibraryProvider.cs" company="Lakstins Family, LLC">
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
// <change date="7/23/2016" author="Brian A. Lakstins" description="Add method to override defaults.">
// <change date="7/26/2016" author="Brian A. Lakstins" description="Add methods for asymmetric Public Key encryption and signing.">
// <change date="2/21/2018" author="Brian A. Lakstins" description="Add methods for random numbers.">
// </changelog>
#endregion

namespace MaxFactry.Core
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Defines the base interface for encrypting data
    /// </summary>
    public interface IMaxEncryptionLibraryProvider : IMaxProvider
    {
        /// <summary>
        /// Encrypts the text using DPAPI (Key provided by local system)
        /// Used to encrypt pass phrases for general encryption routines.
        /// </summary>
        /// <param name="lsText">The text to encrypt</param>
        /// <returns>encrypted text</returns>
        string Protect(string lsText);

        /// <summary>
        /// Decrypts text using DPAPI (Key provided by local system)
        /// Used to decrypt pass phrases for general encryption routines.
        /// </summary>
        /// <param name="lsTextProtected">The encrypted version of text</param>
        /// <returns>The unencrypted version of text</returns>
        string Unprotect(string lsTextProtected);

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="laValue">byte array to be encrypted</param>
        /// <returns>encrypted version of the byte array</returns>
        byte[] Encrypt(byte[] laValue);

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="lsValue">string to be encrypted</param>
        /// <returns>encrypted version of the string</returns>
        string Encrypt(string lsValue);

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="laValue">byte array to be encrypted</param>
        /// <param name="lsPassphrase">Pass phrase used to encrypt the data</param>
        /// <returns>encrypted version of the byte array</returns>
        byte[] Encrypt(byte[] laValue, string lsPassphrase);

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="lsValue">string to be encrypted</param>
        /// <param name="lsPassphrase">Pass phrase used to encrypt the data</param>
        /// <returns>encrypted version of the string</returns>
        string Encrypt(string lsValue, string lsPassphrase);

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="laValue">The data to be encrypted</param>
        /// <returns>Encrypted version of the data</returns>
        byte[] Decrypt(byte[] laValue);

        /// <summary>
        /// Default decryption routine with internal passphrase
        /// </summary>
        /// <param name="lsValue">The string to be decrypted</param>
        /// <returns>Original version of the string</returns>
        string Decrypt(string lsValue);

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="laValue">The data to be encrypted</param>
        /// <param name="lsPassphrase">Pass phrase used to encrypt the data</param>
        /// <returns>Encrypted version of the data</returns>
        byte[] Decrypt(byte[] laValue, string lsPassphrase);

        /// <summary>
        /// Default encryption routine with internal passphrase
        /// </summary>
        /// <param name="lsValue">The data to be encrypted</param>
        /// <param name="lsPassphrase">Pass phrase used to encrypt the data</param>
        /// <returns>Encrypted version of the data</returns>
        string Decrypt(string lsValue, string lsPassphrase);

        /// <summary>
        /// Signs data with a private key to create a unique signature that can be verified with a public key
        /// </summary>
        /// <param name="lsPrivateKey">The private key</param>
        /// <param name="lsData">The data to sign</param>
        /// <returns>Signature that can be verified</returns>
        string SignData(string lsPrivateKey, string lsData);

        /// <summary>
        /// Uses a public key to verify the data matches what was signed with the private key
        /// </summary>
        /// <param name="lsPublicKey">The public key</param>
        /// <param name="lsData">The data to verify</param>
        /// <param name="lsSignature">The signature to use to verify</param>
        /// <returns>True if matching, false if not matching</returns>
        bool VerifyData(string lsPublicKey, string lsData, string lsSignature);

        /// <summary>
        /// Gets a passphrase that is stored
        /// </summary>
        /// <param name="lsName">Name of the passphrase to get</param>
        /// <returns>Clear text version of the passphrase</returns>
        string GetPassphrase(string lsName);

        /// <summary>
        /// Stores a passphrase in memory
        /// </summary>
        /// <param name="lsName">Name to use for the passphrase</param>
        /// <param name="lsValue">The passphrase to store</param>
        void SetPassphrase(string lsName, string lsValue);

        /// <summary>
        /// Sets the application level default passphrase and entropy
        /// </summary>
        /// <param name="lsDefaultPassphrase">Passphrase to use for default encryption when a passphrase is not specified.</param>
        /// <param name="lsDefaultEntropy">Entropy to use for DPAPI encryption (in memory only)</param>
        void SetDefault(string lsDefaultPassphrase, string lsDefaultEntropy);

        /// <summary>
        /// Computes the hash and returns it as a string
        /// </summary>
        /// <param name="lsHash">The Hash type to use</param>
        /// <param name="loValue">The value used to compute the hash</param>
        /// <returns>A unique string for the byte value</returns>		
        string GetHash(string lsHash, object loValue);

        /// <summary>
        /// Gets a random number
        /// </summary>
        /// <param name="lnStart">Start Number</param>
        /// <param name="lnEnd">End Number</param>
        /// <returns>Random number</returns>
        double GetRandomDouble(double lnStart, double lnEnd);

        /// <summary>
        /// Gets a random number
        /// </summary>
        /// <param name="lnStart">Start Number</param>
        /// <param name="lnEnd">End Number</param>
        /// <returns>Random number</returns>
        int GetRandomInt(int lnStart, int lnEnd);
    }
}
