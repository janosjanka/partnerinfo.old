// Copyright (c) János Janka. All rights reserved.

using System.Collections.Generic;
using System.Web.Optimization;
using Owin;

namespace Partnerinfo
{
    public partial class Startup
    {
        public static void ConfigureBundles(IAppBuilder app)
        {
            RegisterBootstrap(BundleCollection);

            RegisterFancytree(BundleCollection);
            RegisterFileUpload(BundleCollection);
            RegisterTinyMCE(BundleCollection);
            RegisterCodeMirror(BundleCollection);
            //RegisterSignalR(BundleCollection);

            RegisterLogging(BundleCollection);
            RegisterIdentity(BundleCollection);
            RegisterInput(BundleCollection);
            RegisterSearch(BundleCollection);
            RegisterDrive(BundleCollection);
            RegisterProject(BundleCollection);
            RegisterPortal(BundleCollection);
            RegisterSecurity(BundleCollection);
        }

        /// <summary>
        /// Registers base scripts and styles.
        /// </summary>
        /// <param name="bundles">Contains and manages the set of registered Bundle objects in an ASP.NET application.</param>
        private static void RegisterBootstrap(BundleCollection bundles)
        {
            bundles.Add(JsBundle("~/assets/bootstrap").Include(
                "~/Assets/js/bootstrap/ecma.js",

                "~/Assets/js/base/globalize/globalize.js",
                "~/Assets/js/base/globalize/cultures/globalize.culture.hu.js",
                "~/Assets/js/base/globalize/cultures/globalize.culture.hu-HU.js",
                "~/Assets/js/base/jquery/jquery.js",
                "~/Assets/js/base/jquery/jquery.ui.js",
                "~/Assets/js/base/form2js/form2js.js",
                "~/Assets/js/base/form2js/jquery.form2js.js",
                "~/Assets/js/base/sammy/jquery.sammy.js",
                "~/Assets/js/base/scrolltofixed/jquery.scrolltofixed.js",
                "~/Assets/js/base/signalr/jquery.signalr.js",
                "~/Assets/js/base/tokeninput/tokeninput.js",
                "~/Assets/js/base/typeahead/typeahead.jquery.js",

                "~/Assets/js/base/knockout/knockout.js",
                "~/Assets/js/base/knockout/mapping.js",
                "~/Assets/js/base/knockout/projections.js",
                "~/Assets/js/base/knockout/validation.js",
                "~/Assets/js/base/knockout/cultures/hu-HU/validation.js",
                //"~/Assets/js/base/knockout/sortable.js",

                "~/Assets/js/bootstrap/en-HU/bootstrap.js",
                "~/Assets/js/bootstrap/hu-HU/bootstrap.js",
                "~/Assets/js/bootstrap/require.js",
                "~/Assets/js/bootstrap/ko-editsession.js",

                "~/Assets/js/bootstrap/winjs-core.js",
                "~/Assets/js/bootstrap/winjs-resources.js",
                "~/Assets/js/bootstrap/winjs-utilities.js",
                "~/Assets/js/bootstrap/winjs-messaging.js",
                "~/Assets/js/bootstrap/winjs-media.js",
                "~/Assets/js/bootstrap/winjs-common.js",
                "~/Assets/js/bootstrap/winjs-promise.js",
                "~/Assets/js/bootstrap/winjs-classes.js",
                "~/Assets/js/bootstrap/winjs-collections.js",

                "~/Assets/js/bootstrap/ui-dropdown.js",
                "~/Assets/js/bootstrap/ui-toggle.js",
                "~/Assets/js/bootstrap/ui-colorpicker.js",
                "~/Assets/js/bootstrap/ui-listview.js",
                "~/Assets/js/bootstrap/ui-dataerrors.js",
                "~/Assets/js/bootstrap/ui-datapager.js",
                "~/Assets/js/bootstrap/ui-datetime.js",
                "~/Assets/js/bootstrap/ui-jquery.js",
                "~/Assets/js/bootstrap/ui-searchbox.js",
                "~/Assets/js/bootstrap/ui-tokeninput.js",
                "~/Assets/js/bootstrap/ui-typeahead.js",
                "~/Assets/js/bootstrap/ui-timespan.js",
                "~/Assets/js/bootstrap/ui-dialog.js",

                "~/Assets/js/_prefix.js",
                    "~/Assets/js/strings/hu-hu/core.js",
                    "~/Assets/js/config.js",
                    "~/Assets/js/core.js",
                    "~/Assets/js/routes.js",
                    "~/Assets/js/types.js",
                    "~/Assets/js/sound.js",
                    "~/Assets/js/ui.js",
                    "~/Assets/js/components.js",
                    "~/Assets/js/expression.js",

                    "~/Assets/js/logging/services/notification.js",
                    "~/Assets/js/logging/viewmodels/notification.js",
                    "~/Assets/js/project/authentication.js",
                "~/Assets/js/_suffix.js"
            ));

            bundles.Add(CssBundle("~/assets/bootstrap").Include(
                "~/Assets/js/base/jquery/jquery.ui.css",

                "~/Assets/js/base/tokeninput/tokeninput.css",

                "~/Assets/ss/bootstrap/font.css",
                "~/Assets/ss/bootstrap/normalize.css",
                "~/Assets/ss/bootstrap/layout.css",

                "~/Assets/ss/bootstrap/menubar.css",
                "~/Assets/ss/bootstrap/button.css",
                "~/Assets/ss/bootstrap/toggle.css",
                "~/Assets/ss/bootstrap/dropdown.css",
                "~/Assets/ss/bootstrap/colorpicker.css",
                "~/Assets/ss/bootstrap/datapager.css",
                "~/Assets/ss/bootstrap/datetime.css",
                "~/Assets/ss/bootstrap/searchbox.css",
                "~/Assets/ss/bootstrap/timespan.css",
                "~/Assets/ss/bootstrap/typeahead.css",
                "~/Assets/ss/bootstrap/listview.css",

                "~/Assets/ss/bootstrap/media.css",
                "~/Assets/ss/bootstrap/list.css",
                "~/Assets/ss/bootstrap/table.css",
                "~/Assets/ss/bootstrap/input.css",
                "~/Assets/ss/bootstrap/box.css",
                "~/Assets/ss/bootstrap/dialog.css",
                "~/Assets/ss/bootstrap/navbar.css",

                "~/Assets/ss/bootstrap/types.css",
                "~/Assets/ss/bootstrap/theme.css",

                "~/Assets/ss/app.css",
                "~/Assets/ss/icons.css"
            ));

            bundles.Add(HtmlBundle("~/assets/security/acl").Include("~/Assets/tt/security/acl.html"));
        }

