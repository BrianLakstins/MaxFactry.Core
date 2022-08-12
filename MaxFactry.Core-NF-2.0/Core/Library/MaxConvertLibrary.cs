// <copyright file="MaxConvertLibrary.cs" company="Lakstins Family, LLC">
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
// <change date="7/17/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="8/24/2014" author="Brian A. Lakstins" description="Update to inherit from MaxBaseByType.">
// <change date="11/10/2014" author="Brian A. Lakstins" description="Add convert ToByteArray for encrypted data support.">
// <change date="11/27/2014" author="Brian A. Lakstins" description="Add ConvertToColumn method for converting delimitted data.">
// <change date="2/3/2015" author="Brian A. Lakstins" description="Add PHP Serialization support.  Add Short Code index support.">
// <change date="1/10/2016" author="Brian A. Lakstins" description="Add short string representation of Guid.">
// <change date="6/2/2016" author="Brian A. Lakstins" description="Add formatting object to a string.">
// <change date="7/12/2016" author="Brian A. Lakstins" description="Add multiple Guid to string conversions.">
// <change date="8/5/2016" author="Brian A. Lakstins" description="Added conversion to sortable string.">
// </changelog>
#endregion

namespace MaxFactry.Core
{
	using System;
	using System.IO;
	using System.Text;

	/// <summary>
	/// Library to provide conversion functionality
	/// </summary>
	public class MaxConvertLibrary : MaxByMethodFactory
	{
        /// <summary>
        /// Configuration key used to store current time zone.
        /// </summary>
        public const string MaxTimeZoneIdKey = "__MaxTimeZoneId";

        /// <summary>
        /// Internal storage of single object
        /// </summary>
        private static MaxConvertLibrary _oInstance = null;

        /// <summary>
        /// Lock object for multi-threaded access.
        /// </summary>
        private static object _oLock = new object();

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static MaxConvertLibrary Instance
        {
            get
            {
                if (null == _oInstance)
                {
                    lock (_oLock)
                    {
                        if (null == _oInstance)
                        {
                            _oInstance = new MaxConvertLibrary();
                        }
                    }
                }

                return _oInstance;
            }
        }

        /// <summary>
        /// Serializes an object to a string using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loObject">Object to serialize.</param>
        /// <returns>string to represent object</returns>
        public static string SerializeObjectToString(Type loType, object loObject)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.SerializeObjectToString(loObject);
        }

        /// <summary>
        /// Serializes an object to a string using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loObject">Object to serialize.</param>
        /// <returns>string to represent object</returns>
        public static string SerializeObjectToString(object loObject)
        {
            return SerializeObjectToString(typeof(object), loObject);
        }

