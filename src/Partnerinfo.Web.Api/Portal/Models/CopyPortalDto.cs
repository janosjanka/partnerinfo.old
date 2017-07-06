// Copyright (c) János Janka. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace Partnerinfo.Portal.Models
{
    public class CopyPortalDto
    {
        [MaxLength(64)]
        [UriPartValidator]
        public string Uri { get; set; }
    }
}