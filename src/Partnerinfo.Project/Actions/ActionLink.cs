// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Project.Actions
{
    /// <summary>
    /// Identifies an action (tree) that will be executed immediately after an event is occurred.
    /// </summary>
    public sealed class ActionLink
    {
        /// <summary>
        /// Gets or sets the primary key for the <see cref="ActionItem" />.
        /// </summary>
        /// <value>
        /// The primary key for the <see cref="ActionItem" />.
        /// </value>
        public int ActionId { get; set; }

        /// <summary>
        /// Gets or sets the primary key for <see cref="ContactItem" />.
        /// </summary>
        /// <value>
        /// The primary key for the <see cref="ContactItem" />.
        /// </value>
        public int? ContactId { get; set; }

        /// <summary>
        /// Gets or sets the custom URI that is appended to the URL.
        /// </summary>
        /// <value>
        /// The custom URI that is appended to the URL.
        /// </value>
        public string CustomUri { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionLink" /> class.
        /// </summary>
        public ActionLink()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionLink" /> class.
        /// </summary>
        /// <param name="actionId">The primary key for the <see cref="ActionItem" />.</param>
        /// <param name="contactId">The primary key for the <see cref="ContactItem" />.</param>
        /// <param name="customUri">The custom URI that is appended to the URL.</param>
        public ActionLink(int actionId, int? contactId = null, string customUri = null)
        {
            ActionId = actionId;
            ContactId = contactId;
            CustomUri = customUri;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public sealed override string ToString()
        {
            if (ContactId == null)
            {
                return $"{ActionId}/{CustomUri}";
            }
            return $"{ActionId}.{ContactId}/{CustomUri}";
        }
    }
}
