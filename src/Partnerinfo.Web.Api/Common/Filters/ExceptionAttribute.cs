// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Filters;

namespace Partnerinfo.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ExceptionAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="context">The context for the action.</param>
        public override void OnException(HttpActionExecutedContext context)
        {
            context.Response = context.Request.CreateResponse(HttpStatusCode.BadRequest, Create(context.Exception));
        }

        /// <summary>
        /// Creates a new fault message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>
        /// A <see cref="PartnerFaultMessage" /> instance that represents validation errors.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        private static FaultMessage Create(AggregateException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            var builder = new StringBuilder(256);
            foreach (var ex in exception.InnerExceptions)
            {
                if (ex.InnerException != null)
                {
                    builder.AppendLine(ex.InnerException.Message);
                }
                else
                {
                    builder.AppendLine(ex.Message);
                }
            }
            return new FaultMessage(builder.ToString());
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
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
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
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            if (exception is AggregateException)
            {
                return Create((AggregateException)exception);
            }
            if (exception is DbEntityValidationException)
            {
                return Create((DbEntityValidationException)exception);
            }
            return new FaultMessage(exception.InnerException?.Message ?? exception.Message);
        }
    }
}