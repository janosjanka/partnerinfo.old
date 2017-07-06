// Copyright (c) János Janka. All rights reserved.

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Portal
{
    public class PortalValidator : IPortalValidator, IPageValidator
    {
        internal static readonly PortalValidator Default = new PortalValidator();

        /// <summary>
        /// Validates the specified <paramref name="portal" /> as an asynchronous operation.
        /// </summary>
        /// <param name="manager">The <see cref="PortalManager" /> that can be used to retrieve user properties.</param>
        /// <param name="portal">The portal to validate.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="ValidationResult" /> of the validation operation.
        /// </returns>
        public virtual async Task<ValidationResult> ValidateAsync(PortalManager manager, PortalItem portal, CancellationToken cancellationToken)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }

            var otherPortal = await manager.FindByUriAsync(portal.Uri, cancellationToken);
            if (otherPortal == null || otherPortal.Id == portal.Id || !string.Equals(otherPortal.Uri, portal.Uri, StringComparison.Ordinal))
            {
                return ValidationResult.Success;
            }
            return ValidationResult.Failed(string.Format(CultureInfo.CurrentCulture, PortalResources.DuplicatedPortalUri, portal.Uri));
        }

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
        public virtual async Task<ValidationResult> ValidatePageAsync(PortalManager manager, PortalItem portal, PageItem page, CancellationToken cancellationToken)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            if (portal == null)
            {
                throw new ArgumentNullException(nameof(portal));
            }
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            var otherPage = await manager.GetPageByUriAsync(portal, page.Uri, cancellationToken);
            if (otherPage == null || otherPage.Id == page.Id || !string.Equals(otherPage.Uri, page.Uri, StringComparison.Ordinal))
            {
                return ValidationResult.Success;
            }
            return ValidationResult.Failed(string.Format(CultureInfo.CurrentCulture, PortalResources.DuplicatedPageUri, page.Uri));
        }
    }
}
