using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Es.Data.Models;
using ES.Common.Enumerations;
using ES.Common.Helper;
using ES.DataAccess.Models;

namespace ES.Business.Managers
{
    public class ProductsManager : ManagerBase
    {
        #region ESSharedProducts
        public static List<ESSharedProducts> GetEsSharedProductses()
        {
            using (var db = GetDataContext())
            {
                try
                {
                    return db.ESSharedProducts.ToList();
                }
                catch (Exception)
                {
                    return new List<ESSharedProducts>();
                }

            }
        }
        #endregion ESSharedProducts

        #region EsCategories

        public static List<EsCategoriesModel> ImpoertEsCategories(FileTypeEnum fileType)
        {
            string filePath;
            var categories = new List<EsCategoriesModel>();
            switch (fileType)
            {
                case FileTypeEnum.Xml:
                    filePath = FileHelper.OpenFile("Open file for import", "Xml files |*.xml");
                    if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return null;

                    break;
                case FileTypeEnum.Xls:
                    filePath = FileHelper.OpenFile("Open file for import", "Excel files |*.xlsx");
                    if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return null;
                    categories = ExcelManager.ImportEsCategories(filePath);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("fileType", fileType, null);
            }
            return categories;
        }
        public static bool ExpoertEsCategories(List<EsCategoriesModel> categories, FileTypeEnum fileType)
        {
            string filePath;
            switch (fileType)
            {
                case FileTypeEnum.Xml:

                    break;
                case FileTypeEnum.Xls:

                    break;
                default:
                    throw new ArgumentOutOfRangeException("fileType", fileType, null);
            }
            return true;
        }
        public static bool EditEsCategories(List<EsCategoriesModel> categories)
        {
            if (categories == null) return false;
            return categories.All(EditEsCategories);
        }

        public static bool EditEsCategories(EsCategoriesModel category)
        {
            if (category == null) return false;
            category.LastModifierId = ApplicationManager.EsUser.UserId;
            category.LastModificationDate = DateTime.Now;
            return TryEditEsCategories(Convert(category));
        }

        private static EsCategories Convert(EsCategoriesModel category)
        {
            if (category == null) return null;
            return new EsCategories()
            {
                Id = category.Id,
                ParentId = category.ParentId,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                HCDCS = category.HcDcs,
                LastModificationDate = category.LastModificationDate,
                LastModifierId = category.LastModifierId
            };
        }
        public static List<EsCategoriesModel> GetEsCategories()
        {
            return Convert(TryGetEsCategories());
        }

        private static List<EsCategoriesModel> Convert(List<EsCategories> list)
        {
            if (list == null) return null;
            var categories = new List<EsCategoriesModel>();
            foreach (var item in list.Where(s=>s.ParentId==null).ToList())
            {
                var subCategory = Convert(item);
                Convert(subCategory, list.Where(s => s.ParentId != null).ToList());
                categories.Add(subCategory);
            }
            return categories;
        }

        private static void Convert(EsCategoriesModel parent, List<EsCategories> list)
        {
            foreach (var item in list.Where(s => s.ParentId == parent.Id).ToList())
            {
                var subCategory = Convert(item);
                Convert(subCategory, list.Where(s => s.ParentId != parent.Id).ToList());
                subCategory.ParentId = parent.Id;
                subCategory.Parent = parent;
                parent.Children.Add(subCategory);
            }
        }
        private static EsCategoriesModel Convert(EsCategories item)
        {
            if (item == null) return null;
            return new EsCategoriesModel
            {
                Id = item.Id,
                ParentId = item.ParentId,
                Name = item.Name,
                Description = item.Description,
                IsActive = item.IsActive,
                HcDcs = item.HCDCS,
                LastModificationDate = item.LastModificationDate,
                LastModifierId = item.LastModifierId
            };
        } 
        private static bool TryEditEsCategories(EsCategories item)
        {
            if (item == null) return false;
            item.Name = !string.IsNullOrEmpty(item.Name)?item.Name.Trim():string.Empty;
            item.Description = !string.IsNullOrEmpty(item.Description)? item.Description.Trim():string.Empty;
            item.LastModificationDate = DateTime.Now;
            item.LastModifierId = ApplicationManager.EsUser.UserId;
            using (var db = GetDataContext())
            {
                try
                {
                    var exCategory = db.EsCategories.SingleOrDefault(s => s.Id == item.Id || s.Name.ToLower()==item.Name.ToLower());
                    item.Name = string.IsNullOrEmpty(item.Name)? string.Empty: item.Name.Length < 150 ? item.Name.Trim() : item.Name.Substring(0, 150).Trim();
                    if (exCategory == null)
                    {
                        db.EsCategories.Add(item);
                    }
                    else
                    {
                        exCategory.ParentId = item.ParentId;
                        exCategory.Name = item.Name;
                        exCategory.Description = item.Description;
                        exCategory.IsActive = item.IsActive;
                        exCategory.HCDCS = item.HCDCS;
                        exCategory.LastModificationDate = item.LastModificationDate;
                        exCategory.LastModifierId = item.LastModifierId;
                    }
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }
        public static bool RemoveEsCategory(EsCategoriesModel esCategories)
        {
            return false;
        }
        private static List<EsCategories> TryGetEsCategories()
        {
            using (var db = GetDataContext())
            {
                try
                {
                    return db.EsCategories.Include("EsCategories1").Include("EsCategories2").OrderBy(s=>s.HCDCS).ThenBy(s=>s.Name).ToList();
                }
                catch (Exception)
                {
                    return new List<EsCategories>();
                }
            }
        }
        #endregion EsCategories
    }
}
