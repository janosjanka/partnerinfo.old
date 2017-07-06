// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Portal
{
    public interface IPageValidator : IPortalValidator
    {
        /// <summary>
        /// Validates the specified <paramref name="page" /> as an asynchronous operation.
        /// </summary>
        /// <param name="manager">The <see cref="PortalManager" /> that can be used to retrieve user properties.</param>
        /// <param name="portal">The portal which owns the page.</param>
        /// <param name="page">The page to validate.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="ValidationResult" /> of the validation operation.
        /// </returns>
        Task<ValidationResult> ValidatePageAsync(PortalManager manager, PortalItem portal, PageItem page, CancellationToken cancellationToken);
    }
}
