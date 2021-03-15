// Copyright (c) János Janka. All rights reserved.

using System.Collections.Immutable;

namespace Partnerinfo.Portal
{
    /// <summary>
    /// Represents compilation options for portals.
    /// </summary>
    public class PortalCompilerOptions
    {
        /// <summary>
        /// Gets or sets the <see cref="PortalItem" /> which owns the page(s).
        /// </summary>
        /// <value>
        /// The <see cref="PortalItem" />.
        /// </value>
        public PortalItem Portal { get; }

        /// <summary>
        /// Gets or sets the content <see cref="PageItem" />.
        /// </summary>
        /// <value>
        /// The content <see cref="PageItem" />.
        /// </value>
        public PageItem ContentPage { get; }

        /// <summary>
        /// Gets or sets a list of master <see cref="PageItem" />s.
        /// </summary>
        /// <value>
        /// A list of master <see cref="PageItem" />s.
        /// </value>
        public ImmutableArray<PageItem> MasterPages { get; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public ImmutableDictionary<string, object> Properties { get; }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        /// <value>
        /// The flags.
        /// </value>
        public PortalCompilerFlags CompilerFlags { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalCompilerOptions" /> class.
        /// </summary>
        /// <param name="portal">The <see cref="PortalItem" /> which owns the page(s).</param>
        /// <param name="contentPage">The content <see cref="PageItem" />.</param>
        /// <param name="masterPages">The master pages.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="compilerFlags">Compiler flags.</param>
        public PortalCompilerOptions(
            PortalItem portal,
            PageItem contentPage,
            ImmutableArray<PageItem> masterPages = default(ImmutableArray<PageItem>),
            ImmutableDictionary<string, object> properties = null,
            PortalCompilerFlags compilerFlags = PortalCompilerFlags.None)
        {
            Portal = portal;
            ContentPage = contentPage;
            MasterPages = masterPages;
            Properties = properties ?? ImmutableDictionary<string, object>.Empty;
            CompilerFlags = compilerFlags;
        }
    }
}
