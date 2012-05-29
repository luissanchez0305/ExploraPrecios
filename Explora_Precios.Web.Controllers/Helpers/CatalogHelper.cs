using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Core;
using SharpArch.Data.NHibernate;
using Explora_Precios.Web.Controllers.ViewModels;
using Explora_Precios.Core.Helper;
using Explora_Precios.Core.DataInterfaces;
using Explora_Precios.Data;

namespace Explora_Precios.Web.Controllers.Helpers
{
	public static class CatalogHelper
	{
		public static CatalogViewModel FromLevelsToCatalog(this Department Department)
		{
			return FromLevelsToCatalog(0, Department.Id);
		}

		public static CatalogViewModel FromLevelsToCatalog(this Category Category)
		{
			return FromLevelsToCatalog(1, Category.Id);
		}

		public static CatalogViewModel FromLevelsToCatalog(this SubCategory SubCategory)
		{
			return FromLevelsToCatalog(2, SubCategory.Id);
		}

		public static CatalogViewModel FromLevelsToCatalog(this ProductType ProductType)
		{
			return FromLevelsToCatalog(3, ProductType.Id);
		}

		public static CatalogViewModel FromLevelsToCatalog(int catalogLevel, int catalogLevelId)
		{
			IDepartmentRepository _dr = new DepartmentRepository();
			ICategoryRepository _cr = new CategoryRepository();
			ISubCategoryRepository _scr = new SubCategoryRepository();
			IProductTypeRepository _ptr = new ProductTypeRepository();

			var response = new CatalogViewModel();
			catalogLevelId = catalogLevelId == 0 ? 1 : catalogLevelId;
			if (catalogLevel == 0)
			{
				var department = _dr.Get(catalogLevelId);
				response.departmentId = catalogLevelId;	
				response.departmentName = department.name;
				response.categories = department.categories.Select(x => new CategoryViewModel()
				{
					categoryId = x.Id,
					categoryTitle = x.name
				}).ToList();
			}
			else if (catalogLevel == 1)
			{
				var category = _cr.Get(catalogLevelId);
				response.categoryName = category.name;
				response.categoryId = catalogLevelId;
				response.departmentId = category.department.Id;
				response.departmentName = category.department.name;

				response.categories = _cr.GetByDepartament(category.department).Select(x => new CategoryViewModel()
				{
					categoryId = x.Id,
					categoryTitle = x.name
				}).ToList();
				response.subCategories = category.subCategories.Select(x => new SubCategoryViewModel()
				{
					subCategoryId = x.Id,
					subCategoryTitle = x.name
				}).ToList();
			}
			else if (catalogLevel == 2)
			{
				var subcategory = _scr.Get(catalogLevelId);
				response.subCategoryName = subcategory.name;
				response.subCategoryId = subcategory.Id;
				response.categoryId = subcategory.category.Id;
				response.categoryName = subcategory.category.name;
				response.departmentName = subcategory.category.department.name;
				response.departmentId = subcategory.category.department.Id;

				response.categories = _cr.GetByDepartament(subcategory.category.department).Select(x => new CategoryViewModel()
				{
					categoryId = x.Id,
					categoryTitle = x.name
				}).ToList();
				response.subCategories = _scr.GetByCategory(subcategory.category).Select(x => new SubCategoryViewModel()
				{
					subCategoryId = x.Id,
					subCategoryTitle = x.name
				}).ToList();
				response.productTypes = subcategory.productTypes.Select(x => new ProductTypeViewModel()
				{
					productTypeId = x.Id,
					productTypeTitle = x.name
				}).ToList();
			}
			else if (catalogLevel == 3)
			{
				var producttype = _ptr.Get(catalogLevelId);
				response.productTypeId = catalogLevelId;
				response.productTypeName = producttype.name;
				response.subCategoryName = producttype.subCategory.name;
				response.subCategoryId = producttype.subCategory.Id;
				response.categoryId = producttype.subCategory.category.Id;
				response.categoryName = producttype.subCategory.category.name;
				response.departmentName = producttype.subCategory.category.department.name;
				response.departmentId = producttype.subCategory.category.department.Id;

				response.categories = _cr.GetByDepartament(producttype.subCategory.category.department).Select(x => new CategoryViewModel() {
					categoryId = x.Id,
					categoryTitle = x.name
				}).ToList();
				response.subCategories = _scr.GetByCategory(producttype.subCategory.category).Select(x => new SubCategoryViewModel() {
					subCategoryId = x.Id,
					subCategoryTitle = x.name
				}).ToList();
				response.productTypes = _ptr.GetBySubCategory(producttype.subCategory).Select(x => new ProductTypeViewModel() {

					productTypeId = x.Id,
					productTypeTitle = x.name
				}).ToList();

			}
			response.departments = _dr.GetAll().Select(x => new DepartmentViewModel()
			{
				departmentId = x.Id,
				departmentTitle = x.name
			}).ToList();

			return response;
		}

