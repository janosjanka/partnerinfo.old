// Copyright (c) János Janka. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace Partnerinfo.Input.Models
{
    public class CommandRequestBindingModel
    {
        /// <summary>
        /// Mail address
        /// </summary>
        [Required]
        public MailAddressItem Mail { get; set; }

        /// <summary>
        /// Command
        /// </summary>
        [Required]
        public CommandBindingModel Command { get; set; }

        /// <summary>
        /// Return URL
        /// </summary>
        public string ReturnUrl { get; set; }
    }

    public class CommandBindingModel
    {
        /// <summary>
        /// Command line name
        /// </summary>
        [Required]
        public string Line { get; set; }

        /// <summary>
        /// Command data
        /// </summary>
        public string Data { get; set; }
    }

    public class CommandResultModel
    {
        /// <summary>
        /// Command uri
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Link to the command
        /// </summary>
        public CommandLinks Links { get; set; }
    }

    public class CommandLinks
    {
        /// <summary>
        /// A link to commmit transaction
        /// </summary>
        public string Commit { get; set; }

        /// <summary>
        /// A link to rollback transaction
        /// </summary>
        public string Rollback { get; set; }
    }
}