using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitElementBipChecker.View;
using RevitElementBipChecker.Viewmodel;

namespace RevitElementBipChecker.Command
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            //Reference r = uidoc.Selection.PickObject(ObjectType.Element);
            //Element element = doc.GetElement(r);
            //string asString = element.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString();
            //MessageBox.Show(asString);

            BipCheckerViewmodel vm = new BipCheckerViewmodel(uidoc);
            MainWindows frMainWindows = new MainWindows(vm);
            frMainWindows.Topmost = true;
            frMainWindows.Show();
            return Result.Succeeded;
        }
    }
}
