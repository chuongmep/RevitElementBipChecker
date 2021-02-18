using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using OOG.Core.RevitData;
using RevitElementBipChecker.Model;
using RevitElementBipChecker.View;

namespace RevitElementBipChecker.Viewmodel
{
    public class BipCheckerViewmodel : ViewmodeBase
    {
        public UIDocument UIdoc;
        public Document Doc;
        public MainWindows frmmain;
        private RevitEvent revitEvent = new RevitEvent();
        private ObservableCollection<ParameterData> data;

        public ObservableCollection<ParameterData> Data
        {
            get
            {
                if (Element == null)
                {
                    Reference pickObject = UIdoc.Selection.PickObject(ObjectType.Element);
                    Element = Doc.GetElement(pickObject);
                    ElementType = Doc.GetElement(Element.GetTypeId());
                }

                if (data == null)
                {
                    data = new ObservableCollection<ParameterData>();
                    var bipNames = Enum.GetNames(typeof(BuiltInParameter));
                    foreach (string bipname in bipNames)
                    {
                        if (!Enum.TryParse(bipname, out BuiltInParameter bip))
                        {
                            continue;
                        }

                        Parameter pradata = Element.get_Parameter(bip);
                        if (pradata != null && IsInstance)
                        {
                            ParameterData parameterData = new ParameterData(pradata, Doc);
                            if (!data.Contains(parameterData))
                            {
                                data.Add(parameterData);
                            }
                        }
                        Parameter pradataType = ElementType.get_Parameter(bip);
                        if (pradataType != null && IsType)
                        {
                            ParameterData parameterData = new ParameterData(pradataType, Doc, false);
                            if (!data.Contains(parameterData))
                            {
                                data.Add(parameterData);
                            }
                        }
                    }
                    foreach (Parameter parameter in Element.Parameters)
                    {
                        var valueString = (StorageType.ElementId == parameter.StorageType) ? PraUtils.GetParameterValue2(parameter, Doc) : parameter.AsValueString();
                        var parameterData = new ParameterData(parameter,Doc,true);

                        if (!data.Contains(parameterData))
                        {
                            data.Add(parameterData);
                        }
                    }
                    //Don't use because include parameter type and instance
                    //ObservableCollection<ParameterData> list = data.GroupBy(x => x.BuiltInParameter).Select(x => x.First()).ToObservableCollection();
                    //data = list;
                    //Sort
                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(data);
                    view.SortDescriptions.Add(new SortDescription("TypeOrInstance", ListSortDirection.Ascending));
                    view.SortDescriptions.Add(new SortDescription("ParameterName", ListSortDirection.Ascending));


                }
                return data;
            }
            set { OnPropertyChanged(ref data, value); }
        }

        private ICollectionView itemsView;
        public ICollectionView ItemsView
        {
            get
            {
                if (itemsView == null)
                {
                    itemsView = CollectionViewSource.GetDefaultView(Data);
                    itemsView.Filter = filterSearchText;
                }
                return itemsView;
            }
            set
            {
                OnPropertyChanged(ref itemsView, value);
            }
        }

        public Autodesk.Revit.DB.Element Element { get; set; }
        public Autodesk.Revit.DB.Element ElementType { get; set; }

        private bool isInstance = true;
        public bool IsInstance
        {
            get
            {
                return isInstance;
            }
            set
            {
                OnPropertyChanged(ref isInstance, value);
                FreshParameter();
            }
        }

        private bool isType;
        public bool IsType
        {
            get => isType;
            set
            {
                OnPropertyChanged(ref isType, value);
                FreshParameter();
            }
        }

        private bool filterSearchText(object item)
        {
            ParameterData paradata = (ParameterData)item;
            if (SearchText != null || SearchText != "")
            {
                return paradata.ParameterName.ToUpper().Contains(SearchText.ToUpper());

            }
            else { return true; }
        }
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                OnPropertyChanged(ref _searchText, value);
                ItemsView.Refresh();
            }
        }

        public ICommand CopyBuiltInParameter { get => new RelayCommand(Copy_BuiltInParameter); }
        public ICommand CopyParameterName { get => new RelayCommand(Copy_ParameterName); }
        public ICommand CopyType { get => new RelayCommand(Copy_Type); }
        public ICommand CopyValue { get => new RelayCommand(Copy_Value); }
        public ICommand CopyParameterGroup { get => new RelayCommand(Copy_ParameterGroup); }
        public ICommand CopyGroupName { get => new RelayCommand(Copy_GroupName); }
        public ICommand CopyGUID { get => new RelayCommand(Copy_Guid); }

        public ICommand OpenExcel
        {
            get => new RelayCommand(ExportData);
        }
        public ICommand PickElement
        {
            get => new RelayCommand(SelectElementEvent);
        }
        public BipCheckerViewmodel(UIDocument uidoc)
        {
            this.UIdoc = uidoc;
            this.Doc = uidoc.Document;
            GetSelectedEle();
        }

        #region CopyAction

        ParameterData GetSelectedItem()
        {
            int selected = frmmain.lsBipChecker.SelectedIndex;
            return frmmain.lsBipChecker.Items[selected] as ParameterData;
        }
        private void Copy_BuiltInParameter()
        {
            ParameterData parameterData = GetSelectedItem();
            Clipboard.SetText(parameterData.BuiltInParameter);
        }
        private void Copy_ParameterName()
        {
            ParameterData parameterData = GetSelectedItem();
            Clipboard.SetText(parameterData.ParameterName);
        }
        private void Copy_Type()
        {
            ParameterData parameterData = GetSelectedItem();
            Clipboard.SetText(parameterData.Type);
        }
        private void Copy_Value()
        {
            ParameterData parameterData = GetSelectedItem();
            Clipboard.SetText(parameterData.Value);
        }
        private void Copy_ParameterGroup()
        {
            ParameterData parameterData = GetSelectedItem();
            Clipboard.SetText(parameterData.ParameterGroup);
        }
        private void Copy_GroupName()
        {
            ParameterData parameterData = GetSelectedItem();
            Clipboard.SetText(parameterData.GroupName);
        }
        private void Copy_Guid()
        {
            ParameterData parameterData = GetSelectedItem();
            Clipboard.SetText(parameterData.GUID);
        }

        #endregion

        void GetSelectedEle()
        {
            ICollection<ElementId> elementIds = UIdoc.Selection.GetElementIds();
            if (elementIds.Count == 1)
            {
                Element = Doc.GetElement(elementIds.First());
            }
        }

        void ExportData()
        {
            try
            {
                DataTable dataTable = Data.ToDataTable2();
                dataTable.Columns.RemoveAt(0);
                dataTable.OpenExcel(out string path);
                Process.Start(path);
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show("Please Close File Data Exported");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        void SelectElementEvent()
        {
            revitEvent.Run(FreshElement, true, Doc, null, false);
        }

        void FreshElement()
        {
            frmmain.Hide();
            Element = null;
            data = null;
            itemsView = null;
            OnPropertyChanged(nameof(ItemsView));
            OnPropertyChanged(nameof(Data));
            frmmain.Show();
        }

        void FreshParameter()
        {
            data = null;
            itemsView = null;
            OnPropertyChanged(nameof(ItemsView));
            OnPropertyChanged(nameof(Data));
        }
    }
}
