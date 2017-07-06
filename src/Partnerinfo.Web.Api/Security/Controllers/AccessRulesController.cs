// Copyright (c) János Janka. All rights reserved.

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Partnerinfo.Identity;
using Partnerinfo.Security.Models;

namespace Partnerinfo.Security.Controllers
{
    /// <summary>
    /// Provides methods that respond to HTTP requests that are made to an ASP.NET Web API.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/security/access-rules/{objectType}/{objectId}")]
    public sealed class AccessRulesController : ApiController
    {
        private readonly SecurityManager _securityManager;
        private readonly UserManager _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessRulesController" /> class.
        /// </summary>
        public AccessRulesController(SecurityManager securityManager, UserManager userManager)
        {
            _securityManager = securityManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("")]
        public async Task<IHttpActionResult> GetAllAsync(AccessObjectType objectType, int objectId, CancellationToken cancellationToken)
        {
            return Ok(await _securityManager.GetAccessRulesAsync(objectType, objectId, cancellationToken));
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("")]
        public async Task<IHttpActionResult> PostAsync(AccessObjectType objectType, int objectId, [FromBody] CreateAccessRuleDto model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            await ApiSecurity.AuthorizeAsync(objectType, objectId, AccessPermission.IsOwner, cancellationToken);

            ValidationResult validationResult;
            if (model.Anyone)
            {
                validationResult = await _securityManager.SetAccessRuleAsync(objectType, objectId, new AccessRuleItem { Permission = model.Permission, Visibility = model.Visibility }, cancellationToken);
            }
            else
            {
                var user = await _userManager.FindByEmailAsync(model.Email, cancellationToken);
                if (user == null)
                {
                    ModelState.AddModelError("", string.Format(SecurityApiResources.UserNotFound, model.Email));
                    return BadRequest();
                }
                validationResult = await _securityManager.SetAccessRuleAsync(objectType, objectId, new AccessRuleItem { User = user, Permission = model.Permission, Visibility = model.Visibility }, cancellationToken);
            }
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return Ok(model);
        }

        /// <summary>
        /// Represents an action that handles only HTTP DELETE requests.
        /// </summary>
        [Route("")]
        public async Task<IHttpActionResult> DeleteAsync(AccessObjectType objectType, int objectId, [FromBody] string email, CancellationToken cancellationToken)
        {
            await ApiSecurity.AuthorizeAsync(objectType, objectId, AccessPermission.IsOwner, cancellationToken);

            AccessRuleItem trustee;
            if (email == null)
            {
                trustee = await _securityManager.GetAccessRuleAsync(objectType, objectId, cancellationToken);
            }
            else
            {
                trustee = await _securityManager.GetAccessRuleByUserEmailAsync(objectType, objectId, email, cancellationToken);
            }
            if (trustee == null)
            {
                ModelState.AddModelError("", string.Format(SecurityApiResources.UserNotFound, email ?? "Anyone"));
                return BadRequest();
            }

            var validationResult = await _securityManager.RemoveAccessRuleAsync(objectType, objectId, trustee, cancellationToken);
            if (!validationResult.Succeeded)
            {
                return this.ValidationContent(validationResult);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}