		/// <summary>
		/// It gets the list of catalog anchors to display
		/// </summary>
		/// <param name="linkLastItem">True if you want to show the last item of the catalog as anchor</param>
		/// <param name="catalog">catalog model</param>
		/// <param name="showAnchor">If true, the return string have anchor in every item</param>
		/// <returns>html string with anchors</returns>
		public static string CatalogToString(bool linkLastItem, CatalogViewModel catalog, bool showAnchor)
		{
			var name = "";
			var catalogId = 0;
			var list = new List<string>();
			var lastLinked = false;
			for (int level_Id = 0; level_Id <= 3 && !lastLinked; level_Id++)
			{
				switch (level_Id)
				{
					case 0:
						name = catalog.departmentName;
						catalogId = catalog.departmentId;
						lastLinked = string.IsNullOrEmpty(catalog.categoryName);
						break;
					case 1:
						name = catalog.categoryName;
						catalogId = catalog.categoryId;
						lastLinked = string.IsNullOrEmpty(catalog.subCategoryName);
						break;
					case 2:
						name = catalog.subCategoryName;
						catalogId = catalog.subCategoryId;
						lastLinked = string.IsNullOrEmpty(catalog.productTypeName);
						break;
					case 3:
						name = catalog.productTypeName;
						catalogId = catalog.productTypeId;
						break;
				}
				if ((!lastLinked || linkLastItem) && showAnchor)
					list.Add("<a href=\"/Home/Products?catlev=" + level_Id.ToString() + "&id=" + catalogId.ToString() + "\">" + name + "</a>");
				else
					list.Add(name);
			}
			var result = "";
			for (int i = 0; i < list.Count; i++)
			{
				result += list[i] + (i < list.Count - 1 ? " - " : "");
			}
			return result;
		}

