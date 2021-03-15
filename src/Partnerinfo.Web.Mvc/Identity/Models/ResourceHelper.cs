// Copyright (c) János Janka. All rights reserved.

using System.Web.Security;

namespace Partnerinfo.Identity.Models
{
    internal static class ResourceHelper
    {
        /// <summary>
        /// Gets membership create status text.
        /// </summary>
        /// <param name="status">The membership status.</param>
        /// <returns>A string that contains localited status text.</returns>
        public static string GetMembershipCreateStatusText(MembershipCreateStatus status)
        {
            string message;

            switch (status)
            {
                case MembershipCreateStatus.Success:
                    message = IdentityAppResources.MembershipCreateStatus_Success;
                    break;
                case MembershipCreateStatus.InvalidUserName:
                    message = IdentityAppResources.MembershipCreateStatus_InvalidUserName;
                    break;
                case MembershipCreateStatus.InvalidPassword:
                    message = IdentityAppResources.MembershipCreateStatus_InvalidPassword;
                    break;
                case MembershipCreateStatus.InvalidQuestion:
                    message = IdentityAppResources.MembershipCreateStatus_InvalidQuestion;
                    break;
                case MembershipCreateStatus.InvalidAnswer:
                    message = IdentityAppResources.MembershipCreateStatus_InvalidAnswer;
                    break;
                case MembershipCreateStatus.InvalidEmail:
                    message = IdentityAppResources.MembershipCreateStatus_InvalidEmail;
                    break;
                case MembershipCreateStatus.DuplicateUserName:
                    message = IdentityAppResources.MembershipCreateStatus_DuplicateUserName;
                    break;
                case MembershipCreateStatus.DuplicateEmail:
                    message = IdentityAppResources.MembershipCreateStatus_DuplicateEmail;
                    break;
                case MembershipCreateStatus.UserRejected:
                    message = IdentityAppResources.MembershipCreateStatus_UserRejected;
                    break;
                case MembershipCreateStatus.InvalidProviderUserKey:
                    message = IdentityAppResources.MembershipCreateStatus_InvalidProviderUserKey;
                    break;
                case MembershipCreateStatus.DuplicateProviderUserKey:
                    message = IdentityAppResources.MembershipCreateStatus_DuplicateProviderUserKey;
                    break;
                default:
                    message = IdentityAppResources.MembershipCreateStatus_ProviderError;
                    break;
            }

            return message;
        }
    }
}