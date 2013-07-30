using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web;

namespace Sitecore.ItemBucket.Kernel.Kernel.Commands
{
    class SaveAndClose :Command
    {
        // Methods
        public override void Execute(CommandContext context)
        {
            Context.ClientPage.SendMessage(this, "item:save(postaction=contenteditor:closepreview)");
        }

        public override CommandState QueryState(CommandContext context)
        {
            Error.AssertObject(context, "context");
            if (((WebUtil.GetQueryString("mo") != "preview") && (WebUtil.GetQueryString("mo") != "popup")) || (WebUtil.GetQueryString("il") != "1"))
            {
                return CommandState.Hidden;
            }
            CommandState state = CommandManager.QueryState("contenteditor:save", context.Items);
            if (state != CommandState.Enabled)
            {
                return state;
            }
            state = CommandManager.QueryState("contenteditor:closepreview", context.Items);
            if (state != CommandState.Enabled)
            {
                return state;
            }
            return base.QueryState(context);
        }

    }
}