        /// <summary>
        /// Registers fancytree scripts and styles.
        /// </summary>
        /// <param name="bundles">Contains and manages the set of registered Bundle objects in an ASP.NET application.</param>
        private static void RegisterFancytree(BundleCollection bundles)
        {
            bundles.Add(JsBundle("~/assets/fancytree").Include(
                "~/Assets/js/base/fancytree/jquery.fancytree.js",
                "~/Assets/js/base/fancytree/jquery.fancytree.dnd.js",
                "~/Assets/js/base/fancytree/jquery.fancytree.edit.js",
                "~/Assets/js/base/fancytree/jquery.fancytree.filter.js",
                "~/Assets/js/base/fancytree/jquery.fancytree.persist.js",
                "~/Assets/js/base/fancytree/jquery.fancytree.table.js"
            ));

            bundles.Add(CssBundle("~/assets/fancytree").Include(
                "~/Assets/js/base/fancytree/jquery.fancytree.css",
                "~/Assets/js/base/fancytree/jquery.fancytree.edit.css"
            ));
        }

        /// <summary>
        /// Registers fileUploader scripts and styles.
        /// </summary>
        /// <param name="bundles">Contains and manages the set of registered Bundle objects in an ASP.NET application.</param>
        private static void RegisterFileUpload(BundleCollection bundles)
        {
            bundles.Add(JsBundle("~/assets/fileupload").Include(
                "~/Assets/js/base/fileupload/jquery.iframe-transport.js",
                "~/Assets/js/base/fileupload/jquery.fileupload.js",
                "~/Assets/js/bootstrap/ui-fileupload.js"));

            bundles.Add(CssBundle("~/assets/fileupload").Include(
                "~/Assets/js/base/fileupload/jquery.fileupload-ui.css"));
        }

