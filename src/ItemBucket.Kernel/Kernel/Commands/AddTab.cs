// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddTab.cs" company="Sitecore">
//   Sitecore
// </copyright>
// <summary>
//   Add a new Content Editor Tab to Content Editor so that users can search for hidden content
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Sitecore.Data.Managers;

namespace Sitecore.ItemBucket.Kernel.Commands
{
    using Sitecore.Configuration;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.ItemBucket.Kernel.Kernel.Commands;
    using Sitecore.ItemBucket.Kernel.Kernel.Util;
    using Sitecore.Resources;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Text;
    using Sitecore.Web;
    using Sitecore.Web.UI.Framework.Scripts;
    using Sitecore.Web.UI.Sheer;
    using System;

    /// <summary>
    /// Add a new Content Editor Tab to Content Editor so that users can search for hidden content
    /// </summary>
    internal class AddTab : BaseCommand
    {
        /// <summary>
        /// Add a new Content Editor Tab to Content Editor so that users can search for hidden content
        /// </summary>
        /// <param name="context">Context of Call</param>
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length == 1)
            {
                string s = context.Parameters[0];
                if (s.IsGuid())
                {
                    if (WebUtil.GetFormValue("scEditorTabs").Contains("contenteditor:launchtab") && WebUtil.GetFormValue("scEditorTabs").Contains(s))
                    {
                        SheerResponse.Eval("scContent.onEditorTabClick(null, null, '" + s + "')");
                    }
                    else
                    {
                        UrlString urlString = new UrlString(Util.Constants.ContentEditorRawUrlAddress);
                        urlString.Add(Util.Constants.OpenItemEditorQueryStringKeyName, s);
                        TrackOpenTab(context);
                        context.Items[0].Uri.AddToUrlString(urlString);
                        UIUtil.AddContentDatabaseParameter(urlString);
                        urlString.Add(Util.Constants.ModeQueryStringKeyName, "preview");
                        urlString.Add("il", "0");
                        urlString.Add(Util.Constants.RibbonQueryStringKeyName, "{D3A2D76F-02E6-49DE-BE90-D23C9771DC8D}");
                        string str3 = context.Parameters["la"] ?? Context.Language.CultureInfo.TwoLetterISOLanguageName;
                        urlString.Add("la", str3);
                        AddLatestVersionToUrlString(urlString, s, str3);
                        SheerResponse.Eval(new ShowEditorTab { Command = "contenteditor:launchtab", Header = Translate.Text(Context.ContentDatabase.GetItem(s).Name), Icon = Images.GetThemedImageSource("Applications/16x16/text_view.png"), Url = urlString.ToString(), Id = s, Closeable = true, Activate = Util.Constants.SettingsItem[Util.Constants.OpenSearchResult] != "New Tab Not Selected" }.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Determins when the table will be disabled or not
        /// </summary>
        /// <param name="context">Context of Call</param>
        /// <returns>Command State</returns>
        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length != 1)
            {
                return CommandState.Disabled;
            }
            Item item = context.Items[0];
            if (base.HasField(item, FieldIDs.LayoutField))
            {
                return base.QueryState(context);
            }
            return CommandState.Hidden;
        }

        #region Private Methods

        /// <summary>
        /// Overide Sitecore choosing which version to open and get the latest instead
        /// </summary>
        /// <param name="urlString">Raw Url String</param>
        /// <param name="itemId">Id of the Item that will have the its version checked</param>
        private static void AddLatestVersionToUrlString(UrlString urlString, string itemId, string language)
        {
            try
            {
                urlString.Remove(Util.Constants.VersionQueryStringKeyName);
                urlString.Add(Util.Constants.VersionQueryStringKeyName, Context.ContentDatabase.GetItem(itemId, LanguageManager.GetLanguage(language)).Versions.GetLatestVersion().Version.ToString());
            }
            catch (Exception exception)
            {
                Log.Audit("Trying to access an item that does exist from the recently opened tabs", exception);
            }
        }
        #endregion

        /// <summary>
        /// Tracks the opened tabs in the session
        /// </summary>
        /// <param name="context">Context of the Command</param>
        private static void TrackOpenTab(CommandContext context)
        {
            if (ClientContext.GetValue("RecentlyOpenedTabs") == null)
            {
                ClientContext.SetValue("RecentlyOpenedTabs", string.Empty);
            }
            object obj2 = ClientContext.GetValue("RecentlyOpenedTabs");
            if ((obj2 != null) && !obj2.ToString().Contains("|" + context.Parameters[0] + "|"))
            {
                ClientContext.SetValue("RecentlyOpenedTabs", string.Concat(new object[] { ClientContext.GetValue("RecentlyOpenedTabs"), "|", context.Parameters[0], "|" }));
            }
        }
    }
}
