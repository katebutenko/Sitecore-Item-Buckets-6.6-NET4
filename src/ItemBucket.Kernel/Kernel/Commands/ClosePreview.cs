using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitecore.ItemBucket.Kernel.Kernel.Commands
{
    class ClosePreview : Sitecore.Shell.Framework.Commands.ContentEditor.ClosePreview
    {
        public override CommandState QueryState(CommandContext context)
        {
            Error.AssertObject(context, "context");
            if (((WebUtil.GetQueryString("mo") == "preview") || (WebUtil.GetQueryString("mo") == "popup")) && !(WebUtil.GetQueryString("il") != "1"))
            {
                return base.QueryState(context);
            }
            return CommandState.Hidden;
        }

    }
}
