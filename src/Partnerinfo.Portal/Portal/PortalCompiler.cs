// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Project.Templating;

namespace Partnerinfo.Portal
{
    public class PortalCompiler
    {
        private readonly StringTemplate _stringTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalCompiler" /> class.
        /// </summary>
        public PortalCompiler(StringTemplate stringTemplate)
        {
            if (stringTemplate == null)
            {
                throw new ArgumentNullException(nameof(stringTemplate));
            }
            _stringTemplate = stringTemplate;
        }

        /// <summary>
        /// Compiles a page and returns with the result.
        /// </summary>
        /// <param name="options">The compiler options that can be used to configure compilation.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.InvalidOperationException">Missing compiler options.</exception>
        public virtual async Task<PortalCompilerResult> CompileAsync(PortalCompilerOptions options, CancellationToken cancellationToken)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.Portal == null || options.ContentPage == null)
            {
                throw new InvalidOperationException("Missing compiler options.");
            }

            PageItem contentPage = options.ContentPage;
            PageItem masterPage = await MergeAsync(options.MasterPages, cancellationToken);
            PageItem compiledPage;

            if (options.CompilerFlags.HasFlag(PortalCompilerFlags.MergeMasterAndContent) && masterPage != null)
            {
                compiledPage = await MergeAsync(ImmutableArray.Create(masterPage, contentPage), cancellationToken);
            }
            else
            {
                compiledPage = contentPage;
            }

            if (options.CompilerFlags.HasFlag(PortalCompilerFlags.InterpolateHtmlContent))
            {
                await InterpolateAsync(compiledPage, options.Properties, cancellationToken);
            }

            return new PortalCompilerResult(masterPage, contentPage, compiledPage);
        }

        /// <summary>
        /// Merges the specified <paramref name="pages" />.
        /// </summary>
        /// <param name="pages">The pages to merge.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The merged page.
        /// </returns>
        protected virtual Task<PageItem> MergeAsync(ImmutableArray<PageItem> pages, CancellationToken cancellationToken)
        {
            var page = pages.FirstOrDefault();
            if (page == null)
            {
                return Task.FromResult(default(PageItem));
            }

            var name = page.Name;
            var description = page.Description;
            var htmlContent = new StringBuilder(page.HtmlContent ?? string.Empty);
            var styleContent = new StringBuilder(page.StyleContent ?? string.Empty);
            var references = new List<ReferenceItem>(page.References);

            foreach (var currentPage in pages.Skip(1))
            {
                if (currentPage.Name != null)
                {
                    name = currentPage.Name;
                }
                if (currentPage.Description != null)
                {
                    description = currentPage.Description;
                }

                htmlContent.Replace("<!-- content-place-holder -->", currentPage.HtmlContent ?? string.Empty);

                if (currentPage.StyleContent != null)
                {
                    styleContent.Append(currentPage.StyleContent);
                }

                references.AddRange(currentPage.References);
            }

            page = pages.Last();
            page = new PageItem
            {
                Id = page.Id,
                Uri = page.Uri,
                Name = name,
                Description = description,
                HtmlContent = htmlContent.ToString(),
                StyleContent = styleContent.ToString()
            };

            foreach (var currentRef in references)
            {
                page.References.Add(currentRef);
            }

            return Task.FromResult(page);
        }

        /// <summary>
        /// Interpolates the content.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        protected virtual async Task InterpolateAsync(PageItem page, ImmutableDictionary<string, object> properties, CancellationToken cancellationToken)
        {
            Debug.Assert(page != null);
            Debug.Assert(properties != null);

            page.HtmlContent = await _stringTemplate.InterpolateAsync(page.HtmlContent, properties, cancellationToken);
        }
    }
}
