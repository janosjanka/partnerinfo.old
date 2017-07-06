// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;

namespace Partnerinfo.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class ValidationAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Called by the Web API framework after the action method executes.
        /// </summary>
        /// <param name="actionExecutedContext">The action executed context.</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);

            var context = actionExecutedContext.ActionContext;
            if (!context.ModelState.IsValid)
            {
                var faultMessage = CreateFaultMessage(context.ModelState);
                context.Response = context.Request.CreateResponse(HttpStatusCode.BadRequest, faultMessage);
            }
        }

        /// <summary>
        /// Creates a new fault message.
        /// </summary>
        /// <param name="modelState">The current state of the model.</param>
        /// <returns>A <see cref="FaultMessage"/> instance that represents validation errors.</returns>
        private FaultMessage CreateFaultMessage(ModelStateDictionary modelState)
        {
            var validationFaultList = new Queue<FaultMember>();
            foreach (string key in modelState.Keys)
            {
                var state = modelState[key];
                if (state.Errors.Count > 0)
                {
                    validationFaultList.Enqueue(new FaultMember(key, state.Errors[0].ErrorMessage));
                }
            }
            return new FaultMessage(validationFaultList);
        }
    }
}