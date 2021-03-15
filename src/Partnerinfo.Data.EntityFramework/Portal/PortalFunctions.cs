// Copyright (c) János Janka. All rights reserved.

using System;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;

namespace Partnerinfo.Portal.EntityFramework
{
    internal static class PortalFunctions
    {
        internal const string GetMediaUriByIdFunction = "GetMediaUriById";
        internal const string GetPageUriByIdFunction = "GetPageUriById";

        /// <summary>
        /// Registers all the portal functions for the specified EDM model.
        /// </summary>
        /// <param name="item">The item to apply the convention to.</param>
        /// <param name="model">The model.</param>
        public static void Register(EdmModel item, DbModel model)
        {
            RegisterGetMediaUriById(item, model);
            RegisterGetPageUriById(item, model);
        }

        /// <summary>
        /// Gets the URI for the specified media <paramref name="id" /> calling a table-value function.
        /// This function is just supported on LINQ To Entities queries.
        /// </summary>
        /// <param name="id">The identifier for the media to be found.</param>
        /// <returns>
        /// The URI for the specified media <paramref name="id" />.
        /// </returns>
        /// <exception cref="System.NotSupportedException">Thrown when called from code.</exception>
        [DbFunction("Partnerinfo", GetMediaUriByIdFunction)]
        public static string GetMediaUriById(int id)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the URI for the specified page <paramref name="id" /> calling a table-value function.
        /// This function is just supported on LINQ To Entities queries.
        /// </summary>
        /// <param name="id">The identifier for the page to be found.</param>
        /// <returns>
        /// The URI for the specified page <paramref name="id" />.
        /// </returns>
        /// <exception cref="System.NotSupportedException">Thrown when called from code.</exception>
        [DbFunction("Partnerinfo", GetPageUriByIdFunction)]
        public static string GetPageUriById(int id)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Registers the <see cref="GetMediaUriById" /> function.
        /// </summary>
        /// <param name="item">The item to apply the convention to.</param>
        /// <param name="model">The model.</param>
        private static void RegisterGetMediaUriById(EdmModel item, DbModel model)
        {
            var idParameter = FunctionParameter.Create(
                "Id",
                model.GetStorePrimitiveType(PrimitiveTypeKind.Int32),
                ParameterMode.In);

            var returnValue = FunctionParameter.Create(
                "Result",
                model.GetStorePrimitiveType(PrimitiveTypeKind.String),
                ParameterMode.ReturnValue);

            item.CreateAndAddFunction(GetMediaUriByIdFunction, new[] { idParameter }, new[] { returnValue });
        }

        /// <summary>
        /// Registers the <see cref="GetPageUriById" /> function.
        /// </summary>
        /// <param name="item">The item to apply the convention to.</param>
        /// <param name="model">The model.</param>
        private static void RegisterGetPageUriById(EdmModel item, DbModel model)
        {
            var idParameter = FunctionParameter.Create(
                "Id",
                model.GetStorePrimitiveType(PrimitiveTypeKind.Int32),
                ParameterMode.In);

            var returnValue = FunctionParameter.Create(
                "Result",
                model.GetStorePrimitiveType(PrimitiveTypeKind.String),
                ParameterMode.ReturnValue);

            item.CreateAndAddFunction(GetPageUriByIdFunction, new[] { idParameter }, new[] { returnValue });
        }
    }
}
