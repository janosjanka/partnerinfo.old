// Copyright (c) Partnerinfo Ltd. All rights reserved.

using System;
using System.Collections.Generic;

namespace Partnerinfo.Portal.ViewModels.Pages
{
    public class PageViewModel : ResourceItem
    {
        public ResourceItem Portal { get; set; }
        public ResourceItem Master { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Description { get; set; }
        public string HtmlContent { get; set; }
        public string StyleContent { get; set; }
        public ICollection<PageViewModel> Children { get; set; }
    }
}