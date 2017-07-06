// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Portal
{
    public class PortalCompilerResult
    {
        /// <summary>
        /// Gets or sets the master <see cref="PageItem" />.
        /// </summary>
        /// <value>
        /// The master <see cref="PageItem" />.
        /// </value>
        public PageItem MasterPage { get; }

        /// <summary>
        /// Gets or sets the content <see cref="PageItem" />.
        /// </summary>
        /// <value>
        /// The content <see cref="PageItem" />.
        /// </value>
        public PageItem ContentPage { get; }

        /// <summary>
        /// Gets or sets the compiled <see cref="PageItem" />.
        /// </summary>
        /// <value>
        /// The compiled <see cref="PageItem" />.
        /// </value>
        public PageItem CompiledPage { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalCompilerResult" /> class.
        /// </summary>
        /// <param name="masterPage">The master <see cref="PageItem" />.</param>
        /// <param name="contentPage">The content <see cref="PageItem" />.</param>
        /// <param name="compiledPage">The compiled <see cref="PageItem" />.</param>
        public PortalCompilerResult(PageItem masterPage, PageItem contentPage, PageItem compiledPage)
        {
            MasterPage = masterPage;
            ContentPage = contentPage;
            CompiledPage = compiledPage;
        }
    }
}
