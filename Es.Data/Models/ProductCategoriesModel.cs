using System;

namespace Es.Data.Models
{
    public class ProductCategoriesModel
    {
        #region External properties
        public Guid Id { get; set; }
        public int CategoriesId { get; set; }
        public Guid ProductId { get; set; }
        #endregion
    }
}
