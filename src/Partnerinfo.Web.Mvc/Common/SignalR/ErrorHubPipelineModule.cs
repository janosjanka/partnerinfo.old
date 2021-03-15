// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using Microsoft.AspNet.SignalR.Hubs;

namespace Partnerinfo.SignalR
{
    public class ErrorHubPipelineModule : HubPipelineModule
    {
        /// <summary>
        /// This is called when an uncaught exception is thrown by a server-side hub method or the incoming component of a
        /// module added later to the <see cref="T:Microsoft.AspNet.SignalR.Hubs.IHubPipeline" />.
        /// Observing the exception using this method will not prevent it from bubbling up to other modules.
        /// </summary>
        /// <param name="exceptionContext">Represents the exception that was thrown during the server-side invocation.
        /// It is possible to change the error or set a result using this context.</param>
        /// <param name="invokerContext">A description of the server-side hub method invocation.</param>
        protected override void OnIncomingError(ExceptionContext exceptionContext, IHubIncomingInvokerContext invokerContext)
        {
            invokerContext.Hub.Clients.Caller.error(Create(exceptionContext.Error));
        }

        /// <summary>
        /// Creates a new fault message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>
        /// A <see cref="PartnerFaultMessage" /> instance that represents validation errors.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        private static FaultMessage Create(DbEntityValidationException exception)
        {
            var list = new Queue<FaultMember>();
            foreach (var e in exception.EntityValidationErrors)
            {
                foreach (var ve in e.ValidationErrors)
                {
                    list.Enqueue(new FaultMember(ve.PropertyName, ve.ErrorMessage));
                }
            }
            return new FaultMessage(list);
        }

        /// <summary>
        /// Creates a new fault message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>
        /// A <see cref="PartnerFaultMessage" /> instance that represents validation errors.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        private static FaultMessage Create(Exception exception)
        {
            if (exception is DbEntityValidationException)
            {
                return Create((DbEntityValidationException)exception);
            }
            return new FaultMessage(exception.InnerException?.Message ?? exception.Message);
        }
    }
}