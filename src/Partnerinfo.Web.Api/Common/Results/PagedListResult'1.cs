// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;

namespace Partnerinfo.Results
{
    /// <summary>
    /// Represents a collection of objects that can be used by repositories.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public class PagedListResult<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedListResult" /> class.
        /// </summary>
        public PagedListResult()
            : this(new List<T>(), new HypermediaLinkDictionary())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedListResult" /> class.
        /// </summary>
        /// <param name="data">The strongly typed list of items that can be serialized by ASP.NET Web API.</param>
        public PagedListResult(IList<T> data)
            : this(data, new HypermediaLinkDictionary())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedListResult" /> class.
        /// </summary>
        /// <param name="data">The strongly typed list of items that can be serialized by ASP.NET Web API.</param>
        public PagedListResult(IList<T> data, HypermediaLinkDictionary links)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (links == null)
            {
                throw new ArgumentNullException("links");
            }

            Data = data;
            Links = links;
        }

        /// <summary>
        /// The strongly typed list of items that can be serialized by ASP.NET Web API.
        /// </summary>
        /// <value>
        /// The strongly typed list of items.
        /// </value>
        public IList<T> Data { get; private set; }

        /// <summary>
        /// Gets or sets hypermdia links that can be used from client applications for identifying resource locations.
        /// </summary>
        /// <value>
        /// The hypermdia links of the unique resource.
        /// </value>
        public HypermediaLinkDictionary Links { get; private set; }
    }
}