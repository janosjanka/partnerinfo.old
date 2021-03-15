// Copyright (c) János Janka. All rights reserved.

using System;
using System.Web.Mvc;

namespace Partnerinfo.Composition
{
    internal class MvcModelBinderProvider : IModelBinderProvider
    {
        private const string ModelBinderContractNameSuffix = "++ModelBinder";

        /// <summary>
        /// Gets the name of the model binder contract.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <returns></returns>
        public static string GetModelBinderContractName(Type modelType)
        {
            return modelType.AssemblyQualifiedName + ModelBinderContractNameSuffix;
        }

        /// <summary>
        /// Returns the model binder for the specified type.
        /// </summary>
        /// <param name="modelType">The type of the model.</param>
        /// <returns>
        /// The model binder for the specified type.
        /// </returns>
        public IModelBinder GetBinder(Type modelType)
        {
            IModelBinder export;
            HttpCompositionProvider.Current.TryGetExport(GetModelBinderContractName(modelType), out export);
            return export;
        }
    }
}