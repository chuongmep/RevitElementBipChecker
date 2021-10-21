using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.DB;
using RevitElementBipChecker.Model;
using InvalidOperationException = Autodesk.Revit.Exceptions.InvalidOperationException;

namespace RevitElementBipChecker.Viewmodel
{
    public class SelectedCommand : BaseElement
    {
        
        public SelectedCommand(BipCheckerViewmodel vm)
        {
            int count = vm.UIDoc.GetSelection().Count;
            if (count>0)
            {
                Element =vm.UIDoc.GetSelection().First();
                vm.Element = Element;
                vm.State = "Element"; 
                vm.GetDatabase();
            }
            

        }

        
    }
}
