// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Linq;

namespace Partnerinfo
{
    public sealed class ValidationResult
    {
        private static readonly ValidationResult s_success = new ValidationResult(true);

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult" /> object.
        /// </summary>
        /// <param name="errors">Failure constructor that takes error messages.</param>
        public ValidationResult(params string[] errors)
            : this((IEnumerable<string>)errors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult" /> object.
        /// </summary>
        /// <param name="errors">Failure constructor that takes error messages.</param>
        public ValidationResult(IEnumerable<string> errors)
        {
            if (errors == null)
            {
                errors = new[] { "An unknown failure has occured." };
            }
            Succeeded = false;
            Errors = errors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult" /> object.
        /// </summary>
        private ValidationResult(bool success)
        {
            Succeeded = success;
            Errors = Enumerable.Empty<string>();
        }

        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the operation was succesful.
        /// </value>
        public bool Succeeded { get; private set; }

        /// <summary>
        /// Gets a collection of error messages.
        /// </summary>
        /// <value>
        /// A collection of error messages.
        /// </value>
        public IEnumerable<string> Errors { get; }

        /// <summary>
        /// Gets a static success result.
        /// </summary>
        /// <value>
        /// The <see cref="ValidationResult" />.
        /// </value>
        public static ValidationResult Success => s_success;

        /// <summary>
        /// Failed helper method.
        /// </summary>
        /// <param name="errors">A list of error messages.</param>
        /// <returns>
        /// The <see cref="ValidationResult" />.
        /// </returns>
        public static ValidationResult Failed(IEnumerable<string> errors) => new ValidationResult(errors);

        /// <summary>
        /// Failed helper method.
        /// </summary>
        /// <param name="errors">A list of error messages.</param>
        /// <returns>
        /// The <see cref="ValidationResult" />.
        /// </returns>
        public static ValidationResult Failed(params string[] errors) => new ValidationResult(errors);
    }
}
