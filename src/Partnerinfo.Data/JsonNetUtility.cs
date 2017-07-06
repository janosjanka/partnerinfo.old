// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Partnerinfo
{
    public static class JsonNetUtility
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
#if DEBUG
            Formatting = Formatting.Indented,
#else
            Formatting = Formatting.None,
#endif
            MissingMemberHandling = MissingMemberHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter { CamelCaseText = true },
                new IsoDateTimeConverter() // DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ"
            }
        };

        /// <summary>
        /// Serialize the JSON representation of the object to XML format.
        /// </summary>
        /// <param name="obj">The JSON object to serialize to XML.</param>
        /// <param name="root">The name of the XML root element.</param>
        /// <returns>
        /// The XML representation of the JSON object.
        /// </returns>
        public static string JObjectToXml(JObject obj, string root = "options")
        {
            if (obj != null)
            {
                string json = obj.ToString(Formatting.None);
                if (json != null)
                {
                    var document = JsonConvert.DeserializeXNode(json, root);
                    if (document.Root.HasElements)
                    {
                        return document.ToString(SaveOptions.None);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Serialize an object to XML format.
        /// </summary>
        /// <param name="obj">The object to serialize to XML.</param>
        /// <param name="root">The name of the XML root element.</param>
        /// <returns>
        /// The XML representation of the JSON object.
        /// </returns>
        public static string ObjectToXml(object obj, string root = "options")
        {
            if (obj != null)
            {
                string json = JsonConvert.SerializeObject(obj, Formatting.None, Settings);
                if (json != null)
                {
                    var document = JsonConvert.DeserializeXNode(json, root);
                    if (document.Root.HasElements)
                    {
                        return document.ToString(SaveOptions.None);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Serialize the XML representation of the object to JSON format.
        /// </summary>
        /// <param name="xml">The XML string to serialize to JObject.</param>
        /// <returns>The <see cref="JObject" /> representation of the XML string.</returns>
        public static JObject XmlToJObject(string xml)
        {
            if (xml != null)
            {
                var document = XDocument.Parse(xml);
                if (document.Root.HasElements)
                {
                    string json = JsonConvert.SerializeXNode(document, Formatting.None, true);
                    if (json != null)
                    {
                        return JObject.Parse(json);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Serialize the XML representation of the object to an object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="xml">The XML string to serialize to an object.</param>
        /// <returns>An object.</returns>
        public static T XmlToObject<T>(string xml)
        {
            if (xml != null)
            {
                var document = XDocument.Parse(xml);
                if (document.Root.HasElements)
                {
                    string json = JsonConvert.SerializeXNode(document, Formatting.None, true);
                    if (json != null)
                    {
                        return JsonConvert.DeserializeObject<T>(json, Settings);
                    }
                }
            }
            return default(T);
        }
    }
}
