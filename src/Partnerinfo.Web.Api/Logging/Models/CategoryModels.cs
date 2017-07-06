// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Logging.Models
{
    public sealed class CategoryQueryDto
    {
        /// <summary>
        /// Project ID
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Sort order
        /// </summary>
        public CategorySortOrder OrderBy { get; set; } = CategorySortOrder.Name;

        /// <summary>
        /// Fields
        /// </summary>
        public CategoryField Fields { get; set; } = CategoryField.None;
    }

    public sealed class CategoryItemDto
    {
        /// <summary>
        /// Project Id
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Color
        /// </summary>
        public ColorInfo Color { get; set; }
    }

    public sealed class CategoryResultDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Color
        /// </summary>
        public ColorInfo Color { get; set; }
    }
}