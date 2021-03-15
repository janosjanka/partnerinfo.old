// Copyright (c) János Janka. All rights reserved.

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Provides URL validation. (UrlAttribute is buggy in .NET 4.5).
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class UrlValidatorAttribute : ValidationAttribute
    {
        /// <summary>
        /// Determines whether the specified value of the object is valid.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <returns>
        /// true if the specified value is valid; otherwise, false.
        /// </returns>
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            string url = (string)value;

            if ((url.Length > 0) && (char.IsWhiteSpace(url[0]) || char.IsWhiteSpace(url[url.Length - 1])))
            {
                return false;
            }

            return Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute);
        }
    }
}
