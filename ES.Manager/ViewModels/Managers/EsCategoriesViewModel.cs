using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using Es.Data.Models;
using ES.Business.Managers;
using ES.Common.Enumerations;
using ES.Common.ViewModels;

namespace ES.Manager.ViewModels.Managers
{
    public class EsCategoriesViewModel : DocumentViewModel
    {
            #region Internal properties
            #endregion Internal properties

            #region External properties

            #region Categories
            private List<EsCategoriesModel> _items;
            public List<EsCategoriesModel> Items
            {
                get
                {
                    return string.IsNullOrEmpty(Filter) ? _items :
                    GetEsCategoriesList(_items, Filter);
                }
                set
                {
                    _items = value;
                    if (_items != null)
                    {
                        //CategoriesGeneration = new ObservableCollection<CategoriesGeneration>(GenerateCategoriesTree(_categories.Where(s => s.ParentId == null).ToList()));
                        //RaisePropertyChanged("CategoriesGeneration");
                    }
                    RaisePropertyChanged("Items");
                    ParentCategories = value;
                }
            }
            #endregion Categories

            private List<EsCategoriesModel> _parentCategories;
            public List<EsCategoriesModel> ParentCategories
            {
                get { return _parentCategories; }
                set
                {
                    _parentCategories = value;
                    RaisePropertyChanged("ParentCategories");
                }

            }
            private EsCategoriesModel _category;

            public EsCategoriesModel Category
            {
                get
                {
                    return _category;
                }
                set
                {
                    if (value == _category) return;
                    _category = value;
                    RaisePropertyChanged("Category");
                    //RaisePropertyChanged("ParentCategories");
                }
            }

            private EsCategoriesModel _selectedItem;

            public EsCategoriesModel SelectedItem
            {
                get
                {
                    return _selectedItem;
                }
                set
                {
                    if (value == _selectedItem) return;
                    _selectedItem = value;
                    RaisePropertyChanged("SelectedItem");
                }
            }
            public string ActivateHeader { get { return !Category.IsActive ? "Activate" : "Deactivate"; } }

            #region Filter

            private Timer _timer;
            private string _filter;
            public string Filter
            {
                get { return _filter ?? string.Empty; }
                set
                {
                    _filter = value.ToLower();
                    RaisePropertyChanged("Filter");
                    DisposeTimer();
                    _timer = new Timer(TimerElapsed, null, 300, 300);
                }
            }
            private void TimerElapsed(object obj)
            {
                RaisePropertyChanged("Items");
                DisposeTimer();
            }
            private void DisposeTimer()
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
            }
            #endregion Filter

            #region Is setting parent

            private bool _isSettingParent;
            public bool IsSettingParent
            {
                get { return _isSettingParent; }
                set
                {
                    if (value == _isSettingParent) return;
                    _isSettingParent = value;
                    RaisePropertyChanged("IsSettingParent");
                }
            }
            #endregion Is setting parent

            #endregion External properties

            #region Constructors

            public EsCategoriesViewModel()
            {
                Initialize();
            }
            #endregion Constructors

            #region internal methods
            private void Initialize()
            {
                Title = "Manage ES categories";

                NewCommand = new RelayCommand<EsCategoriesModel>(OnNew, CanNew);
                EditCommand = new RelayCommand<EsCategoriesModel>(OnEdit, CanEdit);
                RemoveCommand = new RelayCommand<EsCategoriesModel>(OnRemove, CanRemove);
                ActivateCommand = new RelayCommand<EsCategoriesModel>(OnActivate, CanActivate);
                ImportExportCommand = new RelayCommand<ImportExportEnum?>(OnImport, CanImport);

                Items = new List<EsCategoriesModel>();
                OnRefresh(null);
            }

            private bool CanRemove(EsCategoriesModel obj)
            {
                return obj != null;
            }

            private void OnRemove(EsCategoriesModel obj)
            {
                ProductsManager.RemoveEsCategory(obj);
            }

            private bool CanImport(ImportExportEnum? importExport)
            {
                switch (importExport)
                {
                    case ImportExportEnum.ToXml:
                        return false;
                    case ImportExportEnum.ToExcel:
                        return false;
                    case ImportExportEnum.FromXml:
                        return false;
                    case ImportExportEnum.FromExcel:
                        return true;
                    case null:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("importExport", importExport, null);
                }
                return importExport != null;
            }

            private void OnImport(ImportExportEnum? importExport)
            {
                switch (importExport)
                {
                    case ImportExportEnum.ToXml:
                    case ImportExportEnum.ToExcel:
                        ProductsManager.ExpoertEsCategories(Items, importExport == ImportExportEnum.ToXml ? FileTypeEnum.Xml : FileTypeEnum.Xls);
                        break;
                    case ImportExportEnum.FromXml:
                    case ImportExportEnum.FromExcel:
                        ProductsManager.EditEsCategories(ProductsManager.ImpoertEsCategories(importExport == ImportExportEnum.FromXml ? FileTypeEnum.Xml : FileTypeEnum.Xls));
                        OnRefresh(null);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("importExport", importExport, null);
                }
            }

            private bool CanActivate(EsCategoriesModel categories)
            {
                return categories != null;
            }

            private void OnActivate(EsCategoriesModel categories)
            {
                if (!CanActivate(categories)) return;
                Category.IsActive = !Category.IsActive;
                OnEdit(categories);
            }

