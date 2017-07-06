// Copyright (c) János Janka. All rights reserved.

using System.Threading;
using System.Threading.Tasks;

namespace Partnerinfo.Project
{
    public interface IProjectValidator
    {
        /// <summary>
        /// Validates the specified <paramref name="project" /> as an asynchronous operation.
        /// </summary>
        /// <param name="manager">The <see cref="ProjectManager" /> that can be used to retrieve user properties.</param>
        /// <param name="project">The project to validate.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task" /> that represents the asynchronous operation, containing the <see cref="ValidationResult" /> of the validation operation.
        /// </returns>
        Task<ValidationResult> ValidateAsync(ProjectManager manager, ProjectItem project, CancellationToken cancellationToken);
    }
}
