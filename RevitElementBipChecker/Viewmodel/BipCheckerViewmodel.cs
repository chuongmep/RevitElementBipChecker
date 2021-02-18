using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitElementBipChecker.Model;
using RevitElementBipChecker.View;

namespace RevitElementBipChecker.Viewmodel
{
    public class BipCheckerViewmodel : ViewmodeBase
    {
        public UIDocument UIdoc;
        public Document Doc;
        public MainWindows frmmain;
        public ObservableCollection<ParameterData> Data { get; set; }

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
                OnPropertyChanged(ref itemsView,value);
            }
        }

        public Autodesk.Revit.DB.Element Element { get; set; }
        private bool filterSearchText(object item)
        {
            ParameterData paradata = (ParameterData)item;
            if (SearchText != null || SearchText != "")
            {
                return paradata.ParameterName.ToUpper().Contains(SearchText.ToUpper());
                //|| paradata.Type.ToUpper().Contains(SearchText.ToUpper())
                //|| paradata.ReadWrite.ToUpper().Contains(SearchText.ToUpper())
                //|| paradata.Value.ToUpper().Contains(SearchText.ToUpper())
                //|| paradata.StringValue.ToUpper().Contains(SearchText.ToUpper())
                //|| paradata.GroupName.ToUpper().Contains(SearchText.ToUpper())
                //|| paradata.Shared.ToUpper().Contains(SearchText.ToUpper())
                //|| paradata.ParameterGroup.ToUpper().Contains(SearchText.ToUpper());

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
        public BipCheckerViewmodel(UIDocument uidoc)
        {
            this.UIdoc = uidoc;
            this.Doc = uidoc.Document;
            GetListData();
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

        void GetListData()
        {
            Data = new ObservableCollection<ParameterData>();
            Reference pickObject = UIdoc.Selection.PickObject(ObjectType.Element);
            Element = Doc.GetElement(pickObject);
            var bipNames = Enum.GetNames(typeof(BuiltInParameter));
            foreach (string bipname in bipNames)
            {
                if (!Enum.TryParse(bipname, out BuiltInParameter bip))
                {
                    continue;
                }
                Parameter pradata = Element.get_Parameter(bip);
                if (pradata != null)
                {
                    ParameterData parameterData = new ParameterData(pradata, Doc);
                    if (!Data.Contains(parameterData))
                    {
                        Data.Add(parameterData);
                    }
                }
            }
            ObservableCollection<ParameterData> list = Data.GroupBy(x => x.BuiltInParameter).Select(x => x.First()).ToObservableCollection();
            Data = list;
            //Sort
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Data);
            view.SortDescriptions.Add(new SortDescription("ParameterName", ListSortDirection.Ascending));
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
        
    }
}
