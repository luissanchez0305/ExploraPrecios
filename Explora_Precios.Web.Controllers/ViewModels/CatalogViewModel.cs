using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explora_Precios.Web.Controllers.ViewModels
{
	public class CatalogViewModel
	{
		public CatalogViewModel()
		{
			departments = new List<DepartmentViewModel>();
			categories = new List<CategoryViewModel>();
			subCategories = new List<SubCategoryViewModel>();
			productTypes = new List<ProductTypeViewModel>();
		}

		public IList<DepartmentViewModel> departments;
		public IList<CategoryViewModel> categories;
		public IList<SubCategoryViewModel> subCategories;
		public IList<ProductTypeViewModel> productTypes;
		public int departmentId { get; set; }
		public int categoryId { get; set; }
		public int subCategoryId { get; set; }
		public int productTypeId { get; set; }
		public string departmentName { get; set; }
		public string categoryName { get; set; }
		public string subCategoryName { get; set; }
		public string productTypeName { get; set; }  
	}

	public class FilterViewModel
	{
		public FilterViewModel(int CatalogLevel, int CatalogId)
		{
			FilterType = FilterViewModel.ItemFilterTypes.None;
			FilterBrands = new List<FilterItemViewModel>();
			FilterPrices = new List<FilterItemViewModel>();
		}

		public FilterViewModel()
		{
			FilterType = FilterViewModel.ItemFilterTypes.None;
			FilterBrands = new List<FilterItemViewModel>();
			FilterPrices = new List<FilterItemViewModel>();
		}

		public enum ItemFilterTypes { None, Sale, Price, Brand, SaleBrand, PriceBrand }
		public IEnumerable<FilterItemViewModel> FilterPrices { get; set; }
		public IEnumerable<FilterItemViewModel> FilterBrands { get; set; }
		public ItemFilterTypes FilterType { get; set; }
		public int CurrentCatalogLevel { get; set; }
		public int CurrentCatalogId { get; set; }
		public float CurrentMinPrice { get; set; }
		public float CurrentMaxPrice { get; set; }
		public string CurrentBrand { get; set; }
		public FilterItemViewModel UndoPriceFilter { get; set; }
		public FilterItemViewModel UndoSaleFilter { get; set; }
		public FilterItemViewModel UndoBrandFilter { get; set; }
	}

	public class FilterItemViewModel
	{
		public string Url { get; set; }
		public string Name { get; set; }
	}

	public class CatalogItemViewModel
	{
		public int dbId { get; set; }
		public bool selected { get; set; }
		public int levelId { get; set; }
		public int catalogId { get; set; }
		public int clientId { get; set; }
		public string clientAddress { get; set; }
		public string type { get; set; }
		public string url { get; set; }
		public string name { get; set; }
		public CatalogViewModel catalogProduct { get; set; }
		public List<ProductViewModel> products { get; set; } 
	}
}
