// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Partnerinfo.Portal;

namespace Partnerinfo.Input.Processors
{
    public class PageModuleCommandProcessor : ICommandProcessor
    {
        /// <summary>
        /// Called by the runtime to execute a command.
        /// </summary>
        /// <param name="context">The <see cref="CommandContext" /> to associate with this command and execution.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="CommandResult" /> of the run task, which determines whether the command remains in the executing state, or transitions to the closed state.
        /// </returns>
        public virtual async Task<CommandResult> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            if (context.Command.Object.Id == null)
            {
                return context.CreateResult(CommandStatusCode.NoAction);
            }
            var services = context.Resolve<PortalManager>();
            var page = await GetPageAsync(services, context.Command.Object.Id, cancellationToken);
            if (string.IsNullOrEmpty(page?.HtmlContent))
            {
                return context.CreateResult(CommandStatusCode.NoAction);
            }
            var editor = new PageModuleEditor(page.HtmlContent);
            if (editor.IsEmpty)
            {
                return context.CreateResult(CommandStatusCode.NoAction);
            }
            var element = editor.GetElementById(context.Command.Object.Object.Id);
            if (element == null)
            {
                return context.CreateResult(CommandStatusCode.NoAction);
            }
            switch (context.Command.Line)
            {
                case "UPDATE": UpdateModule(context, editor, element); break;
                case "DELETE": DeleteModule(context, editor, element); break;
                case "INSERT": InsertModule(context, editor, element); break;
                default: return context.CreateResult(CommandStatusCode.NoAction);
            }
            page.HtmlContent = editor.ToString();
            //await services.Portal.SetPageContentAsync(page.Id, page.HtmlContent, page.StyleContent, cancellationToken);
            //await services.SaveAsync(cancellationToken);
            return context.CreateResult(CommandStatusCode.NoAction);
        }

        /// <summary>
        /// Inserts a new module.
        /// </summary>
        private void InsertModule(CommandContext context, PageModuleEditor editor, HtmlNode element)
        {
        }

        /// <summary>
        /// Updates a HTML module.
        /// </summary>
        private void UpdateModule(CommandContext context, PageModuleEditor editor, HtmlNode element)
        {
            if (editor.IsTypeOf(element, PageModuleTypeClass.Html))
            {
                UpdateHtmlModule(context, editor, element);
            }
            else if (editor.IsTypeOf(element, PageModuleTypeClass.Image))
            {
                UpdateImageModule(context, editor, element);
            }
        }

        /// <summary>
        /// Updates a HTML module.
        /// </summary>
        private void UpdateHtmlModule(CommandContext context, PageModuleEditor editor, HtmlNode element)
        {
            editor.SetModuleContent(element, context.Command.HtmlContent);
        }

        /// <summary>
        /// Updates an image module.
        /// </summary>
        private void UpdateImageModule(CommandContext context, PageModuleEditor editor, HtmlNode element)
        {
            string imageUrl = string.IsNullOrWhiteSpace(context.Command.TextContent) ? null : context.Command.TextContent.Trim();
            dynamic moduleOptions = editor.GetModuleOptions(element);
            if (moduleOptions == null)
            {
                moduleOptions = new { image = new { url = imageUrl } };
            }
            else if (moduleOptions.image == null)
            {
                moduleOptions.image = new { url = imageUrl };
            }
            else
            {
                moduleOptions.image.url = imageUrl;
            }
            editor.SetModuleOptions(element, moduleOptions);
        }

        /// <summary>
        /// Updates a HTML module.
        /// </summary>
        private void DeleteModule(CommandContext context, PageModuleEditor editor, HtmlNode element)
        {
            editor.Delete(element);
        }

        /// <summary>
        /// Gets a page using the given ID that can be both a integer or a string value.
        /// </summary>
        private Task<PageItem> GetPageAsync(PortalManager manager, string id, CancellationToken cancellationToken)
        {
            int pageId;
            if (int.TryParse(id, out pageId))
            {
                //return services.Portal.FindPageAsync(pageId, cancellationToken);
            }
            var uriParts = id.Split(new[] { '/' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (uriParts.Length == 0)
            {
                return Task.FromResult<PageItem>(null);
            }
            //return services.Portal.FindPageAsync(uriParts[0], uriParts.Length > 1 ? uriParts[1] : default(string), cancellationToken);
            return Task.FromResult<PageItem>(null);
        }
    }
}
