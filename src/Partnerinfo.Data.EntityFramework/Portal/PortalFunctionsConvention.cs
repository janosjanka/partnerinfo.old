// Copyright (c) János Janka. All rights reserved.

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Partnerinfo.Portal.EntityFramework
{
    public class PortalFunctionsConvention : IStoreModelConvention<EdmModel>
    {
        /// <summary>
        /// Applies this convention to an item in the model.
        /// </summary>
        /// <param name="item">The item to apply the convention to.</param>
        /// <param name="model">The model.</param>
        public void Apply(EdmModel item, DbModel model)
        {
            PortalFunctions.Register(item, model);
        }
    }
}
