// <copyright file="MaxConvertLibraryDefaultProvider.cs" company="Lakstins Family, LLC">
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
// <change date="7/18/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="8/24/2014" author="Brian A. Lakstins" description="Update to inherit from MaxProviderBase.">
// <change date="9/26/2014" author="Brian A. Lakstins" description="Update to handle doubles that are too large or small without throwing an exception.">
// <change date="10/17/2014" author="Brian A. Lakstins" description="Updated int string parsing to require a string longer than 0 characters of non whitespace">
// <change date="10/22/2014" author="Brian A. Lakstins" description="Creating new MaxExceptions instead of handling generic Exceptions.">
// <change date="11/10/2014" author="Brian A. Lakstins" description="Add convert ToByteArray for encrypted data support.">
// <change date="11/27/2014" author="Brian A. Lakstins" description="Add ConvertToColumn method for converting delimitted data.">
// <change date="2/3/2015" author="Brian A. Lakstins" description="Add PHP Serialization support.  Add Short Code index support.">
// <change date="2/24/2015" author="Brian A. Lakstins" description="Updated short code integration based on WordPress short code documentation.">
// <change date="6/1/2015" author="Brian A. Lakstins" description="Updated to work with Micro Framework.">
// <change date="1/10/2016" author="Brian A. Lakstins" description="Updated to meet .NET Micro Framework restrictions.">
// <change date="1/10/2016" author="Brian A. Lakstins" description="Add short string representation of Guid.">
// <change date="3/8/2016" author="Brian A. Lakstins" description="Update handling of converting double to int.">
// <change date="3/21/2016" author="Brian A. Lakstins" description="Update handling of converting string to int.">
// <change date="6/2/2016" author="Brian A. Lakstins" description="Add formatting object to a string.">
// <change date="7/12/2016" author="Brian A. Lakstins" description="Add multiple Guid to string conversions.">
// <change date="8/5/2016" author="Brian A. Lakstins" description="Added conversion to sortable string.">
// <change date="8/11/2016" author="Brian A. Lakstins" description="Added creating sortable string from int or double.">
// <change date="8/29/2016" author="Brian A. Lakstins" description="Improve number checking.">
// <change date="4/20/2017" author="Brian A. Lakstins" description="Make copy of MaxIndex when serializing so that the original is not changed.  Fix object checking functionality.">
// <change date="4/30/2017" author="Brian A. Lakstins" description="Fix issue with checking object type when serializing.">
// <change date="5/24/2017" author="Brian A. Lakstins" description="Update double parsing.">
// <change date="6/13/2017" author="Brian A. Lakstins" description="Updated for changed to MaxIndex">
// <change date="6/13/2017" author="Brian A. Lakstins" description="Updated to be easier to read when dealing with deserializing a MaxIndex">
// <change date="10/24/2017" author="Brian A. Lakstins" description="Updated for nesting MaxIndex objects">
// <change date="5/23/2018" author="Brian A. Lakstins" description="Add support for sorting long type">
// <change date="7/3/2019" author="Brian A. Lakstins" description="Updated exception logging to use structured logging.">
// <change date="1/20/2021" author="Brian A. Lakstins" description="Updated object checking to not include types only with default deserialization types.">
// <change date="7/29/2024" author="Brian A. Lakstins" description="Change default string for date conversion to ISO format.">
// </changelog>
#endregion

