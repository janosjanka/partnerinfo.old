// Copyright (c) János Janka. All rights reserved.

using System;

namespace Partnerinfo.Portal.EntityFramework
{
    internal static class Mapper
    {
        public static PortalEntity Map(PortalItem source, PortalEntity target)
        {
            target.Name = source.Name;
            target.Uri = source.Uri;
            target.Domain = source.Domain;
            target.Description = source.Description;
            target.GATrackingId = source.GATrackingId;
            target.ModifiedDate = DateTime.UtcNow;
            return target;
        }

        public static PortalItem Map(PortalEntity source, PortalItem target)
        {
            target.Id = source.Id;
            target.Name = source.Name;
            target.Uri = source.Uri;
            target.Domain = source.Domain;
            target.Description = source.Description;
            target.GATrackingId = source.GATrackingId;
            target.CreatedDate = source.CreatedDate;
            target.ModifiedDate = source.ModifiedDate;
            return target;
        }

        public static PortalPage Map(PageItem source, PortalPage target)
        {
            target.MasterId = source.Master?.Id;
            target.Uri = source.Uri;
            target.Name = source.Name;
            target.Description = source.Description;
            target.HtmlContent = source.HtmlContent;
            target.StyleContent = source.StyleContent;
            target.ModifiedDate = DateTime.UtcNow;
            return target;
        }

        public static PageItem Map(PortalPage source, PageItem target)
        {
            target.Id = source.Id;
            target.Uri = source.Uri;
            target.Name = source.Name;
            target.Description = source.Description;
            target.HtmlContent = source.HtmlContent;
            target.StyleContent = source.StyleContent;

            if (source.ReferenceList != null)
            {
                var references = ReferenceSerializer.Deserialize(source.ReferenceList);
                foreach (var reference in references)
                {
                    target.References.Add(reference);
                }
            }
            else
            {
                target.References.Clear();
            }

            target.ModifiedDate = source.ModifiedDate;
            return target;
        }
    }
}
