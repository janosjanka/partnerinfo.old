// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Partnerinfo.Portal
{
    public class PageLayers
    {
        /// <summary>
        /// A <see cref="PageLayers" /> instance.
        /// </summary>
        public static readonly PageLayers Empty = new PageLayers();

        /// <summary>
        /// Gets or sets the content <see cref="PageItem" />.
        /// </summary>
        /// <value>
        /// The content <see cref="PageItem" />.
        /// </value>
        public PageItem ContentPage { get; }

        /// <summary>
        /// Gets or sets the master <see cref="PageItem" />s.
        /// </summary>
        /// <value>
        /// The master <see cref="PageItem" />s.
        /// </value>
        public ImmutableArray<PageItem> MasterPages { get; }

        /// <summary>
        /// Prevents a default instance of the <see cref="PageLayers" /> class from being created.
        /// </summary>
        internal PageLayers()
        {
            MasterPages = ImmutableArray<PageItem>.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageLayers" /> class.
        /// </summary>
        /// <param name="contentPage">The content page.</param>
        /// <param name="masterPages">The master pages.</param>
        internal PageLayers(PageItem contentPage, ImmutableArray<PageItem> masterPages)
        {
            ContentPage = contentPage;
            MasterPages = masterPages;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PageLayers" /> class or returns a cached version of the immutable object.
        /// </summary>
        /// <param name="contentPage">The content page.</param>
        /// <returns>
        /// The <see cref="PageLayers" />.
        /// </returns>
        public static PageLayers Create(PageItem contentPage)
        {
            if (contentPage == null)
            {
                return Empty;
            }
            return new PageLayers(contentPage, ImmutableArray<PageItem>.Empty);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PageLayers" /> class or returns a cached version of the immutable object.
        /// </summary>
        /// <param name="contentPage">The content page.</param>
        /// <param name="masterPages">The master pages.</param>
        /// <returns>
        /// The <see cref="PageLayers" />.
        /// </returns>
        public static PageLayers Create(PageItem contentPage, IEnumerable<PageItem> masterPages)
        {
            if (contentPage == null)
            {
                return Empty;
            }
            if (masterPages == null)
            {
                throw new ArgumentNullException(nameof(masterPages));
            }
            return new PageLayers(contentPage, masterPages.ToImmutableArray());
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PageLayers" /> class or returns a cached version of the immutable object.
        /// </summary>
        /// <param name="contentPage">The content page.</param>
        /// <param name="masterPages">The master pages.</param>
        /// <returns>
        /// The <see cref="PageLayers" />.
        /// </returns>
        public static PageLayers Create(PageItem contentPage, ImmutableArray<PageItem> masterPages)
        {
            if (contentPage == null)
            {
                return Empty;
            }
            return new PageLayers(contentPage, masterPages);
        }
    }
}
