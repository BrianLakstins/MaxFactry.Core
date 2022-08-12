// <copyright file="MaxMetaLibraryDefaultProvider.cs" company="Lakstins Family, LLC">
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
// <change date="6/8/2016" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.Core.Provider
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using MaxFactry.Core;

    /// <summary>
    /// Library to provide encryption functionality
    /// </summary>
    public class MaxMetaLibraryDefaultProvider : MaxProvider, IMaxMetaLibraryProvider
    {
        /// <summary>
        /// Initializes a new instance of the MaxMetaLibraryDefaultProvider class
        /// </summary>
        public MaxMetaLibraryDefaultProvider()
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
        /// Gets the key for the property
        /// </summary>
        /// <param name="loObject">Object that contains the property</param>
        /// <param name="lsProperty">Name of the property</param>
        /// <returns>Key to use for storing meta data related to the property</returns>
        public virtual string GetKey(object loObject, string lsProperty)
        {
            string lsR = loObject.GetType().ToString() + "." + lsProperty;
            string lsRConditional = this.GetKeyConditional(loObject, lsProperty);
            if (null != lsRConditional)
            {
                lsR = lsRConditional;
            }

            return lsR;
        }

        /// <summary>
        /// Gets some text related to the property
        /// </summary>
        /// <param name="loObject">Object that contains the property</param>
        /// <param name="lsProperty">Name of the property</param>
        /// <param name="lsMetaName">Name of the text to get for the property</param>
        /// <returns>String of text related to the property</returns>
        public virtual string GetMetaText(object loObject, string lsProperty, string lsMetaName)
        {
            string lsR = string.Empty;
            string lsKey = this.GetKey(loObject, lsProperty);
            if (lsKey != null && lsKey != string.Empty)
            {
                lsKey = lsMetaName + ":" + lsKey;
                lsR = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeAny, lsKey) as string;
            }

            if (null == lsR || lsR == string.Empty)
            {
                lsR = this.GetMetaTextConditional(loObject, lsProperty, lsMetaName);
            }

            return lsR;
        }

        /// <summary>
        /// Gets a list of properties that have some kind of meta display information associated with them
        /// </summary>
        /// <param name="loObject">Object that contains the properties</param>
        /// <returns>List of property names</returns>
        public virtual MaxIndex GetPropertyDisplayList(object loObject)
        {
            MaxIndex loR = this.GetPropertyDisplayListConditional(loObject);
            return loR;
        }


