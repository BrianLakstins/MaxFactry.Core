// <copyright file="MaxMetaAttribute.cs" company="Lakstins Family, LLC">
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
// <change date="6/10/2016" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.Core
{
    using System;

    /// <summary>
    /// Attribute to add information to properties
    /// Custom attributes are not supported on .NET Micro Framework. Some of the system attributes, which do not get stripped by compiler, are accessible using properties, like yourType.IsSerializable.
    /// <see href="http://informatix.miloush.net/microframework/FAQ.aspx?Core%2fReflection"/> 
    /// </summary>
    public class MaxMetaAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the MaxMetaAttribute class.
        /// </summary>
        public MaxMetaAttribute()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets the key used to store information about a property
        /// </summary>
        public string Key
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the a label for the property
        /// </summary>
        public string Label
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of a property (normally matches property name)
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a header for this property when shown in a list
        /// </summary>
        public string ListHeader
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a description for the property
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a maximum value for the property
        /// </summary>
        public string Maximum
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a minimum value for the property
        /// </summary>
        public string Minimum
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the amount a property is incremented when changed
        /// </summary>
        public string Increment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the relative order this property should display when showing multiple properties
        /// </summary>
        public double DisplayOrder
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a hint to display to the user when this property is requested
        /// </summary>
        public string Hint
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a group to use for this property
        /// </summary>
        public string Group
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a format to use for this property
        /// </summary>
        public string Format
        {
            get;
            set;
        }
    }
}