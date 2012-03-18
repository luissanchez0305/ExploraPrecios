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
            //response.categories = _cr.GetAll().Select(y => new CategoryViewModel()
            //{
            //    categoryId = y.Id,
            //    categoryTitle = y.name
            //}).ToList();
            //response.subCategories = _scr.GetAll().Select(z => new SubCategoryViewModel()
            //{
            //    subCategoryId = z.Id,
            //    subCategoryTitle = z.name
            //}).ToList();
            //response.productTypes = _ptr.GetAll().Select(u => new ProductTypeViewModel()
            //{
            //    productTypeId = u.Id,
            //    productTypeTitle = u.name
            //}).ToList();

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

        public static List<FilterViewModel> FilterLoad(IList<Product> products, bool isSearch, string val) {
            var result = new List<FilterViewModel>();
            if (products.Count > 1)
            {
                // obtener la lista de precios
                var pricesList = products.SelectMany(x => x.clients).Select(y => y.price).OrderBy(z => z).ToList();
                // obtener la lista de marcas
                var brandsList = products.Select(x => x.brand.name).OrderBy(y => y).Distinct().ToList();

                var differencesList = new List<float>();
                // obtener la lista de diferencias entre cada precio (precio[1] - precio[0])
                for (int i = 1; i < pricesList.Count(); i++)
                {
                    differencesList.Add(pricesList[i] - pricesList[i - 1]);
                }
                // obtener promedio de diferencias
                var sumDifferences = differencesList.Average().RoundToInteger();

                var lowestPrice = pricesList.First();
                var bigestPrice = pricesList.Last();

                var rangesList = new List<KeyValuePair<int, int>>();
                var rangesListFinal = new List<KeyValuePair<int, int>>();
                // obtener lista de rango de precios
                var incValue = lowestPrice.RoundToInteger() + sumDifferences;
                rangesList.Add(new KeyValuePair<int, int>(0, incValue));
                // lista de rango de precios preliminar
                while (incValue < bigestPrice && sumDifferences != 0)
                {
                    rangesList.Add(new KeyValuePair<int, int>(incValue + 1, incValue + sumDifferences));
                    incValue += sumDifferences;
                }

                // remover todos los rangos que no regresan ningun producto
                //rangesList.RemoveAll(x => pricesList.Where(y => y >= x.Key && y <= x.Value).Count() == 0);

                // si la lista de rango supera los 5 rangos eliminar hasta que sea de ese tamaño o menor
                if (rangesList.Count >= 5)
                {
                    var lapse = rangesList.Count / 5;
                    var i = 0;
                    var currentRangeHigh=rangesList[i + lapse - 1].Value + 1;
                    rangesListFinal.Add(new KeyValuePair<int, int>(rangesList[0].Key, currentRangeHigh));

                    var nextRangeLow = currentRangeHigh + 1;
                    i += lapse;


                    while ((i + lapse - 1) < rangesList.Count)
                    {
                        currentRangeHigh = rangesList[i + lapse - 1].Value + 1;
                        rangesListFinal.Add(new KeyValuePair<int, int>(nextRangeLow, currentRangeHigh));
                        nextRangeLow = currentRangeHigh + 1;
                        i += lapse;
                    }

                    // hacer lista de rangos de precios de cantidad de elementos par
                    //if (rangesList.Count % 2 != 0)
                    //{
                    //    rangesList.Add(new KeyValuePair<int, int>(rangesList[rangesList.Count - 2].Key, rangesList[rangesList.Count - 1].Value));
                    //    rangesList.RemoveAt(rangesList.Count - 2);
                    //    rangesList.RemoveAt(rangesList.Count - 2);
                    //}

                    //while (rangesList.Count > 5)
                    //{
                    //    for (int i = 0; i < (rangesList.Count / 2) - 1; i++)
                    //    {
                    //        rangesList.Add(new KeyValuePair<int, int>(rangesList[0].Key, rangesList[1].Value));
                    //        rangesList.RemoveAt(0);
                    //        rangesList.RemoveAt(0);
                    //    }
                    //}
                }

                // crear parametro de listado de productos
                var valSplitted = val.Split(',');
                var productParameter = "&currentDisplay=" + valSplitted[0] + "," + valSplitted[1] + "&s=" + (isSearch ? "1" : "0");

                // crear lista de filtros por precios
                if (rangesListFinal.Count > 1)
                {
                    result.Add(new FilterViewModel()
                    {
                        value = "Precios",
                        items = rangesListFinal.Select(x => new FilterItemViewModel()
                        {
                            name = "De $" + x.Key.ToString("#,0.00") + " a $" + x.Value.ToString("#,0.00"),
                            url = "/Home/Filter?f=p&filterData=" + x.Key.ToString() + "," + x.Value.ToString() + productParameter
                        }).ToList()
                    });
                    result[0].items[0].name = result[0].items[0].name.Replace("0.00", "0.01");
                    result[0].items.Insert(0, new FilterItemViewModel() { name = "Ofertas", url = "/Home/Filter?f=o&filterData=" + productParameter });
                }

                // crear lista de filtros por marcas
                if (brandsList.Count > 1)
                {
                    result.Add(new FilterViewModel()
                    {
                        value = "Marcas",
                        items = brandsList.Select(x => new FilterItemViewModel()
                        {
                            name = x,
                            url = "/Home/Filter?f=b&filterData=" + x + productParameter
                        }).ToList()
                    });
                }
            }
            return result;
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
