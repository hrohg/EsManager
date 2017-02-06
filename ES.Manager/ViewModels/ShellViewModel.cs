using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Es.Data.Models;
using ES.Common.Helper;
using ES.Business.Managers;
using ES.DataAccess.Models;
using ES.Manager.Enumerations;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace ES.Manager.ViewModels
{
    public class ShellViewModel
    {
        #region Contructors

        public ShellViewModel()
        {
            Initialize();
        }
        #endregion

        #region Internal methods

        private void Initialize()
        {
            ImportSharedProductsToXmlCommand = new RelayCommand(OnImportSharedProductsToXml);
            ExportSharedProductsToXmlCommand = new RelayCommand<ExportProductEnum>(OnExportSharedProductsToXml);
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
            var data = XmlHelper.Read(filePath.FileName, typeof (List<ESSharedProducts>));
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

        #endregion

        #region External methods

        #endregion

        #region Commands

        public ICommand ImportSharedProductsToXmlCommand { get; private set; }
        public ICommand ExportSharedProductsToXmlCommand { get; private set; }

        #endregion
    }
}
