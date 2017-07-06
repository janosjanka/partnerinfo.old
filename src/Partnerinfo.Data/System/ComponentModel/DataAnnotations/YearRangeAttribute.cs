// Copyright (c) János Janka. All rights reserved.

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Specifies the year range constraints for the value of a data field.
    /// </summary>
    public class YearRangeAttribute : RangeAttribute
    {
        public YearRangeAttribute(int relativeFrom, int relativeTo)
            : base(
                typeof(DateTime),
                DateTime.UtcNow.AddYears(relativeFrom).ToShortDateString(),
                DateTime.UtcNow.AddYears(relativeTo).ToShortDateString())
        {
        }
    }
}
