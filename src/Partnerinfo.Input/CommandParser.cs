// Copyright (c) János Janka. All rights reserved.

using System.Text.RegularExpressions;

namespace Partnerinfo.Input
{
    public class CommandParser : ICommandParser
    {
        //
        // UPDATE ! PAGE: panorama/koszonjuk >> MODULE: id
        // UPDATE!PAGE: panorama/koszonjuk >> MODULE: id
        // UPDATE ! PAGE: panorama/koszonjuk
        // UPDATE!PAGE: panorama
        //
        private static readonly Regex s_regex = new Regex(@"^(?<command>\w+)\s*!(\s*((?<type>\w+)\s*:\s*(?<id>\S+)\s*>{0,2}))+$", RegexOptions.Compiled);

        /// <summary>
        /// An instance of the <see cref="CommandParser" /> class that is used by the <see cref="CommandInvoker" /> class.
        /// </summary>
        public static readonly CommandParser Default = new CommandParser();

        /// <summary>
        /// Parses the given text.
        /// </summary>
        /// <param name="commandText">The command text to parse.</param>
        /// <param name="htmlContent">The html content to parse.</param>
        /// <param name="textContent">The text content to parse.</param>
        /// <returns>
        /// A <see cref="Command" /> object.
        /// </returns>
        public virtual Command Parse(string commandText, string htmlContent = null, string textContent = null)
        {
            if (commandText == null)
            {
                return new Command { HtmlContent = htmlContent, TextContent = textContent };
            }
            var match = s_regex.Match(commandText);
            if (!match.Success)
            {
                throw new CommandParserException("The header is corrupted.");
            }
            var txtCommand = new Command { Line = match.Groups["command"].Value, HtmlContent = htmlContent, TextContent = textContent };
            var txtProperty = txtCommand.Object = new CommandObject();
            var types = match.Groups["type"];
            var ids = match.Groups["id"];
            for (int i = 0, j = types.Captures.Count - 1; i <= j; ++i)
            {
                txtProperty.Type = types.Captures[i].Value;
                txtProperty.Id = ids.Captures[i].Value;
                if (i < j)
                {
                    txtProperty = txtProperty.Object = new CommandObject();
                }
            }
            return txtCommand;
        }
    }
}
