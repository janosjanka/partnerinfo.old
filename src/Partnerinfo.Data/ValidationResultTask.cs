// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Partnerinfo
{
    internal static class ValidationResultTask
    {
        /// <summary>
        /// Gets a static success result task.
        /// </summary>
        public static readonly Task<ValidationResult> Success = Task.FromResult(ValidationResult.Success);

        /// <summary>
        /// Failed helper method.
        /// </summary>
        /// <param name="errors">A list of error messages.</param>
        /// <returns>
        /// The <see cref="ValidationResult" />.
        /// </returns>
        public static Task<ValidationResult> Failed(IEnumerable<string> errors) => Task.FromResult(ValidationResult.Failed(errors));

        /// <summary>
        /// Failed helper method.
        /// </summary>
        /// <param name="errors">A list of error messages.</param>
        /// <returns>
        /// The <see cref="ValidationResult" />.
        /// </returns>
        public static Task<ValidationResult> Failed(params string[] errors) => Task.FromResult(ValidationResult.Failed(errors));
    }
}