        /// <summary>
        /// Registers tinyMCE scripts and styles.
        /// </summary>
        /// <param name="bundles">Contains and manages the set of registered Bundle objects in an ASP.NET application.</param>
        private static void RegisterTinyMCE(BundleCollection bundles)
        {
            bundles.Add(JsBundle("~/assets/tinymce").Include(
                "~/Assets/js/base/tinymce/tinymce.min.js",
                "~/Assets/js/tinymce/_prefix.js",
                    "~/Assets/js/strings/HU-HU/tinymce.js",
                    "~/Assets/js/tinymce/pi.tinymce.preinit.js",
                    "~/Assets/js/tinymce/plugins/actionlink.js",
                    "~/Assets/js/tinymce/plugins/codemirror.js",
                    "~/Assets/js/tinymce/plugins/placeholders.js",
                    "~/Assets/js/tinymce/plugins/projectforms.js",
                    "~/Assets/js/tinymce/tinymce.js",
                 "~/Assets/js/tinymce/_suffix.js"
            ));

            bundles.Add(CssBundle("~/assets/tinymce").Include(
                "~/Assets/ss/tinymce/tinymce.css"));

            bundles.Add(CssBundle("~/assets/tinymce/content").Include(
                "~/Assets/ss/tinymce/tinymce.content.css"));
        }

        /// <summary>
        /// Registers CodeMirror scripts and styles.
        /// </summary>
        /// <param name="bundles">Contains and manages the set of registered Bundle objects in an ASP.NET application.</param>
        private static void RegisterCodeMirror(BundleCollection bundles)
        {
            bundles.Add(JsBundle("~/assets/codemirror").Include(
                "~/Assets/js/base/beautify/beautify.js",
                "~/Assets/js/base/beautify/beautify-css.js",
                "~/Assets/js/base/beautify/beautify-html.js",
                "~/Assets/js/base/codemirror/lib/codemirror.js",
                "~/Assets/js/base/codemirror/addon/edit/closebrackets.js",
                "~/Assets/js/base/codemirror/addon/edit/closetag.js",
                "~/Assets/js/base/codemirror/addon/dialog/dialog.js",
                "~/Assets/js/base/codemirror/addon/search/search.js",
                "~/Assets/js/base/codemirror/addon/search/searchcursor.js",
                "~/Assets/js/base/codemirror/addon/edit/matchbrackets.js",
                "~/Assets/js/base/codemirror/addon/edit/matchtags.js",
                "~/Assets/js/base/codemirror/addon/fold/xml-fold.js",
                "~/Assets/js/base/codemirror/addon/selection/active-line.js",
                "~/Assets/js/base/codemirror/mode/javascript/javascript.js",
                "~/Assets/js/base/codemirror/mode/xml/xml.js",
                "~/Assets/js/base/codemirror/mode/css/css.js",
                "~/Assets/js/bootstrap/ui-codemirror.js"));

            bundles.Add(CssBundle("~/assets/codemirror").Include(
                "~/Assets/js/base/codemirror/lib/codemirror.css",
                "~/Assets/js/base/codemirror/addon/dialog/dialog.css",
                "~/Assets/js/base/codemirror/theme/visual-studio.css"));
        }

        /// <summary>
        /// Registers SignalR scripts and styles.
        /// </summary>
        /// <param name="bundles">Contains and manages the set of registered Bundle objects in an ASP.NET application.</param>
        private static void RegisterSignalR(BundleCollection bundles)
        {
            bundles.Add(JsBundle("~/assets/signalr").Include(
                "~/Assets/js/base/signalr/jquery.signalr.js"));
        }

