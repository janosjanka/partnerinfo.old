// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Partnerinfo.Logging;
using Partnerinfo.Project.Actions;

namespace Partnerinfo.Project.Templating
{
    public class StringTemplate : IDisposable
    {
        private bool _disposed = false;

        /// <summary>
        /// Represents a reserved key that can be used to generate impersonated action links.
        /// </summary>
        public static readonly string IdentityIdProperty = "_ContactId";

        /// <summary>
        /// Defines a regex for parsing template expressions.
        ///     (1) {{ A.1000.4343/customuri }} - Action
        ///     (2) {{ #A.531 }}                - ActionLink (deprecated)
        ///     (3) {{ recipient.firstName }}
        /// </summary>
        private static readonly Regex s_exprRegex = new Regex(
            @"\{\{\s{0,1}(\S*?)\.(\S*?)\s{0,1}\}\}",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Represents a thread safe accessor cache for localized property name / value pairs.
        /// </summary>
        private StringTemplateCache _accessorCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringTemplate" /> class.
        /// </summary>
        public StringTemplate(IActionLinkService actionLinkService)
        {
            if (actionLinkService == null)
            {
                throw new ArgumentNullException(nameof(actionLinkService));
            }
            ActionLinkService = actionLinkService;
            _accessorCache = new StringTemplateCache();
        }

        /// <summary>
        /// Project manager
        /// </summary>
        protected IActionLinkService ActionLinkService { get; set; }

        /// <summary>
        /// Compiles a HTML template asynchronously using the specified compiler options.
        /// </summary>
        /// <param name="text">The text to interpolate.</param>
        /// <param name="properties">A set of key/value pairs.</param>
        /// <returns>
        /// A <see cref="String" /> that contains the interpolated text.
        /// </returns>
        public virtual Task<string> InterpolateAsync(string text, ImmutableDictionary<string, object> properties, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }
            _accessorCache.Put(properties);
            var accessors = MakeAccessors(properties);
            return Task.FromResult(ReplaceAll(text, properties, accessors));
        }

        /// <summary>
        /// Creates property accessors for the specified source types.
        /// </summary>
        private ImmutableDictionary<string, ObjectDescriptor> MakeAccessors(ImmutableDictionary<string, object> properties)
        {
            Debug.Assert(properties != null);

            return ImmutableDictionary.CreateRange(StringComparer.OrdinalIgnoreCase,
                from pair in properties
                where pair.Value != null
                select new KeyValuePair<string, ObjectDescriptor>(
                    pair.Key,
                    new ObjectDescriptor(pair.Value, _accessorCache.Get(pair.Value.GetType()))));
        }

        /// <summary>
        /// Replaces all the placeholders using the specified source object.
        /// </summary>
        /// <param name="template">The template to parse.</param>
        /// <param name="accessors">The accessor functions.</param>
        /// <param name="options">A set of key/value pairs that can be used to configure the HTML compiler.</param>
        /// <returns></returns>
        private string ReplaceAll(string template, ImmutableDictionary<string, object> options, ImmutableDictionary<string, ObjectDescriptor> accessors)
        {
            Debug.Assert(template != null);
            Debug.Assert(options != null);
            Debug.Assert(accessors != null);

            object temp;
            var hasContactId = options.TryGetValue(IdentityIdProperty, out temp);
            var contactId = hasContactId ? (int?)temp : null;

            template = s_exprRegex.Replace(template, match =>
            {
                if (match.Groups.Count >= 3)
                {
                    string type = match.Groups[1].Value;
                    string value = match.Groups[2].Value;

                    int actionLinkId;
                    if (type.Equals("#a", StringComparison.OrdinalIgnoreCase) && int.TryParse(value, out actionLinkId))
                    {
                        return ActionEventConverter.ActionLink(new ActionEventArgs(ObjectType.Action, actionLinkId, contactId));
                    }

                    ObjectDescriptor descriptor;
                    if (accessors.TryGetValue(type, out descriptor))
                    {
                        Func<object, object> accessor;
                        if (descriptor.Instance != null && descriptor.Accessors.TryGetValue(value, out accessor))
                        {
                            return accessor(descriptor.Instance)?.ToString() ?? string.Empty;
                        }
                        return string.Empty;
                    }
                }
                return string.Empty;
            });

            if (hasContactId)
            {
                template = ActionLinkService.ReplaceLinks(template, p => p.ContactId = contactId);
            }

            return template;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// If disposing, calls dispose on the Context. Always nulls out the Context
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _accessorCache.Dispose();
                _disposed = true;
            }
        }

        /// <summary>
        /// Throws a <see cref="ObjectDisposedException" /> if the context has already been disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException" />
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}