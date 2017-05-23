using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Es.Data.Models;
using ES.Common.ViewModels;
using ES.Business.Managers;
using ES.DataAccess.Models;
using ES.Manager.Enumerations;
using ES.Manager.ViewModels.Managers;
using Xceed.Wpf.AvalonDock;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace ES.Manager.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        #region External properties
        #region Avalon dock
        public ObservableCollection<DocumentViewModel> Documents { get; set; }
        public ObservableCollection<PaneViewModel> Tools { get; set; }
        #endregion Avalon dock
        #endregion External properties

        #region Contructors

        public ShellViewModel()
        {
            Initialize();
            ApplicationManager.Instance.SetEsUser = new EsUserModel { UserId = 1 };
        }
        #endregion

        #region Internal methods

        private void Initialize()
        {
            Documents = new ObservableCollection<DocumentViewModel>();
            Tools = new ObservableCollection<PaneViewModel>();
            ImportSharedProductsToXmlCommand = new RelayCommand(OnImportSharedProductsToXml);
            ExportSharedProductsToXmlCommand = new RelayCommand<ExportProductEnum>(OnExportSharedProductsToXml);
            ManageEsCategoriesCommand = new RelayCommand(OnManageEsCategories);
        }

        private void AddDocument(DocumentViewModel vm)
        {
            if (vm.IsClosable)
            {
                vm.OnClosed += OnRemoveDocument;
            }
            vm.IsActive = true;
            vm.IsSelected = true;
            vm.Id = Guid.NewGuid();
            Documents.Add(vm);
        }
        private void OnRemoveDocument(PaneViewModel vm)
        {
            if (vm == null) return;
            vm.OnClosed -= OnRemoveDocument;
            Documents.Remove((DocumentViewModel)vm);
        }

        private void OnImportSharedProductsToXml(object o)
        {
            var products = ProductsManager.GetEsSharedProductses();
            if (products == null || products.Count == 0) return;
            var filePath = new OpenFileDialog();
            filePath.Filter = "Xml file (*.xml) | *.xml";

            if (filePath.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            var data = XmlHelper.Read(filePath.FileName, typeof(List<ESSharedProducts>));
        }
        private void OnExportSharedProductsToXml(ExportProductEnum exportProductEnum)
        {
            var products = ProductsManager.GetEsSharedProductses();
            if (products == null || products.Count == 0) return;
            var filePath = new SaveFileDialog();
            filePath.Filter = "Xml file (*.xml) | *.xml";

            if (filePath.ShowDialog() != true)
            {
                return;
            }
            switch (exportProductEnum)
            {
                case ExportProductEnum.ExportSharedProductsEnum:
                    XmlHelper.Save(products, filePath.FileName);
                    break;
                case ExportProductEnum.ExportSharedProductsToEsProductEnum:
                    var esProducts = new List<EsProductModel>();
                    foreach (var item in products)
                    {
                        var product = new EsProductModel()
                        {
                            Code = item.Code,
                            Barcode = item.Barcode,
                            HcdCs = item.HCDCS,
                            Description = item.Description,
                            Mu = item.Mu,
                            CostPrice = item.CostPrice,
                            Price = item.Price,
                            ExpiryDays = item.ExpiryDays,
                            IsWeight = item.IsWeight
                        };
                        esProducts.Add(product);
                        XmlHelper.Save(esProducts, filePath.FileName);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("exportProductEnum", exportProductEnum, null);
            }


        }

        private void OnManageEsCategories(object o)
        {
            AddDocument(new EsCategoriesViewModel());
        }
        #endregion

        #region External methods

        #endregion

        #region Commands

        public ICommand ImportSharedProductsToXmlCommand { get; private set; }
        public ICommand ExportSharedProductsToXmlCommand { get; private set; }
        public ICommand ManageEsCategoriesCommand { get; private set; }
        #endregion
    }
}
