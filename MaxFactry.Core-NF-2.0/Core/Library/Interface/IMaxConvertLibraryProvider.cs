// <copyright file="IMaxConvertLibraryProvider.cs" company="Lakstins Family, LLC">
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
    /// Defines the base interface for interacting with the MaxConvertLibrary
	/// </summary>
	public interface IMaxConvertLibraryProvider : IMaxProvider
	{
		/// <summary>
		/// Deserializes text to an object using the provider associated with the Type.
		/// </summary>
        /// <param name="lsSerializationText">Text that represents the serialized object.</param>
        /// <param name="loTypeExpected">Type of object expected to be deserialized into.</param>
        /// <returns>object version of serialized text</returns>
        object DeserializeObject(string lsSerializationText, Type loTypeExpected);

        /// <summary>
        /// Deserializes text to an object using the provider associated with the Type.
        /// </summary>
        /// <param name="laSerializedData">Data that represents the serialized object.</param>
        /// <param name="loTypeExpected">Type of object expected to be deserialized into.</param>
        /// <returns>object version of serialized text</returns>
        object DeserializeObject(byte[] laSerializedData, Type loTypeExpected);

        /// <summary>
        /// Serializes an object to a byte array using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to serialize.</param>
        /// <returns>byte array to represent object</returns>
        byte[] SerializeObjectToBinary(object loObject);

        /// <summary>
        /// Serializes an object to a string using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to serialize.</param>
        /// <returns>string to represent object</returns>
        string SerializeObjectToString(object loObject);

        /// <summary>
        /// Converts an object to a string using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>string to represent object</returns>
        string ConvertToString(object loObject);

        /// <summary>
        /// Converts an object to a string that can be sorted using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>string to represent object</returns>
        string ConvertToSortString(object loObject);

        /// <summary>
        /// Formats an object to a string
        /// </summary>
        /// <param name="loObject">Object to format.</param>
        /// <param name="lsFormat">Formatting string</param>
        /// <returns>formatted version of the object</returns>
        string Format(object loObject, string lsFormat);

        /// <summary>
        /// Converts an object to a DateTime using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>DateTime version of the object</returns>
        DateTime ConvertToDateTime(object loObject);

        /// <summary>
        /// Converts an object to a DateTime using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>DateTime version of the object</returns>
        DateTime ConvertToDateTimeFromUtc(object loObject);

        /// <summary>
        /// Converts an object to a DateTime using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>DateTime version of the object</returns>
        DateTime ConvertToDateTimeUtc(object loObject);

        /// <summary>
        /// Converts an object to a Guid using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>Guid version of the object</returns>
        Guid ConvertToGuid(object loObject);

        /// <summary>
        /// Converts an object to a Boolean using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>Boolean version of the object</returns>
        bool ConvertToBoolean(object loObject);

        /// <summary>
        /// Converts an object to a Int using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>Int version of the object</returns>
        int ConvertToInt(object loObject);

        /// <summary>
        /// Converts an object to a Double using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>Double version of the object</returns>
        double ConvertToDouble(object loObject);

        /// <summary>
        /// Converts an object to a Long using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>Long version of the object</returns>
        long ConvertToLong(object loObject);

        /// <summary>
        /// Converts an object to a Long using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>Byte Array version of the object</returns>
        byte[] ConvertToByteArray(object loObject);

        /// <summary>
		/// Converts a line of text into an array based on the separator
		/// </summary>
		/// <param name="lsLine">The line of text</param>
		/// <param name="lsSeparator">The separator text</param>
		/// <returns>An array of columns</returns>
        string[] ConvertToColumnList(string lsLine, string lsSeparator);

        /// <summary>
        /// Deserializes text that was serialized using the PHP serialize algorithm.
        /// </summary>
        /// <param name="lsSerializationText">The text to deserialize.</param>
        /// <returns>An object represented by the serialized text.</returns>
        object DeserializeFromPHPString(string lsSerializationText);

        /// <summary>
        /// Serializes an object using the PHP serialize algorithm
        /// </summary>
        /// <param name="loObject">Object to serialize.</param>
        /// <returns>String representing the object.</returns>
        string SerializeToPHPString(object loObject);

        /// <summary>
        /// Parses text and returns an index of content and short codes.
        /// </summary>
        /// <param name="lsText">Text to parse.</param>
        /// <returns>An index of short codes and content.</returns>
        MaxIndex ConvertToShortCodeIndex(string lsText);

        /// <summary>
        /// Converts a guid into a short string
        /// </summary>
        /// <param name="loId">Guid to convert</param>
        /// <returns>22 character string representation of the guid</returns>
        string ConvertGuidToShortString(Guid loId);

        /// <summary>
        /// Converts a short string back into a guid
        /// </summary>
        /// <param name="lsShortString">22 character short string</param>
        /// <returns>Guid that matches the string</returns>
        Guid ConvertShortStringToGuid(string lsShortString);

        /// <summary>
        /// Converts a Guid to Base32
        /// Base32 uses letters and numbers that can be read
        /// <see cref="https://tools.ietf.org/html/rfc4648#section-6"/>
        /// </summary>
        /// <param name="loId">Guid to Convert</param>
        /// <returns>String with numbers and letters from Base32 alphabet</returns>
        string ConvertGuidToBase32(Guid loId);

        /// <summary>
        /// Converts a Base32 string to Guid
        /// Case insensitive
        /// </summary>
        /// <param name="lsBase32">Base32 string to convert</param>
        /// <returns>26 character long string with letters from Base32 alphabet</returns>
        Guid ConvertBase32ToGuid(string lsBase32);

        /// <summary>
        /// Converts a Guid to Base32 using Base32 Hex alphabet
        /// Base32 uses letters and numbers that can be read and they are ordered for sorting in the same order as the
        /// byte array representation of the Guid.
        /// <see cref="https://tools.ietf.org/html/rfc4648#section-7"/>
        /// </summary>
        /// <param name="loId">Guid to Convert</param>
        /// <returns>26 character long string with letters from Base32 hex alphabet</returns>
        string ConvertGuidToBase32Hex(Guid loId);

        /// <summary>
        /// Converts a Base32 hex string to Guid
        /// Case insensitive
        /// </summary>
        /// <param name="lsBase32">Base32 string to convert</param>
        /// <returns>Guid representing the Base32 hex string</returns>
        Guid ConvertBase32HexToGuid(string lsBase32);

        /// <summary>
        /// Converts a byte array into a base 32 string
        /// <see href="https://tools.ietf.org/html/rfc4648#section-6"/>
        /// Not optimized for long data
        /// </summary>
        /// <param name="laData">Data array to convert</param>
        /// <param name="lsAlphabet">Alphabet to use for conversion</param>
        /// <returns>String representation of the data</returns>
        string ConvertByteArrayToBase32(byte[] laData, string lsAlphabet);

        /// <summary>
        /// Converts a Base32 string back into a guid
        /// <see href="https://tools.ietf.org/html/rfc4648#section-6"/>
        /// </summary>
        /// <param name="lsBase32">Base32 string that was a Guid</param>
        /// <param name="lsAlphabet">Alphabet to use for conversion</param>
        /// <returns>Guid that matches the string</returns>
        byte[] ConvertBase32ToByteArray(string lsBase32, string lsAlphabet);

        /// <summary>
        /// Converts a Guid to a series of just Alphanumeric characters using a 32 bit conversion
        /// </summary>
        /// <param name="loId">Guid to convert</param>
        /// <returns>24 character long string of just Alphanumeric characters (case sensitive)</returns>
        string ConvertGuidToAlphabet32(Guid loId);

        /// <summary>
        /// Converts a series of series of just Alphanumeric characters to a Guid using a 32 bit conversion
        /// </summary>
        /// <param name="lsAlphabet32">24 character long string of just Alphanumeric characters (case sensitive)</param>
        /// <returns>Guid based on the characters</returns>
        Guid ConvertAlphabet32ToGuid(string lsAlphabet32);

        /// <summary>
        /// Converts a byte array to just Alphanumeric characters using a 32 bit conversion and the supplied list of characters
        /// </summary>
        /// <param name="laData">byte array of data</param>
        /// <param name="lsAlphabet">string of characters to use for encoding</param>
        /// <returns>string representing the byte array using just the characters in the alphabet</returns>
        string ConvertByteArrayToAlphabet32(byte[] laData, string lsAlphabet);

        /// <summary>
        /// Converts alphanumeric characters to a binary array using the supplied alphabet
        /// </summary>
        /// <param name="lsAlphabet32">string to be encoded</param>
        /// <param name="lsAlphabet">string of characters to use for encoding</param>
        /// <returns>binary array based on the encoding</returns>
        byte[] ConvertAlphabet32ToByteArray(string lsAlphabet32, string lsAlphabet);

        /// <summary>
        /// Converts a Guid to a series of just Alphanumeric characters using a 64 bit conversion
        /// </summary>
        /// <param name="loId">Guid to convert</param>
        /// <returns>22 character long string of just Alphanumeric characters (case sensitive)</returns>
        string ConvertGuidToAlphabet64(Guid loId);

        /// <summary>
        /// Converts a series of series of just Alphanumeric characters to a Guid using a 64 bit conversion
        /// </summary>
        /// <param name="lsAlphabet64">22 character long string of just Alphanumeric characters (case sensitive)</param>
        /// <returns>Guid based on the characters</returns>
        Guid ConvertAlphabet64ToGuid(string lsAlphabet64);

        /// <summary>
        /// Converts a byte array to just Alphanumeric characters using a 64 bit conversion and the supplied list of characters
        /// </summary>
        /// <param name="laData">byte array of data</param>
        /// <param name="lsAlphabet">string of characters to use for encoding</param>
        /// <returns>string representing the byte array using just the characters in the alphabet</returns>
        string ConvertByteArrayToAlphabet64(byte[] laData, string lsAlphabet);

        /// <summary>
        /// Converts alphanumeric characters to a binary array using the supplied alphabet
        /// </summary>
        /// <param name="lsAlphabet64">string to be encoded</param>
        /// <param name="lsAlphabet">string of characters to use for encoding</param>
        /// <returns>binary array based on the encoding</returns>
        byte[] ConvertAlphabet64ToByteArray(string lsAlphabet64, string lsAlphabet);

        string ConvertToSlug(string lsText, int lnMaxLength);

        string[] ConvertCSVToArray(string lsText);
    }
}
