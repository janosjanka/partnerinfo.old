// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace System.Net.Http
{
    internal static class HttpRequestHeadersExtensions
    {
        public static bool IsAjaxRequest(this HttpRequestHeaders headers)
        {
            IEnumerable<string> values;
            return headers.TryGetValues("X-Requested-With", out values) && values.Contains("XMLHttpRequest");
        }
    }
}