namespace MaxFactry.Core.Provider
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Defines the default class for implementing the MaxConvertLibrary functionality
    /// </summary>
    public class MaxConvertLibraryDefaultProvider : MaxProvider, IMaxConvertLibraryProvider
    {
        /// <summary>
        /// All letters (upper and lower) and numbers
        /// </summary>
        private readonly string _sAlphabetAlphanumeric = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        /// <summary>
        /// Capital letters and numbers avoiding conflict with 1 and 0
        /// </summary>
        private readonly string _sAlphabetBase32 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567=";

        /// <summary>
        /// Capital letters and numbers in sortable order
        /// </summary>
        private readonly string _sAlphabetBase32Hex = "0123456789ABCDEFGHIJKLMNOPQRSTUV=";

        /// <summary>
        /// Valid hex based number characters.
        /// </summary>
        private readonly string _sValidHex = "0123456789ABCDEF";

        /// <summary>
        /// Valid white space characters.
        /// </summary>
        private readonly string _sWhiteSpace = " \r\n\t";

        /// <summary>
        /// Valid short code name characters.
        /// </summary>
        private readonly string _sShortCodeNameValid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-_0123456789";

        /// <summary>
        /// Valid number characters.
        /// </summary>
        private readonly string _sValidNumber = "0123456789";

        /// <summary>
        /// Suffix for items in MaxIndex collection to set type of object that was serialized.
        /// this.ConvertGuidToShortString(new Guid("0f052bf5-808a-412f-a71f-5258f586c03b"));
        /// </summary>
        public readonly string _sSerialTypeKeySuffix = "9SsFD4qAL0GnH1JY9YbAOw";

        /// <summary>
        /// Initializes a new instance of the MaxConvertLibraryDefaultProvider class.
        /// </summary>
        public MaxConvertLibraryDefaultProvider()
        {
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="lsName">Name of the provider.</param>
        /// <param name="loConfig">Configuration information.</param>
        public override void Initialize(string lsName, MaxIndex loConfig)
        {
            base.Initialize(lsName, loConfig);
        }

        /// <summary>
        /// Serializes an object to a string using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to serialize.</param>
        /// <returns>string to represent object</returns>
        public virtual string SerializeObjectToString(object loObject)
        {
            string lsR = null;
            if (loObject != null)
            {
                lsR = this.SerializeToString(loObject);
            }

            return lsR;
        }

        /// <summary>
        /// Serializes an object to a string using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to serialize.</param>
        /// <returns>string to represent object</returns>
        public virtual string SerializeToString(object loObject)
        {
            loObject = this.AddValueType(loObject);
            return this.SerializeToStringConditional(loObject);
        }

        /// <summary>
        /// Serializes an object to a byte array using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to serialize.</param>
        /// <returns>byte array to represent object</returns>
        public virtual byte[] SerializeObjectToBinary(object loObject)
        {
            byte[] loR = null;
            if (loObject != null)
            {
                string lsSerializedText = this.SerializeToString(loObject);
                loR = System.Text.Encoding.UTF8.GetBytes(lsSerializedText);
            }

            return loR;
        }

        /// <summary>
        /// Deserializes text to an object using the provider associated with the Type.
        /// </summary>
        /// <param name="lsSerializationText">Text that represents the serialized object.</param>
        /// <param name="loTypeExpected">Type of object expected to be deserialized into.</param>
        /// <returns>object version of serialized text</returns>
        public virtual object DeserializeObject(string lsSerializationText, Type loTypeExpected)
        {
            object loReturn = this.DeserializeFromString(lsSerializationText, loTypeExpected);
            return loReturn;
        }

        /// <summary>
        /// Deserializes text to an object using the provider associated with the Type.
        /// </summary>
        /// <param name="lsSerializationText">Text that represents the serialized object.</param>
        /// <param name="loTypeExpected">Type of object expected to be deserialized into.</param>
        /// <returns>object version of serialized text</returns>
        public virtual object DeserializeFromString(string lsSerializationText, Type loTypeExpected)
        {
            return this.DeserializeFromStringConditional(lsSerializationText, loTypeExpected);
        }
        /// <summary>
        /// Deserializes text to an object using the provider associated with the Type.
        /// </summary>
        /// <param name="laSerializedData">Data that represents the serialized object.</param>
        /// <param name="loTypeExpected">Type of object expected to be deserialized into.</param>
        /// <returns>object version of serialized text</returns>
        public virtual object DeserializeObject(byte[] laSerializedData, Type loTypeExpected)
        {
            object loR = null;
            if (null != laSerializedData)
            {
                string lsSerializedText = new string(System.Text.Encoding.UTF8.GetChars(laSerializedData));
                loR = this.DeserializeFromString(lsSerializedText, loTypeExpected);
            }

            return loR;
        }


        public virtual string SerializeToJson(object loObject)
        {
            loObject = this.AddValueType(loObject);
            return this.SerializeToJsonConditional(loObject);
        }

        public virtual object DeserializeFromJson(string lsJson, Type loExpectedType)
        {
            return this.DeserializeFromJsonConditional(lsJson, loExpectedType);
        }

        public virtual string SerializeToXml(object loObject)
        {
            loObject = this.AddValueType(loObject);
            return this.SerializeToXmlConditional(loObject);
        }

        public virtual object DeserializeFromXml(string lsXml, Type loExpectedType)
        {
            return this.DeserializeFromXmlConditional(lsXml, loExpectedType);
        }

        /// <summary>
        /// Serializes an object using the PHP serialize algorithm
        /// </summary>
        /// <param name="loObject">Object to serialize.</param>
        /// <returns>String representing the object.</returns>
        public virtual string SerializeToPHPString(object loObject)
        {
            return this.SerializeToPHPString(loObject, new System.Text.UTF8Encoding());
        }

        /// <summary>
        /// Deserializes text that was serialized using the PHP serialize algorithm.
        /// </summary>
        /// <param name="lsSerializationText">The text to deserialize.</param>
        /// <returns>An object represented by the serialized text.</returns>
        public virtual object DeserializeFromPHPString(string lsSerializationText)
        {
            int lnLength = 0;
            object loObject = this.DeserializeFromPHPString(lsSerializationText, 0, new System.Text.UTF8Encoding(), out lnLength);
            if (lnLength.Equals(lsSerializationText.Length))
            {
                return loObject;
            }

            throw new MaxException("Length did not match");
        }

        /// <summary>
        /// Converts an object to a string
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>string to represent object</returns>
        public virtual string ConvertToString(object loObject)
        {
            string lsR = string.Empty;
            if (null != loObject)
            {
                if (loObject is string)
                {
                    lsR = (string)loObject;
                }
                else if (loObject is byte[])
                {
                    char[] laChar = System.Text.UTF8Encoding.UTF8.GetChars((byte[])loObject);
                    lsR = new string(laChar);
                }
                else if (loObject is DateTime)
                {
                    //// Use same format as javascript date .toISOString()
                    lsR = ((DateTime)loObject).ToString("o", CultureInfo.InvariantCulture);
                }
                else
                {
                    lsR = loObject.ToString();
                }
            }

            return lsR;
        }

        /// <summary>
        /// Converts an object to a string that can be sorted using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>string to represent object</returns>
        public virtual string ConvertToSortString(object loObject)
        {
            string lsR = string.Empty;
            if (null != loObject)
            {
                if (loObject is string)
                {
                    lsR = ((string)loObject).ToLower();
                }
                else if (loObject is byte[])
                {
                    char[] laChar = System.Text.UTF8Encoding.UTF8.GetChars((byte[])loObject);
                    lsR = new string(laChar);
                }
                else if (loObject is DateTime)
                {
                    lsR = ((DateTime)loObject).ToString("s") + ((DateTime)loObject).Millisecond.ToString("D4");
                }
                else if (loObject is int)
                {
                    int lnInt = (int)loObject;
                    int lnLen = int.MaxValue.ToString().Length;
                    string lsIntBase = ((int)loObject).ToString();
                    string lsInt = string.Empty;
                    for (int lnChar = 0; lnChar < lsIntBase.Length; lnChar++)
                    {
                        if (this._sValidNumber.IndexOf(lsIntBase[lnChar].ToString()) >= 0)
                        {
                            int lnNum = int.Parse(lsIntBase[lnChar].ToString());
                            if (lnInt < 0)
                            {
                                //// Reverse for negative number
                                lsInt += (9 - lnNum).ToString();
                            }
                            else
                            {
                                lsInt += lnNum.ToString();
                            }
                        }
                    }

                    while (lsInt.Length < lnLen)
                    {
                        lsInt = "0" + lsInt;
                    }

                    if (lnInt >= 0)
                    {
                        lsR = "0" + lsInt;
                    }
                    else
                    {
                        lsR = "_" + lsInt;
                    }
                }
                else if (loObject is long)
                {
                    long lnLong = (long)loObject;
                    long lnLen = long.MaxValue.ToString().Length;
                    string lsLongBase = ((long)loObject).ToString();
                    string lsLong = string.Empty;
                    for (int lnChar = 0; lnChar < lsLongBase.Length; lnChar++)
                    {
                        if (this._sValidNumber.IndexOf(lsLongBase[lnChar].ToString()) >= 0)
                        {
                            int lnNum = int.Parse(lsLongBase[lnChar].ToString());
                            if (lnLong < 0)
                            {
                                //// Reverse for negative number
                                lsLong += (9 - lnNum).ToString();
                            }
                            else
                            {
                                lsLong += lnNum.ToString();
                            }
                        }
                    }

                    while (lsLong.Length < lnLen)
                    {
                        lsLong = "0" + lsLong;
                    }

                    if (lnLong >= 0)
                    {
                        lsR = "0" + lsLong;
                    }
                    else
                    {
                        lsR = "_" + lsLong;
                    }
                }
                else if (loObject is double)
                {
                    double lnDouble = (double)loObject;
                    //// Use e notation for sorting given that there could be 53 digits in the number before the e.
                    string lsE = lnDouble.ToString("e53");
                    string[] laE = lsE.Split('e');
                    string lsEDouble = string.Empty;
                    for (int lnChar = 0; lnChar < laE[0].Length; lnChar++)
                    {
                        if (this._sValidNumber.IndexOf(laE[0][lnChar].ToString()) >= 0)
                        {
                            int lnNum = int.Parse(laE[0][lnChar].ToString());
                            if (lnDouble < 0)
                            {
                                //// Reverse for negative number
                                lsEDouble += (9 - lnNum).ToString();
                            }
                            else
                            {
                                lsEDouble += lnNum.ToString();
                            }
                        }
                    }

                    while (lsEDouble.Length < 60)
                    {
                        lsEDouble += 0;
                    }

                    int lnExponent = int.Parse(laE[1]);
                    //// Handle negative exponents so they sort with positive
                    lnExponent += 500;
                    if (lnDouble < 0)
                    {
                        //// Reverse for negative number
                        lnExponent = 1000 - lnExponent;
                    }

                    string lsExponent = lnExponent.ToString();
                    while (lsExponent.Length < 5)
                    {
                        lsExponent = 0 + lsExponent;
                    }

                    if (lnDouble >= 0)
                    {
                        lsR = "0" + lsExponent + lsEDouble;
                    }
                    else
                    {
                        lsR = "_" + lsExponent + lsEDouble;
                    }
                }
                else
                {
                    lsR = loObject.ToString();
                }
            }

            lsR += "\n";

            return lsR;
        }

        /// <summary>
        /// Formats an object to a string
        /// </summary>
        /// <param name="loObject">Object to format.</param>
        /// <param name="lsFormat">Formatting string</param>
        /// <returns>formatted version of the object</returns>
        public virtual string Format(object loObject, string lsFormat)
        {
            string lsR = string.Empty;
            if (loObject is DateTime)
            {
                if ((DateTime)loObject > DateTime.MinValue)
                {
                    lsR = ((DateTime)loObject).ToString(lsFormat);
                }
            }
            else if (loObject is double)
            {
                if ((double)loObject > double.MinValue)
                {
                    lsR = ((double)loObject).ToString(lsFormat);
                }
            }
            else if (loObject is int)
            {
                if ((int)loObject > int.MinValue)
                {
                    lsR = ((int)loObject).ToString(lsFormat);
                }
            }
            else if (loObject is long)
            {
                if ((long)loObject > long.MinValue)
                {
                    lsR = ((long)loObject).ToString(lsFormat);
                }
            }
            else if (null != loObject)
            {
                lsR = loObject.ToString();
            }

            return lsR;
        }

        /// <summary>
        /// Converts an object to a DateTime using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>DateTime version of the object</returns>
        public virtual DateTime ConvertToDateTime(object loObject)
        {
            DateTime ldR = DateTime.MinValue;
            if (null != loObject)
            {
                if (loObject is DateTime)
                {
                    ldR = (DateTime)loObject;
                }
                else if (loObject is long)
                {
                    ldR = new DateTime((long)loObject);
                }
                else if (loObject is string)
                {
                    if (!DateTime.TryParse(loObject as string, out ldR))
                    {
                        //// Try parsing as milliseconds from 1/1/1970.
                        bool lbIsNumber = true;
                        foreach (char loChar in (string)loObject)
                        {
                            if (!char.IsNumber(loChar))
                            {
                                lbIsNumber = false;
                            }
                        }

                        if (lbIsNumber)
                        {
                            double lnNumber = double.MinValue;
                            if (double.TryParse((string)loObject, out lnNumber))
                            {
                                ldR = new DateTime(1970, 1, 1).AddMilliseconds(lnNumber);
                                ldR = DateTime.SpecifyKind(ldR, DateTimeKind.Utc);
                            }
                        }
                    }
                }
            }

            if (DateTime.MinValue != ldR && ldR.Kind == DateTimeKind.Unspecified)
            {
                //// Convert to time zone specified in configuration
                object loTimeZoneId = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeProcess, MaxConvertLibrary.MaxTimeZoneIdKey);
                if (null == loTimeZoneId || (loTimeZoneId is string && string.IsNullOrEmpty((string)loTimeZoneId)))
                {
                    loTimeZoneId = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeAny, MaxConvertLibrary.MaxTimeZoneIdKey);
                }

                if (null != loTimeZoneId && loTimeZoneId is string)
                {
                    string lsTimeZoneId = (string)loTimeZoneId;
#if net4_52 || netcore1
                    TimeZoneInfo loTimeZone = TimeZoneInfo.FindSystemTimeZoneById(lsTimeZoneId);
                    if (null != loTimeZone)
                    {
                        ldR = TimeZoneInfo.ConvertTimeToUtc(ldR, loTimeZone);
                    }
#endif
                }
            }

            return ldR;
        }

        /// <summary>
        /// Converts an object to a DateTime using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>DateTime version of the object</returns>
        public virtual DateTime ConvertToDateTimeFromUtc(object loObject)
        {
            DateTime ldR = this.ConvertToDateTime(loObject);
            if (DateTime.MinValue != ldR && ldR.Kind == DateTimeKind.Utc)
            {
                //// Convert to time zone specified in configuration
                object loTimeZoneId = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeProcess, MaxConvertLibrary.MaxTimeZoneIdKey);
                if (null == loTimeZoneId || (loTimeZoneId is string && string.IsNullOrEmpty((string)loTimeZoneId)))
                {
                    loTimeZoneId = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeAny, MaxConvertLibrary.MaxTimeZoneIdKey);
                }

                if (null != loTimeZoneId && loTimeZoneId is string)
                {
                    string lsTimeZoneId = (string)loTimeZoneId;
#if net4_52 || netcore1
                    TimeZoneInfo loTimeZone = TimeZoneInfo.FindSystemTimeZoneById(lsTimeZoneId);
                    if (null != loTimeZone)
                    {
                        ldR = TimeZoneInfo.ConvertTime(ldR, loTimeZone);
                    }
#endif
                }

                if (ldR.Kind == DateTimeKind.Utc)
                {
                    ldR = ldR.ToLocalTime();
                }
            }


            return ldR;
        }

        /// <summary>
        /// Converts an object to a DateTime using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>DateTime version of the object</returns>
        public virtual DateTime ConvertToDateTimeUtc(object loObject)
        {
            DateTime ldR = this.ConvertToDateTime(loObject);
            if (DateTime.MinValue != ldR)
            {
                ldR = ldR.ToUniversalTime();
            }

            return ldR;
        }

        /// <summary>
        /// Converts an object to a Guid using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>Guid version of the object</returns>
        public virtual Guid ConvertToGuid(object loObject)
        {
            if (null != loObject)
            {
                if (loObject is Guid)
                {
                    return (Guid)loObject;
                }
                else if (loObject is byte[])
                {
                    return new Guid((byte[])loObject);
                }
                else if (loObject is long)
                {
                    byte[] laGuid = new byte[16];
                    byte[] laVal = BitConverter.GetBytes((long)loObject);
                    for (int lnVal = 0; lnVal < 16; lnVal++)
                    {
                        if (lnVal < laVal.Length)
                        {
                            laGuid[lnVal] = laVal[lnVal];
                        }
                        else
                        {
                            laGuid[lnVal] = 1;
                        }
                    }

                    return new Guid(laGuid);
                }
                else if (loObject is int)
                {
                    byte[] laGuid = new byte[16];
                    byte[] laVal = BitConverter.GetBytes((int)loObject);
                    for (int lnVal = 0; lnVal < 16; lnVal++)
                    {
                        if (lnVal < laVal.Length)
                        {
                            laGuid[lnVal] = laVal[lnVal];
                        }
                        else
                        {
                            laGuid[lnVal] = 2;
                        }
                    }

                    return new Guid(laGuid);
                }
                else if (null != loObject)
                {
                    try
                    {
                        string lsObject = loObject.ToString();
                        if (lsObject.Length >= 32)
                        {
                            //// Attempt to decode from 16 hex numbers which may contain dashes or other non-hex characters
                            byte[] laGuid = this.ConvertGuidStringtoByte(lsObject);
                            if (null != laGuid)
                            {
                                return new Guid(laGuid);
                            }
                        }
                        else if (lsObject.Length == 26)
                        {
                            return this.ConvertBase32ToGuid(lsObject);
                        }
                        else if (lsObject.Length == 24)
                        {
                            return this.ConvertAlphabet32ToGuid(lsObject);
                        }
                        else if (lsObject.Length == 22)
                        {
                            try
                            {
                                return this.ConvertAlphabet64ToGuid(lsObject);
                            }
                            catch
                            {
                                return this.ConvertShortStringToGuid(lsObject);
                            }
                        }
                    }
                    catch (Exception loE)
                    {
                        MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Guid {Object} could not be parsed.", loE, loObject));
                    }
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Converts an object to a Boolean using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>Boolean version of the object</returns>
        public virtual bool ConvertToBoolean(object loObject)
        {
            if (null != loObject)
            {
                if (loObject is bool)
                {
                    return (bool)loObject;
                }
                else if (loObject is byte)
                {
                    if ((byte)loObject == 1)
                    {
                        return true;
                    }
                }
                else if (loObject is short)
                {
                    if ((short)loObject == 1)
                    {
                        return true;
                    }
                }
                else if (loObject is int)
                {
                    if ((int)loObject == 1)
                    {
                        return true;
                    }
                }
                else if (loObject is long)
                {
                    if ((long)loObject == 1)
                    {
                        return true;
                    }
                }
                else if (loObject is string)
                {
                    if (((string)loObject).ToLower().IndexOf('t') > -1)
                    {
                        return true;
                    }
                    else if (((string)loObject).ToLower().IndexOf('y') > -1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Converts an object to a Int using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>Int version of the object</returns>
        public virtual int ConvertToInt(object loObject)
        {
            if (null != loObject)
            {
                if (loObject is int)
                {
                    return (int)loObject;
                }
                else if (loObject is string && ((string)loObject).Trim().Length > 0)
                {
                    string lsValue = ((string)loObject).Trim();
                    if (lsValue.IndexOf(".") > 0)
                    {
                        lsValue = lsValue.Substring(0, lsValue.IndexOf("."));
                    }

                    try
                    {
                        return int.Parse(lsValue);
                    }
                    catch (Exception loE)
                    {
                        MaxException loMaxException = new MaxException("Error in ConvertToInt for [" + (string)loObject + "]", loE);
                        return int.MinValue;
                    }
                }
                else if (loObject is Guid)
                {
                    byte[] laInt = new byte[4];
                    byte[] laVal = ((Guid)loObject).ToByteArray();
                    for (int lnVal = 0; lnVal < 16; lnVal++)
                    {
                        if (lnVal < laInt.Length)
                        {
                            laInt[lnVal] = laVal[lnVal];
                        }
                    }

                    return BitConverter.ToInt32(laInt, 0);
                }
                else if (loObject is double)
                {
                    if ((double)loObject == double.MinValue)
                    {
                        return int.MinValue;
                    }

                    try
                    {
                        string lsNum = ((double)loObject).ToString("F");
                        if (lsNum.IndexOf('.') > 0)
                        {
                            return int.Parse(lsNum.Split(new char[] { '.' })[0]);
                        }

                        return int.Parse(lsNum);
                    }
                    catch (Exception loE)
                    {
                        MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error converting double {Object} to int", loE, loObject));
                    }
                }
                else
                {
                    string lsNum = loObject.ToString().Trim();
                    if (lsNum.Length > 0)
                    {
                        try
                        {
                            return int.Parse(lsNum);
                        }
                        catch (Exception loE)
                        {
                            MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error converting string {Object} to int", loE, loObject));
                        }
                    }
                }
            }

            return int.MinValue;
        }

        /// <summary>
        /// Converts an object to a Double using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>Double version of the object</returns>
        public virtual double ConvertToDouble(object loObject)
        {
            if (null != loObject)
            {
                if (loObject is double)
                {
                    return (double)loObject;
                }
                else if (loObject is byte || loObject is short || loObject is int || loObject is long || loObject is float)
                {
                    return Convert.ToDouble(loObject.ToString());
                }
                else if (loObject is string && ((string)loObject).Trim().Length > 0)
                {
                    string lsNumber = (string)loObject;
                    string lsDouble = string.Empty;
                    try
                    {
                        //// Remove everything except numbers, periods, and E
                        int lnPeriodCount = 0;
                        int lnECount = 0;
                        int lnPlusMinusCount = 0;
                        bool lbHasNumber = false;
                        foreach (char loChar in lsNumber.ToCharArray())
                        {
                            if ((this._sValidNumber.IndexOf(loChar) >= 0))
                            {
                                lsDouble += loChar;
                                lbHasNumber = true;
                            }
                            else if (loChar == '.')
                            {
                                lsDouble += loChar;
                                lnPeriodCount++;
                            }
                            else if (loChar == 'E')
                            {
                                lsDouble += loChar;
                                lnECount++;
                            }
                            else if (loChar == '-' || loChar == '+')
                            {
                                lsDouble += loChar;
                                lnPlusMinusCount++;
                            }
                        }

                        if (lbHasNumber)
                        {
                            if (lnPeriodCount <= 1 && lnECount == 0 && lnPlusMinusCount <= 1)
                            {
                                return double.Parse(lsDouble);
                            }

                            if (lsDouble.IndexOf("E") > 0 && lnECount == 1 && lsDouble.IndexOf("E") < lsDouble.Length - 1)
                            {
                                // Handle "-1.79769313486232E+308" coming from Lucene and any other too large or small numbers.
                                string[] laExponentialNotation = lsDouble.Split('E');
                                double lnDecimal = double.Parse(laExponentialNotation[0]);
                                int lnExponent = int.Parse(laExponentialNotation[1].Substring(1));
                                if (laExponentialNotation[0].Equals('-'))
                                {
                                    lnExponent = -1 * lnExponent;
                                }

                                if (lnDecimal < -1.7976931348623157 && lnExponent >= 308)
                                {
                                    // MinValue = "-1.7976931348623157E+308"
                                    return double.MinValue;
                                }
                                else if (lnDecimal > 1.7976931348623157 && lnExponent >= 308)
                                {
                                    // MaxValue = "1.7976931348623157E+308"
                                    return double.MaxValue;
                                }
                                else if (lnExponent < -308)
                                {
                                    // -308 expontent is hard to tell from zero
                                    return 0;
                                }
                                else if (lnExponent == -308)
                                {
                                    // return the smallest negative Double value for anything really small
                                    if (lnDecimal < 0)
                                    {
                                        return -1 * double.Epsilon;
                                    }

                                    return double.Epsilon;
                                }
                                else
                                {
                                    return lnDecimal * Math.Pow(10, lnExponent);
                                }
                            }
                        }
                    }
                    catch (Exception loE)
                    {
                        if (lsDouble.Length > 0)
                        {
                            MaxException loMaxException = new MaxException("Error converting string to double in ConvertToDouble [" + lsNumber + "][" + lsDouble + "]", loE);
                        }
                    }
                }
            }

            return double.MinValue;
        }

        /// <summary>
        /// Converts an object to a Long using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>Long version of the object</returns>
        public virtual long ConvertToLong(object loObject)
        {
            if (null != loObject)
            {
                if (loObject is long)
                {
                    return (long)loObject;
                }
                else if (loObject is string && ((string)loObject).Trim().Length > 0)
                {
                    try
                    {
                        return long.Parse((string)loObject);
                    }
                    catch (Exception loE)
                    {
                        MaxException loMaxException = new MaxException("Error in ConvertToLong [" + (string)loObject + "]", loE);
                        return long.MinValue;
                    }
                }
                else if (loObject is Guid)
                {
                    byte[] laLong = new byte[8];
                    byte[] laVal = ((Guid)loObject).ToByteArray();
                    for (int lnVal = 0; lnVal < 16; lnVal++)
                    {
                        if (lnVal < laLong.Length)
                        {
                            laLong[lnVal] = laVal[lnVal];
                        }
                    }

                    return BitConverter.ToInt64(laLong, 0);
                }
                else
                {
                    string lsNum = loObject.ToString().Trim();
                    if (lsNum.Length > 0)
                    {
                        try
                        {
                            return long.Parse(lsNum);
                        }
                        catch (Exception loE)
                        {
                            throw new MaxException("Error in ConvertToInt [" + loObject.ToString() + "]", loE);
                        }
                    }
                }
            }

            return long.MinValue;
        }

        /// <summary>
        /// Converts an object to a Long using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to convert.</param>
        /// <returns>Byte Array version of the object</returns>
        public virtual byte[] ConvertToByteArray(object loObject)
        {
            if (null != loObject)
            {
                if (loObject is byte[])
                {
                    return (byte[])loObject;
                }
                else if (loObject is string)
                {
                    try
                    {
                        return System.Text.UTF8Encoding.UTF8.GetBytes((string)loObject);
                    }
                    catch (Exception loE)
                    {
                        MaxException loMaxException = new MaxException("Error in ConvertToByteArray [" + (string)loObject + "]", loE);
                        return null;
                    }
                }
            }

            return null;
        }

        /// <summary>
		/// Converts a line of text into an array based on the separator
		/// </summary>
		/// <param name="lsLine">The line of text</param>
		/// <param name="lsSeparator">The separator text</param>
		/// <returns>An array of columns</returns>
		public string[] ConvertToColumnList(string lsLine, string lsSeparator)
        {
            if ("," == lsSeparator && lsLine.IndexOf("\"") >= 0)
            {
                // ,"Front (Standard): 3-1/4"" x 1-7/8"", 4 color(s) max ",
                int lnStart = 0;
                MaxIndex loList = new MaxIndex();
                while (lnStart < lsLine.Length)
                {
                    int lnOffset = 0;
                    string lsEnd = ",";
                    bool lbQuoteStart = false;
                    if (lsLine.Substring(lnStart, 1) == "\"")
                    {
                        lnOffset = 1;
                        lsEnd = "\",";
                        lbQuoteStart = true;
                    }

                    int lnEnd = lsLine.IndexOf(lsEnd, lnStart + lnOffset);

                    if (lnEnd <= 0)
                    {
                        lnEnd = lsLine.Length - lnOffset;
                    }
                    else if (lbQuoteStart)
                    {
                        //// Check to make sure that the end is not X"",
                        if (lnEnd - lnStart > 4)
                        {
                            string lsEndCheck = lsLine.Substring(lnEnd + lsEnd.Length - 4, 4);
                            while (lsEndCheck.Substring(1, 2) == "\"\"" && lsEndCheck.Substring(0, 1) != "\"")
                            {
                                lnEnd = lsLine.IndexOf(lsEnd, lnEnd + 1);
                                lsEndCheck = lsLine.Substring(lnEnd + lsEnd.Length - 4, 4);
                            }
                        }
                    }

                    loList.Add(lsLine.Substring(lnStart + lnOffset, lnEnd - lnStart - lnOffset));
                    lnStart = lnEnd + lnOffset;
                    if (lnStart != lsLine.Length)
                    {
                        lnStart++;
                    }
                }

                object[] laValue = loList.GetSortedValueList();
                string[] laReturn = new string[laValue.Length];
                for (int lnV = 0; lnV < laValue.Length; lnV++)
                {
                    laReturn[lnV] = laValue[lnV].ToString();
                }

                return laReturn;
            }
            else
            {
                char[] laSeparator = lsSeparator.ToCharArray();
                return lsLine.Split(laSeparator);
            }
        }

        /// <summary>
        /// Parses text and returns an index of content and short codes.
        /// </summary>
        /// <param name="lsText">Text to parse.</param>
        /// <returns>An index of short codes and content.</returns>
        public MaxIndex ConvertToShortCodeIndex(string lsText)
        {
            MaxIndex loR = new MaxIndex();
            if (null != lsText && lsText.Length > 0)
            {
                bool lbInShortCode = false;
                bool lbInName = false;
                bool lbInValue = false;
                bool lbInEscape = false;
                bool lbInShortCodeName = false;
                int lnShortCodeStart = int.MinValue;
                string lsContent = string.Empty;
                string lsName = string.Empty;
                string lsValue = string.Empty;
                char lsValueDelim = char.MinValue;
                string lsShortCodeName = string.Empty;
                MaxIndex loShortCodeIndex = null;
                string lsShortCodeType = "START";
                string lsShortCodeTag = string.Empty;

                for (int lnCurrent = 0; lnCurrent < lsText.Length; lnCurrent++)
                {
                    if (lbInShortCode)
                    {
                        lsShortCodeTag += lsText[lnCurrent];

                        if (lnShortCodeStart.Equals(lnCurrent - 1))
                        {
                            //// Handle the first character after a short code is started
                            if (lsText[lnCurrent].Equals('[') || this._sWhiteSpace.IndexOf(lsText[lnCurrent]) >= 0)
                            {
                                //// handle [[ to allow a short code to be escaped or assume not a short code if white space is between the "[" and the first character.
                                lbInShortCode = false;
                                lsContent += "[" + lsText[lnCurrent];
                            }
                            else
                            {
                                //// Save any current content
                                if (lsContent.Length > 0)
                                {
                                    loR.Add(lsContent);
                                }

                                //// Start a new short code name
                                lbInShortCodeName = true;
                                lsShortCodeType = "END";
                                lsShortCodeName = string.Empty;
                                if (!lsText[lnCurrent].Equals('/'))
                                {
                                    lsShortCodeType = "START";
                                    lsShortCodeName += lsText[lnCurrent];
                                }
                            }
                        }
                        else if (lbInShortCodeName && this._sShortCodeNameValid.IndexOf(lsText[lnCurrent]) < 0)
                        {
                            //// End of the Short code name
                            lbInShortCodeName = false;
                            lsName = string.Empty;
                            lsValue = string.Empty;
                            lsContent = string.Empty;

                            //// Create a short code Index
                            loShortCodeIndex = new MaxIndex();
                            loShortCodeIndex.Add("TYPE", lsShortCodeType);
                            loShortCodeIndex.Add("SHORTCODENAME", lsShortCodeName);
                            //// Assume that the next character is the start of an attribute name.
                            lbInName = true;
                        }
                        else if (lbInShortCodeName)
                        {
                            //// Add to the short code name
                            lsShortCodeName += lsText[lnCurrent];
                        }
                        else if (lbInName && this._sShortCodeNameValid.IndexOf(lsText[lnCurrent]) >= 0)
                        {
                            //// Add to the attribute name
                            lsName += lsText[lnCurrent];
                        }
                        else if (lbInName && lsText[lnCurrent] == '=')
                        {
                            //// End of the attribute name
                            lbInName = false;
                            lbInValue = true;
                            lbInEscape = false;
                            lsValueDelim = char.MinValue;
                        }
                        else if (lbInValue)
                        {
                            if (lsValue.Length.Equals(0) && lsValueDelim == char.MinValue)
                            {
                                if (lsText[lnCurrent] == '\'' || lsText[lnCurrent] == '"')
                                {
                                    //// Set the text delimiter to the first character if it is a single or double quote.
                                    lsValueDelim = lsText[lnCurrent];
                                }
                                else if (this._sWhiteSpace.IndexOf(lsText[lnCurrent]) < 0)
                                {
                                    //// Start of a value with out any delimiters, so set any whitespace as the delimitter
                                    lsValueDelim = ' ';
                                    lsValue += lsText[lnCurrent];
                                }
                            }
                            else if (lbInEscape)
                            {
                                //// Add any character if the previous character was the escape character.
                                lsValue += lsText[lnCurrent];
                                lbInEscape = false;
                            }
                            else if (lsText[lnCurrent] == '\\')
                            {
                                //// This is the escape character, so process the next character as escaped.
                                lbInEscape = true;
                            }
                            else if (lsText[lnCurrent] == lsValueDelim || (lsText[lnCurrent] == ']' && lsValueDelim == ' '))
                            {
                                //// End of the attribute value.  Add the pair.
                                loShortCodeIndex.Add(lsName, lsValue);
                                //// Reset values for the next pair.
                                lbInName = true;
                                lbInValue = false;
                                lsName = string.Empty;
                                lsValue = string.Empty;
                                lbInEscape = false;
                                lsValueDelim = char.MinValue;
                            }
                            else
                            {
                                //// Add to the value
                                lsValue += lsText[lnCurrent];
                            }
                        }

                        if (lsText[lnCurrent] == ']' && !lbInValue)
                        {
                            //// End of the short code.
                            loShortCodeIndex.Add("SHORTCODETAG", lsShortCodeTag);
                            loR.Add(loShortCodeIndex);
                            lbInShortCode = false;
                        }
                    }
                    else
                    {
                        if (lsText[lnCurrent].Equals('[') && lsText.IndexOf(']', lnCurrent) > 0)
                        {
                            //// Attempt to start a new short code
                            lnShortCodeStart = lnCurrent;
                            lbInShortCode = true;
                            lsShortCodeTag = lsText[lnCurrent].ToString();
                        }
                        else
                        {
                            //// Add to content between short codes.
                            lsContent += lsText[lnCurrent];
                        }
                    }
                }

                if (lsContent.Length > 0)
                {
                    //// Add to content between after any short codes.
                    loR.Add(lsContent);
                }
            }

            return loR;
        }

        /// <summary>
        /// Converts a guid into a short string
        /// </summary>
        /// <param name="loId">Guid to convert</param>
        /// <returns>22 character string representation of the guid</returns>
        public virtual string ConvertGuidToShortString(Guid loId)
        {
            string lsR = Convert.ToBase64String(loId.ToByteArray());
            lsR = lsR.Replace("/", "_");
            lsR = lsR.Replace("+", "-");
            lsR = lsR.Substring(0, 22);
            return lsR;
        }

        /// <summary>
        /// Converts a short string back into a guid
        /// </summary>
        /// <param name="lsShortString">22 character short string</param>
        /// <returns>Guid that matches the string</returns>
        public virtual Guid ConvertShortStringToGuid(string lsShortString)
        {
            string lsId = lsShortString.Replace("_", "/");
            lsId = lsId.Replace("-", "+");
            lsId += "==";
            byte[] laId = Convert.FromBase64String(lsId);
            Guid loR = new Guid(laId);
            return loR;
        }

        /// <summary>
        /// Converts a Guid to Base32
        /// Base32 uses letters and numbers that can be read
        /// <see cref="https://tools.ietf.org/html/rfc4648#section-6"/>
        /// </summary>
        /// <param name="loId">Guid to Convert</param>
        /// <returns>String with numbers and letters from Base32 alphabet</returns>
        public virtual string ConvertGuidToBase32(Guid loId)
        {
            byte[] laGuid = this.ConvertGuidStringtoByte(loId.ToString());
            string lsR = this.ConvertByteArrayToBase32(laGuid, this._sAlphabetBase32);
            return lsR.TrimEnd(this._sAlphabetBase32[this._sAlphabetBase32.Length - 1]);
        }

        /// <summary>
        /// Converts a Base32 string to Guid
        /// Case insensitive
        /// </summary>
        /// <param name="lsBase32">Base32 string to convert</param>
        /// <returns>26 character long string with letters from Base32 alphabet</returns>
        public virtual Guid ConvertBase32ToGuid(string lsBase32)
        {
            byte[] laData = this.ConvertBase32ToByteArray(lsBase32.ToUpper(), this._sAlphabetBase32);
            return new Guid(laData);
        }

        /// <summary>
        /// Converts a Guid to Base32 using Base32 Hex alphabet
        /// Base32 uses letters and numbers that can be read and they are ordered for sorting in the same order as the
        /// byte array representation of the Guid.
        /// <see cref="https://tools.ietf.org/html/rfc4648#section-7"/>
        /// </summary>
        /// <param name="loId">Guid to Convert</param>
        /// <returns>26 character long string with letters from Base32 hex alphabet</returns>
        public virtual string ConvertGuidToBase32Hex(Guid loId)
        {
            byte[] laGuid = this.ConvertGuidStringtoByte(loId.ToString());
            string lsR = this.ConvertByteArrayToBase32(laGuid, this._sAlphabetBase32Hex);
            return lsR.TrimEnd(this._sAlphabetBase32Hex[this._sAlphabetBase32Hex.Length - 1]);
        }

        /// <summary>
        /// Converts a Base32 hex string to Guid
        /// Case insensitive
        /// </summary>
        /// <param name="lsBase32">Base32 string to convert</param>
        /// <returns>Guid representing the Base32 hex string</returns>
        public virtual Guid ConvertBase32HexToGuid(string lsBase32)
        {
            byte[] laData = this.ConvertBase32ToByteArray(lsBase32.ToUpper(), this._sAlphabetBase32Hex);
            return new Guid(laData);
        }

        /// <summary>
        /// Converts a byte array into a base 32 string
        /// <see href="https://tools.ietf.org/html/rfc4648#section-6"/>
        /// Not optimized for long data
        /// </summary>
        /// <param name="laData">Data array to convert</param>
        /// <param name="lsAlphabet">Alphabet to use for conversion</param>
        /// <returns>String representation of the data</returns>
        public virtual string ConvertByteArrayToBase32(byte[] laData, string lsAlphabet)
        {
            char[] laAlphabet = lsAlphabet.ToCharArray();
            //// 8 is the number of bits per byte
            //// 5 is the number of bits that base32 can describe at once
            int lnCharacterCount = (int)Math.Ceiling(laData.Length / 5d) * 8;
            //// Allocate the result
            char[] laBase32 = new char[lnCharacterCount];
            int lnCurrent = 0;

            //// Convert 5 bytes to 8 characters
            for (int lnB = 0; lnB < laData.Length; lnB = lnB + 5)
            {
                //// Use the left 5 bits of the first byte shifted 3 right (ABCDEFGH becomes 000ABCDE)
                laBase32[lnCurrent++] = laAlphabet[(byte)(laData[lnB] >> 3)];

                if (lnB + 1 < laData.Length)
                {
                    //// Use the right 3 bits of the first byte shifted 2 left (ABCDEFGH becomes 000FGH00) OR'd the left 2 bits of the second byte shifted 6 right (IJKLMNOP becomes 000000IJ) for (000FGHIJ) 
                    laBase32[lnCurrent++] = laAlphabet[(byte)(((laData[lnB] << 2) & 31) | (laData[lnB + 1] >> 6))];
                    //// Use the middle 5 bits of the second byte shifted 1 right (IJKLMNOP becomes 000KLMNO) 
                    laBase32[lnCurrent++] = laAlphabet[(byte)((laData[lnB + 1] >> 1) & 31)];
                }
                else
                {
                    //// Use the right 3 bits of the first byte shifted 2 left (ABCDEFGH becomes 000FGH00)
                    laBase32[lnCurrent++] = laAlphabet[(byte)((laData[lnB] << 2) & 31)];
                    //// Add padding
                    laBase32[lnCurrent++] = laAlphabet[laAlphabet.Length - 1];
                }

                if (lnB + 2 < laData.Length)
                {
                    //// Use the right 1 bits of the second byte shifted 4 left (IJKLMNOP becomes 000P0000) OR'd with the left 4 bits of the third byte shifted 4 right (QRSTUVWX becomes 0000QRST) for (000PQRST) 
                    laBase32[lnCurrent++] = laAlphabet[(byte)(((laData[lnB + 1] << 4) & 31) | (laData[lnB + 2] >> 4))];
                }
                else if (lnB + 1 < laData.Length)
                {
                    //// Use the right 1 bits of the second byte shifted 4 left (IJKLMNOP becomes 000P0000)
                    laBase32[lnCurrent++] = laAlphabet[(byte)((laData[lnB + 1] << 4) & 31)];
                }
                else
                {
                    //// Add padding
                    laBase32[lnCurrent++] = laAlphabet[laAlphabet.Length - 1];
                }

                if (lnB + 3 < laData.Length)
                {
                    //// Use the right 4 bits of the third byte shifted 1 left (QRSTUVWX becomes 000UVWX0) OR'd with the left 1 bits of the fourth byte shifted 7 right (YZABCDEF becomes 0000000Y) for (000UVWXY) 
                    laBase32[lnCurrent++] = laAlphabet[(byte)(((laData[lnB + 2] << 1) & 31) | (laData[lnB + 3] >> 7))];
                    //// Use the middle 5 bits of the fourth byte shifted 2 right (YZABCDEF becomes 000ZABCD) 
                    laBase32[lnCurrent++] = laAlphabet[(byte)((laData[lnB + 3] >> 2) & 31)];
                }
                else if (lnB + 2 < laData.Length)
                {
                    //// Use the right 4 bits of the third byte shifted 1 left (QRSTUVWX becomes 000UVWX0) 
                    laBase32[lnCurrent++] = laAlphabet[(byte)((laData[lnB + 2] << 1) & 31)];
                    //// Add padding
                    laBase32[lnCurrent++] = laAlphabet[laAlphabet.Length - 1];
                }
                else
                {
                    //// Add padding
                    laBase32[lnCurrent++] = laAlphabet[laAlphabet.Length - 1];
                    laBase32[lnCurrent++] = laAlphabet[laAlphabet.Length - 1];
                }

                if (lnB + 4 < laData.Length)
                {
                    //// Use the right 2 bits of the fourth byte shifted 1 left (YZABCDEF becomes 000EF000) OR'd with the left 3 bits of the fifth byte shifted 5 right (GHIJKLMN becomes 00000GHI) for (000EFGHI) 
                    laBase32[lnCurrent++] = laAlphabet[(byte)(((laData[lnB + 3] << 3) & 31) | (laData[lnB + 4] >> 5))];
                    //// Use the right 5 bits of the fifth byte shifted 0 right (GHIJKLMN becomes 000JKLMN) 
                    laBase32[lnCurrent++] = laAlphabet[(byte)(laData[lnB + 4] & 31)];
                }
                else if (lnB + 3 < laData.Length)
                {
                    //// Use the right 2 bits of the fourth byte shifted 1 left (YZABCDEF becomes 000EF000)
                    laBase32[lnCurrent++] = laAlphabet[(byte)((laData[lnB + 3] << 3) & 31)];
                    //// Add padding
                    laBase32[lnCurrent++] = laAlphabet[laAlphabet.Length - 1];
                }
                else
                {
                    //// Add padding
                    laBase32[lnCurrent++] = laAlphabet[laAlphabet.Length - 1];
                    laBase32[lnCurrent++] = laAlphabet[laAlphabet.Length - 1];
                }
            }

            return new string(laBase32);
        }

        /// <summary>
        /// Converts a Base32 string back into a guid
        /// <see href="https://tools.ietf.org/html/rfc4648#section-6"/>
        /// </summary>
        /// <param name="lsBase32">Base32 string that was a Guid</param>
        /// <param name="lsAlphabet">Alphabet to use for conversion</param>
        /// <returns>Guid that matches the string</returns>
        public virtual byte[] ConvertBase32ToByteArray(string lsBase32, string lsAlphabet)
        {
            //// Remove padding characters
            char[] laBase32 = lsBase32.TrimEnd(lsAlphabet[lsAlphabet.Length - 1]).ToCharArray();
            //// Determine how long the output should be
            int lnByteCount = laBase32.Length * 5 / 8;
            byte[] laData = new byte[lnByteCount];
            int lnIndex = 0;

            //// Convert 8 characters to 5 bytes then repeat
            for (int lnC = 0; lnC < laBase32.Length; lnC = lnC + 8)
            {
                //// First byte
                if (lnC + 1 < laBase32.Length && lnIndex < laData.Length)
                {
                    //// Use the first 5 bits of first character (shifted left 3) (000ABCDE becomes ABCDE000) and the first 3 bits of second character (shifted right 2) (000FGHIJ becomes 00000FGH) for the first byte
                    laData[lnIndex++] = (byte)((lsAlphabet.IndexOf(laBase32[lnC]) << 3) | (lsAlphabet.IndexOf(laBase32[lnC + 1]) >> 2));
                }
                else if (lnIndex < laData.Length)
                {
                    //// Use first character for left 5 bits of first byte (000ABCDE becomes ABCDE000)
                    laData[lnIndex++] = (byte)(lsAlphabet.IndexOf(laBase32[lnC]) << 3);
                }

                //// Second byte
                if (lnC + 3 < laBase32.Length && lnIndex < laData.Length)
                {
                    //// Use the last 2 bits of second character (shifted left 6) (000FGHIJ becomes IJ000000) and the 5 bits of third character (shifted left 1) (000KLMNO becomes 00KLMNO0) and the first bit of the fourth character (shifted right 4) (000PQRST becomes 0000000P) for the second byte
                    laData[lnIndex++] = (byte)((lsAlphabet.IndexOf(laBase32[lnC + 1]) << 6) | (lsAlphabet.IndexOf(laBase32[lnC + 2]) << 1) | (lsAlphabet.IndexOf(laBase32[lnC + 3]) >> 4));
                }
                else if (lnC + 2 < laBase32.Length && lnIndex < laData.Length)
                {
                    //// Use the last 2 bits of second character (shifted left 6) (000FGHIJ becomes IJ000000) and the 5 bits of third character (shifted left 1) (000KLMNO becomes 00KLMNO0) for the second byte
                    laData[lnIndex++] = (byte)((lsAlphabet.IndexOf(laBase32[lnC + 1]) << 6) | (lsAlphabet.IndexOf(laBase32[lnC + 2]) << 1));
                }
                else if (lnC + 1 < laBase32.Length && lnIndex < laData.Length)
                {
                    //// Use the last 2 bits of second character (shifted left 6) (000FGHIJ becomes IJ000000) for the second byte
                    laData[lnIndex++] = (byte)(lsAlphabet.IndexOf(laBase32[lnC + 1]) << 6);
                }

                //// Third byte
                if (lnC + 4 < laBase32.Length && lnIndex < laData.Length)
                {
                    //// Use the last 4 bits of fourth character (shifted left 4) (000PQRST becomes QRST0000) and the first 4 bits of fifth character (shifted right 1) (000UVWXY becomes 0000UVWX) for the third byte
                    laData[lnIndex++] = (byte)((lsAlphabet.IndexOf(laBase32[lnC + 3]) << 4) | (lsAlphabet.IndexOf(laBase32[lnC + 4]) >> 1));
                }
                else if (lnC + 3 < laBase32.Length && lnIndex < laData.Length)
                {
                    //// Use the last 4 bits of fourth character (shifted left 4) (000PQRSTU becomes QRSTU000) for the third byte
                    laData[lnIndex++] = (byte)(lsAlphabet.IndexOf(laBase32[lnC + 3]) << 4);
                }

                //// Fourth byte
                if (lnC + 6 < laBase32.Length && lnIndex < laData.Length)
                {
                    //// Use the last 1 bits of fifth character (shifted left 7) (000UVWXY becomes Y0000000) and the 5 bits of sixth character (shifted left 2) (000ZABCD becomes 0ZABCDE0) and the first 2 bits of the seventh character (shifted right 3) (000EFGHI becomes 000000EF) for the fourth byte
                    laData[lnIndex++] = (byte)((lsAlphabet.IndexOf(laBase32[lnC + 4]) << 7) | (lsAlphabet.IndexOf(laBase32[lnC + 5]) << 2) | (lsAlphabet.IndexOf(laBase32[lnC + 6]) >> 3));
                }
                else if (lnC + 5 < laBase32.Length && lnIndex < laData.Length)
                {
                    //// Use the last 1 bits of fifth character (shifted left 7) (000UVWXY becomes Y0000000) and the 5 bits of sixth character (shifted left 2) (000ZABCD becomes 0ZABCDE0) for the fourth byte
                    laData[lnIndex++] = (byte)((lsAlphabet.IndexOf(laBase32[lnC + 4]) << 7) | (lsAlphabet.IndexOf(laBase32[lnC + 5]) << 2));
                }
                else if (lnC + 4 < laBase32.Length && lnIndex < laData.Length)
                {
                    //// Use the last 1 bits of fifth character (shifted left 7) for the fourth byte
                    laData[lnIndex++] = (byte)(lsAlphabet.IndexOf(laBase32[lnC + 4]) << 7);
                }

                //// fifth byte
                if (lnC + 7 < laBase32.Length && lnIndex < laData.Length)
                {
                    //// Use the last 3 bits of seventh character (shifted left 5) (000EFGHI becomes GHI00000) and the 5 bits of eighth character for the third byte
                    laData[lnIndex++] = (byte)((lsAlphabet.IndexOf(laBase32[lnC + 6]) << 5) | lsAlphabet.IndexOf(laBase32[lnC + 7]));
                }
                else if (lnC + 6 < laBase32.Length && lnIndex < laData.Length)
                {
                    //// Use the last 3 bits of seventh character (shifted left 5) (000EFGHI becomes GHI00000)
                    laData[lnIndex++] = (byte)(lsAlphabet.IndexOf(laBase32[lnC + 6]) << 5);
                }
            }

            return laData;
        }

        /// <summary>
        /// Converts a Guid to a series of just Alphanumeric characters using a 32 bit conversion
        /// </summary>
        /// <param name="loId">Guid to convert</param>
        /// <returns>24 character long string of just Alphanumeric characters (case sensitive)</returns>
        public virtual string ConvertGuidToAlphabet32(Guid loId)
        {
            byte[] laGuid = this.ConvertGuidStringtoByte(loId.ToString()); //// laGuid should always be 16 long
            return this.ConvertByteArrayToAlphabet32(laGuid, this._sAlphabetAlphanumeric);
        }

        /// <summary>
        /// Converts a series of series of just Alphanumeric characters to a Guid using a 32 bit conversion
        /// </summary>
        /// <param name="lsAlphabet32">24 character long string of just Alphanumeric characters (case sensitive)</param>
        /// <returns>Guid based on the characters</returns>
        public virtual Guid ConvertAlphabet32ToGuid(string lsAlphabet32)
        {
            byte[] laData = this.ConvertAlphabet32ToByteArray(lsAlphabet32, this._sAlphabetAlphanumeric);
            return new Guid(laData);
        }

        /// <summary>
        /// Converts a byte array to just Alphanumeric characters using a 32 bit conversion and the supplied list of characters
        /// </summary>
        /// <param name="laData">byte array of data</param>
        /// <param name="lsAlphabet">string of characters to use for encoding</param>
        /// <returns>string representing the byte array using just the characters in the alphabet</returns>
        public virtual string ConvertByteArrayToAlphabet32(byte[] laData, string lsAlphabet)
        {
            //// There are 8 bits in every byte representing 0 to 255
            //// I want to use Alphabet to represent the same thing
            //// The number of characters in my Alphabet is Alphabet.length
            //// I want to convert base 2 stored in 8 bits to base Alphabet.length
            //// Converting an unsigned int to any base seems easy according to Wikipedia
            //// https://en.wikipedia.org/wiki/Base36
            uint lnBase = Convert.ToUInt32((lsAlphabet.Length - 1).ToString());

            //// every 4 bytes (32 bits) could be converted to a UInt and then converted to a base Alphabet.length string...
            //// The base Alphabet.length string will be at most 2 ^ 32 < Alphabet.length ^ X characters long and needs to be padded to be that long to convert back
            int lnLength = 0;
            while (uint.MaxValue > Math.Pow(lnBase, lnLength))
            {
                lnLength++;
            }

            string lsR = string.Empty;
            char[] laAlphabet = lsAlphabet.ToCharArray();
            for (int lnD = 0; lnD < laData.Length; lnD += 4)
            {
                uint lnInput = BitConverter.ToUInt32(laData, lnD);
                string lsInput = string.Empty;
                while (lnInput > 0)
                {
                    lsInput = laAlphabet[lnInput % lnBase] + lsInput;
                    lnInput = lnInput / lnBase;
                }

                while (lsInput.Length < lnLength)
                {
                    lsInput += laAlphabet[laAlphabet.Length - 1];
                }

                lsR += lsInput;
            }

            return lsR;
        }

        /// <summary>
        /// Converts alphanumeric characters to a binary array using the supplied alphabet
        /// </summary>
        /// <param name="lsAlphabet32">string to be encoded</param>
        /// <param name="lsAlphabet">string of characters to use for encoding</param>
        /// <returns>binary array based on the encoding</returns>
        public virtual byte[] ConvertAlphabet32ToByteArray(string lsAlphabet32, string lsAlphabet)
        {
            //// We can use one less than the total alphabet length for encoding (the last one is for padding)
            uint lnBase = Convert.ToUInt32((lsAlphabet.Length - 1).ToString());
            //// Determine the length of the Alphabet text for every 4 bytes (32 bits) 
            int lnLength = 0;
            while (uint.MaxValue > Math.Pow(lnBase, lnLength))
            {
                lnLength++;
            }

            byte[] laR = new byte[lsAlphabet32.Length * 4 / lnLength];
            int lnChunk = 0;
            char[] laAlphabet = lsAlphabet.ToCharArray();
            for (int lnStart = 0; lnStart < lsAlphabet32.Length; lnStart += lnLength)
            {
                string lsChunk = lsAlphabet32.Substring(lnStart, lnLength);
                uint lnNum = 0;
                int lnPos = 0;
                foreach (char lsChar in lsChunk.ToCharArray())
                {
                    if (lsChar != laAlphabet[laAlphabet.Length - 1])
                    {
                        lnNum = (lnNum * lnBase) + Convert.ToUInt32(lsAlphabet.IndexOf(lsChar).ToString());
                        lnPos++;
                    }
                }

                byte[] laInt = BitConverter.GetBytes(lnNum);
                laR[lnChunk] = laInt[0];
                laR[lnChunk + 1] = laInt[1];
                laR[lnChunk + 2] = laInt[2];
                laR[lnChunk + 3] = laInt[3];
                lnChunk += 4;
            }

            return laR;
        }

        /// <summary>
        /// Converts a Guid to a series of just Alphanumeric characters using a 64 bit conversion
        /// </summary>
        /// <param name="loId">Guid to convert</param>
        /// <returns>22 character long string of just Alphanumeric characters (case sensitive)</returns>
        public virtual string ConvertGuidToAlphabet64(Guid loId)
        {
            byte[] laGuid = this.ConvertGuidStringtoByte(loId.ToString()); //// laGuid should always be 16 long
            return this.ConvertByteArrayToAlphabet64(laGuid, this._sAlphabetAlphanumeric);
        }

        /// <summary>
        /// Converts a series of series of just Alphanumeric characters to a Guid using a 64 bit conversion
        /// </summary>
        /// <param name="lsAlphabet64">22 character long string of just Alphanumeric characters (case sensitive)</param>
        /// <returns>Guid based on the characters</returns>
        public virtual Guid ConvertAlphabet64ToGuid(string lsAlphabet64)
        {
            byte[] laData = this.ConvertAlphabet64ToByteArray(lsAlphabet64, this._sAlphabetAlphanumeric);
            return new Guid(laData);
        }

        /// <summary>
        /// Converts a byte array to just Alphanumeric characters using a 64 bit conversion and the supplied list of characters
        /// </summary>
        /// <param name="laData">byte array of data</param>
        /// <param name="lsAlphabet">string of characters to use for encoding</param>
        /// <returns>string representing the byte array using just the characters in the alphabet</returns>
        public virtual string ConvertByteArrayToAlphabet64(byte[] laData, string lsAlphabet)
        {
            //// There are 8 bits in every byte representing 0 to 255
            //// I want to use Alphabet to represent the same thing
            //// The number of characters in my Alphabet is Alphabet.length
            //// I want to convert base 2 stored in 8 bits to base Alphabet.length
            //// Converting an unsigned int to any base seems easy according to Wikipedia
            //// https://en.wikipedia.org/wiki/Base36
            uint lnBase = Convert.ToUInt32((lsAlphabet.Length - 1).ToString());

            //// every 4 bytes (32 bits) could be converted to a UInt and then converted to a base Alphabet.length string...
            //// The base Alphabet.length string will be at most 2 ^ 64 < Alphabet.length ^ X characters long and needs to be padded to be that long to convert back
            int lnLength = 0;
            while (ulong.MaxValue > Math.Pow(lnBase, lnLength))
            {
                lnLength++;
            }

            string lsR = string.Empty;
            char[] laAlphabet = lsAlphabet.ToCharArray();
            for (int lnD = 0; lnD < laData.Length; lnD += 8)
            {
                ulong lnInput = BitConverter.ToUInt64(laData, lnD);
                string lsInput = string.Empty;
                while (lnInput > 0)
                {
                    //TODO: Warning opcode 'conv.ovf.i.un'-- overflow will not throw exception
                    lsInput = laAlphabet[lnInput % lnBase] + lsInput;
                    lnInput = lnInput / lnBase;
                }

                while (lsInput.Length < lnLength)
                {
                    lsInput += laAlphabet[laAlphabet.Length - 1];
                }

                lsR += lsInput;
            }

            return lsR;
        }

        /// <summary>
        /// Converts alphanumeric characters to a binary array using the supplied alphabet
        /// </summary>
        /// <param name="lsAlphabet64">string to be encoded</param>
        /// <param name="lsAlphabet">string of characters to use for encoding</param>
        /// <returns>binary array based on the encoding</returns>
        public virtual byte[] ConvertAlphabet64ToByteArray(string lsAlphabet64, string lsAlphabet)
        {
            //// We can use one less than the total alphabet length for encoding (the last one is for padding)
            uint lnBase = Convert.ToUInt32((lsAlphabet.Length - 1).ToString());
            //// Determine the length of the Alphabet text for every 8 bytes (64 bits) 
            int lnLength = 0;
            while (ulong.MaxValue > Math.Pow(lnBase, lnLength))
            {
                lnLength++;
            }

            byte[] laR = new byte[lsAlphabet64.Length * 8 / lnLength];
            int lnChunk = 0;
            char[] laAlphabet = lsAlphabet.ToCharArray();
            for (int lnStart = 0; lnStart < lsAlphabet64.Length; lnStart += lnLength)
            {
                string lsChunk = lsAlphabet64.Substring(lnStart, lnLength);
                ulong lnNum = 0;
                int lnPos = 0;
                foreach (char lsChar in lsChunk.ToCharArray())
                {
                    if (lsChar != laAlphabet[laAlphabet.Length - 1])
                    {
                        lnNum = (lnNum * lnBase) + Convert.ToUInt64(lsAlphabet.IndexOf(lsChar).ToString());
                        lnPos++;
                    }
                }

                byte[] laInt = BitConverter.GetBytes(lnNum);
                laR[lnChunk] = laInt[0];
                laR[lnChunk + 1] = laInt[1];
                laR[lnChunk + 2] = laInt[2];
                laR[lnChunk + 3] = laInt[3];
                laR[lnChunk + 4] = laInt[4];
                laR[lnChunk + 5] = laInt[5];
                laR[lnChunk + 6] = laInt[6];
                laR[lnChunk + 7] = laInt[7];
                lnChunk += 8;
            }

            return laR;
        }

        /// <summary>
        /// Converts a string representing a guid to an array of bytes
        /// </summary>
        /// <param name="lsGuid">string representing guid</param>
        /// <returns>array of bytes in guid</returns>
        protected byte[] ConvertGuidStringtoByte(string lsGuid)
        {
            byte[] laGuid = new byte[16];
            if (null != lsGuid && lsGuid.Length >= 32)
            {
                try
                {
                    //// Attempt to decode from 16 hex numbers which may contain dashes
                    char[] laRaw = lsGuid.ToUpper().ToCharArray();
                    int lnGuid = 0;
                    int lnHex = 0;
                    while (lnGuid < laGuid.Length && lnHex < laRaw.Length)
                    {
                        int lnLeft = this._sValidHex.IndexOf(laRaw[lnHex]);
                        lnHex++;
                        while (lnLeft < 0)
                        {
                            lnLeft = this._sValidHex.IndexOf(laRaw[lnHex]);
                            lnHex++;
                        }

                        int lnRight = this._sValidHex.IndexOf(laRaw[lnHex]);
                        lnHex++;

                        int lnVal = lnGuid;
                        if (lnGuid <= 3)
                        {
                            lnVal = 3 - lnGuid;
                        }
                        else if (lnGuid == 4)
                        {
                            lnVal = 5;
                        }
                        else if (lnGuid == 5)
                        {
                            lnVal = 4;
                        }
                        else if (lnGuid == 6)
                        {
                            lnVal = 7;
                        }
                        else if (lnGuid == 7)
                        {
                            lnVal = 6;
                        }

                        laGuid[lnVal] = (byte)(lnLeft << 4 | lnRight);
                        lnGuid++;
                    }
                }
                catch
                {
                    return null;
                }
            }

            return laGuid;
        }

        /// <summary>
        /// Serializes an object using the PHP serialize algorithm
        /// </summary>
        /// <param name="loObject">Object to serialize.</param>
        /// <param name="loEncoding">Encoding of the serialized text.</param>
        /// <returns>String representing the object.</returns>
        protected string SerializeToPHPString(object loObject, Encoding loEncoding)
        {
            string lsR = string.Empty;

            if (loObject is string)
            {
                string lsObject = (string)loObject;
                lsR = "s:" + loEncoding.GetBytes(lsObject).Length + ":\"" + lsObject + "\";";
            }
            else if (loObject is bool)
            {
                string lsValue = "0";
                if ((bool)loObject)
                {
                    lsValue = "1";
                }

                lsR = "b:" + lsValue + ";";
            }
            else if (loObject is int)
            {
                lsR = "i:" + ((int)loObject).ToString() + ";";
            }
            else if (loObject is long)
            {
                lsR = "i:" + ((long)loObject).ToString() + ";";
            }
            else if (loObject is double)
            {
                lsR = "d:" + ((double)loObject).ToString() + ";";
            }
            else if (loObject is MaxIndex)
            {
                MaxIndex loIndex = (MaxIndex)loObject;
                string[] laKey = loIndex.GetSortedKeyList();
                StringBuilder loR = new StringBuilder();
                loR.Append("a:" + loIndex.Count + ":{");
                for (int lnK = 0; lnK < laKey.Length; lnK++)
                {
                    loR.Append(this.SerializeToPHPString(laKey[lnK], loEncoding));
                    loR.Append(this.SerializeToPHPString(loIndex[laKey[lnK]], loEncoding));
                }

                loR.Append("}");
                lsR = loR.ToString();
            }
            else
            {
                lsR = "N;";
            }

            return lsR;
        }

        /// <summary>
        /// Deserializes text that was serialized using the PHP serialize algorithm.
        /// </summary>
        /// <param name="lsSerializationText">The text to deserialize.</param>
        /// <param name="lnStart">Location in text to start.</param>
        /// <param name="loEncoding">Encoding of the serialized text.</param>
        /// <param name="lnLength">Length of the text that was processed.</param>
        /// <returns>An object represented by the serialized text.</returns>
        protected object DeserializeFromPHPString(string lsSerializationText, int lnStart, Encoding loEncoding, out int lnLength)
        {
            lnLength = 0;
            if (lsSerializationText == null || lsSerializationText.Length.Equals(0))
            {
                return null;
            }

            if (lsSerializationText[lnStart] == 'N')
            {
                //// N;
                lnLength = 2;
                return null;
            }
            else if (lsSerializationText[lnStart] == 'b')
            {
                //// b:#;
                char lsBool = lsSerializationText[lnStart + 2];
                lnLength = 4;
                if (lsBool.Equals(1))
                {
                    return true;
                }

                return false;
            }
            else if (lsSerializationText[lnStart] == 'i')
            {
                //// i:#;
                int lnBegin = lsSerializationText.IndexOf(":", lnStart) + 1;
                int lnEnd = lsSerializationText.IndexOf(";", lnBegin);
                lnLength = 3 + lnEnd - lnBegin;
                string lsNum = lsSerializationText.Substring(lnBegin, lnEnd - lnBegin);
                long lnLong = long.Parse(lsNum);
                return lnLong;
            }
            else if (lsSerializationText[lnStart] == 'd')
            {
                //// d:#;
                int lnBegin = lsSerializationText.IndexOf(":", lnStart) + 1;
                int lnEnd = lsSerializationText.IndexOf(";", lnBegin);
                lnLength = 3 + lnEnd - lnBegin;
                string lsNum = lsSerializationText.Substring(lnBegin, lnEnd - lnBegin);
                double lnDouble = 0;
                if (double.TryParse(lsNum, out lnDouble))
                {
                    return lnDouble;
                }

                return null;
            }
            else if (lsSerializationText[lnStart] == 's')
            {
                //// s:#:"STRING";
                int lnBegin = lsSerializationText.IndexOf(":", lnStart) + 1;
                int lnEnd = lsSerializationText.IndexOf(":", lnBegin);
                lnLength = 3 + lnEnd - lnBegin + 3;
                string lsNum = lsSerializationText.Substring(lnBegin, lnEnd - lnBegin);
                int lnByteCount = int.Parse(lsNum);
                int lnStringLength = lnByteCount;
                if ((lnEnd + 2 + lnStringLength) >= lsSerializationText.Length)
                {
                    lnStringLength = lsSerializationText.Length - 2 - lnEnd;
                }

                string lsString = lsSerializationText.Substring(lnEnd + 2, lnStringLength);
                while (loEncoding.GetBytes(lsString).Length > lnByteCount)
                {
                    lnStringLength--;
                    lsString = lsSerializationText.Substring(lnEnd + 2, lnStringLength);
                }

                lnLength += lnStringLength;
                return lsString;
            }
            else if (lsSerializationText[lnStart] == 'a')
            {
                //// a:#:{ARRAY}
                MaxIndex loR = new MaxIndex();
                int lnBegin = lsSerializationText.IndexOf(":", lnStart) + 1;
                int lnEnd = lsSerializationText.IndexOf(":", lnBegin);
                string lsNum = lsSerializationText.Substring(lnBegin, lnEnd - lnBegin);
                lnLength = 3 + lnEnd - lnBegin + 1;
                int lnCount = int.Parse(lsNum);
                int lnSubLength = 0;
                lnStart += lnLength;
                for (int lnC = 0; lnC < lnCount; lnC++)
                {
                    object loKey = this.DeserializeFromPHPString(lsSerializationText, lnStart, loEncoding, out lnSubLength);
                    lnLength += lnSubLength;
                    lnStart += lnSubLength;
                    object loValue = this.DeserializeFromPHPString(lsSerializationText, lnStart, loEncoding, out lnSubLength);
                    lnLength += lnSubLength;
                    lnStart += lnSubLength;
                    loR.Add(loKey.ToString(), loValue);
                }

                lnLength++;
                return loR;
            }
            else
            {
                return null;
            }
        }

        public virtual string ConvertToSlug(string lsText, int lnMaxLength)
        {
            string lsR = string.Empty;
            if (null != lsText)
            {
                bool lbIsPreviousDash = false;
                char loCurrentChar;
                int lnT = 0;
                while (lsR.Length < lnMaxLength && lnT < lsText.Length)
                {
                    loCurrentChar = lsText[lnT];
                    if ((loCurrentChar >= 'a' && loCurrentChar <= 'z') || (loCurrentChar >= '0' && loCurrentChar <= '9') || (loCurrentChar >= 'A' && loCurrentChar <= 'Z'))
                    {
                        if (lbIsPreviousDash)
                        {
                            lsR += "-" + loCurrentChar.ToString();
                            lbIsPreviousDash = false;
                        }
                        else
                        {
                            lsR += loCurrentChar.ToString();
                        }
                    }
                    else if (loCurrentChar == ' ' || loCurrentChar == ',' || loCurrentChar == '.' || loCurrentChar == '/' || loCurrentChar == '\\' || loCurrentChar == '-' || loCurrentChar == '_' || loCurrentChar == '=')
                    {
                        if (!lbIsPreviousDash && lsR.Length > 0)
                        {
                            lbIsPreviousDash = true;
                        }
                    }
                    else
                    {
                        string lsRemap = this.RemapInternationalCharToAscii(loCurrentChar);
                        if (lsRemap != null)
                        {
                            if (lbIsPreviousDash)
                            {
                                lsR += "-" + lsRemap;
                                lbIsPreviousDash = false;
                            }
                            else
                            {
                                lsR += lsRemap;
                            }
                        }
                    }

                    lnT++;
                }

                if (lsR.Length > lnMaxLength)
                {
                    if (lsR.Substring(lnMaxLength - 1, 1) == "-")
                    {
                        lsR = lsR.Substring(0, lnMaxLength - 1);
                    }
                    else
                    {
                        lsR = lsR.Substring(0, lnMaxLength);
                    }
                }
            }

            return lsR;
        }

        /// <summary>
        /// Converts some csv text to an array of values
        /// </summary>
        /// <param name="lsText"></param>
        /// <returns>array of field values</returns>
        public string[] ConvertCSVToArray(string lsText)
        {
            ArrayList loResult = new ArrayList();
            string lsPart = lsText;
            string lsEndField = ",";
            int lnStart = 0;
            if (lsPart.StartsWith("\""))
            {
                lsEndField = "\",";
                lnStart = 1;
            }

            while (lsPart.IndexOf(lsEndField) >= 0)
            {
                if (lsPart.IndexOf(lsEndField) == 0)
                {
                    loResult.Add(string.Empty);
                }
                else
                {
                    loResult.Add(lsPart.Substring(lnStart, lsPart.IndexOf(lsEndField) - (lsEndField.Length - 1)));
                }

                lsPart = lsPart.Substring(lsPart.IndexOf(lsEndField) + lsEndField.Length);
                if (lsPart.StartsWith("\""))
                {
                    lsEndField = "\",";
                    lnStart = 1;
                }
                else
                {
                    lnStart = 0;
                    lsEndField = ",";
                }
            }

            if (lsPart.StartsWith("\"") && lsPart.EndsWith("\""))
            {
                lsPart = lsPart.Substring(1, lsPart.Length - 1);
            }

            loResult.Add(lsPart);
            return (string[])loResult.ToArray(typeof(string));
        }

        /// <summary>
        /// Creates a slug.
        /// References:
        /// http://www.unicode.org/reports/tr15/tr15-34.html
        /// http://meta.stackexchange.com/questions/7435/non-us-ascii-characters-dropped-from-full-profile-url/7696#7696
        /// http://stackoverflow.com/questions/25259/how-do-you-include-a-webpage-title-as-part-of-a-webpage-url/25486#25486
        /// http://stackoverflow.com/questions/3769457/how-can-i-remove-accents-on-a-string
        /// https://msdn.microsoft.com/en-us/library/ebza6ck1(v=vs.110).aspx - normalize method - .Normalize(NormalizationForm.FormKD)
        /// http://stackoverflow.com/questions/25259/how-does-stack-overflow-generate-its-seo-friendly-urls
        /// </summary>
        /// <param name="lsText">Text to convert.  Should be the case you want the output to be and should be normalized to FormKD if supported</param>
        /// <returns>Slug version of text</returns>
        public virtual string ConvertToSlugStackOverFlow1(string lsText)
        {
            string lsR = string.Empty;
            if (null != lsText)
            {
                bool lbIsPreviousDash = false;
                char loCurrentChar;
                int lnT = 0;
                while (lnT < lsText.Length && lsR.Length < 200)
                {
                    loCurrentChar = lsText[lnT];
                    if ((loCurrentChar >= 'a' && loCurrentChar <= 'z') || (loCurrentChar >= '0' && loCurrentChar <= '9') || (loCurrentChar >= 'A' && loCurrentChar <= 'Z'))
                    {
                        if (lbIsPreviousDash)
                        {
                            lsR += "-" + loCurrentChar.ToString();
                            lbIsPreviousDash = false;
                        }
                        else
                        {
                            lsR += loCurrentChar.ToString();
                        }
                    }
                    else if (loCurrentChar == ' ' || loCurrentChar == ',' || loCurrentChar == '.' || loCurrentChar == '/' || loCurrentChar == '\\' || loCurrentChar == '-' || loCurrentChar == '_' || loCurrentChar == '=')
                    {
                        if (!lbIsPreviousDash && lsR.Length > 0)
                        {
                            lbIsPreviousDash = true;
                        }
                    }
                    else
                    {
                        string lsSwap = this.ConvertEdgeCases(loCurrentChar);
                        if (lsSwap != null)
                        {
                            if (lbIsPreviousDash)
                            {
                                lsR += "-" + lsSwap;
                                lbIsPreviousDash = false;
                            }
                            else
                            {
                                lsR += lsSwap;
                            }
                        }
                    }

                    lnT++;
                }
            }

            return lsR;
        }

        protected virtual string ConvertEdgeCases(char loChar)
        {
            string lsR = null;
            switch (loChar)
            {
                case 'ı':
                    lsR = "i";
                    break;
                case 'ł':
                    lsR = "l";
                    break;
                case 'Ł':
                    lsR = "l";
                    break;
                case 'đ':
                    lsR = "d";
                    break;
                case 'ß':
                    lsR = "ss";
                    break;
                case 'ø':
                    lsR = "o";
                    break;
                case 'Þ':
                    lsR = "th";
                    break;
            }

            return lsR;
        }

        /// <summary>
        /// Produces optional, URL-friendly version of a title, "like-this-one". 
        /// hand-tuned for speed, reflects performance refactoring contributed
        /// by John Gietzen (user otac0n) 
        /// </summary>
        public virtual string ConvertToSlugStackOverFlow2(string lsText)
        {
            string lsR = string.Empty;
            if (null != lsText)
            {
                bool lbIsPreviousDash = false;
                char loCurrentChar;
                int lnT = 0;
                while (lsR.Length < 80 && lnT < lsText.Length)
                {
                    loCurrentChar = lsText[lnT];
                    if ((loCurrentChar >= 'a' && loCurrentChar <= 'z') || (loCurrentChar >= '0' && loCurrentChar <= '9') || (loCurrentChar >= 'A' && loCurrentChar <= 'Z'))
                    {
                        if (lbIsPreviousDash)
                        {
                            lsR += "-" + loCurrentChar.ToString();
                            lbIsPreviousDash = false;
                        }
                        else
                        {
                            lsR += loCurrentChar.ToString();
                        }
                    }
                    else if (loCurrentChar == ' ' || loCurrentChar == ',' || loCurrentChar == '.' || loCurrentChar == '/' || loCurrentChar == '\\' || loCurrentChar == '-' || loCurrentChar == '_' || loCurrentChar == '=')
                    {
                        if (!lbIsPreviousDash && lsR.Length > 0)
                        {
                            lbIsPreviousDash = true;
                        }
                    }
                    else
                    {
                        string lsRemap = this.RemapInternationalCharToAscii(loCurrentChar);
                        if (lsRemap != null)
                        {
                            if (lbIsPreviousDash)
                            {
                                lsR += "-" + lsRemap;
                                lbIsPreviousDash = false;
                            }
                            else
                            {
                                lsR += lsRemap;
                            }
                        }
                    }

                    lnT++;
                }
            }

            return lsR;
        }

        protected virtual string RemapInternationalCharToAscii(char loChar)
        {
            string lsTest = loChar.ToString().ToLower();
            if ("àåáâäãåą".IndexOf(lsTest) > -1)
            {
                return "a";
            }
            else if ("èéêëę".IndexOf(lsTest) > -1)
            {
                return "e";
            }
            else if ("ìíîïı".IndexOf(lsTest) > -1)
            {
                return "i";
            }
            else if ("òóôõöøőð".IndexOf(lsTest) > -1)
            {
                return "o";
            }
            else if ("ùúûüŭů".IndexOf(lsTest) > -1)
            {
                return "u";
            }
            else if ("çćčĉ".IndexOf(lsTest) > -1)
            {
                return "c";
            }
            else if ("żźž".IndexOf(lsTest) > -1)
            {
                return "z";
            }
            else if ("śşšŝ".IndexOf(lsTest) > -1)
            {
                return "s";
            }
            else if ("ñń".IndexOf(lsTest) > -1)
            {
                return "n";
            }
            else if ("ýÿ".IndexOf(lsTest) > -1)
            {
                return "y";
            }
            else if ("ğĝ".IndexOf(lsTest) > -1)
            {
                return "g";
            }
            else if (loChar == 'ř')
            {
                return "r";
            }
            else if (loChar == 'ł')
            {
                return "l";
            }
            else if (loChar == 'đ')
            {
                return "d";
            }
            else if (loChar == 'ß')
            {
                return "ss";
            }
            else if (loChar == 'Þ')
            {
                return "th";
            }
            else if (loChar == 'ĥ')
            {
                return "h";
            }
            else if (loChar == 'ĵ')
            {
                return "j";
            }
            else
            {
                return null;
            }
        }

        public bool IsNative(object loValue)
        {
            return this.IsNativeConditional(loValue);
        }

        protected virtual object AddValueType(object loObject)
        {
            if (loObject is MaxIndex)
            {
                MaxIndex loIndex = loObject as MaxIndex;
                MaxIndex loR = new MaxIndex();
                foreach (object loKey in loIndex.Keys)
                {
                    object loValue = loIndex[loKey];
                    if (null != loValue)
                    {
                        if (!this.IsNative(loValue))
                        {
                            loR.Add(loKey.ToString() + this._sSerialTypeKeySuffix, loValue.GetType().AssemblyQualifiedName);
                            loValue = this.AddValueType(loValue);
                        }
                    }

                    loR.Add(loKey, loValue);
                }

                return loR;
            }

            return loObject;
        }

#if net2  || netcore2
        protected bool IsNativeConditional(object loValue)
        {
            if (loValue is string || loValue is long || loValue is bool || (loValue is double && loValue.ToString().Contains(".")))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Serializes an object to a string using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to serialize.</param>
        /// <returns>string to represent object</returns>
        public virtual string SerializeToStringConditional(object loObject)
        {
            return this.SerializeToXmlConditional(loObject);
        }

        /// <summary>
        /// Deserializes text to an object using the provider associated with the Type.
        /// </summary>
        /// <param name="lsSerializationText">Text that represents the serialized object.</param>
        /// <param name="loTypeExpected">Type of object expected to be deserialized into.</param>
        /// <returns>object version of serialized text</returns>
        public virtual object DeserializeFromStringConditional(string lsSerializationText, Type loTypeExpected)
        {
            if (lsSerializationText.IndexOf("<?xml") == 0)
            {
                return this.DeserializeFromXmlConditional(lsSerializationText, loTypeExpected);
            }

            return this.DeserializeFromJsonConditional(lsSerializationText, loTypeExpected);
        }

        /// <summary>
        /// Serializes an object to a string using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to serialize.</param>
        /// <returns>string to represent object</returns>
        public virtual string SerializeToXmlConditional(object loObject)
        {
            string lsR = string.Empty;
            if (loObject is MaxIndex)
            {
                loObject = ((MaxIndex)loObject).GetSortedList();
            }

            System.Xml.Serialization.XmlSerializer loSerializer = new System.Xml.Serialization.XmlSerializer(loObject.GetType());
            System.IO.StringWriter loWriter = new System.IO.StringWriter();
            try
            {
                loSerializer.Serialize(loWriter, loObject);
                lsR = loWriter.ToString();
            }
            finally
            {
                loWriter.Dispose();
            }

            return lsR;
        }

        /// <summary>
        /// Deserializes text to an object using the provider associated with the Type.
        /// </summary>
        /// <param name="lsSerializationText">Text that represents the serialized object.</param>
        /// <param name="loTypeExpected">Type of object expected to be deserialized into.</param>
        /// <returns>object version of serialized text</returns>
        public virtual object DeserializeFromXmlConditional(string lsSerializationText, Type loTypeExpected)
        {
            System.Xml.Serialization.XmlSerializer loSerializer = null;
            if (loTypeExpected == typeof(MaxIndex))
            {
                loSerializer = new System.Xml.Serialization.XmlSerializer(typeof(MaxIndexItemStructure[]));
            }
            else
            {
                loSerializer = new System.Xml.Serialization.XmlSerializer(loTypeExpected);
            }

            System.IO.StringReader loReader = new System.IO.StringReader(lsSerializationText);
            object loR = null;
            try
            {
                loR = loSerializer.Deserialize(loReader);
            }
            finally
            {
                loReader.Dispose();
            }

            if (loTypeExpected == typeof(MaxIndex) && null != loR)
            {
                MaxIndex loResult = new MaxIndex();
                foreach (MaxIndexItemStructure loItem in (MaxIndexItemStructure[])loR)
                {
                    if (!loItem.Key.EndsWith(_sSerialTypeKeySuffix))
                    {
                        loResult.Add(loItem.Key, loItem.Value);
                    }
                }

                loR = loResult;
            }

            return loR;
        }

        /// <summary>
        /// Serializes an object to a string using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to serialize.</param>
        /// <returns>string to represent object</returns>
        public virtual string SerializeToJsonConditional(object loObject)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes text to an object using the provider associated with the Type.
        /// </summary>
        /// <param name="lsSerializationText">Text that represents the serialized object.</param>
        /// <param name="loTypeExpected">Type of object expected to be deserialized into.</param>
        /// <returns>object version of serialized text</returns>
        public virtual object DeserializeFromJsonConditional(string lsSerializationText, Type loTypeExpected)
        {
            throw new NotImplementedException();
        }

#elif netcore1 || netstandard1_2
        protected bool IsNativeConditional(object loValue)
        {
            if (loValue is string || loValue is long || loValue is bool || (loValue is double && loValue.ToString().Contains(".")))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Serializes an object to a string using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to serialize.</param>
        /// <returns>string to represent object</returns>
        public virtual string SerializeToStringConditional(object loObject)
        {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes text to an object using the provider associated with the Type.
        /// </summary>
        /// <param name="lsSerializationText">Text that represents the serialized object.</param>
        /// <param name="loTypeExpected">Type of object expected to be deserialized into.</param>
        /// <returns>object version of serialized text</returns>
        public virtual object DeserializeFromStringConditional(string lsSerializationText, Type loTypeExpected)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serializes an object to a string using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to serialize.</param>
        /// <returns>string to represent object</returns>
        public virtual string SerializeToXmlConditional(object loObject)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes text to an object using the provider associated with the Type.
        /// </summary>
        /// <param name="lsSerializationText">Text that represents the serialized object.</param>
        /// <param name="loTypeExpected">Type of object expected to be deserialized into.</param>
        /// <returns>object version of serialized text</returns>
        public virtual object DeserializeFromXmlConditional(string lsSerializationText, Type loTypeExpected)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serializes an object to a string using the provider associated with the Type.
        /// </summary>
        /// <param name="loObject">Object to serialize.</param>
        /// <returns>string to represent object</returns>
        public virtual string SerializeToJsonConditional(object loObject)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes text to an object using the provider associated with the Type.
        /// </summary>
        /// <param name="lsSerializationText">Text that represents the serialized object.</param>
        /// <param name="loTypeExpected">Type of object expected to be deserialized into.</param>
        /// <returns>object version of serialized text</returns>
        public virtual object DeserializeFromJsonConditional(string lsSerializationText, Type loTypeExpected)
        {
            throw new NotImplementedException();
        }
#endif


    }
}
