using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitElementBipChecker.Model;
using RevitElementBipChecker.View;
using RevitElementBipChecker.Viewmodel;

namespace RevitElementBipChecker.Command
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            BipCheckerViewmodel vm = new BipCheckerViewmodel(uidoc);
            MainWindows frMainWindows = new MainWindows(vm);
            frMainWindows.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            frMainWindows.SetRevitAsWindowOwner();
            frMainWindows.Show();
            return Result.Succeeded;
        }
    }
}
