// Copyright (c) János Janka. All rights reserved.

using System.Composition.Convention;
using System.Composition.Hosting;
using System.Data.Entity;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace Partnerinfo.Composition
{
    /// <summary>
    /// A <see cref="AppContainerConfiguration" /> that loads parts in the currently-executing ASP.NET MVC web application.
    /// </summary>
    /// <remarks>
    /// This implementation emulates CompositionContainer and the composition-container based MVC
    /// integration for all types under the Parts namespace, for controllers, and for model binders. This will
    /// aid migration from one composition engine to the other, but this decision should be revisited if it
    /// causes confusion.
    /// </remarks>
    internal class AppContainerConfiguration : ContainerConfiguration
    {
        public AppContainerConfiguration()
        {
            WithAssemblies(new[]
            {
                typeof(UniqueItem).Assembly,
                typeof(PartnerDbContext).Assembly,
                typeof(Project.InMemory.ProjectCache).Assembly,

                typeof(Drive.DriveManager).Assembly,

                typeof(Input.CommandInvoker).Assembly,
                typeof(Input.Processors.PageModuleCommandProcessor).Assembly,
                typeof(Input.MailClient.CommandPop3Client).Assembly,

                typeof(Logging.LogManager).Assembly,

                typeof(Project.Actions.WorkflowInvoker).Assembly,
                typeof(Project.Actions.SequenceActionActivity).Assembly,

                typeof(Portal.PortalCompiler).Assembly,

                typeof(ApiConfig).Assembly,
                typeof(Startup).Assembly
            });

            WithDefaultConventions(DefineConventions());
        }

        private static AttributedModelProvider DefineConventions()
        {
            var convention = new ConventionBuilder();
            RegisterAspNetConventions(convention);
            RegisterCommonConventions(convention);
            RegisterSecurityConventions(convention);
            RegisterIdentityConventions(convention);
            RegisterLoggingConventions(convention);
            RegisterInputConventions(convention);
            RegisterProjectConventions(convention);
            RegisterPortalConventions(convention);
            return convention;
        }

        /// <summary>
        /// Exports data storage classes and interfaces
        /// </summary>
        private static void RegisterAspNetConventions(ConventionBuilder convention)
        {
            convention.ForTypesDerivedFrom<IController>().Export();
            convention.ForTypesDerivedFrom<IHttpController>().Export();
        }

        /// <summary>
        /// Exports data storage classes and interfaces
        /// </summary>
        private static void RegisterCommonConventions(ConventionBuilder convention)
        {
            convention.ForType<ServiceResolver>().Export<IServiceResolver>().Shared();
            convention.ForType<PartnerDbContext>().Export<DbContext>().Export<PartnerDbContext>();
            convention.ForType<PersistenceServices>().ExportInterfaces().Shared(SharingBoundaries.HttpRequest);
        }

        /// <summary>
        /// Exports identity classes and interfaces
        /// </summary>
        private static void RegisterLoggingConventions(ConventionBuilder convention)
        {
            convention.ForType<Logging.Rules.RuleEvaluator>().ExportInterfaces().Shared();
            convention.ForType<Logging.Rules.RuleProcessor>().ExportInterfaces().Shared();
            convention.ForType<Logging.Rules.RuleInvoker>().Export().Shared();

            convention.ForType<Logging.EntityFramework.CategoryStore>().Export<Logging.ICategoryStore>();
            convention.ForType<Logging.EntityFramework.EventStore>().Export<Logging.IEventStore>();
            convention.ForType<Logging.EntityFramework.RuleStore>().Export<Logging.IRuleStore>();
            convention.ForType<Logging.Reporting.EventReport>().Export<Logging.IEventReport>().Shared();
            convention.ForType<Logging.EventManager>()
                .ImportProperty(p => p.ReportGenerator)
                .Export()
                .Shared(SharingBoundaries.HttpRequest);
            convention.ForType<Logging.CategoryManager>().Export().Shared(SharingBoundaries.HttpRequest);
            convention.ForType<Logging.RuleManager>().Export().Shared(SharingBoundaries.HttpRequest);
            convention.ForType<Logging.LogManager>()
                .ImportProperty(p => p.RuleInvoker)
                .Export()
                .Shared(SharingBoundaries.HttpRequest);
        }

        /// <summary>
        /// Exports security classes and interfaces
        /// </summary>
        private static void RegisterSecurityConventions(ConventionBuilder convention)
        {
            convention.ForType<Security.AccessRuleValidator>().Export<Security.IAccessRuleValidator>().Shared();
            convention.ForType<Security.EntityFramework.SecurityStore>().Export<Security.IAccessRuleStore>();
            convention.ForType<Security.SecurityManager>().Export().Shared(SharingBoundaries.HttpRequest);
        }

        /// <summary>
        /// Exports identity classes and interfaces
        /// </summary>
        private static void RegisterIdentityConventions(ConventionBuilder convention)
        {
            convention.ForType<Identity.EntityFramework.UserStore>().Export<Identity.IUserStore>();
            convention.ForType<Identity.UserManager>().Export().Shared(SharingBoundaries.HttpRequest);
        }

        /// <summary>
        /// Exports identity classes and interfaces
        /// </summary>
        private static void RegisterInputConventions(ConventionBuilder convention)
        {
            convention.ForType<Input.EntityFramework.CommandStore>().Export<Input.ICommandStore>();
            convention.ForType<ServiceResolver>().Export<IServiceResolver>(c => c.AsContractName("Commanding"));

            convention.ForType<Input.CommandProcessor>().Export<Input.ICommandProcessor>(c => c.AsContractName("Commanding")).Shared();
            convention.ForType<Input.Processors.PageModuleCommandProcessor>()
                .Export<Input.ICommandProcessor>(c => c.AddMetadata("Processor", new Input.CommandMetadata("PAGE", "MODULE")))
                .Shared();

            convention.ForType<Input.CommandInvoker>()
                .SelectConstructor(c => new Input.CommandInvoker(
                    c.Import<IServiceResolver>(ic => ic.AsContractName("Commanding")),
                    c.Import<Input.ICommandProcessor>(ic => ic.AsContractName("Commanding"))))
                .Export<Input.ICommandInvoker>();
            convention.ForType<Input.MailClient.CommandPop3Client>().Export<Input.CommandClient>();
            convention.ForType<Input.CommandManager>()
                .ImportProperty(i => i.Invoker, c => c.AllowDefault())
                .Export()
                .Shared(SharingBoundaries.HttpRequest);
        }

        /// <summary>
        /// Exports project classes and interfaces
        /// </summary>
        private static void RegisterProjectConventions(ConventionBuilder convention)
        {
            convention.ForType<Project.Actions.ControlFlowActionActivity>()
                .Export<Project.Actions.IActionActivity>(c =>
                    c.AsContractName("Workflow")).Shared();

            convention.ForType<Project.Actions.RedirectActionActivity>()
                .Export<Project.Actions.IActionActivity>(c =>
                    c.AddMetadata("Action", new Project.Actions.ActionActivityMetadata(Project.ActionType.Redirect))).Shared();

            convention.ForType<Project.Actions.SequenceActionActivity>()
                .Export<Project.Actions.IActionActivity>(c =>
                    c.AddMetadata("Action", new Project.Actions.ActionActivityMetadata(Project.ActionType.Sequence))).Shared();

            convention.ForType<Project.Actions.ScheduleActionActivity>()
                .Export<Project.Actions.IActionActivity>(c =>
                    c.AddMetadata("Action", new Project.Actions.ActionActivityMetadata(Project.ActionType.Schedule))).Shared();

            convention.ForType<Project.Actions.ConditionActionActivity>()
                .Export<Project.Actions.IActionActivity>(c =>
                    c.AddMetadata("Action", new Project.Actions.ActionActivityMetadata(Project.ActionType.Condition))).Shared();

            convention.ForType<Project.Actions.AuthenticateActionActivity>()
                .Export<Project.Actions.IActionActivity>(c =>
                    c.AddMetadata("Action", new Project.Actions.ActionActivityMetadata(Project.ActionType.Authenticate))).Shared();

            convention.ForType<Project.Actions.RegisterActionActivity>()
                .Export<Project.Actions.IActionActivity>(c =>
                    c.AddMetadata("Action", new Project.Actions.ActionActivityMetadata(Project.ActionType.Register))).Shared();

            convention.ForType<Project.Actions.UnregisterActionActivity>()
                .Export<Project.Actions.IActionActivity>(c =>
                    c.AddMetadata("Action", new Project.Actions.ActionActivityMetadata(Project.ActionType.Unregister))).Shared();

            convention.ForType<Project.Actions.SetTagsActionActivity>()
                .Export<Project.Actions.IActionActivity>(c =>
                    c.AddMetadata("Action", new Project.Actions.ActionActivityMetadata(Project.ActionType.SetTags))).Shared();

            convention.ForType<Project.Actions.SendMailActionActivity>()
                .Export<Project.Actions.IActionActivity>(c =>
                    c.AddMetadata("Action", new Project.Actions.ActionActivityMetadata(Project.ActionType.SendMail))).Shared();

            convention.ForType<Project.Actions.LogActionActivity>()
                .Export<Project.Actions.IActionActivity>(c =>
                    c.AddMetadata("Action", new Project.Actions.ActionActivityMetadata(Project.ActionType.Log))).Shared();

            convention.ForType<Project.Actions.WorkflowInvoker>()
                .SelectConstructor(c => new Project.Actions.WorkflowInvoker(c.Import<Project.Actions.IActionActivity>(ic => ic.AsContractName("Workflow"))))
                .ImportProperty(p => p.Resolver)
                .Export()
                .Shared();

            convention.ForType<Project.EntityFramework.ProjectStore>().Export<Project.IProjectStore>();
            convention.ForType<Project.InMemory.ProjectCache>().Export<Project.IProjectCache>().Shared();
            convention.ForType<Project.ProjectValidator>().Export<Project.IProjectValidator>().Shared();

            convention.ForType<Project.Templating.StringTemplate>().Export().Shared();
            convention.ForType<Project.Actions.ActionLinkService>().Export<Project.Actions.IActionLinkService>().Shared();
            convention.ForType<Project.Mail.MailClientService>().Export<Project.Mail.IMailClientService>();
            convention.ForType<Project.ProjectManager>().Export().Shared(SharingBoundaries.HttpRequest);
        }

        /// <summary>
        /// Exports portal classes and interfaces
        /// </summary>
        private static void RegisterPortalConventions(ConventionBuilder convention)
        {
            convention.ForType<Portal.MediaStreamStore>().Export<Portal.IMediaStreamStore>().Shared();
            convention.ForType<Portal.EntityFramework.PortalStore>().Export<Portal.IPortalStore>();

            convention.ForType<Portal.PortalValidator>().Export<Portal.IPortalValidator>().Shared();

            convention.ForType<Portal.PortalManager>()
                .ImportProperty(p => p.MediaStreamStore)
                .Export()
                .Shared(SharingBoundaries.HttpRequest);

            convention.ForType<Portal.PortalCompiler>().Export().Shared(SharingBoundaries.HttpRequest);
        }
    }
}
