// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Microsoft.Web.WebPages.OAuth;
using Partnerinfo.Identity.Models;
using WebMatrix.WebData;

namespace Partnerinfo.Identity.Controllers
{
    /// <summary>
    /// Provides methods that respond to HTTP requests that are made to an ASP.NET MVC Web site.
    /// </summary>
    [Authorize]
    public sealed class AccountController : Controller
    {
        private readonly UserManager _manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController" /> class.
        /// </summary>
        public AccountController(UserManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        [Authorize(Roles = "Admin")]
        public ActionResult Index() => View();

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> Switch(int id, string returnUrl, CancellationToken cancellationToken)
        {
            var user = await _manager.FindByIdAsync(id, cancellationToken);
            if (user == null)
            {
                return HttpNotFound();
            }
            var authTicket = new FormsAuthenticationTicket(1, user.Email.Address, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), false, string.Empty);
            var encTicket = FormsAuthentication.Encrypt(authTicket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            cookie.Domain = FormsAuthentication.CookieDomain;
            HttpContext.Response.Cookies.Set(cookie);
            if (Request.IsAjaxRequest())
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            return RedirectToLocal(returnUrl);
        }

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        public ActionResult Settings(AccountOperation? message)
        {
            ViewBag.StatusMessage =
                message == AccountOperation.ChangePasswordSuccess ? IdentityAppResources.Account_AccountMessageType_ChangePasswordSuccess
                : message == AccountOperation.SetPasswordSuccess ? IdentityAppResources.Account_AccountMessageType_SetPasswordSuccess
                : message == AccountOperation.RemoveLoginSuccess ? IdentityAppResources.Account_AccountMessageType_RemoveLoginSuccess
                : string.Empty;
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("settings");
            return View();
        }

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Settings(PasswordBindingModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));

            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("settings");

            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("settings", new RouteValueDictionary { { "message", AccountOperation.ChangePasswordSuccess } });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, IdentityAppResources.Account_PasswordError);
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing OldPassword field.
                var state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("settings", new RouteValueDictionary { { "message", AccountOperation.SetPasswordSuccess } });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError(string.Empty, e);
                    }
                }
            }

            return View(model);
        }

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        [AllowAnonymous]
        public ActionResult ResetPassword(string token = null)
        {
            return View(new LoginBindingModel { PasswordToken = token });
        }

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        [HttpPost, AllowAnonymous]
        public ActionResult ResetPassword(LoginBindingModel model)
        {
            if (model.PasswordToken == null)
            {
                ModelState.Remove("Password");
                if (ModelState.IsValid)
                {
                    string resetToken = WebSecurity.GeneratePasswordResetToken(model.Email);
                    string resetTokenUrl = Url.Action("resetpassword", "account", new RouteValueDictionary { { "token", resetToken } }, Request.Url.Scheme, null);
                    using (var client = new SmtpClient())
                    {
                        var message = new MailMessage();
                        message.To.Add(model.Email);
                        message.Subject = "Reset your Partnerinfo TV password";
                        message.Body = "<p>Click here:</p>" + string.Format("<a href='{0}'>{0}</a>", resetTokenUrl);
                        message.IsBodyHtml = true;
                        client.Send(message);
                    }
                    return RedirectToLocal();
                }
            }
            else
            {
                ModelState.Remove("Email");
                if (ModelState.IsValid)
                {
                    if (WebSecurity.ResetPassword(model.PasswordToken, model.Password))
                    {
                        return RedirectToLocal();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Password has been reset.");
                    }
                }
            }
            return View(model);
        }

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        [HttpPost, AllowAnonymous]
        public async Task<ActionResult> Login(LoginBindingModel loginInfo, string returnUrl, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid && WebSecurity.Login(loginInfo.Email, loginInfo.Password, loginInfo.RememberMe))
            {
                await UpdateUserLoginDataAsync(loginInfo.Email, cancellationToken);
                return RedirectToLocal(returnUrl);
            }
            ModelState.AddModelError(string.Empty, IdentityAppResources.MembershipLogInStatus_InvalidUserNameOrPassword);
            return View(loginInfo);
        }

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            WebSecurity.Logout();
            return Redirect("/");
        }

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        [AllowAnonymous]
        public ActionResult Signup() => View();

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult Signup(SignupBindingModel signupInfo)
        {
            if (!ModelState.IsValid)
            {
                return View(signupInfo);
            }
            if (signupInfo.FirstName != null)
            {
                signupInfo.FirstName = StringUtility.Titlify(signupInfo.FirstName);
            }
            if (signupInfo.LastName != null)
            {
                signupInfo.LastName = StringUtility.Titlify(signupInfo.LastName);
            }
            DateTime now = DateTime.UtcNow;
            try
            {
                WebSecurity.CreateUserAndAccount(
                    signupInfo.Email.ToLower(),
                    signupInfo.Password,
                    new
                    {
                        Name = signupInfo.FirstName == null ? signupInfo.LastName : signupInfo.LastName == null ? signupInfo.FirstName : string.Join(" ", signupInfo.LastName, signupInfo.FirstName),
                        FirstName = signupInfo.FirstName,
                        LastName = signupInfo.LastName,
                        Gender = signupInfo.Gender,
                        Birthday = signupInfo.Birthday,
                        LastEventDate = now,
                        LastLoginDate = now,
                        CreatedDate = now,
                        ModifiedDate = now
                    });
                WebSecurity.Login(signupInfo.Email, signupInfo.Password);
                return RedirectToLocal();
            }
            catch (MembershipCreateUserException ex)
            {
                ModelState.AddModelError(string.Empty, ResourceHelper.GetMembershipCreateStatusText(ex.StatusCode));
            }
            return View(signupInfo);
        }

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult OAuthLogin(string provider, string returnUrl)
        {
            return new OAuthLoginResult(provider, Url.Action("oauthlogincallback", new RouteValueDictionary { { "returnurl", returnUrl } }));
        }

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        [AllowAnonymous]
        public async Task<ActionResult> OAuthLoginCallback(string returnUrl, CancellationToken cancellationToken)
        {
            OAuth.OAuthGoogleClient.RewriteRequest();

            var result = OAuthWebSecurity.VerifyAuthentication(Url.Action("oauthlogincallback", new RouteValueDictionary { { "returnurl", returnUrl } }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("OAuthLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                string userName = OAuthWebSecurity.GetUserName(result.Provider, result.ProviderUserId);
                await UpdateUserLoginDataAsync(userName, cancellationToken);
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account.
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name.
                string extraData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                var model = CreateSignupInfo(result.ExtraData);
                model.ExtraData = extraData;
                return View("OAuthLoginConfirmation", model);
            }
        }

        /// <summary>
        /// Creates a new signup info from a data dictionary.
        /// </summary>
        private SignupBindingModel CreateSignupInfo(IDictionary<string, string> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            string temp;
            return new SignupBindingModel
            {
                Email = data["email"],
                LastName = data.TryGetValue("lastName", out temp) ? temp : null,
                FirstName = data.TryGetValue("firstName", out temp) ? temp : null,
                Gender = string.Equals(data.TryGetValue("gender", out temp) ? temp : null, "female", StringComparison.OrdinalIgnoreCase) ? PersonGender.Female : PersonGender.Male,
                Birthday = Convert.ToDateTime(data.TryGetValue("birthday", out temp) ? temp : DateTime.UtcNow.ToString())
            };
        }

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<ActionResult> OAuthLoginConfirmation(SignupBindingModel model, string returnUrl, CancellationToken cancellationToken)
        {
            ModelState.Remove("Password");
            string provider = null;
            string providerUserId = null;
            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExtraData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }
            if (ModelState.IsValid)
            {
                var user = await _manager.FindByEmailAsync(model.Email, cancellationToken);
                if (user == null)
                {
                    await _manager.CreateAsync(new UserItem
                    {
                        Email = MailAddressItem.Create(model.Email, model.FirstName == null ? model.LastName : model.LastName == null ? model.FirstName : string.Join(" ", model.LastName, model.FirstName)),
                        LastName = model.LastName,
                        FirstName = model.FirstName,
                        Gender = model.Gender,
                        Birthday = model.Birthday,
                        LastIPAddress = Request.UserHostAddress,
                        LastLoginDate = DateTime.UtcNow
                    }, cancellationToken);

                    OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.Email);
                    OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("UserName", IdentityAppResources.MembershipCreateStatus_DuplicateUserName);
                }
            }
            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        [AllowAnonymous]
        public ActionResult OAuthLoginFailure() => View();

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);

            AccountOperation? message = null;

            // Only disassociate the account if the currently logged in user is the owner.
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential.
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = AccountOperation.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("settings", new { Message = message });
        }

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        [ChildActionOnly]
        public ActionResult OAuthLoginList()
        {
            var accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            var externalLogins = new List<OAuthProvider>();
            foreach (var account in accounts)
            {
                var clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);
                externalLogins.Add(new OAuthProvider
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                    ExtraData = clientData.ExtraData
                });
            }
            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_OAuthLoginList", externalLogins);
        }

        /// <summary>
        /// Creates a ViewResult object that renders a view to the response.
        /// </summary>
        private ActionResult RedirectToLocal(string returnUrl = null)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect(Url.Action("index", "home", new RouteValueDictionary { { "area", null } }, "http", null));
            }
        }

        /// <summary>
        /// Updates the current user login data.
        /// </summary>
        private async Task UpdateUserLoginDataAsync(string userName, CancellationToken cancellationToken)
        {
            var user = await _manager.FindByEmailAsync(userName, cancellationToken);
            if (user != null)
            {
                user.LastIPAddress = Request.UserHostAddress;
                user.LastLoginDate = DateTime.UtcNow;
                await _manager.UpdateAsync(user, cancellationToken);
            }
        }
    }
}