		public static void LoadFilters(this HomeViewModel HomeModel, int CatalogLevel = 0, int CatalogId = 1, string search = "")
		{
			var searchFilter = search.Length > 1 ? "&s=" + search : "";
			// crear parametro de listado de productos
			var FilterDefaultParameters = "&o=false" + (search.Length > 1 ? searchFilter : "&cl=" + CatalogLevel + "&ci=" + CatalogId);
			var ProductsDefaultParameters = search.Length > 1 ? searchFilter : "catLev=" + CatalogLevel + "&id=" + CatalogId;
			// obtener la lista de marcas
			var brandsList = HomeModel.allProducts.Select(x => x.brand.name).OrderBy(y => y).Distinct().ToList();
			if (HomeModel.allProducts.Count > 1)
			{
				HomeModel.Filter.HasFilterPrices = true;
			}

			// crear lista de filtros por marcas
			if (brandsList.Count > 1)
			{
				var priceFilter = "";
				if (HomeModel.Filter.FilterType == FilterViewModel.ItemFilterTypes.PriceBrand || HomeModel.Filter.FilterType == FilterViewModel.ItemFilterTypes.Price)
					priceFilter = "&p=" + HomeModel.Filter.CurrentMinPrice + "," + HomeModel.Filter.CurrentMaxPrice;
				var saleFilter = "";
				if (HomeModel.Filter.FilterType == FilterViewModel.ItemFilterTypes.SaleBrand || HomeModel.Filter.FilterType == FilterViewModel.ItemFilterTypes.Sale)
					saleFilter = FilterDefaultParameters.Replace("o=false", "o=true");
				else
					saleFilter = FilterDefaultParameters;
				HomeModel.Filter.FilterBrands = brandsList.Select(brand => new FilterItemViewModel()
					{
						Name = brand,
						Url = "/Home/Filter?b=" + brand + priceFilter + saleFilter
					});
			}

			// crear "deshacer" filtros
			// Price
			var ReturnTo = search.Length > 0 ? "Search" : "Products";
			if (HomeModel.Filter.FilterType == FilterViewModel.ItemFilterTypes.Price || HomeModel.Filter.FilterType == FilterViewModel.ItemFilterTypes.PriceBrand)
			{
				HomeModel.Filter.UndoPriceFilter = new FilterItemViewModel
				{
					Name = ((float)(HomeModel.Filter.CurrentMinPrice + 0.01)).Money() + " - " + HomeModel.Filter.CurrentMaxPrice.Money(),
					Url = HomeModel.Filter.FilterType == FilterViewModel.ItemFilterTypes.PriceBrand ?
						"/Home/Filter?b=" + HomeModel.Filter.CurrentBrand + FilterDefaultParameters :
						"/Home/" + ReturnTo + "?" + ProductsDefaultParameters
				};

				if (HomeModel.Filter.FilterType == FilterViewModel.ItemFilterTypes.PriceBrand)
					HomeModel.Filter.UndoBrandFilter = new FilterItemViewModel
					{
						Name = HomeModel.Filter.CurrentBrand,
						Url = HomeModel.Filter.FilterType == FilterViewModel.ItemFilterTypes.PriceBrand ?
							"/Home/Filter?p=" + HomeModel.Filter.CurrentMinPrice + "," + (int)HomeModel.Filter.CurrentMaxPrice + FilterDefaultParameters :
							"/Home/" + ReturnTo + "?" + ProductsDefaultParameters
					};
			}

			// Sale
			if (HomeModel.Filter.FilterType == FilterViewModel.ItemFilterTypes.Sale || HomeModel.Filter.FilterType == FilterViewModel.ItemFilterTypes.SaleBrand)
			{
				HomeModel.Filter.UndoSaleFilter = new FilterItemViewModel
				{
					Name = "Solo Ofertas",
					Url = HomeModel.Filter.FilterType == FilterViewModel.ItemFilterTypes.SaleBrand ?
						"/Home/Filter?b=" + HomeModel.Filter.CurrentBrand + FilterDefaultParameters :
						"/Home/" + ReturnTo + "?" + ProductsDefaultParameters
				};

				if (HomeModel.Filter.FilterType == FilterViewModel.ItemFilterTypes.SaleBrand)
					HomeModel.Filter.UndoBrandFilter = new FilterItemViewModel
					{
						Name = HomeModel.Filter.CurrentBrand,
						Url = HomeModel.Filter.FilterType == FilterViewModel.ItemFilterTypes.SaleBrand ?
							"/Home/Filter?" + FilterDefaultParameters.Replace("o=false", "o=true") :
							"/Home/" + ReturnTo + "?" + ProductsDefaultParameters
					};
			}

			// Brand
			if (HomeModel.Filter.FilterType == FilterViewModel.ItemFilterTypes.Brand)
			{
				HomeModel.Filter.UndoBrandFilter = new FilterItemViewModel
				{
					Name = HomeModel.Filter.CurrentBrand,
					Url = "/Home/" + ReturnTo + "?" + ProductsDefaultParameters
				};
			}
		}

		public static Department GetProductDepartment(this Product p)
		{
			IDepartmentRepository _dr = new DepartmentRepository();
			ICategoryRepository _cr = new CategoryRepository();
			ISubCategoryRepository _scr = new SubCategoryRepository();
			IProductTypeRepository _ptr = new ProductTypeRepository();

			switch (p.level_Id)
			{
				case 0: return _dr.Get(p.catalog_Id);
				case 1: return _cr.Get(p.catalog_Id).department;
				case 2: return _scr.Get(p.catalog_Id).category.department;
				default: return _ptr.Get(p.catalog_Id).subCategory.category.department;
			}
		}

		public static Object GetCatalogParent(int level_Id, int catalog_Id)
		{
			IDepartmentRepository _dr = new DepartmentRepository();
			ICategoryRepository _cr = new CategoryRepository();
			ISubCategoryRepository _scr = new SubCategoryRepository();
			IProductTypeRepository _ptr = new ProductTypeRepository();

			switch (level_Id)
			{ 
				case 1:
					return _dr.getByCategory(_cr.Get(catalog_Id));
				case 2:
					return _cr.GetBySubCategory(_scr.Get(catalog_Id));
				case 3:
					return _scr.GetByProductType(_ptr.Get(catalog_Id));
				default:
					return null;
			}
		}
	}
}
