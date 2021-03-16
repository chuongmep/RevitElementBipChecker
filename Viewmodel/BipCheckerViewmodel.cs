#region NameSpace
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using OOG.Core.RevitData;
using RevitElementBipChecker.Model;
using RevitElementBipChecker.View;
#endregion

namespace RevitElementBipChecker.Viewmodel
{
    public class BipCheckerViewmodel : ViewmodeBase
    {
        public UIDocument UIdoc;
        public Document Doc;
        public MainWindows frmmain;
        private RevitEvent revitEvent = new RevitEvent();
        public const string DefaultValue = "<null>";
        private ObservableCollection<ParameterData> data;

        public ObservableCollection<ParameterData> Data
        {
            get
            {
                if (Element == null)
                {
                    try
                    {
                        Reference pickObject = UIdoc.Selection.PickObject(ObjectType.Element);
                        Element = Doc.GetElement(pickObject);
                        Doc = Element.Document;
                        ElementId = Element.Id.ToString();
                        Name = Element.Name;
                        CategoryName = Element.Category.Name;
                        if (Element.CanHaveTypeAssigned())
                        {
                            ElementType = Doc.GetElement(Element.GetTypeId());
                        }
                        this.State = "Element Current";
                    }
                    catch (Autodesk.Revit.Exceptions.OperationCanceledException) { }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                if (data == null)
                {
                    data = new ObservableCollection<ParameterData>();
                    if (IsInstance)
                    {
                        foreach (Parameter parameter in Element.Parameters)
                        {

                            var parameterData = new ParameterData(parameter, Element.Document);
                            data.Add(parameterData);
                        }
                    }

                    if (IsType && ElementType != null)
                    {
                        foreach (Parameter parameter in ElementType.Parameters)
                        {
                            var parameterData = new ParameterData(parameter, Element.Document, false);
                            data.Add(parameterData);
                        }
                    }

                    ObservableCollection<ParameterData> list = data.GroupBy(x => x.Parameter.Id).Select(x => x.First()).ToObservableCollection();
                    data = list;
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
            set => OnPropertyChanged(ref itemsView, value);
        }

        public Autodesk.Revit.DB.Element Element { get; set; }
        public Autodesk.Revit.DB.Element ElementType { get; set; }

        private bool isInstance = true;
        public bool IsInstance
        {
            get => isInstance;
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

        private string state;
        public string State
        {
            get => state;
            set => OnPropertyChanged(ref state, value);
        }

        private string elementId = DefaultValue;
        public string ElementId
        {
            get => elementId;
            set => OnPropertyChanged(ref elementId, value);
        }

        private string name = DefaultValue;
        public string Name
        {
            get => name;
            set => OnPropertyChanged(ref name, value);
        }

        private string categoryName = DefaultValue;
        public string CategoryName
        {
            get => categoryName;
            set => OnPropertyChanged(ref categoryName, value);
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

        public ICommand PickLinkElement { get => new RelayCommand(PickLink_Element_Event); }

        public ICommand OpenExcel
        {
            get => new RelayCommand(ExportData);
        }
        public ICommand OpenJson
        {
            get => new RelayCommand(ExportJson);
        }
        public ICommand PickElement
        {
            get => new RelayCommand(SelectElementEvent);
        }
        public BipCheckerViewmodel(UIDocument uidoc)
        {
            this.UIdoc = uidoc;
            this.Doc = uidoc.Document;
            Selected();
            PickFirst();
        }

        void Selected()
        {
            try
            {
                Element = UIdoc.GetSelection().First();
                if (Element != null)
                {
                    if (Element.CanHaveTypeAssigned())
                    {
                        ElementType = Doc.GetElement(Element.GetTypeId());
                    }
                    Name = Element.Name;
                    CategoryName = Element.Category.Name;
                    ElementId = Element.Id.ToString();
                }
            }
            catch (System.InvalidOperationException) { }
        }
        void PickFirst()
        {
            if (Element == null)
            {
                bool isintance = DialogUtils.QuestionMsg("Select Option Snoop Element");
                if (!isintance)
                {
                    PickLink_Element();
                }
            }
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
                MessageBox.Show(e.ToString());
            }
        }

        void ExportJson()
        {
            DataTable dataTable = Data.ToDataTable2();
            dataTable.Columns.RemoveAt(0);
            dataTable.WriteJson(out string path);
            Process.Start("explorer.exe",path);
        }

        void PickLink_Element_Event()
        {
            revitEvent.Run(PickLink_Element, true, null, null, false);
        }
        private void PickLink_Element()
        {
            frmmain?.Hide();
            try
            {
                Reference pickObject =
                    UIdoc.Selection.PickObject(ObjectType.LinkedElement, "Select Element In Linked ");
                Element e = Doc.GetElement(pickObject);
                if (e is RevitLinkInstance)
                {
                    RevitLinkInstance linkInstance = e as RevitLinkInstance;
                    Document linkDocument = linkInstance.GetLinkDocument();
                    Element = linkDocument.GetElement(pickObject.LinkedElementId);
                    ElementId = Element.Id.ToString();
                    Name = Element.Name;
                    CategoryName = Element.Category.Name;
                    ElementType = linkDocument.GetElement(Element.GetTypeId());
                    this.State = "Element In Linked";
                }

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException) { }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());

            }
            data = null;
            itemsView = null;
            OnPropertyChanged(nameof(Data));
            frmmain?.Show();
        }
        void SelectElementEvent()
        {
            revitEvent.Run(FreshElement, true, null, null, false);
        }

        void FreshElement()
        {
            frmmain.Hide();
            Element = null;
            data = null;
            itemsView = null;
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