        /// <summary>
        /// Registers search scripts and styles.
        /// </summary>
        /// <param name="bundles">Contains and manages the set of registered Bundle objects in an ASP.NET application.</param>
        private static void RegisterSearch(BundleCollection bundles)
        {
            bundles.Add(JsBundle("~/assets/search").Include(
                "~/Assets/js/base/jcarousel/jquery.jcarousel-core.js",
                "~/Assets/js/base/jcarousel/jquery.jcarousel-control.js",
                "~/Assets/js/_prefix.js",
                    "~/Assets/js/search/types.js",
                    "~/Assets/js/search/engine.js",
                    "~/Assets/js/search/engine.ui.js",
                    "~/Assets/js/search/services/playlist.js",
                    "~/Assets/js/search/modules/login.js",
                    "~/Assets/js/search/modules/banner.js",
                    "~/Assets/js/search/modules/media.js",
                    "~/Assets/js/search/modules/playlist.js",
                    "~/Assets/js/search/modules/logging.js",
                    "~/Assets/js/search/providers/youtube.js",
                    "~/Assets/js/search/providers/dailymotion.js",
                "~/Assets/js/_suffix.js"));

            bundles.Add(CssBundle("~/assets/search").Include(
                "~/Assets/ss/search/modules/banner.css",
                "~/Assets/ss/search/modules/media.css",
                "~/Assets/ss/search/modules/playlist.css",
                "~/Assets/ss/search/medialist.css",
                "~/Assets/ss/search/base.css",
                "~/Assets/ss/search/icons.css"));

            bundles.Add(HtmlBundle("~/assets/search/search").Include("~/Assets/tt/search/search.html"));
            bundles.Add(HtmlBundle("~/assets/search/youtube").Include("~/Assets/tt/search/providers/youtube.html"));
            bundles.Add(HtmlBundle("~/assets/search/dailymotion").Include("~/Assets/tt/search/providers/dailymotion.html"));
        }

        /// <summary>
        /// Registers drive scripts and styles.
        /// </summary>
        /// <param name="bundles">Contains and manages the set of registered Bundle objects in an ASP.NET application.</param>
        private static void RegisterDrive(BundleCollection bundles)
        {
            bundles.Add(JsBundle("~/assets/drive").Include(
                "~/Assets/js/_prefix.js",
                    "~/Assets/js/drive/document.js",
                "~/Assets/js/_suffix.js"));

            bundles.Add(CssBundle("~/assets/drive")
                .IncludeDirectory("~/Assets/ss/drive", "*.css", false));

            bundles.Add(HtmlBundle("~/assets/drive/files").Include("~/Assets/tt/drive/files.html"));
        }

        /// <summary>
        /// Registers identity scripts and styles.
        /// </summary>
        /// <param name="bundles">Contains and manages the set of registered Bundle objects in an ASP.NET application.</param>
        private static void RegisterLogging(BundleCollection bundles)
        {
            bundles.Add(JsBundle("~/assets/logging")
                .IncludeDirectory("~/Assets/js/logging/services", "*.js", false)
                .IncludeDirectory("~/Assets/js/logging/viewmodels", "*.js", false)
                .IncludeDirectory("~/Assets/js/logging/views", "*.js", false));

            bundles.Add(CssBundle("~/assets/logging")
                .IncludeDirectory("~/Assets/ss/logging", "*.css", false));

            bundles.Add(HtmlBundle("~/assets/logging/events").Include("~/Assets/tt/logging/events.html"));
            bundles.Add(HtmlBundle("~/assets/logging/rules").Include("~/Assets/tt/logging/rules.html"));
        }

        /// <summary>
        /// Registers identity scripts and styles.
        /// </summary>
        /// <param name="bundles">Contains and manages the set of registered Bundle objects in an ASP.NET application.</param>
        private static void RegisterIdentity(BundleCollection bundles)
        {
            bundles.Add(JsBundle("~/assets/identity")
                .IncludeDirectory("~/Assets/js/identity/strings/hu-hu", "*.js", false)
                .IncludeDirectory("~/Assets/js/identity/services", "*.js", false)
                .IncludeDirectory("~/Assets/js/identity/viewmodels", "*.js", false)
                .IncludeDirectory("~/Assets/js/identity/views", "*.js", false));

            bundles.Add(HtmlBundle("~/assets/identity/users").Include("~/Assets/tt/identity/users.html"));
        }

        /// <summary>
        /// Registers input scripts and styles.
        /// </summary>
        /// <param name="bundles">Contains and manages the set of registered Bundle objects in an ASP.NET application.</param>
        private static void RegisterInput(BundleCollection bundles)
        {
            bundles.Add(JsBundle("~/assets/input").Include(
                "~/Assets/js/_prefix.js",
                    "~/Assets/js/input/services/command.js",
                    "~/Assets/js/input/views/command.js",
                "~/Assets/js/_suffix.js"
            ));
        }

