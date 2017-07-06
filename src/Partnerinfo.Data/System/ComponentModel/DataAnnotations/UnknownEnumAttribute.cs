// Copyright (c) János Janka. All rights reserved.

namespace System.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class UnknownEnumAttribute : ValidationAttribute
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
                return false;
            }
            return !string.Equals(Enum.GetName(value.GetType(), value), "Unknown", StringComparison.Ordinal);
        }
    }
}
