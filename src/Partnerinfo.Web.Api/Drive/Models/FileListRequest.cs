// Copyright (c) János Janka. All rights reserved.

using Partnerinfo.Drive;
using Partnerinfo.Models;

namespace Partnerinfo.Drive.Models
{
    public class FileListRequest : PagedListRequest
    {
        public int? ParentId { get; set; }
        public FileType Type { get; set; }
        public string Name { get; set; }
    }
}