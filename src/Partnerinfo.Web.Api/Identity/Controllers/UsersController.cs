// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Partnerinfo.Identity.Models;

namespace Partnerinfo.Identity.Controllers
{
    /// <summary>
    /// Provides methods that respond to HTTP requests that are made to an ASP.NET Web API.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/identity/users")]
    public class UsersController : ApiController
    {
        private readonly UserManager _manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController" /> class.
        /// </summary>
        public UsersController(UserManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [AllowAnonymous]
        [Route("")]
        public async Task<IHttpActionResult> GetAllAsync([FromUri] UserQueryDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (model == null)
            {
                model = new UserQueryDto();
            }
            return Ok(await _manager.FindAllAsync(model.Name, model.OrderBy, model.Page, model.Limit, model.Fields, cancellationToken));
        }

        /// <summary>
        /// Represents an action that handles only HTTP GET requests.
        /// </summary>
        [Route("{id}")]
        public async Task<IHttpActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var user = await _manager.FindByIdAsync(id, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}