// Copyright (c) János Janka. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using Partnerinfo.Identity;
using Partnerinfo.Input.Models;

namespace Partnerinfo.Input.Controllers
{
    /// <summary>
    /// Defines API actions for event categories.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/input/commands")]
    public class CommandsController : ApiController
    {
        private const string GetRouteName = "Input.Commands.Get";

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandsController" /> class.
        /// </summary>
        public CommandsController(CommandManager commandManager, UserManager userManager)
        {
            if (commandManager == null)
            {
                throw new ArgumentNullException("commandManager");
            }
            if (userManager == null)
            {
                throw new ArgumentNullException("userManager");
            }
            CommandManager = commandManager;
            UserManager = userManager;
        }

        public CommandManager CommandManager { get; private set; }

        public UserManager UserManager { get; private set; }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route("~/c/{uri}", Name = GetRouteName), AllowAnonymous]
        public async Task<IHttpActionResult> GetAsync(string uri, bool rollback = false, string returnUrl = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var command = await CommandManager.FindByUriAsync(uri, cancellationToken);
            if (command == null || !CommandManager.SupportsInvoker)
            {
                return NotFound();
            }
            if (!rollback)
            {
                await CommandManager.Invoker.InvokeAsync(command.Line, command.Data, command.Data, cancellationToken);
            }
            await CommandManager.DeleteAsync(command, cancellationToken);
            if (returnUrl != null)
            {
                return Redirect(returnUrl);
            }
            return Ok();
        }

        /// <summary>
        /// Represents an action that handles only HTTP POST requests.
        /// </summary>
        [Route(""), AllowAnonymous]
        public async Task<IHttpActionResult> PostAsync([FromBody] CommandRequestBindingModel model, CancellationToken cancellationToken)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await UserManager.FindByEmailAsync(model.Mail.Address, cancellationToken);
            if (user == null)
            {
                return BadRequest(string.Format(InputApiResources.UserWasNotFound, model.Mail.Address));
            }
            var command = new CommandItem { Line = model.Command.Line, Data = model.Command.Data };
            await CommandManager.CreateAsync(command, cancellationToken);
            await CommandManager.MailService.SendAsync(model.Mail, command, model.ReturnUrl, cancellationToken);
            return CreatedAtRoute(GetRouteName, new RouteValueDictionary { { "uri", command.Uri } },
                new CommandResultModel
                {
                    Uri = command.Uri,
                    Links = new CommandLinks
                    {
                        Commit = Url.Route(GetRouteName, new RouteValueDictionary { { "uri", command.Uri }, { "returnurl", model.ReturnUrl } }),
                        Rollback = Url.Route(GetRouteName, new RouteValueDictionary { { "uri", command.Uri }, { "rollback", true }, { "returnurl", model.ReturnUrl } })
                    }
                });
        }
    }
}