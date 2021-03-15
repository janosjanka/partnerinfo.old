// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Composition;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Partnerinfo.Composition
{
    internal class ApiFilterProvider : IFilterProvider
    {
        /// <summary>
        /// Returns an enumeration of filters.
        /// </summary>
        /// <param name="configuration">The HTTP configuration.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>
        /// An enumeration of filters.
        /// </returns>
        public IEnumerable<FilterInfo> GetFilters(HttpConfiguration configuration, HttpActionDescriptor actionDescriptor)
        {
            return Enumerable.Concat(
                Resolve(actionDescriptor.ControllerDescriptor.GetFilters(), FilterScope.Controller),
                Resolve(actionDescriptor.GetFilters(), FilterScope.Action));
        }

        private static IEnumerable<FilterInfo> Resolve(Collection<IFilter> filters, FilterScope scope)
        {
            var context = HttpCompositionProvider.Current;
            for (int i = 0; i < filters.Count; ++i)
            {
                var info = new FilterInfo(filters[i], scope);
                context.SatisfyImports(info.Instance);
                yield return info;
            }
        }
    }
}