        /// <summary>
        /// Deserializes text to an object using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="lsSerializationText">Text that represents the serialized object.</param>
        /// <param name="loTypeExpected">Type of object expected to be deserialized into.</param>
        /// <returns>object version of serialized text</returns>
        public static object DeserializeObject(Type loType, string lsSerializationText, Type loTypeExpected)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.DeserializeObject(lsSerializationText, loTypeExpected);
        }

        /// <summary>
        /// Deserializes text to an object using the provider associated with the Type.
        /// </summary>
        /// <param name="lsSerializationText">Text that represents the serialized object.</param>
        /// <param name="loTypeExpected">Type of object expected to be deserialized into.</param>
        /// <returns>object version of serialized text</returns>
        public static object DeserializeObject(string lsSerializationText, Type loTypeExpected)
        {
            return DeserializeObject(typeof(object), lsSerializationText, loTypeExpected);
        }

        /// <summary>
        /// Serializes an object to a byte array using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loObject">Object to serialize.</param>
        /// <returns>byte array to represent object</returns>
        public static byte[] SerializeObjectToBinary(Type loType, object loObject)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.SerializeObjectToBinary(loObject);
        }

        /// <summary>
        /// Serializes an object to a byte array using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to serialize.</param>
        /// <returns>byte array to represent object</returns>
        public static byte[] SerializeObjectToBinary(object loObject)
        {
            return SerializeObjectToBinary(typeof(object), loObject);
        }

        /// <summary>
        /// Deserializes text to an object using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="laSerializedData">Data that represents the serialized object.</param>
        /// <param name="loTypeExpected">Type of object expected to be deserialized into.</param>
        /// <returns>object version of serialized text</returns>
        public static object DeserializeObject(Type loType, byte[] laSerializedData, Type loTypeExpected)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.DeserializeObject(laSerializedData, loTypeExpected);
        }

        /// <summary>
        /// Deserializes text to an object using the provider associated with the Type.
        /// </summary>
        /// <param name="laSerializedData">Data that represents the serialized object.</param>
        /// <param name="loTypeExpected">Type of object expected to be deserialized into.</param>
        /// <returns>object version of serialized text</returns>
        public static object DeserializeObject(byte[] laSerializedData, Type loTypeExpected)
        {
            return DeserializeObject(typeof(object), laSerializedData, loTypeExpected);
        }

        /*
        /// <summary>
        /// Deserializes data to a MaxIndex object using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="laSerializedData">Data that represents the serialized object.</param>
        /// <returns>object version of serialized text</returns>
        public static MaxIndex DeserializeIndex(Type loType, byte[] laSerializedData)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.DeserializeIndex(laSerializedData);
        }

        /// <summary>
        /// Deserializes data to a MaxIndex object using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="lsSerializedData">Data that represents the serialized object.</param>
        /// <returns>object version of serialized text</returns>
        public static MaxIndex DeserializeIndex(Type loType, string lsSerializedData)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.DeserializeIndex(lsSerializedData);
        }

        /// <summary>
        /// Serializes a MaxIndex object to a byte array using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loIndex">Index to serialize.</param>
        /// <returns>byte array to represent object</returns>
        public static byte[] SerializeIndexToBinary(Type loType, MaxIndex loIndex)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.SerializeIndexToBinary(loIndex);
        }

        /// <summary>
        /// Serializes a MaxIndex object to a byte array using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loIndex">Index to serialize.</param>
        /// <returns>JSON serialized string</returns>
        public static string SerializeIndexToString(Type loType, MaxIndex loIndex)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.SerializeIndexToString(loIndex);
        }

        */

        /// <summary>
        /// Deserializes text that was serialized using the PHP serialize algorithm.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="lsSerializationText">The text to deserialize.</param>
        /// <returns>An object represented by the serialized text.</returns>
        public static object DeserializeFromPHPString(Type loType, string lsSerializationText)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.DeserializeFromPHPString(lsSerializationText);
        }

        /// <summary>
        /// Serializes an object using the PHP serialize algorithm
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loObject">Object to serialize.</param>
        /// <returns>String representing the object.</returns>
        public static string SerializeToPHPString(Type loType, object loObject)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.SerializeToPHPString(loObject);
        }

        /// <summary>
        /// Converts an object to a string using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>string to represent object</returns>
        public static string ConvertToString(Type loType, object loObject)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertToString(loObject);
        }

        /// <summary>
        /// Converts an object to a string that can be used for sorting using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>string to represent object</returns>
        public static string ConvertToSortString(Type loType, object loObject)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertToSortString(loObject);
        }

        /// <summary>
        /// Formats an object to a string
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loObject">Object to format.</param>
        /// <param name="lsFormat">Formatting string</param>
        /// <returns>formatted version of the object</returns>
        public static string Format(Type loType, object loObject, string lsFormat)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.Format(loObject, lsFormat);
        }

        /// <summary>
        /// Converts an object to a DateTime using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>DateTime version of the object</returns>
        public static DateTime ConvertToDateTime(Type loType, object loObject)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertToDateTime(loObject);
        }

        /// <summary>
        /// Converts an object to a DateTime using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>DateTime version of the object</returns>
        public static DateTime ConvertToDateTimeFromUtc(Type loType, object loObject)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertToDateTimeFromUtc(loObject);
        }

        /// <summary>
        /// Converts an object to a DateTime using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>DateTime version of the object</returns>
        public static DateTime ConvertToDateTimeUtc(Type loType, object loObject)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertToDateTimeUtc(loObject);
        }

        /// <summary>
        /// Converts an object to a Guid using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>Guid version of the object</returns>
        public static Guid ConvertToGuid(Type loType, object loObject)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertToGuid(loObject);
        }

        /// <summary>
        /// Converts an object to a Boolean using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>Boolean version of the object</returns>
        public static bool ConvertToBoolean(Type loType, object loObject)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertToBoolean(loObject);
        }

        /// <summary>
        /// Converts an object to a Int using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>Int version of the object</returns>
        public static int ConvertToInt(Type loType, object loObject)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertToInt(loObject);
        }

        /// <summary>
        /// Converts an object to a Double using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>Double version of the object</returns>
        public static double ConvertToDouble(Type loType, object loObject)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertToDouble(loObject);
        }

        /// <summary>
        /// Converts an object to a Long using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>Long version of the object</returns>
        public static long ConvertToLong(Type loType, object loObject)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertToLong(loObject);
        }

        /// <summary>
        /// Converts an object to a Long using the provider associated with the Type.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>Byte Array version of the object</returns>
        public static byte[] ConvertToByteArray(Type loType, object loObject)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertToByteArray(loObject);
        }

        /// <summary>
        /// Converts a line of text into an array based on the separator
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="lsLine">The line of text</param>
        /// <param name="lsSeparator">The separator text</param>
        /// <returns>An array of columns</returns>
        public static string[] ConvertToColumnList(Type loType, string lsLine, string lsSeparator)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertToColumnList(lsLine, lsSeparator);
        }

        /// <summary>
        /// Parses text and returns an index of content and short codes.
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="lsText">Text to parse.</param>
        /// <returns>An index of short codes and content.</returns>
        public static MaxIndex ConvertToShortCodeIndex(Type loType, string lsText)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertToShortCodeIndex(lsText);
        }

        /// <summary>
        /// Converts a guid into a short string
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loId">Guid to convert</param>
        /// <returns>22 character string representation of the guid</returns>
        public static string ConvertGuidToShortString(Type loType, Guid loId)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertGuidToShortString(loId);
        }

        /// <summary>
        /// Converts a short string back into a guid
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="lsShortString">22 character short string</param>
        /// <returns>Guid that matches the string</returns>
        public static Guid ConvertShortStringToGuid(Type loType, string lsShortString)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertShortStringToGuid(lsShortString);
        }

        /// <summary>
        /// Converts a Guid to Base32
        /// Base32 uses letters and numbers that can be read
        /// <see cref="https://tools.ietf.org/html/rfc4648#section-6"/>
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loId">Guid to Convert</param>
        /// <returns>String with numbers and letters from Base32 alphabet</returns>
        public static string ConvertGuidToBase32(Type loType, Guid loId)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertGuidToBase32(loId);
        }

        /// <summary>
        /// Converts a Base32 string to Guid
        /// Case insensitive
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="lsBase32">Base32 string to convert</param>
        /// <returns>26 character long string with letters from Base32 alphabet</returns>
        public static Guid ConvertBase32ToGuid(Type loType, string lsBase32)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertBase32ToGuid(lsBase32);
        }

        /// <summary>
        /// Converts a Guid to Base32 using Base32 Hex alphabet
        /// Base32 uses letters and numbers that can be read and they are ordered for sorting in the same order as the
        /// byte array representation of the Guid.
        /// <see cref="https://tools.ietf.org/html/rfc4648#section-7"/>
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loId">Guid to Convert</param>
        /// <returns>26 character long string with letters from Base32 hex alphabet</returns>
        public static string ConvertGuidToBase32Hex(Type loType, Guid loId)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertGuidToBase32Hex(loId);
        }

        /// <summary>
        /// Converts a Base32 hex string to Guid
        /// Case insensitive
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="lsBase32">Base32 string to convert</param>
        /// <returns>Guid representing the Base32 hex string</returns>
        public static Guid ConvertBase32HexToGuid(Type loType, string lsBase32)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertBase32HexToGuid(lsBase32);
        }

        /// <summary>
        /// Converts a byte array into a base 32 string
        /// <see href="https://tools.ietf.org/html/rfc4648#section-6"/>
        /// Not optimized for long data
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="laData">Data array to convert</param>
        /// <param name="lsAlphabet">Alphabet to use for conversion</param>
        /// <returns>String representation of the data</returns>
        public static string ConvertByteArrayToBase32(Type loType, byte[] laData, string lsAlphabet)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertByteArrayToBase32(laData, lsAlphabet);
        }

        /// <summary>
        /// Converts a Base32 string back into a guid
        /// <see href="https://tools.ietf.org/html/rfc4648#section-6"/>
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="lsBase32">Base32 string that was a Guid</param>
        /// <param name="lsAlphabet">Alphabet to use for conversion</param>
        /// <returns>Guid that matches the string</returns>
        public static byte[] ConvertBase32ToByteArray(Type loType, string lsBase32, string lsAlphabet)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertBase32ToByteArray(lsBase32, lsAlphabet);
        }

        /// <summary>
        /// Converts a Guid to a series of just Alphanumeric characters using a 32 bit conversion
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loId">Guid to convert</param>
        /// <returns>24 character long string of just Alphanumeric characters (case sensitive)</returns>
        public static string ConvertGuidToAlphabet32(Type loType, Guid loId)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertGuidToAlphabet32(loId);
        }

        /// <summary>
        /// Converts a series of series of just Alphanumeric characters to a Guid using a 32 bit conversion
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="lsAlphabet32">24 character long string of just Alphanumeric characters (case sensitive)</param>
        /// <returns>Guid based on the characters</returns>
        public static Guid ConvertAlphabet32ToGuid(Type loType, string lsAlphabet32)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertAlphabet32ToGuid(lsAlphabet32);
        }

        /// <summary>
        /// Converts a byte array to just Alphanumeric characters using a 32 bit conversion and the supplied list of characters
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="laData">byte array of data</param>
        /// <param name="lsAlphabet">string of characters to use for encoding</param>
        /// <returns>string representing the byte array using just the characters in the alphabet</returns>
        public static string ConvertByteArrayToAlphabet32(Type loType, byte[] laData, string lsAlphabet)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertByteArrayToAlphabet32(laData, lsAlphabet);
        }

        /// <summary>
        /// Converts alphanumeric characters to a binary array using the supplied alphabet
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="lsAlphabet32">string to be encoded</param>
        /// <param name="lsAlphabet">string of characters to use for encoding</param>
        /// <returns>binary array based on the encoding</returns>
        public static byte[] ConvertAlphabet32ToByteArray(Type loType, string lsAlphabet32, string lsAlphabet)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertAlphabet32ToByteArray(lsAlphabet32, lsAlphabet);
        }

        /// <summary>
        /// Converts a Guid to a series of just Alphanumeric characters using a 64 bit conversion
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="loId">Guid to convert</param>
        /// <returns>22 character long string of just Alphanumeric characters (case sensitive)</returns>
        public static string ConvertGuidToAlphabet64(Type loType, Guid loId)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertGuidToAlphabet64(loId);
        }

        /// <summary>
        /// Converts a series of series of just Alphanumeric characters to a Guid using a 64 bit conversion
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="lsAlphabet64">22 character long string of just Alphanumeric characters (case sensitive)</param>
        /// <returns>Guid based on the characters</returns>
        public static Guid ConvertAlphabet64ToGuid(Type loType, string lsAlphabet64)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertAlphabet64ToGuid(lsAlphabet64);
        }

        /// <summary>
        /// Converts a byte array to just Alphanumeric characters using a 64 bit conversion and the supplied list of characters
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="laData">byte array of data</param>
        /// <param name="lsAlphabet">string of characters to use for encoding</param>
        /// <returns>string representing the byte array using just the characters in the alphabet</returns>
        public static string ConvertByteArrayToAlphabet64(Type loType, byte[] laData, string lsAlphabet)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertByteArrayToAlphabet64(laData, lsAlphabet);
        }

        public static string ConvertToSlug(Type loType, string lsText, int lnMaxLength)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertToSlug(lsText, lnMaxLength);
        }

        public static string ConvertToSlug(string lsText, int lnMaxLength)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(typeof(object));
            return loProvider.ConvertToSlug(lsText, lnMaxLength);
        }

        public static string[] ConvertCSVToArray(string lsText)
        {
            return ConvertCSVToArray(typeof(object), lsText);
        }

        public static string[] ConvertCSVToArray(Type loType, string lsText)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertCSVToArray(lsText);
        }

        /// <summary>
        /// Converts alphanumeric characters to a binary array using the supplied alphabet
        /// </summary>
        /// <param name="loType">Type of object used to determine provider</param>
        /// <param name="lsAlphabet64">string to be encoded</param>
        /// <param name="lsAlphabet">string of characters to use for encoding</param>
        /// <returns>binary array based on the encoding</returns>
        public static byte[] ConvertAlphabet64ToByteArray(Type loType, string lsAlphabet64, string lsAlphabet)
        {
            IMaxConvertLibraryProvider loProvider = Instance.GetConvertLibraryProvider(loType);
            return loProvider.ConvertAlphabet64ToByteArray(lsAlphabet64, lsAlphabet);
        }

        /// <summary>
        /// Gets a singleton provider based on the type.  Returns MaxConvertLibraryProvider if none configured.
        /// </summary>
        /// <param name="loType">Type to use to get the provider.</param>
        /// <returns>Provider that implements IMaxConvertLibraryProvider.</returns>
        public IMaxConvertLibraryProvider GetConvertLibraryProvider(Type loType)
        {
            IMaxConvertLibraryProvider loR = this.GetProvider(loType) as IMaxConvertLibraryProvider;
            if (null == loR)
            {
                loR = MaxFactryLibrary.CreateSingletonProvider(typeof(MaxFactry.Core.Provider.MaxConvertLibraryDefaultProvider)) as IMaxConvertLibraryProvider;
            }

            if (null == loR)
            {
                throw new MaxException("Error casting [" + loR.GetType() + "] for Provider");
            }

            return loR;
        }

        /// <summary>
        /// Handles up to 20 element array.  Anything longer has more than 2.432902x10^18 permutations.  Max ULong is 1.84x10^19
        /// https://en.wikipedia.org/wiki/Heap%27s_algorithm
        /// </summary>
        /// <param name="laSource"></param>
        /// <param name="lnPermutation">There are laSource.length! permutations available.  Permution count is after so many swaps.</param>
        /// <returns></returns>
        /*
        public static object[] GetPermutation(object[] laSource, ulong lnPermutation)
        {
            if (laSource.Length > 20)
            {
                throw new MaxException("Source cannot be more than 20 elements");
            }

            ulong lnLength = Convert.ToUInt64(laSource.Length);
            ulong lnPermutationMax = lnLength;
            for (ulong lnP = 2; lnP < lnLength; lnP++)
            {
                lnPermutationMax = lnPermutationMax * lnP;
            }

            if (lnPermutation >= lnPermutationMax)
            {
                return null;
            }

            object[] laR = new object[laSource.Length];
            Array.Copy(laSource, 0, laR, 0, laSource.Length);

            if (lnPermutation == 0)
            {
                return laR;
            }

            int[] laCount = new int[laSource.Length];
            for (int lnC = 0; lnC < laCount.Length; lnC++)
            {
                laCount[lnC] = 0;
            }

            ulong lnSwap = 0;
            for (int lnLoop = 0; lnLoop < laSource.Length; )
            {
                if (laCount[lnLoop] < lnLoop)
                {
                    if (lnLoop % 2 == 0) //even
                    {
                        object loTemp = laR[0];
                        laR[0] = laR[lnLoop];
                        laR[lnLoop] = loTemp;
                    }
                    else // odd
                    {
                        object loTemp = laR[lnLoop];
                        laR[lnLoop] = laR[laCount[lnLoop]];
                        laR[laCount[lnLoop]] = loTemp;
                    }

                    lnSwap++;
                    if (lnPermutation == lnSwap)
                    {
                        return laR;
                    }

                    laCount[lnLoop]++;
                    lnLoop = 1;
                }
                else
                {
                    laCount[lnLoop] = 0;
                    lnLoop++;
                }
            }

            return laR;
        }
        */
	}
}