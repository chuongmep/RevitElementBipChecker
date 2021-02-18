﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;

namespace RevitElementBipChecker.Model
{
  public static class DialogUtils
    {
        /// <summary>
        /// Return Result Has Select Element Current Or Not
        /// </summary>
        /// <param name="Caption"></param>
        /// <returns></returns>
        public static bool QuestionMsg(string Caption)
        {
            
            var dialog = new TaskDialog(Caption);

            dialog.MainIcon = TaskDialogIcon.TaskDialogIconNone;
            dialog.MainInstruction = Caption;

            dialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Element Current");
            dialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Element In RvtLinked");

            return dialog.Show() == TaskDialogResult.CommandLink1;
        }
    }
}
