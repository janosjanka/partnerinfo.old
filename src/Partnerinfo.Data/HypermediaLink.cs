// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo
{
    /// <summary>
    /// Represents a hypermedia link from the containing resource to a URI.
    /// http://tools.ietf.org/html/draft-kelly-json-hal-06
    /// </summary>
    public sealed class HypermediaLink
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HypermediaLink"/> class.
        /// </summary>
        public HypermediaLink()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HypermediaLink" /> class.
        /// </summary>
        /// <param name="href">A URI or a URL template.</param>
        /// <exception cref="System.ArgumentNullException">href</exception>
        public HypermediaLink(string href)
        {
            Href = href;
        }

        /// <summary>
        /// A URI or a URI template
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// Indicates whether the <see cref="Href" /> is a URI template: "/orders{?id}"
        /// </summary>
        public bool Templated { get; set; }

        /// <summary>
        /// A hint to indicate the media type expected when dereferencing the target resource
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// A URL that should provide further information about the deprecation
        /// </summary>
        public string Deprecation { get; set; }

        /// <summary>
        /// A secondary key for selecting Link Objects which share the same relation type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A URI that hints about the profile of the target resource
        /// </summary>
        public string Profile { get; set; }

        /// <summary>
        /// Human-readable identifier for this link
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Indicates the language of the target resource
        /// </summary>
        public string Hreflang { get; set; }
    }
}