#if net2  || netcore1 || netstandard1_2
        /// <summary>
        /// Gets the key for the property
        /// </summary>
        /// <param name="loObject">Object that contains the property</param>
        /// <param name="lsProperty">Name of the property</param>
        /// <returns>Key to use for storing meta data related to the property</returns>
        protected virtual string GetKeyConditional(object loObject, string lsProperty)
        {
            string lsR = loObject.GetType().ToString() + ".";
            string lsKey = lsProperty;
            MaxIndex loPropertyIndex = MaxFactryLibrary.GetPropertyList(loObject);
            object loProperty = loPropertyIndex[lsProperty];
            if (null != loProperty && loProperty is PropertyInfo)
            {
                object[] laDisplayAttribute = this.GetCustomAttributesConditional((PropertyInfo)loProperty);
                if (null != laDisplayAttribute && laDisplayAttribute.Length == 1)
                {
                    if (!string.IsNullOrEmpty(((MaxMetaAttribute)laDisplayAttribute[0]).Key))
                    {
                        lsKey = loObject.GetType().ToString() + "." + ((MaxMetaAttribute)laDisplayAttribute[0]).Key;
                    }
                    else if (!string.IsNullOrEmpty(((MaxMetaAttribute)laDisplayAttribute[0]).Name))
                    {
                        lsKey = loObject.GetType().ToString() + "." + ((MaxMetaAttribute)laDisplayAttribute[0]).Name;
                    }
                }
            }

            lsR += lsKey;
            return lsR;
        }

        /// <summary>
        /// Gets some text related to the property
        /// </summary>
        /// <param name="loObject">Object that contains the property</param>
        /// <param name="lsProperty">Name of the property</param>
        /// <param name="lsMetaName">Name of the text to get for the property</param>
        /// <returns>String of text related to the property</returns>
        protected virtual string GetMetaTextConditional(object loObject, string lsProperty, string lsMetaName)
        {
            string lsR = string.Empty;
            MaxIndex loPropertyIndex = MaxFactryLibrary.GetPropertyList(loObject);
            object loProperty = loPropertyIndex[lsProperty];
            if (null != loProperty && loProperty is PropertyInfo)
            {
                if (lsMetaName.StartsWith("MaxMeta"))
                {
                    object[] laAttribute = this.GetCustomAttributesConditional((PropertyInfo)loProperty);
                    if (null != laAttribute && laAttribute.Length == 1)
                    {
                        if (lsMetaName.EndsWith(".Key"))
                        {
                            lsR = ((MaxMetaAttribute)laAttribute[0]).Key;
                        }
                        else if (lsMetaName.EndsWith(".Description"))
                        {
                            lsR = ((MaxMetaAttribute)laAttribute[0]).Description;
                        }
                        else if (lsMetaName.EndsWith(".Increment"))
                        {
                            lsR = ((MaxMetaAttribute)laAttribute[0]).Increment;
                        }
                        else if (lsMetaName.EndsWith(".Label"))
                        {
                            lsR = ((MaxMetaAttribute)laAttribute[0]).Label;
                        }
                        else if (lsMetaName.EndsWith(".ListHeader"))
                        {
                            lsR = ((MaxMetaAttribute)laAttribute[0]).ListHeader;
                        }
                        else if (lsMetaName.EndsWith(".Maximum"))
                        {
                            lsR = ((MaxMetaAttribute)laAttribute[0]).Maximum;
                        }
                        else if (lsMetaName.EndsWith(".Name"))
                        {
                            lsR = ((MaxMetaAttribute)laAttribute[0]).Name;
                        }
                        else if (lsMetaName.EndsWith(".Group"))
                        {
                            lsR = ((MaxMetaAttribute)laAttribute[0]).Group;
                        }
                        else if (lsMetaName.EndsWith(".Hint"))
                        {
                            lsR = ((MaxMetaAttribute)laAttribute[0]).Hint;
                        }
                        else if (lsMetaName.EndsWith(".Minimum"))
                        {
                            lsR = ((MaxMetaAttribute)laAttribute[0]).Minimum;
                        }
                        else if (lsMetaName.EndsWith(".DisplayOrder"))
                        {
                            lsR = ((MaxMetaAttribute)laAttribute[0]).DisplayOrder.ToString();
                        }
                        else if (lsMetaName.EndsWith(".Format"))
                        {
                            lsR = ((MaxMetaAttribute)laAttribute[0]).Format;
                        }
                    }
                }
            }

            return lsR;
        }

        /// <summary>
        /// Gets a list of properties that have some kind of meta display information associated with them
        /// </summary>
        /// <param name="loObject">Object that contains the properties</param>
        /// <returns>List of property names</returns>
        protected virtual MaxIndex GetPropertyDisplayListConditional(object loObject)
        {
            MaxIndex loR = new MaxIndex();
            MaxIndex loPropertyIndex = MaxFactryLibrary.GetPropertyList(loObject);
            string[] laKey = loPropertyIndex.GetSortedKeyList();
            foreach (string lsKey in laKey)
            {
                object loProperty = loPropertyIndex[lsKey];
                if (null != loProperty && loProperty is PropertyInfo)
                {
                    object[] laDisplayAttribute = this.GetCustomAttributesConditional((PropertyInfo)loProperty);
                    if (null != laDisplayAttribute && laDisplayAttribute.Length == 1)
                    {
                        if (!string.IsNullOrEmpty(((MaxMetaAttribute)laDisplayAttribute[0]).Name))
                        {
                            loR.Add(MaxConvertLibrary.ConvertToSortString(typeof(object), ((MaxMetaAttribute)laDisplayAttribute[0]).DisplayOrder) + Guid.NewGuid().ToString(), lsKey);
                        }
                    }
                }
            }

            return loR;
        }
#endif

#if netcore2
        protected virtual object[] GetCustomAttributesConditional(PropertyInfo loProperty)
        {
            return loProperty.GetCustomAttributes(typeof(MaxMetaAttribute), true);
        }
#elif netstandard1_4 || netcore1
        protected virtual object[] GetCustomAttributesConditional(PropertyInfo loProperty)
        {
            System.Collections.Generic.IEnumerable<Attribute> loList = loProperty.GetCustomAttributes(typeof(MaxMetaAttribute), true);
            System.Collections.IEnumerator loEnumerator = loList.GetEnumerator();
            System.Collections.Generic.List<object> loR = new System.Collections.Generic.List<object>();
            while (loEnumerator.MoveNext())
            {
                loR.Add(loEnumerator.Current);
            }

            return loR.ToArray();
        }
#elif netstandard1_2 || net2
        protected virtual object[] GetCustomAttributesConditional(PropertyInfo loProperty)
        {
            return loProperty.GetCustomAttributes(typeof(MaxMetaAttribute), true);
        }
#endif
    }
}
