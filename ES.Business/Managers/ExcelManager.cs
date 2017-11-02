using System;
using System.Collections.Generic;
using Es.Data.Models;
using ES.Business.Helpers;

namespace ES.Business.Managers
{
    public class ExcelManager
    {
        public static List<EsCategoriesModel> ImportEsCategories(string filePath)
        {
            var xlApp = new ExcelDataContent(filePath);
            var xlWSh = xlApp.GetWorksheet();
            if (xlWSh == null) return null;
            xlWSh.Activate();
            var items = new List<EsCategoriesModel>();
            var nextRow = 2;
            try
            {
                //Code, Name, Description  
                while (!string.IsNullOrEmpty(xlWSh.Cells[nextRow, 1].Text))
                {
                    string hcDcs = xlWSh.Cells[nextRow, 1].Value.ToString();
                    string name = xlWSh.Cells[nextRow, 2].Value.ToString();
                    string description = xlWSh.Cells[nextRow, 3].Value.ToString();
                    var item = new EsCategoriesModel
                    {
                        HcDcs = hcDcs.Trim(),
                        Name = name.Trim(),
                        Description = description.Trim(),
                        IsActive = true
                    };
                    items.Add(item);
                    nextRow++;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                xlApp.Dispose();
            }
            return items;
        }
    }
}
