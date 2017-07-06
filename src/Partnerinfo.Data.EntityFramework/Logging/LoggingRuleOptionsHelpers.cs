// Copyright (c) János Janka. All rights reserved.

using Newtonsoft.Json;

namespace Partnerinfo.Logging.EntityFramework
{
    internal static class LoggingRuleOptionsHelpers
    {
        /// <summary>
        /// Serializes a <see cref="LoggingRuleOptions" /> object to JSON.
        /// </summary>
        /// <param name="options">The <see cref="LoggingRuleOptions" /> to serialize.</param>
        /// <returns>
        /// The JSON text.
        /// </returns>
        public static string Serialize(LoggingRuleOptions options)
        {
            return JsonConvert.SerializeObject(options, Formatting.None, JsonNetUtility.Settings);
        }

        /// <summary>
        /// Deserializes a JSON text to a <see cref="LoggingRuleOptions" /> object.
        /// </summary>
        /// <param name="json">The JSON text to deserialize.</param>
        /// <returns>
        /// The <see cref="LoggingRuleOptions" /> object.
        /// </returns>
        public static LoggingRuleOptions Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<LoggingRuleOptions>(json, JsonNetUtility.Settings);
        }
    }
}
