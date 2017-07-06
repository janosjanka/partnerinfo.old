// Copyright (c) János Janka. All rights reserved.

namespace Partnerinfo.Logging
{
    public enum ObjectType : byte
    {
        /// <summary>
        /// Message
        /// </summary>
        Message = 0,

        /// <summary>
        /// Project
        /// </summary>
        Project = 10,

        /// <summary>
        /// Project Action Link
        /// </summary>
        Action = 20,

        /// <summary>
        /// Project Mail Message
        /// </summary>
        MailMessage = 30,

        /// <summary>
        /// Portal
        /// </summary>
        Portal = 100,

        /// <summary>
        /// Portal Page
        /// </summary>
        Page = 110
    }
}
