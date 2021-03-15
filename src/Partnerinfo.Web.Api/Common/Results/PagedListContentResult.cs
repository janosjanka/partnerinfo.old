// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Partnerinfo.Results
{
    /// <summary>
    /// Represents an action result that performs content negotiation and returns an <see cref="HttpStatusCode.OK"/>
    /// response when it succeeds.
    /// </summary>
    /// <typeparam name="T">The type of content in the entity body.</typeparam>
    public class PagedListContentResult<T> : OkNegotiatedContentResult<PagedListResult<T>>
    {
        private readonly PagedListResult<T> _content;
        private readonly int _offset;
        private readonly int _limit;
        private readonly string _routeName;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedListResult{T}" /> class with the values provided.
        /// </summary>
        /// <param name="list">The content value to negotiate and format in the entity body.</param>
        /// <param name="contentNegotiator">The content negotiator to handle content negotiation.</param>
        /// <param name="request">The request message which led to this result.</param>
        /// <param name="formatters">The formatters to use to negotiate and format the content.</param>
        public PagedListContentResult(IList<T> list, int offset, int limit, string routeName, IContentNegotiator contentNegotiator, HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters)
            : base(null, contentNegotiator, request, formatters)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            _content = new PagedListResult<T>(list.Take(limit).ToList());
            _offset = offset;
            _limit = limit;
            HasPrev = offset > 0;
            HasNext = list.Count > limit;
            _routeName = routeName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedListResult" /> class.
        /// </summary>
        /// <param name="list">The content.</param>
        /// <param name="controller">The controller.</param>
        public PagedListContentResult(IList<T> list, int offset, int limit, string routeName, ApiController controller)
            : base(new PagedListResult<T>(list.Take(limit).ToList()), controller)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            _content = new PagedListResult<T>(list.Take(limit).ToList());
            _offset = offset;
            _limit = limit;
            HasPrev = offset > 0;
            HasNext = list.Count > limit;
            _routeName = routeName;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cursor can be moved to a previous position.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the cursor can be moved.
        /// </value>
        public bool HasPrev { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cursor can be moved to a next position.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the cursor can be moved.
        /// </value>
        public bool HasNext { get; set; }

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage" /> asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that, when completed, contains the <see cref="T:System.Net.Http.HttpResponseMessage" />.
        /// </returns>
        public override Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var url = Request.GetUrlHelper();
            var values = Request.GetRouteData().Values;

            if (HasPrev)
            {
                int prevOffset = _offset - _limit;
                if (prevOffset <= 0)
                {
                    prevOffset = 0;
                }
                values["offset"] = prevOffset;
                _content.Links.SetLink("prev", url.Link(_routeName, _routeName));
            }

            if (HasNext)
            {
                values["offset"] = _offset + _limit;
                _content.Links.SetLink("next", url.Link(_routeName, _routeName));
            }

            return base.ExecuteAsync(cancellationToken);
        }
    }
}