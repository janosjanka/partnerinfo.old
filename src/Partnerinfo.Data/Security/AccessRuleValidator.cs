// Copyright (c) János Janka. All rights reserved.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Security
{
    public class AccessRuleValidator : IAccessRuleValidator
    {
        public static readonly AccessRuleValidator Default = new AccessRuleValidator();

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
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual async Task<ValidationResult> ValidateAsync(SecurityManager manager, AccessObjectType objectType, int objectId, AccessRuleItem rule, CancellationToken cancellationToken)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            if (rule == null)
            {
                return ValidationResult.Failed(string.Format(CultureInfo.CurrentCulture, SecurityResources.UserPermissionRequired));
            }
            if (rule.Anyone)
            {
                return await ValidateAnyoneAsync(manager, objectType, objectId, rule, cancellationToken);
            }
            return await ValidateUserAsync(manager, objectType, objectId, rule, cancellationToken);
        }

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
        protected virtual Task<ValidationResult> ValidateAnyoneAsync(SecurityManager manager, AccessObjectType objectType, int objectId, AccessRuleItem rule, CancellationToken cancellationToken)
        {
            if (rule.Permission != AccessPermission.CanView)
            {
                return ValidationResultTask.Failed(string.Format(CultureInfo.CurrentCulture, SecurityResources.AccessPermissionNotSupported));
            }
            if (rule.Visibility == AccessVisibility.Unknown)
            {
                return ValidationResultTask.Failed(string.Format(CultureInfo.CurrentCulture, SecurityResources.AnyoneVisibilityRequired));
            }
            return ValidationResultTask.Success;
        }

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
        protected virtual Task<ValidationResult> ValidateUserAsync(SecurityManager manager, AccessObjectType objectType, int objectId, AccessRuleItem rule, CancellationToken cancellationToken)
        {
            if (rule.Permission == AccessPermission.Unknown)
            {
                return ValidationResultTask.Failed(string.Format(CultureInfo.CurrentCulture, SecurityResources.UserPermissionRequired));
            }
            if (rule.Visibility != AccessVisibility.Unknown)
            {
                return ValidationResultTask.Failed(string.Format(CultureInfo.CurrentCulture, SecurityResources.AccessVisibilityNotSupported));
            }
            return AtLeastOneOwnerRequired(manager, objectType, objectId, rule, cancellationToken);
        }

        /// <summary>
        /// Checks whether at least one owner exists on the specified <paramref name="objectType" />.
        /// </summary>
        /// <param name="manager">The <see cref="SecurityManager" /> that can be used to retrieve user properties.</param>
        /// <param name="objectType">The type of the shared object to be found.</param>
        /// <param name="objectId">The primary key of the shared object to be found.</param>
        /// <param name="rule">The rule to validate.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="ValidationResult" /> of the validation operation.
        /// </returns>
        protected async Task<ValidationResult> AtLeastOneOwnerRequired(SecurityManager manager, AccessObjectType objectType, int objectId, AccessRuleItem rule, CancellationToken cancellationToken)
        {
            Debug.Assert(manager != null);
            Debug.Assert(rule != null);

            if (rule.Anyone || rule.Permission == AccessPermission.IsOwner)
            {
                // No need to check access rules if this operation does not have an effect on owner rights.
                return ValidationResult.Success;
            }

            var owners = await manager.GetOwnersAsync(objectType, objectId, cancellationToken);
            if (owners.Data.Count(ace => ace.User.Id != rule.User.Id) == 0)
            {
                return ValidationResult.Failed(string.Format(CultureInfo.CurrentCulture, SecurityResources.OwnerRequired));
            }

            return ValidationResult.Success;
        }
    }
}