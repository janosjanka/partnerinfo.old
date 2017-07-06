// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Web.Http;
using Partnerinfo.Results;

namespace Partnerinfo
{
    public static class ApiControllerExtensions
    {
        public static PagedListContentResult<T> PagedListResult<T>(this ApiController controller, IList<T> list, int offset, int limit, string routeName)
        {
            return new PagedListContentResult<T>(list, offset, limit, routeName, controller);
        }

        /// <summary>
        /// Creates a new <see cref="ValidationContentResult" />.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="result">The validation result.</param>
        /// <returns>
        /// The <see cref="ValidationContentResult" />.
        /// </returns>
        public static ValidationContentResult ValidationContent(this ApiController controller, ValidationResult result)
        {
            return new ValidationContentResult(result, controller);
        }
    }
}