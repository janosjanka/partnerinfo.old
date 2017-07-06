// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Xml.Linq;

namespace Partnerinfo.Portal.EntityFramework
{
    /// <summary>
    /// Serializes or deserializes <see cref="ReferenceItem" />s. 
    /// </summary>
    internal static class ReferenceSerializer
    {
        private const string ListElementName = "references";
        private const string ItemElementName = "reference";

        /// <summary>
        /// Deserializes a <see cref="string" /> to a list of <see cref="ReferenceItem" />s.
        /// </summary>
        /// <param name="references">The references to deserialize.</param>
        /// <returns>
        /// A list of <see cref="ReferenceItem" />s.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        public static ImmutableArray<ReferenceItem> Deserialize(string references)
        {
            if (references == null)
            {
                throw new ArgumentNullException(nameof(references));
            }

            var xReferences = XElement.Parse(references);
            if (xReferences.IsEmpty)
            {
                return ImmutableArray<ReferenceItem>.Empty;
            }

            var referenceList = ImmutableArray.CreateBuilder<ReferenceItem>();

            // This method is perfomance-critical so we avoid using LINQ or
            // other built-in serialization mechanism.

            foreach (var xReference in xReferences.Elements(ItemElementName))
            {
                referenceList.Add(ReferenceItem.Create(
                    type: (string)xReference.Attribute("type"),
                    uri: (string)xReference.Attribute("uri")));
            }

            return referenceList.ToImmutable();
        }

        /// <summary>
        /// Serializes a list of <see cref="ReferenceItem" />s.
        /// </summary>
        /// <param name="references">The references to serialize.</param>
        /// <returns>
        /// A <see cref="string" /> representing the references.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static string Serialize(IEnumerable<ReferenceItem> references)
        {
            if (references == null)
            {
                throw new ArgumentNullException(nameof(references));
            }

            var xReferences = new XElement(ListElementName);

            // This method is perfomance-critical so we avoid using LINQ or
            // other built-in serialization mechanism.

            foreach (var reference in references)
            {
                xReferences.Add(new XElement(ItemElementName,
                    new XAttribute("type", reference.Type),
                    new XAttribute("uri", reference.Uri)));
            }

            return xReferences.ToString(SaveOptions.DisableFormatting);
        }
    }
}