            private bool CanEdit(EsCategoriesModel categories)
            {
                return categories != null;
            }

            private void OnEdit(EsCategoriesModel categories)
            {
                ProductsManager.EditEsCategories(categories);
            }

            private bool CanNew(EsCategoriesModel categories)
            {
                return true;
            }

            private void OnNew(EsCategoriesModel categories)
            {
                Category = new EsCategoriesModel();
            }
            private List<EsCategoriesModel> GetEsCategoriesList(List<EsCategoriesModel> categories, string key = null)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    key = key.ToLower();
                }
                var list = new List<EsCategoriesModel>();
                foreach (var item in categories)
                {
                    if (string.IsNullOrEmpty(key) ||
                        (!string.IsNullOrEmpty(item.HcDcs) && item.HcDcs.ToLower().Contains(key) ||
                         (!string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Contains(key)) ||
                         (!string.IsNullOrEmpty(item.Description) && item.Description.ToLower().Contains(key))))
                    {
                        list.Add(item);
                    }
                    else
                    {
                        list.AddRange(GetEsCategoriesList(item.Children, key));
                    }
                }
                return list;
            }

            #endregion Internal methods

            #region External methods

            #endregion Extrnal methods

            #region Commands

            public ICommand NewCommand { get; private set; }
            public ICommand EditCommand { get; private set; }
            public ICommand RemoveCommand { get; private set; }
            public ICommand ActivateCommand { get; private set; }
            public ICommand ImportExportCommand { get; private set; }

            #region Refresh command

            private ICommand _refreshCommand;

            public ICommand RefreshCommand
            {
                get
                {
                    if (_refreshCommand == null) _refreshCommand = new RelayCommand(OnRefresh);
                    return _refreshCommand;
                }
            }
            private void OnRefresh(object obj)
            {
                Items = ProductsManager.GetEsCategories();
                Category = new EsCategoriesModel();
            }
            #endregion Refresh command

            #region Selection changed
            private ICommand _selectedItemChangedCommand;
            public ICommand SelectedItemChangedCommand
            {
                get
                {
                    if (_selectedItemChangedCommand == null)
                        _selectedItemChangedCommand = new RelayCommand(args => SelectedItemChanged(args));
                    return _selectedItemChangedCommand;
                }
            }

            private void SelectedItemChanged(object args)
            {
                SelectedItem = args as EsCategoriesModel;
                Category = SelectedItem;
            }
            #endregion Selection changed

            #region Parent selection changed
            private ICommand _parentItemSelectionChangedCommand;
            public ICommand ParentItemSelectionChangedCommand
            {
                get
                {
                    if (_parentItemSelectionChangedCommand == null)
                        _parentItemSelectionChangedCommand = new RelayCommand(ParentItemSelectionChanged);
                    return _parentItemSelectionChangedCommand;
                }
            }
            private void ParentItemSelectionChanged(object args)
            {
                IsSettingParent = false;
                var parentItem = args as EsCategoriesModel;
                if (parentItem != null && parentItem.Id == Category.Id) return;
                var lastparent = Category.Parent;
                if (lastparent != null)
                {
                    lastparent.Children.Remove(Category);
                }
                else
                {
                    Items.Remove(Category);
                }



                Category.Parent = parentItem;
                Category.ParentId = parentItem != null ? parentItem.Id : (Guid?)null;
                if (parentItem != null)
                {
                    parentItem.Children.Add(Category);
                }
                RaisePropertyChanged("Items");
                RaisePropertyChanged("Category");
            }
            #endregion Parent selection changed

            #region Set parent command
            private ICommand _setParentCommand;
            public ICommand SetParentCommand
            {
                get
                {
                    return _setParentCommand ?? (_setParentCommand = new RelayCommand<EsCategoriesModel>(OnSetParent, CanSetParent));
                }
            }
            private bool CanSetParent(EsCategoriesModel category)
            {
                return category != null;
            }
            private void OnSetParent(EsCategoriesModel category)
            {
                IsSettingParent = true;
            }
            #endregion Set parent command

            #region Remove parent command
            private ICommand _removeParentCommand;

            public ICommand RemoveParentCommand
            {
                get
                {
                    return _removeParentCommand ?? (_removeParentCommand = new RelayCommand<EsCategoriesModel>(OnRemoveParent, CanRemoveParent));
                }
            }

            private bool CanRemoveParent(EsCategoriesModel category)
            {
                return category != null && category.Parent != null;
            }

            private void OnRemoveParent(EsCategoriesModel category)
            {
                var parent = category.Parent;
                parent.Children.Remove(category);
                category.Parent = null;
                category.ParentId = null;
                RaisePropertyChanged("Category");
                RaisePropertyChanged("Items");

            }
            #endregion Remove parent command

            #endregion Commands
    }

    public class CategoriesGeneration
    {
        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }
        public List<CategoriesGeneration> Children { get; set; }
        private EsCategoriesModel _category;

        public EsCategoriesModel Category
        {
            get { return _category; }
            set
            {
                if (value == _category) return;
                _category = value;
            }
        }

        public string Name
        {
            get { return string.Format("{0} {1} {2}", Category.HcDcs, Category.Name, Category.ShortDescription); }
        }

        public string Description
        {
            get { return Category.Description; }
        }
    }
}
