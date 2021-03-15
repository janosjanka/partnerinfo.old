// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Security
{
    public interface IAccessRuleValidator
    {
        /// <summary>
        /// Validates the specified <paramref name="rule" /> as an asynchronous operation.
        /// </summary>
        /// <param name="manager">The <see cref="SecurityManager" /> that can be used to retrieve user properties.</param>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key of the shared object to be found.</param>
        /// <param name="rule">The rule to validate.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="ValidationResult" /> of the validation operation.
        /// </returns>
        Task<ValidationResult> ValidateAsync(SecurityManager manager, AccessObjectType objectType, int objectId, AccessRuleItem rule, CancellationToken cancellationToken);
    }
}