        /// <summary>
        /// Registers project scripts and styles.
        /// </summary>
        /// <param name="bundles">Contains and manages the set of registered Bundle objects in an ASP.NET application.</param>
        private static void RegisterProject(BundleCollection bundles)
        {
            bundles.Add(JsBundle("~/assets/project/app").Include(
                "~/Assets/js/_prefix.js",
                    "~/Assets/js/project/app.js",
                "~/Assets/js/_suffix.js"
            ));

            bundles.Add(JsBundle("~/assets/project")
                .IncludeDirectory("~/Assets/js/project/strings/hu-hu", "*.js", false)
                .IncludeDirectory("~/Assets/js/project/services", "*.js", false)
                .IncludeDirectory("~/Assets/js/project/viewmodels", "*.js", false)
                .IncludeDirectory("~/Assets/js/project/views", "*.js", false));

            bundles.Add(CssBundle("~/assets/project")
                .IncludeDirectory("~/Assets/ss/project", "*.css", false));

            bundles.Add(HtmlBundle("~/assets/project/project").Include("~/Assets/tt/project/project.html"));
            bundles.Add(HtmlBundle("~/assets/project/projects").Include("~/Assets/tt/project/projects.html"));
            bundles.Add(HtmlBundle("~/assets/project/projects-menu").Include("~/Assets/tt/project/projects-menu.html"));
            bundles.Add(HtmlBundle("~/assets/project/businesstags").Include("~/Assets/tt/project/businesstags.html"));
            bundles.Add(HtmlBundle("~/assets/project/contact").Include("~/Assets/tt/project/contact.html"));
            bundles.Add(HtmlBundle("~/assets/project/contacts").Include("~/Assets/tt/project/contacts.html"));
            bundles.Add(HtmlBundle("~/assets/project/mail").Include("~/Assets/tt/project/mail.html"));
            bundles.Add(HtmlBundle("~/assets/project/mail-picker").Include("~/Assets/tt/project/mail-picker.html"));
            bundles.Add(HtmlBundle("~/assets/project/mails").Include("~/Assets/tt/project/mails.html"));
            bundles.Add(HtmlBundle("~/assets/project/action").Include("~/Assets/tt/project/action.html"));
            bundles.Add(HtmlBundle("~/assets/project/actions").Include("~/Assets/tt/project/actions.html"));
            bundles.Add(HtmlBundle("~/assets/project/actionpicker").Include("~/Assets/tt/project/actionpicker.html"));
            bundles.Add(HtmlBundle("~/assets/project/actionlink").Include("~/Assets/tt/project/actionlink.html"));
        }

