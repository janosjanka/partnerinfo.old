// Copyright (c) János Janka. All rights reserved.

using System.Text;

namespace Partnerinfo.Input
{
    public class Command
    {
        /// <summary>
        /// Command line text
        /// </summary>
        public string Line { get; set; }

        /// <summary>
        /// The victim of the command
        /// </summary>
        public CommandObject Object { get; set; }

        /// <summary>
        /// Html Content
        /// </summary>
        public string HtmlContent { get; set; }

        /// <summary>
        /// Text Content
        /// </summary>
        public string TextContent { get; set; }

        /// <summary>
        /// Converts the value of this instance to a <see cref="System.String" />.
        /// </summary>
        /// <returns>
        /// A string whose value is the same as this instance.
        /// </returns>
        public override string ToString()
        {
            var obj = Object;
            var str = new StringBuilder(Line);
            str.Append(" ! ");
            while (obj != null)
            {
                str.Append(obj);
                if ((obj = obj.Object) == null)
                {
                    break;
                }
                str.Append(" >> ");
            }
            return str.ToString();
        }
    }
}
