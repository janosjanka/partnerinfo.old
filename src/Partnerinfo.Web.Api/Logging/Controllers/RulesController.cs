// Copyright (c) János Janka. All rights reserved.

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Routing;
using Partnerinfo.Identity;
using Partnerinfo.Logging.Models;
using Partnerinfo.Logging.Results;

namespace Partnerinfo.Logging.Controllers
{
    /// <summary>
    /// Provides methods that respond to HTTP requests that are made to an ASP.NET Web API.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/logging/rules")]
    public sealed class RulesController : ApiController
    {
        private readonly UserManager _userManager;
        private readonly RuleManager _ruleManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RulesController" /> class.
        /// </summary>
        /// <param name="userManager">The <see cref="UserManager" /> that the <see cref="RulesController" /> operates against.</param>
        /// <param name="ruleManager">The <see cref="RuleManager" /> that the <see cref="RulesController" /> operates againts.</param>
        public RulesController(UserManager userManager, RuleManager ruleManager)
        {
            _userManager = userManager;
            _ruleManager = ruleManager;
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("")]
        [ResponseType(typeof(ListResult<RuleResultDto>))]
        public async Task<IHttpActionResult> GetAllAsync([FromUri] RuleQueryDto model, CancellationToken cancellationToken)
        {
            return new RuleListContentResult(
                await _ruleManager.FindAllAsync(ApiSecurity.CurrentUserId, model?.Fields ?? RuleField.None, cancellationToken), this);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{id}")]
        [ResponseType(typeof(RuleResultDto))]
        public Task<IHttpActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return GetByIdAsync(id, RuleField.None, cancellationToken);
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{id}")]
        [ResponseType(typeof(RuleResultDto))]
        public async Task<IHttpActionResult> GetByIdAsync(int id, RuleField fields, CancellationToken cancellationToken)
        {
            var rule = await _ruleManager.FindByIdAsync(id, fields, cancellationToken);
            if (rule == null)
            {
                return NotFound();
            }
            return new RuleItemContentResult(rule, this);
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("")]
        [ResponseType(typeof(RuleResultDto))]
        public async Task<IHttpActionResult> PostAsync([FromBody] RuleItemDto model, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(ApiSecurity.CurrentUserId, cancellationToken);
            if (user == null)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }
            var rule = ModelMapper.ToRuleItem(model);
            var validationResult = await _ruleManager.CreateAsync(rule, user, cancellationToken);
            if (validationResult.Succeeded == false)
            {
                return this.ValidationContent(validationResult);
            }
            return new RuleItemContentResult(HttpStatusCode.Created, rule, this);
        }

        /// <summary>
        /// Represents an action that handles only HTTP PUT requests.
        /// </summary>
        [Route("{id}")]
        [ResponseType(typeof(RuleResultDto))]
        public async Task<IHttpActionResult> PutAsync(int id, [FromBody] RuleItemDto model, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var rule = ModelMapper.ToRuleItem(model);
            rule.Id = id;
            var validationResult = await _ruleManager.UpdateAsync(rule, cancellationToken);
            if (validationResult.Succeeded == false)
            {
                return this.ValidationContent(validationResult);
            }
            return new RuleItemContentResult(rule, this);
        }

        /// <summary>
        /// Represents an action that handles only HTTP DELETE requests.
        /// </summary>
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var rule = await _ruleManager.FindByIdAsync(id, RuleField.None, cancellationToken);
            if (rule != null)
            {
                await _ruleManager.DeleteAsync(rule, cancellationToken);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}