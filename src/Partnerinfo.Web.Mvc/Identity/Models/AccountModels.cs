// Copyright (c) János Janka. All rights reserved.

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Partnerinfo.Identity.Models
{
    public enum AccountOperation : byte
    {
        ChangePasswordSuccess,
        SetPasswordSuccess,
        RemoveLoginSuccess
    }

    public class SignupBindingModel
    {
        /// <summary>
        /// Gets or sets the email address that can be used to search for users.
        /// </summary>
        /// <value>
        /// The email address that can be used to search for users.
        /// </value>
        [Display(Name = "SignupInfo_Email", ResourceType = typeof(IdentityAppResources))]
        [Required(ErrorMessageResourceName = "SignupInfo_Email_Required", ErrorMessageResourceType = typeof(IdentityAppResources))]
        [StringLength(256, ErrorMessageResourceName = "SignupInfo_Email_StringLength", ErrorMessageResourceType = typeof(IdentityAppResources))]
        [Email(ErrorMessageResourceName = "SignupInfo_Email_RegularExpression", ErrorMessageResourceType = typeof(IdentityAppResources))]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password that can be used to sign up.
        /// </summary>
        /// <value>
        /// The password that can be used to sign up.
        /// </value>
        [Display(Name = "SignupInfo_Password", ResourceType = typeof(IdentityAppResources))]
        [Required(ErrorMessageResourceName = "SignupInfo_Password_Required", ErrorMessageResourceType = typeof(IdentityAppResources))]
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessageResourceName = "SignupInfo_Password_StringLength", ErrorMessageResourceType = typeof(IdentityAppResources))]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the first name for the current user.
        /// </summary>
        /// <value>
        /// The first name for the current user.
        /// </value>
        [Display(Name = "SignupInfo_FirstName", ResourceType = typeof(IdentityAppResources))]
        [StringLength(128, ErrorMessageResourceName = "SignupInfo_FirstName_StringLength", ErrorMessageResourceType = typeof(IdentityAppResources))]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name for the current user.
        /// </summary>
        /// <value>
        /// The last name for the current user.
        /// </value>
        [Display(Name = "SignupInfo_LastName", ResourceType = typeof(IdentityAppResources))]
        [StringLength(128, ErrorMessageResourceName = "SignupInfo_LastName_StringLength", ErrorMessageResourceType = typeof(IdentityAppResources))]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the gender of the user.
        /// </summary>
        /// <value>
        /// The gender of the user.
        /// </value>
        [Display(Name = "SignupInfo_Gender", ResourceType = typeof(IdentityAppResources))]
        public PersonGender Gender { get; set; }

        /// <summary>
        /// Gets or sets the birthday of the user.
        /// </summary>
        /// <value>
        /// The birthday of the user.
        /// </value>
        [Display(Name = "SignupInfo_Birthday", ResourceType = typeof(IdentityAppResources))]
        [YearRange(-120, -13, ErrorMessageResourceName = "SignupInfo_Birthday_YearRange", ErrorMessageResourceType = typeof(IdentityAppResources))]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// Gets or sets the extra OAuth data for the current <see cref="SignupBindingModel" />.
        /// </summary>
        /// <value>
        /// The extra OAuth data for the current <see cref="SignupBindingModel" />
        /// </value>
        public string ExtraData { get; set; }
    }

    public class LoginBindingModel
    {
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        /// <value>
        /// The email address of the user.
        /// </value>
        [Display(Name = "LoginInfo_Email", ResourceType = typeof(IdentityAppResources))]
        [Required(ErrorMessageResourceName = "LoginInfo_Email_Required", ErrorMessageResourceType = typeof(IdentityAppResources))]
        [StringLength(256, ErrorMessageResourceName = "LoginInfo_Email_StringLength", ErrorMessageResourceType = typeof(IdentityAppResources))]
        [Email(ErrorMessageResourceName = "LoginInfo_Email_RegularExpression", ErrorMessageResourceType = typeof(IdentityAppResources))]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password that can be used to log in.
        /// </summary>
        /// <value>
        /// The password that can be used to log in.
        /// </value>
        [Display(Name = "LoginInfo_Password", ResourceType = typeof(IdentityAppResources))]
        [Required(ErrorMessageResourceName = "LoginInfo_Password_Required", ErrorMessageResourceType = typeof(IdentityAppResources))]
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessageResourceName = "LoginInfo_Password_StringLength", ErrorMessageResourceType = typeof(IdentityAppResources))]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the password token that can be used to reset the password.
        /// </summary>
        /// <value>
        /// The password token that can be used to reset the password.
        /// </value>
        [IgnoreDataMember]
        public string PasswordToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the browser stores the authentication cookie.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the browser stores the authentication cookie; otherwise, <c>false</c>.
        /// </value>
        public bool RememberMe { get; set; }
    }

    public class PasswordBindingModel
    {
        /// <summary>
        /// Gets or sets the old password of the user.
        /// </summary>
        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceName = "AccountPassword_OldPassword_Required", ErrorMessageResourceType = typeof(IdentityAppResources))]
        [Display(Name = "AccountPassword_OldPassword", ResourceType = typeof(IdentityAppResources))]
        public string OldPassword { get; set; }

        /// <summary>
        /// Gets or sets the new password of the user.
        /// </summary>
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessageResourceName = "AccountPassword_NewPassword_StringLength", ErrorMessageResourceType = typeof(IdentityAppResources))]
        [Display(Name = "AccountPassword_NewPassword", ResourceType = typeof(IdentityAppResources))]
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets the confirmed password of the user.
        /// </summary>
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessageResourceName = "AccountPassword_ConfirmPassword_Compare", ErrorMessageResourceType = typeof(IdentityAppResources))]
        [Display(Name = "AccountPassword_ConfirmPassword", ResourceType = typeof(IdentityAppResources))]
        public string ConfirmPassword { get; set; }
    }
}