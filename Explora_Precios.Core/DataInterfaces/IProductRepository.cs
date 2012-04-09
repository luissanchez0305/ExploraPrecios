using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Core;
using SharpArch.Core.PersistenceSupport;

namespace Explora_Precios.Core.DataInterfaces
{
    public enum Precision { High, Medium, Low }
    public enum IsActivated { NoMatter, Yes, No }
	public class EdgePrices { public float Min { get; set; } public float Max { get; set; } }
    public interface IProductRepository : IRepository<Product>
    {
        Product GetbyReference(string Reference);
        IList<Product> GetbyReference(string Reference, Precision Precision);
        List<Product> GetbyDepartment(int DepartmentId);
        List<Product> GetbyCategory(int CategoryId);
        List<Product> GetbySubCategory(int SubCategoryId);
        List<Product> GetbyProductType(int ProductTypeId); 
        IList<Product> GetbySearchText(string Text);
        IList<Product> GetbySearchText(string Text, IsActivated IsActivated);
	    EdgePrices GetbyDepartmentEdgePrices(int departmentId);
		EdgePrices GetbyCategoryEdgePrices(int categoryId);
		EdgePrices GetbySubCategoryEdgePrices(int subCategoryId);
		EdgePrices GetbyProductTypeEdgePrices(int productTypeId);
    }
}
