// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;

namespace Partnerinfo.Portal.Models
{
    public class PageItemDto : ResourceItem
    {
        public ResourceItem Portal { get; set; }
        public ResourceItem Master { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Description { get; set; }
        public string HtmlContent { get; set; }
        public string StyleContent { get; set; }
        public ICollection<ReferenceItemDto> References { get; set; }
        public ICollection<PageItemDto> Children { get; set; }
    }
}