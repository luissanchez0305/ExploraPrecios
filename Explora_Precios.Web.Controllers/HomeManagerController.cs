using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Explora_Precios.Data;
using Explora_Precios.Web.Controllers.ViewModels;
using Explora_Precios.Core;
using Explora_Precios.ApplicationServices;
using SharpArch.Web.NHibernate;
using System.IO;
using System.Net;
using Explora_Precios.Core.DataInterfaces;

namespace Explora_Precios.Web.Controllers
{

    [HandleError]
    public class HomeManagerController : Controller
    {
        IClientRepository _clientRepository;
        IBrandRepository _brandRepository;
        ICategoryRepository _categoryRepository;
        IQualityRepository _qualityRepository;
        IProduct_QualityRepository _product_QualityRepository;
        IProductRepository _productRepository;
        IProductTypeRepository _productTypeRepository;
        ISubCategoryRepository _subCategoryRepository;
        //IClient_Product _clientProductRepository;

        public HomeManagerController(IClientRepository clientRepository, IBrandRepository brandRepository, IQualityRepository qualityRepository, IProduct_QualityRepository product_QualityRepository, IProductRepository productRepository, ISubCategoryRepository subCategoryRepository, IProductTypeRepository productTypeRepository, ICategoryRepository categoryRepository)
        {
            _clientRepository = clientRepository;
            _brandRepository = brandRepository;
            _qualityRepository = qualityRepository;
            _product_QualityRepository = product_QualityRepository;
            _productRepository = productRepository;
            _subCategoryRepository = subCategoryRepository;
            _productTypeRepository = productTypeRepository;
            _categoryRepository = categoryRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UpdatePrices()
        {
            var clienteVM = new HomeManagerViewModel();
            clienteVM.prodList = new List<ProductViewModel>();
            return View(clienteVM);
        }

        // Update or search the products specified
        [AcceptVerbs(HttpVerbs.Post)]
        [Transaction]
        public ActionResult DoUpdatePrices(string button)
        {
            var model = new HomeManagerViewModel();
            bool hasErrors = !TryUpdateModel(model);
            model.LoadProperties();

            if (hasErrors)
            {
                var listErrors = ModelState.Values.Where(x => x.Errors.Count > 1);
                return View("UpdatePrices", model);
            }
            var client = _clientRepository.Get(model.clientItemId);
            if (button == "Search")
            {
                int level = model.prodTypeId != 0 ? 3 :
                    model.subCatId != 0 ? 2 :
                    model.catId != 0 ? 1 : 0;
                int catalogId = model.prodTypeId != 0 ? model.prodTypeId :
                    model.subCatId != 0 ? model.subCatId :
                    model.catId != 0 ? model.catId : model.depId;
                var _clientServices = new ClientServices();
                var clientProducts = _clientServices.GetClientItems(_clientRepository.Get(model.clientItemId), level, catalogId);

                model.prodList = clientProducts.Select(x => new ProductViewModel()
                {
                    productObj = _productRepository.getbyReference(x.product.productReference),
                    productDescription = x.product.description,
                    productName = x.product.name,
                    productRef = x.product.productReference,
                    productBrandId = x.product.brand.Id,
                    productBrand = x.product.brand.name,
                    productImageUrl = x.product.image.url,
                    qualities = x.product.qualities.Select(y => new QualityViewModel()
                    {
                        active = true,
                        name = y.quality.name,
                        value = y.value
                    }).ToList()
                }).ToList();

                // Introduce quality names List in products model
                var qualityNamesList = _qualityRepository.GetAll().Select(x => x.name).ToList();
                if ((clientProducts.Count > 0 && clientProducts[0].product.qualities.Count > 0 && clientProducts[0].product.qualities[0].quality.name.Length == 0) || 
                    (clientProducts.Count > 0 && clientProducts[0].product.qualities.Count == 0))
                {
                    foreach (var product in model.prodList)
                    {
                        product.qualityNames = qualityNamesList;
                    }

                }
            }
            else if (button == "Save")
            {
                // TODO save prices
                var qualityList = new List<Quality>();
                foreach (var product in model.prodList)
                {
                    // recopila los qualities ya creados de los no creados
                    foreach (var quality in product.qualities)
                    {
                        // verifica si el quality esta activo y no ha sido ingresado ya en algun otro producto
                        if (quality.active && qualityList.SingleOrDefault(x => x.name == quality.name) == null)
                        {
                            // verifica si no ha sido creado en bases de datos antes
                            var qualityObj = _qualityRepository.getByName(quality.name);
                            if (qualityObj != null)
                                // utiliza el ya creado en bases de datos y lo ingresa a la lista de qualities
                                qualityList.Add(qualityObj);
                            else
                            {
                                // crea uno nuevo y lo ingresa a la lista de qualities
                                var newQuality = new Quality()
                                {
                                    name = quality.name
                                };
                                qualityList.Add(newQuality);
                            }
                        }
                    }

                    var productObj = CreateUpdateProduct(product, client,
                        model.prodTypeId != 0 ? 3 : model.subCatId != 0 ? 2 : model.catId != 0 ? 1 : 0,
                        model.prodTypeId != 0 ? model.prodTypeId : model.subCatId != 0 ? model.subCatId : model.catId != 0 ? model.catId : model.depId,
                        qualityList);

                    var productFound = client.products.ToList().FindIndex(x => x.product.productReference == productObj.productReference);
                    if (productFound == -1)
                        client.products.Add(new Client_Product() { product = productObj, client = client });
                    else
                    {
                        client.products[productFound].product = productObj;
                    }
                    _clientRepository.SaveOrUpdate(client);
                }
                model = new HomeManagerViewModel();
                model.prodList = new List<ProductViewModel>();
            }
            return View("UpdatePrices", model);
        }

        private Product CreateUpdateProduct(ProductViewModel product, 
            Client client, 
            int level_Id, 
            int catalog_Id, 
            List<Quality> qualtityList)
        {
            var brand = _brandRepository.GetByBrandName(product.productBrand.Trim());
            var productObj = new Product();
            // Is a new product
            if (product.productId == 0)
            {
                productObj.level_Id = level_Id;
                productObj.catalog_Id = catalog_Id;
                productObj.productReference = product.productRef;
                productObj.brand = brand != null ? brand : new Brand() { name = product.productBrand.Trim() };
                productObj.clients = new List<Client_Product>();
                productObj.clients.Add(new Client_Product() { client = client, product = productObj, price = 0, url = "" });
                productObj.description = product.productDescription;
                productObj.image = new Image()
                {
                    imageObj = new WebClient().DownloadData(product.productImageUrl),
                    url = product.productImageUrl
                };
                productObj.name = product.productName;
                productObj.productReference = product.productRef;
                productObj.qualities = new List<Product_Quality>();

                foreach (var qualityModel in product.qualities)
                {
                    if (qualityModel.active)
                    {
                        var qualityObj = qualtityList.Single(x => x.name == qualityModel.name);
                        productObj.qualities.Add(new Product_Quality()
                        {
                            quality = qualityObj,
                            value = qualityModel.value,
                            product = productObj
                        });

                    }
                }
            }
            // Product to be update
            else if (product.productId != 0)
            {
                // TODO
                productObj.level_Id = level_Id;
                productObj.catalog_Id = catalog_Id;
                productObj.productReference = product.productRef;
                productObj.brand = brand != null ? brand : new Brand() { name = product.productBrand };
                productObj.clients = new List<Client_Product>();
                productObj.clients.Add(new Client_Product() { client = client, product = productObj, price = 0, url = "" });
                productObj.description = product.productDescription;
                productObj.image = new Image()
                {
                    imageObj = new WebClient().DownloadData(product.productImageUrl),
                    url = product.productImageUrl
                };
                productObj.name = product.productName;
                productObj.productReference = product.productRef;
                productObj.qualities = new List<Product_Quality>();
                //foreach (var qualityModel in product.qualities)
                //{
                //    if (qualityModel.active)
                //    {
                //        var quality = _qualityRepository.getByName(qualityModel.name);
                //        productObj.qualities.Add(new Product_Quality()
                //        {
                //            quality = quality != null ? quality : new Quality()
                //            {
                //                name = qualityModel.name
                //            },
                //            value = qualityModel.value,
                //            product = productObj
                //        });
                //    }
                //}

            }

            //try
            //{
            //    _productRepository.SaveOrUpdate(productObj);
            //}
            //catch
            //{
                return productObj;
            //}

            //return qualtityList;
        }

        public JsonResult getJsonResult(string text, IEnumerable<SelectListItem> items)
        {
            var _list = new List<SelectListItem>()
            { 
                new SelectListItem()
                { 
                    Text="Seleccione "+text, 
                    Value="0"
                }
            };

            _list.AddRange(items);
            return new JsonResult
            {
                Data = (_list).Select(x => new { Id = x.Value, Name = x.Text }).ToArray()
            };
        }

        public JsonResult FindCategoryByDepartmentId(int Id)
        {
            var categories = from c in _categoryRepository.GetByDepartamentId(Id)
                             select new SelectListItem { Value = c.Id.ToString(), Text = c.name };
            return getJsonResult("una categoria", categories);
        }

        public JsonResult FindSubCategoryByCategoryId(int Id)
        {
            var subCategories = from s in _subCategoryRepository.GetByCategoryId(Id)
                                select new SelectListItem  { Value = s.Id.ToString(), Text = s.name };
            return getJsonResult("una subcategoria", subCategories);
        }

        public JsonResult FindProductTypeBySubCategoryId(int Id)
        {
            var productTypes = from p in _productTypeRepository.GetBySubCategoryId(Id)
                        select new SelectListItem { Value = p.Id.ToString(), Text = p.name };
            return getJsonResult("un tipo de producto", productTypes);
        }
    }

}
