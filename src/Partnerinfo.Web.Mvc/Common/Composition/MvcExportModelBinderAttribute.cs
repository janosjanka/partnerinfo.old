// Copyright (c) János Janka. All rights reserved.

using System;
using System.Composition;
using System.Web.Mvc;

namespace Partnerinfo.Composition
{
    /// <summary>
    /// Marks a type as being a model binder for a specified model type. The type decorated with
    /// this attribute must implement the <see cref="IModelBinder"/> interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
    internal class MvcExportModelBinderAttribute : ExportAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MvcExportModelBinderAttribute" /> class.
        /// </summary>
        /// <param name="modelType">The model type bound by the model binder.</param>
        public MvcExportModelBinderAttribute(Type modelType)
            : base(MvcModelBinderProvider.GetModelBinderContractName(modelType), typeof(IModelBinder))
        {
        }
    }
}