// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;

namespace Partnerinfo
{
    internal static class DbMetadataExtensions
    {
        public static EdmFunction CreateAndAddFunction(this EdmModel item, string name,
            IList<FunctionParameter> parameters, IList<FunctionParameter> returnValues, string body = null)
        {
            var payload = new EdmFunctionPayload
            {
                StoreFunctionName = name,
                Parameters = parameters,
                ReturnParameters = returnValues,
                Schema = "Portal"
            };
            var function = EdmFunction.Create(name, "Partnerinfo", item.DataSpace, payload, null);
            item.AddItem(function);
            return function;
        }

        /// <summary>
        ///     Translate a conceptual primitive type to an adequate store specific primitive type according to the
        ///     provider configuration of the model.
        /// </summary>
        /// <param name="model">A DbModel instance with provider information</param>
        /// <param name="typeKind">A PrimitiveTypeKind instance representing the conceptual primitive type to be translated</param>
        /// <returns>An EdmType instance representing the store primitive type</returns>
        public static EdmType GetStorePrimitiveType(this DbModel model, PrimitiveTypeKind typeKind)
        {
            return model
                .ProviderManifest
                .GetStoreType(
                    TypeUsage.CreateDefaultTypeUsage(
                        PrimitiveType.GetEdmPrimitiveType(typeKind))).EdmType;
        }
    }
}