        /// <summary>
        /// Registers portal scripts and styles.
        /// </summary>
        /// <param name="bundles">Contains and manages the set of registered Bundle objects in an ASP.NET application.</param>
        private static void RegisterPortal(BundleCollection bundles)
        {
            bundles.Add(JsBundle("~/assets/portal/app").Include(
                "~/Assets/js/_prefix.js",
                    "~/Assets/js/portal/app.js",
                "~/Assets/js/_suffix.js"
            ));

            bundles.Add(JsBundle("~/assets/portal")
                .IncludeDirectory("~/Assets/js/portal/strings/hu-hu", "*.js", false)
                .IncludeDirectory("~/Assets/js/portal/services", "*.js", false)
                .IncludeDirectory("~/Assets/js/portal/viewmodels", "*.js", false)
                .IncludeDirectory("~/Assets/js/portal/views", "*.js", false));

            bundles.Add(CssBundle("~/assets/portal")
                .IncludeDirectory("~/Assets/ss/portal", "*.css", false));

            bundles.Add(JsBundle("~/assets/portal/designer")
                .Include("~/Assets/js/bootstrap/ui-htmltree.js",
                         "~/Assets/js/bootstrap/ui-csseditor.js",
                         "~/Assets/js/strings/HU-HU/portal.js",
                         "~/Assets/js/portal/modules/edit.js",
                         "~/Assets/js/portal/modules/event/editor.js")
                .IncludeDirectory("~/Assets/js/portal/managers", "*.js", false)
                .IncludeDirectory("~/Assets/js/portal/toolwins", "*.js", false)
                .Include("~/Assets/js/portal/designer.js"));

            bundles.Add(CssBundle("~/assets/portal/designer")
                .Include("~/Assets/ss/bootstrap/csseditor.css",
                         "~/Assets/ss/portal/modules/event.css")
                .IncludeDirectory("~/Assets/ss/portal/designer", "*.css", false)
                .IncludeDirectory("~/Assets/ss/portal/toolwins", "*.css", false));

            bundles.Add(JsBundle("~/assets/portal/modules").Include(
                "~/Assets/js/_prefix.js",
                    "~/Assets/js/portal/services/portal.js",
                    "~/Assets/js/portal/services/page.js",
                    "~/Assets/js/portal/common.js",
                    "~/Assets/js/portal/analytics.js",
                    "~/Assets/js/portal/context.js",
                    "~/Assets/js/portal/engine.js",
                    "~/Assets/js/portal/modules/base.js",
                    "~/Assets/js/portal/modules/live.js",

                    "~/Assets/js/portal/modules/event/module.js",
                    "~/Assets/js/portal/modules/animatedpanel/cycle.js",
                    "~/Assets/js/portal/modules/animatedpanel/module.js",
                    "~/Assets/js/portal/modules/content/module.js",
                    "~/Assets/js/portal/modules/form/module.js",
                    "~/Assets/js/portal/modules/frame/module.js",
                    "~/Assets/js/portal/modules/image/module.js",
                    "~/Assets/js/portal/modules/link/module.js",
                    "~/Assets/js/portal/modules/panel/module.js",
                    "~/Assets/js/portal/modules/search/module.js",
                    "~/Assets/js/portal/modules/scroller/module.js",
                    "~/Assets/js/portal/modules/html/module.js",
                    "~/Assets/js/portal/modules/timer/module.js",
                    "~/Assets/js/portal/modules/video/module.js",
                "~/Assets/js/_suffix.js"
            ));

            bundles.Add(CssBundle("~/assets/portal/modules").IncludeDirectory(
                "~/Assets/ss/portal/modules", "*.css", false
            ));

            bundles.Add(HtmlBundle("~/assets/portal/media").Include("~/Assets/tt/portal/media.html"));
            bundles.Add(HtmlBundle("~/assets/portal/page").Include("~/Assets/tt/portal/page.html"));
            bundles.Add(HtmlBundle("~/assets/portal/portal").Include("~/Assets/tt/portal/portal.html"));
            bundles.Add(HtmlBundle("~/assets/portal/portals").Include("~/Assets/tt/portal/portals.html"));
            bundles.Add(HtmlBundle("~/assets/portal/designer/menu").Include("~/Assets/tt/portal/designer/menu.html"));
            bundles.Add(HtmlBundle("~/assets/portal/toolwins/style").Include("~/Assets/tt/portal/toolwins/style.html"));
            bundles.Add(HtmlBundle("~/assets/portal/modules/event/list").Include("~/Assets/js/portal/modules/event/list.html"));
        }

        /// <summary>
        /// Registers security scripts and styles.
        /// </summary>
        /// <param name="bundles">Contains and manages the set of registered Bundle objects in an ASP.NET application.</param>
        private static void RegisterSecurity(BundleCollection bundles)
        {
            bundles.Add(JsBundle("~/assets/security")
                .IncludeDirectory("~/Assets/js/security/strings/hu-hu", "*.js", false)
                .IncludeDirectory("~/Assets/js/security/services", "*.js", false)
                .IncludeDirectory("~/Assets/js/security/viewmodels", "*.js", false)
                .IncludeDirectory("~/Assets/js/security/views", "*.js", false));

            bundles.Add(HtmlBundle("~/assets/security/acl").Include("~/Assets/tt/security/acl.html"));
        }

        private static Bundle JsBundle(string path)
        {
#if DEBUG
            var bundle = new Bundle(path + ".js");
#else
            var bundle = new ScriptBundle(path + ".js");
#endif
            bundle.Orderer = new ForEachOrderer();
            return bundle;
        }

        private static Bundle CssBundle(string path)
        {
#if DEBUG
            var bundle = new Bundle(path + ".css");
#else
            var bundle = new StyleBundle(path + ".css");
#endif
            bundle.Orderer = new ForEachOrderer();
            return bundle;
        }

        private static Bundle HtmlBundle(string path)
        {
            var bundle = new Bundle(path + ".html");
#if !DEBUG
            //bundle.Transforms.Add(new HtmlMinify());
#endif
            return bundle;
        }
    }

    internal class ForEachOrderer : IBundleOrderer
    {
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }
}