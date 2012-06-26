using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Core.DataInterfaces;
using System.Web.Mvc;
using Explora_Precios.Web.Controllers.ViewModels;
using Explora_Precios.Data;
using Explora_Precios.Web.Controllers.Helpers;
using Explora_Precios.Core;
using Explora_Precios.ApplicationServices;
using SharpArch.Web.NHibernate;
using System.IO;
using SharpArch.Core.PersistenceSupport;
using NHibernate;

namespace Explora_Precios.Web.Controllers
{
	public class ManagerController : PrimaryController
	{
		IProductRepository _productRepository;
		IQualityRepository _qualityRepository;
		ICategoryRepository _categoryRepository;
		ISubCategoryRepository _subCategoryRepository;
		IDepartmentRepository _departmentRepository;
		IProductTypeRepository _productTypeRepository;
		IClientRepository _clientRepository;
		IBrandRepository _brandRepository;
		IClient_ProductRepository _clientProductRepository;
		IProduct_QualityRepository _productQualityRepository;
		ICatalog_AddressRepository _catalogAddressRepository;
		IProductLogRepository _productLogRepository;
		IUser_ProductRepository _userProductRepository;
		IProductCounterRepository _productCounterRepository;
		IClientCounterRepository _clientCounterRepository;
		private int clientIdToBeShown = -1;

		public ManagerController(IProductRepository productRepository, IQualityRepository qualityRepository, ICategoryRepository categoryRepository,
			ISubCategoryRepository subCategoryRepository, IProductTypeRepository productTypeRepository, IDepartmentRepository departmentRepository,
			IClientRepository clientRepository, IBrandRepository brandRepository, IClient_ProductRepository clientProductRepository,
			IProduct_QualityRepository productQualityRepository, ICatalog_AddressRepository catalogAddressRepository,
			IProductLogRepository productLogRepository, IUser_ProductRepository userProductRepository, IProductCounterRepository productCounterRepository,
			IClientCounterRepository clientCounterRepository)
		{
			_productRepository = productRepository;
			_qualityRepository = qualityRepository;
			_categoryRepository = categoryRepository;
			_subCategoryRepository = subCategoryRepository;
			_productRepository = productRepository;
			_productTypeRepository = productTypeRepository;
			_departmentRepository = departmentRepository;
			_clientRepository = clientRepository;
			_brandRepository = brandRepository;
			_clientProductRepository = clientProductRepository;
			_productQualityRepository = productQualityRepository;
			_catalogAddressRepository = catalogAddressRepository;
			_productLogRepository = productLogRepository;
			_userProductRepository = userProductRepository;
			_productCounterRepository = productCounterRepository;
			_clientCounterRepository = clientCounterRepository;
		}

		#region Llamados de Views

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult MngClients(int? id)
		{
			var clientList = GetClientModelList(id.HasValue ? id.Value : -1);
			if (id == 0)
			{
				clientList.Insert(0, new ClientViewModel { clientId = 0, choosen = true });
			}
			return View(clientList);
		}

		private IList<ClientViewModel> GetClientModelList(int id)
		{
			return _clientRepository.GetAll().Select(c => new ClientViewModel
			{
				clientId = c.Id,
				url = c.url,
				isActive = c.isActive,
				catalogAddress = c.catalogAddress,
				clientName = c.name,
				facebookId = c.facebookId,
				facebookPublish = c.facebookPublish,
				choosen = id == c.Id
			}).ToList();
		}

		public ActionResult MngCatalog()
		{ return View(); }

		public ActionResult MngCatalogClient()
		{
			return GetAllClientViewModel(false);
		}

		public ActionResult MngProductsByClient()
		{
			return GetAllClientViewModel(true);
		}

		public ActionResult MngProductByReference(string param)
		{
			if (string.IsNullOrEmpty(param))
				return View();

			var productFound = _productRepository.GetbyReference(param);

			ViewData.Model = LoadProductModel(productFound);
			return Json(new
			{
				html = this.RenderViewToString("PartialViews/ProductForm", ViewData),
				found = productFound != null,
				result = "success"
			});
		}

		public ActionResult MngProductsLocal()
		{
			var CatalogVM = new CatalogViewModel();
			CatalogVM.departments = _departmentRepository.GetAll().Select(
				dep => new DepartmentViewModel() { departmentId = dep.Id, departmentTitle = dep.name, categories = dep.categories.Select(
					cat => new CategoryViewModel() { departmentId = dep.Id, categoryId = cat.Id, categoryTitle = cat.name, subCategories = cat.subCategories.Select(
						subCat => new SubCategoryViewModel() { categoryId = cat.Id, subCategoryId = subCat.Id, subCategoryTitle = subCat.name, productTypes = subCat.productTypes.Select(
							prodType => new ProductTypeViewModel(){ subCategoryId= subCat.Id, productTypeId = prodType.Id, productTypeTitle = prodType.name }).ToList() })
						.ToList() })                            
					.ToList() })
				.ToList();
			return View(CatalogVM);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		[Transaction]
		public ActionResult SaveClient()
		{
			var ClientModel = new ClientViewModel();
			var result = TryUpdateModel(ClientModel);
			var clientObj = _clientRepository.Get(ClientModel.clientId) ?? new Client();

			clientObj.name = ClientModel.clientName;
			clientObj.url = ClientModel.url;
			clientObj.facebookId = ClientModel.facebookId;
			clientObj.facebookPublish = ClientModel.facebookPublish;
			clientObj.isActive = ClientModel.isActive;
			clientObj.catalogAddress = ClientModel.catalogAddress;

			_clientRepository.SaveOrUpdate(clientObj);

			var clientList = GetClientModelList(ClientModel.clientId);
			return View("MngClients", clientList);
		}

		public ActionResult SearchRepeated(int? masterProductId, int? productId)
		{
			var repeatedProductModel = new List<RepeatedProductsViewModel>();
			if (productId != null)
			{
				// TODO: modificar los productos repetidos de este producto y unificarlos
			}
			else
			{
				var allProductList = _productRepository.GetAll();
				foreach (var masterProduct in allProductList)
				{
					// compilacion de un producto y sus repetidos
					var productCompilation = new RepeatedProductsViewModel();
					foreach (var product in allProductList)
					{
						if (product.Id != masterProduct.Id && repeatedProductModel.SingleOrDefault(p => p.masterProduct.productId == masterProduct.Id) == null)
						{
							// se busca la cantidad de caracteres diferentes si la diferencia es menor a 25% se agrega como posible repetido
							//var diff = CommonUtilities.Levenshtein(masterProduct.productReference, product.productReference);
							//var bigStringLength = masterProduct.productReference.Length >= product.productReference.Length ? masterProduct.productReference.Length : product.productReference.Length;
							//if (diff * 100 / bigStringLength < 10)

							if (CommonUtilities.Contain(masterProduct.productReference, product.productReference, 2))
							{
								if (!repeatedProductModel.Exists(p => p.masterProduct.productId == masterProduct.Id))
								{
									productCompilation.masterProduct = LoadProductModel(masterProduct);
								}
								var productRepeated = new RepeatedProductViewModel();
								productRepeated.productRepeated = LoadProductModel(product);
								productRepeated.isChecked = false;
								productCompilation.repeatedProducts.Add(productRepeated);
								repeatedProductModel.Add(productCompilation);
							}
						}
					}
				}
			}

			return View(repeatedProductModel);
		}
		#endregion

		#region Llamados AJAX

		// AJAX
		[AcceptVerbs(HttpVerbs.Post)]
		[Transaction]
		public ActionResult DoCreateProduct(FormCollection f)
		{
			var commonUtilities = new CommonUtilities();
			var productVM = new ProductViewModel();
			var currentPage = Request.QueryString["page"];
			clientIdToBeShown = !string.IsNullOrEmpty(Request.QueryString["client"]) && Request.QueryString["client"] != "undefined" ? int.Parse(Request.QueryString["client"]) : clientIdToBeShown;
			bool hasErrors = !TryUpdateModel(productVM);
			// verificar si todos los clientes tienen # reference
			var missingReferences = productVM.clientList.Where(c => c.reference.Length == 0).Count() > 0;
			var missingBrand = productVM.productBrand.Length == 0;
			productVM.catalogProduct = CatalogHelper.FromLevelsToCatalog(productVM.catalogLevel, productVM.catalogId);
			if (hasErrors || missingReferences || missingBrand)
			{
				//ViewData.Model = productVM;
				return Json(new
				{
					result = "fail",
					//html = this.RenderViewToString("PartialViews/ProductsForm", ViewData),
					msg = "Falta: " + (missingReferences ?  "**Referencia en algun cliente " : "") + (missingBrand ? "**Marca" : "")
				});
			}

			var oldPrice = new List<float>();
			var newPrice = new List<float>();
			var oldSpecialPrice = new List<float>();
			var newSpecialPrice = new List<float>();

			var useAsMain = productVM.clientList.SingleOrDefault(cpVM => cpVM.isMain);
			var mainProductReference = (useAsMain != null ? useAsMain.reference : productVM.productRef).Replace("/", "").Replace("-", "").Replace(" ", "");
			//var productCreatedAlready = useAsMain != null;

			var listProductStatuses = new Dictionary<int, Explora_Precios.ApplicationServices.ClientServices.ItemType>();
			var brand = _brandRepository.GetByBrandName(productVM.productBrand);
			//Product oProduct = _productRepository.Get(productVM.productId);
			Product oProduct = _productRepository.GetbyReference(mainProductReference);
			productVM.productId = oProduct != null ? oProduct.Id : 0;

			if (productVM.productId == 0)
			{
				oProduct = new Product();
				var image = new Image() { url = productVM.productImageUrl };

				if (!string.IsNullOrEmpty(productVM.productImageUrl))
				{
					image.imageObj = GetImage(productVM.productImageUrl);
				}

				oProduct.image = image;
				foreach (var client_product in productVM.clientList.Where(client => client.productStatus != ClientServices.ItemType.Possible_Related))
				{
					listProductStatuses.Add(client_product.clientId, ClientServices.ItemType.Local);
					var newClientProduct = GetClient(oProduct, client_product);

					oldPrice.Add(0);
					oldSpecialPrice.Add(0);
					newPrice.Add(newClientProduct.price);
					newSpecialPrice.Add(newClientProduct.specialPrice);

					//newClientProduct.page = commonUtilities.GetXHTML(client_product.url);
					newClientProduct.productReference = client_product.reference;
					oProduct.clients.Add(newClientProduct);
				}

				if (productVM.qualities != null)
					foreach (var quality in productVM.qualities.Where(q => q.active))
					{
						if (string.IsNullOrEmpty(quality.name) || oProduct.qualities.SingleOrDefault(qual => qual.quality.name == quality.name) == null)
							oProduct.qualities.Add(new Product_Quality()
							{
								quality = GetQuality(quality),
								value = quality.value.Trim(),
								product = oProduct
							});
					}
			}
			else
			{
				oProduct = _productRepository.Get(productVM.productId);

				foreach (var client in productVM.clientList)
				{
					if (client.isMain || client.isActive)
					{
						var clientProductObj = oProduct.clients.SingleOrDefault(x => x.client.Id == client.clientId);
						listProductStatuses.Add(client.clientId, client.productStatus);
						if (clientProductObj != null)
						{
							oldPrice.Add(clientProductObj.price);
							oldSpecialPrice.Add(clientProductObj.specialPrice);
							newPrice.Add(client.price);
							newSpecialPrice.Add(client.specialPrice);

							clientProductObj.dateModified = ChangeClientProduct(clientProductObj, client) ? DateTime.Now : (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
							clientProductObj.isActive = client.isActive;
							clientProductObj.price = client.price;
							clientProductObj.specialPrice = client.specialPrice;
							clientProductObj.url = client.url;
							clientProductObj.productReference = client.reference;
							if (string.IsNullOrEmpty(client.page) || client.pageStatus != ClientServices.PageStatus.PageEqual)
								clientProductObj.page = "";//commonUtilities.GetXHTML(client.url);
						}
						else if (clientProductObj == null)
						{
							oldPrice.Add(0);
							oldSpecialPrice.Add(0);
							newPrice.Add(client.price);
							newSpecialPrice.Add(client.specialPrice);

							var newClientProduct = new Client_Product()
							{
								client = _clientRepository.Get(client.clientId),
								isActive = true,
								name = "",
								price = client.price,
								product = oProduct,
								url = client.url,
								dateCreated = DateTime.Now,
								page = "",//commonUtilities.GetXHTML(client.url),
								productReference = client.reference,
								dateModified = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue,
								dateReported = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue
							};
							if (client.specialPrice > 0)
								newClientProduct.specialPrice = client.specialPrice;
							oProduct.clients.Add(newClientProduct);
						}
					}
				}

				// agrega los nuevos clientes o edita los ya existentes 
				// a la lista de clientes del producto
				//oProduct.clients = productVM.clientList.Select(x => GetClient(oProduct, productVM, x)).ToList();
				var image = oProduct.image;
				if (!string.IsNullOrEmpty(productVM.productImageUrl) && productVM.productImageUrl != oProduct.image.url)
				{
					image.imageObj = GetImage(productVM.productImageUrl);
					image.url = productVM.productImageUrl;
					oProduct.image = image;
				}

				if (productVM.qualities != null)
				{
					// quita los qualities que no estan activados
					for (var i = 0; i < oProduct.qualities.Count; i++)
					{
						var pQuality = oProduct.qualities[i];
						var pVMQuality = productVM.qualities.FirstOrDefault(x => x.Id == pQuality.quality.Id);
						if (pVMQuality != null && !pVMQuality.active)
						{
							oProduct.qualities.Remove(pQuality);
							i--;
							//_productQualityRepository.Delete(pQuality);
						}
					}

					// actualiza o agrega los qualities
					foreach (var quality in productVM.qualities.Where(x => x.active))
					{
						// si el quality no es nuevo
						if (quality.Id != 0 && !string.IsNullOrEmpty(quality.name))
						{
							var pQuality = oProduct.qualities.FirstOrDefault(x => x.quality.Id == quality.Id);
							pQuality.value = quality.value;
						}
						// si el quality es nuevo
						else if (!string.IsNullOrEmpty(quality.name))
						{
							var newQuality = GetQuality(quality);
							var newPQuality = new Product_Quality()
							{
								product = oProduct,
								quality = newQuality,
								value = quality.value
							};

							oProduct.qualities.Add(newPQuality);
						}
					}
				}
			}

				//oProduct.image = image;
				oProduct.brand = brand ?? new Brand() { name = productVM.productBrand.Trim() };
				oProduct.description = productVM.productDescription;
				oProduct.name = productVM.productName;
				oProduct.productReference = mainProductReference;
				oProduct.catalog_Id = productVM.catalogId;
				oProduct.level_Id = productVM.catalogLevel;

			var newProductObj = _productRepository.SaveOrUpdate(oProduct);


			if (oldPrice.Count > 0)
			{
				for(var priceIndex = 0; priceIndex < oldPrice.Count; priceIndex++)
				{
					var clientProduct = newProductObj.clients[priceIndex];
					if ((float)oldPrice[priceIndex] != (float)newPrice[priceIndex] || (float)oldSpecialPrice[priceIndex] != (float)newSpecialPrice[priceIndex])
						_productLogRepository.Insert(new ProductLog()
						{
							changeDate = DateTime.Now,
							fromPrice = (float)oldPrice[priceIndex],
							fromSpecialPrice = (float)oldSpecialPrice[priceIndex],
							toPrice = (float)newPrice[priceIndex],
							toSpecialPrice = (float)newSpecialPrice[priceIndex],
							operation = 0,
							user_Id = 1,
							client_Id = clientProduct.client.Id,
							product_Id = clientProduct.product.Id
						});
				}
			}
			var pModel = LoadProductModel(oProduct, listProductStatuses);
			pModel.clientList.ForEach(delegate(ClientViewModel clientProduct) {
				var vmClientProduct = productVM.clientList.SingleOrDefault(x => x.clientId == clientProduct.clientId && (x.isMain || x.isActive));
				clientProduct.oldPrice = vmClientProduct != null ? vmClientProduct.price : clientProduct.price;
			});
			ViewData.Model = pModel;

			if (Session["CurrentProductList"] == null)
				LoadSessionProductsList(_catalogAddressRepository.GetByClientCatalogDetails(
					_clientRepository.Get(productVM.clientList[0].clientId), productVM.catalogLevel, productVM.catalogId).First().Id, null);
			
			 var productList = (List<ProductViewModel>)Session["CurrentProductList"];
			 if (int.Parse(currentPage) < 0)
				 productList.Add(pModel);
			 else
			 {
				 //productList.RemoveAt(int.Parse(currentPage));
				 productList[int.Parse(currentPage)] = pModel;
			 }
			 Session["CurrentProductList"] = productList;

			return Json(new
			{
				result = "success",
				html = this.RenderViewToString("PartialViews/ProductsForm", ViewData)
			});
			//return View(source, pModel);
		}

		// AJAX
		[AcceptVerbs(HttpVerbs.Post)]
		[Transaction]
		public ActionResult DoSaveClientCatalog(FormCollection f)
		{
			var catalogVM = new List<CatalogItemViewModel>();
			bool hasErrors = !TryUpdateModel(catalogVM, "CatalogList");

			if (hasErrors)
			{
				return Json(new
				{
					result = "error"
				});

			}

			var clientId = catalogVM[0].clientId;
			var isOnSite = catalogVM[0].type.IndexOf("OnSite") > -1;

			foreach (var CatalogItem in catalogVM.Where(cat => cat.selected))
			{
				var catalogObj = _catalogAddressRepository.Get(CatalogItem.dbId);
				if (catalogObj != null)
				{
					catalogObj.catalog_Id = CatalogItem.catalogId;
					catalogObj.level_Id = CatalogItem.levelId;
					catalogObj.url = CatalogItem.url;
				}
				else
				{
					catalogObj = new Catalog_Address()
					{
						catalog_Id = CatalogItem.catalogId,
						client = _clientRepository.Get(CatalogItem.clientId),
						level_Id = CatalogItem.levelId,
						url = CatalogItem.url[0] != '/' ? "/" + CatalogItem.url : CatalogItem.url,
						manualFeed = false
					};
				}

				_catalogAddressRepository.SaveOrUpdate(catalogObj);
			}

			return GetCatalogClientList(_clientRepository.Get(clientId), isOnSite ? ClientServices.ItemType.OnSite_NotOnDB : ClientServices.ItemType.OnDB_NotOnSite);
		}
		
		// AJAX
		[AcceptVerbs(HttpVerbs.Post)]
		[Transaction]
		public ActionResult DoSaveCatalog(FormCollection f)
		{
			var departmentVM = new List<DepartmentViewModel>();
			var categoryVM = new List<CategoryViewModel>();
			var subCategoryVM = new List<SubCategoryViewModel>();
			var productTypeVM = new List<ProductTypeViewModel>();

			bool hasErrorsDep = !TryUpdateModel(departmentVM, "Department");
			bool hasErrorsCat = !TryUpdateModel(categoryVM, "Category");
			bool hasErrorsSubCat = !TryUpdateModel(subCategoryVM, "SubCategory");
			bool hasErrorsProdType = !TryUpdateModel(productTypeVM, "ProductType");

			if (hasErrorsDep || hasErrorsCat || hasErrorsSubCat || hasErrorsProdType)
			{
				return Json(new
				{
					result = "error"
				});
			}

			var type = departmentVM.Count > 0 ? "Department" : categoryVM.Count > 0 ? "Category" : subCategoryVM.Count > 0 ? "SubCategory" : productTypeVM.Count > 0 ? "ProductType" : "";

			try
			{
				if (type.Length > 0)
				{
					if (type == "Department")
					{
						foreach (var departmentItem in departmentVM)
						{
							var departmentObj = _departmentRepository.Get(departmentItem.departmentId);
							departmentObj.name = departmentItem.departmentTitle;
							_departmentRepository.SaveOrUpdate(departmentObj);
						}
					}
					else if (type == "Category")
					{
						foreach (var categoryItem in categoryVM)
						{
							var categoryObj = _categoryRepository.Get(categoryItem.categoryId);
							categoryObj.name = categoryItem.categoryTitle;
							_categoryRepository.SaveOrUpdate(categoryObj);
						}
					}
					else if (type == "SubCategory")
					{
						foreach (var subCategoryItem in subCategoryVM)
						{
							var subCategoryObj = _subCategoryRepository.Get(subCategoryItem.subCategoryId);
							subCategoryObj.name = subCategoryItem.subCategoryTitle;
							_subCategoryRepository.SaveOrUpdate(subCategoryObj);
						}
					}
					else
					{
						foreach (var productTypeItem in productTypeVM)
						{
							var productTypeObj = _productTypeRepository.Get(productTypeItem.productTypeId);
							productTypeObj.name = productTypeItem.productTypeTitle;
							_productTypeRepository.SaveOrUpdate(productTypeObj);
						}
					}
					return Json(new
					{
						result = "success"  
					});
				}
			}
			catch (Exception e)
			{
				return Json(new
				{
					result = "fail",
					msg = e.Message
				});
			}

			return Json(new
			{
				result = "empty"
			});

		}

		//Ajax
		public ActionResult GetCatalogCascade(int levelId, int catalogId)
		{
			try
			{
				ViewData.Model = CatalogHelper.FromLevelsToCatalog(levelId, catalogId); ;
				return Json(new
				{
					result = "success",
					html = this.RenderViewToString("PartialViews/ShowCatalogCascade", ViewData)
				});
			}
			catch (Exception e)
			{
				return Json(new
				{
					result = "fail",
					msg = e.Message,
					html = ""
				});
			}
		}

		//Ajax
		public ActionResult DeleteClientCatalogItem(int itemId)
		{
			try
			{
				if (_catalogAddressRepository.Get(itemId) != null)
					_catalogAddressRepository.DeleteCatalogItem(itemId);
				return Json(new
				{
					result = "success",
					msg = ""
				}); 
			}
			catch (Exception e)
			{
				return Json(new
				{
					result = "fail",
					msg = e.Message
				});
			}
		}

		public string UpdateCountersText()
		{
			try
			{
				UpdateCounters();
				return "Exito, Contadores actualizados!";
			}
			catch
			{
				return "Fail, Contadores actualizados!";
			}
		}

		public ActionResult UpdateCounters()
		{
			try
			{
				new AutomaticServices(_clientProductRepository, _productRepository, _productCounterRepository, _clientCounterRepository).UpdateCounters();
			}
			catch
			{
				throw;
			}

			return Json(new
			{
				result = "success",
				msg = ""
			});
		}

		public string LoadAutomaticUpdateText()
		{
			var autoUpdate = LoadAutomaticUpdate();
			if ((((JsonResult)autoUpdate).Data).ToString().Contains("Success"))
			{
				return "Exito, Productos Actualizados!";
			}
			return "Fail, Productos Actualizados!";
		}

		// AJAX
		public ActionResult LoadAutomaticUpdate()
		{ 
			try
			{
				new AutomaticServices(_clientProductRepository, _catalogAddressRepository, _productLogRepository, _userProductRepository).UpdateProducts(_clientProductRepository.GetAllActive());

				return Json(new
				{
					result = "success",
					msg = ""
				});
			}
			catch (Exception e)
			{
				return Json(new
				{
					result = "fail",
					msg = e.Message
				});
			}
			
		}
		
		// AJAX
		public ActionResult LoadUnattachedProducts(int client_Id)
		{
			var isEmpty = false;
			var responseProductList = new List<ProductViewModel>();
			clientIdToBeShown = client_Id;
			try
			{
				var clientObj = _clientRepository.Get(client_Id);
				var clientProducts = _clientProductRepository.GetByClient(clientObj);
				// Obtenemos todos los products del cliente
				foreach (var clientProduct in clientProducts.Where(cp => !string.IsNullOrEmpty(cp.url)))
				{
					// se intenta buscar el catalogo al que pertenece ese producto en el cliente
					var catalogAddress = _catalogAddressRepository.GetByClientCatalogDetails(clientProduct.client, clientProduct.product.level_Id, clientProduct.product.catalog_Id);
					// si el catalogo no se encuentra entonces se debe buscar algun catalogo a un nivel superior al que el producto esta asignado
					if (catalogAddress.Count == 0 && clientProduct.product.level_Id  > 0)
					{ 
						var level_Id = clientProduct.product.level_Id;
						var catalog_Id = clientProduct.product.catalog_Id;
						while(level_Id > 0 && catalogAddress.Count == 0)
						{
							// se obtiene el id del catalogo padre al nivel que se esta buscando
							if (level_Id == 1)
								catalog_Id = ((Department)CatalogHelper.GetCatalogParent(level_Id, catalog_Id)).Id;
							else if(level_Id == 2)
								catalog_Id = ((Category)CatalogHelper.GetCatalogParent(level_Id, catalog_Id)).Id;
							else if(level_Id == 3)
								catalog_Id = ((SubCategory)CatalogHelper.GetCatalogParent(level_Id, catalog_Id)).Id;

							/* y se hace la busqueda del catalogo de un nivel superior y el id del catalogo padre en el cliente que se esta buscando, 
							 * si existe entonces el producto no esta sin catalogo y no se agrega a la lista */
							catalogAddress = _catalogAddressRepository.GetByClientCatalogDetails(clientProduct.client, level_Id - 1, catalog_Id);
							level_Id--;
						}
						if (catalogAddress.Count == 0)
							responseProductList.Add(LoadProductModel(clientProduct.product));
					}
				}
				if (responseProductList.Count > 0)
					ViewData.Model = responseProductList.First();
				else
					isEmpty = true;
				Session["CurrentProductList"] = responseProductList;
				return Json(new
				{
					result = isEmpty ? "empty" : "success",
					total = responseProductList.Count(),
					client = client_Id,
					html = responseProductList.Count > 0 ? this.RenderViewToString("PartialViews/ProductsForm", ViewData) : ""
				});
			}
			catch (Exception e)
			{
				return Json(new
				{
					result = "fail",
					msg = e.Message,
					html = ""
				});
			}
		}

		public ActionResult GetProductPage(int page)
		{
			var productToShow = new ProductViewModel();
			var sessionProductVMsList = ((List<ProductViewModel>)Session["CurrentProductList"]);
			var isEmpty = false;
			if (sessionProductVMsList.Count == 0)
				isEmpty = true;
			else
				productToShow = sessionProductVMsList.ElementAt(page);


			ViewData.Model = productToShow;

			return Json(new
			{
				result = isEmpty ? "empty" : "success",
				total = sessionProductVMsList.Count(),
				html = this.RenderViewToString("PartialViews/ProductsForm", ViewData)
			});
		}

		// AJAX
		public ActionResult GetCatalogProducts(bool loadSession, int? clientId, int? itemId, int? page, string display) 
		{
			var productToShow = new ProductViewModel();
			try
			{
				var clientProductIdList = new List<int>();
				var isEmpty = false;
				var sessionProductVMsList = new List<ProductViewModel>();
				var catalogAddress = _catalogAddressRepository.Get(itemId.Value);
				if (loadSession)
				{
					clientId = catalogAddress.client.Id;
					sessionProductVMsList = LoadSessionProductsList(itemId, catalogAddress, out isEmpty).GetProductsOfType(display, clientId.Value);
					productToShow = sessionProductVMsList.FirstOrDefault();
					if (productToShow == null)
					{
						isEmpty = true;
						productToShow = LoadProductModel(LoadEmptyProduct(ClientServices.ItemType.Local, catalogAddress.level_Id, catalogAddress.catalog_Id, catalogAddress.client));
					}
				}
				else
				{
					if (Session["CurrentProductList"] == null)
						sessionProductVMsList = LoadSessionProductsList(itemId, null, out isEmpty);
					else
						sessionProductVMsList = ((List<ProductViewModel>)Session["CurrentProductList"]).GetProductsOfType(display, clientId.Value);

					if (sessionProductVMsList.Count == 0)
					{
						isEmpty = true;
						productToShow = LoadProductModel(LoadEmptyProduct(ClientServices.ItemType.Local, catalogAddress.level_Id, catalogAddress.catalog_Id, catalogAddress.client));
					}
					else
						productToShow = sessionProductVMsList.ElementAt(page.HasValue ? page.Value : 0);
				}

				var clientObj = _clientRepository.Get(clientId.Value);
				if (string.IsNullOrEmpty(productToShow.clientList[0].page))
					productToShow.clientList[0].pageStatus = ClientServices.PageStatus.PageNotSet;
				else
				{
					var clientIndex = getClientIndex(productToShow.clientList, clientObj);
					var commonUtilities = new CommonUtilities();
					var currentPageString = commonUtilities.GetXHTML(productToShow.clientList[clientIndex].url);
					//currentPageString = currentPageString.Replace(currentPageString.Substring(currentPageString.IndexOf("window.HDUSeed="), 50), "");
					var dbProductPageString = productToShow.clientList[clientIndex].page;
					//dbProductPageString = dbProductPageString.Replace(dbProductPageString.Substring(dbProductPageString.IndexOf("window.HDUSeed="), 50), "");
					var comparedPages = string.Compare(currentPageString, dbProductPageString);
					if (comparedPages != 0)
						productToShow.clientList[clientIndex].pageStatus = ClientServices.PageStatus.PageChanged;
					else
						productToShow.clientList[clientIndex].pageStatus = ClientServices.PageStatus.PageEqual;
				}

				ViewData.Model = productToShow;

				return Json(new
				{
					result = isEmpty ? "empty" : "success",
					total = sessionProductVMsList.Count(),
					client = clientId.Value,
					html = this.RenderViewToString("PartialViews/ProductsForm", ViewData)
				});
			}
			catch (Exception e)
			{
				return Json(new
				{
					result = "fail",
					msg = e.Message,
					html = ""
				});
			}
		}

		// AJAX
		public ActionResult GetReferenceRelation(string reference, int clientId)
		{
			var clientList = ProductsRelated(reference, clientId);

			return Json(new
			{
				result = "success",
				data = string.Join(";", clientList.Select(c => c.clientId + "," + c.clientName + "," + c.price + "," + c.url + "," + c.reference).ToArray()),
				found = clientList.Count > 0
			});
		}

		private List<ClientViewModel> ProductsRelated(string reference, int clientId)
		{
			var clientProductVM = new List<ClientViewModel>();
			// busca otros productos con esa referencia
			var productsFound = _productRepository.GetbyReference(reference, Precision.Medium);

			// si encuentra alguno se lo agrega al client product del producto nuevo
			foreach (var productFound in productsFound)
			{
				// solo necesito el primer client product (con el que fue creada la referencia) para relacionar el producto nuevo con el del client product
				var clientProductFound = productFound.clients[0];
				if (clientProductFound.client.Id != clientId)
					clientProductVM.Add(new ClientViewModel
					{
						brandName = productFound.brand.name,
						clientId = clientProductFound.client.Id,
						clientName = clientProductFound.client.name,
						reference = productFound.productReference,
						specialPrice = clientProductFound.specialPrice,
						url = clientProductFound.url,
						oldPrice = clientProductFound.price,
						price = clientProductFound.price,
						productStatus = ClientServices.ItemType.Possible_Related
					});
			}
			return clientProductVM;
		}

		private List<ProductViewModel> LoadSessionProductsList(int? itemId, Catalog_Address catalogAddress)
		{
			var isEmpty = false;
			return LoadSessionProductsList(itemId, catalogAddress, out isEmpty);
		}

		private List<ProductViewModel> LoadSessionProductsList(int? itemId, Catalog_Address catalogAddress, out bool isEmpty)
		{
			var sessionResponse = new List<ProductViewModel>();
			clientIdToBeShown = catalogAddress.client.Id;
			var productList = new List<Product>();
			isEmpty = false;

			// buscamos la lista de productos en nuestra base de datos
			switch (catalogAddress.level_Id)
			{
				case 0: productList = _productRepository.GetbyDepartment(catalogAddress.catalog_Id);
					break;
				case 1: productList = _productRepository.GetbyCategory(catalogAddress.catalog_Id);
					break;
				case 2: productList = _productRepository.GetbySubCategory(catalogAddress.catalog_Id);
					break;
				case 3: productList = _productRepository.GetbyProductType(catalogAddress.catalog_Id);
					break;
			}

			// sacamos solo los del cliente que se escogio
			var clientProductsList = productList.SelectMany(prod => prod.clients).Where(client_prod => client_prod.client.Id == catalogAddress.client.Id).ToList();
			
			// buscamos la lista de productos en la direccion del catalogo 
			var clientService = new ClientServices(_clientRepository, _productRepository, _qualityRepository, _brandRepository);
			var productsOnSite = clientService.GetClientItems(catalogAddress.client, catalogAddress.level_Id, catalogAddress.catalog_Id, true);

			if (clientProductsList.Count > 0 || productsOnSite.Count > 0)
			{
				var productVMObj = new ProductViewModel();
				// se verifica que productos estan en nuestra base de datos y no estan en el sitio
				foreach (var clientProduct in clientProductsList)
				{
					// se deja solo el cliente que se escogio
					//for (int i = 0; i < product.clients.Count; i++)
					//{
					//    if (product.clients[i].client.Id == catalogAddress.client.Id)
					//    {
					//        clientIdToBeShown = catalogAddress.client.Id;
					//    }
					//}
					var productOnSite = new Product();
					var listProductsOnSite = productsOnSite.Where(onSite => onSite.clients[0].productReference.ToLower() == clientProduct.productReference.ToLower());
					if (listProductsOnSite.Count() == 0)
						listProductsOnSite = productsOnSite.Where(onSite => onSite.productReference.ToLower().Contains(clientProduct.productReference.ToLower()));
					
					if (listProductsOnSite.Count() > 1)
						productOnSite = listProductsOnSite.SingleOrDefault(onSite => onSite.clients[0].url == clientProduct.url);
					else if (listProductsOnSite.Count() == 1)
						productOnSite = listProductsOnSite.ElementAt(0);
					else
						productOnSite = null;

					if (productOnSite != null)
					{
						productVMObj = LoadProductModel(clientProduct.product);

						//if (productVMObj.productImageUrl != productOnSite.image.url)
						//    productVMObj.hasChanged = true;
						//productVMObj.productImageUrl = productOnSite.image.url;

						var clientIndex = getClientIndex(productVMObj.clientList, catalogAddress.client);
						productVMObj.clientList[clientIndex].oldPrice = clientProduct.price;

						if (productVMObj.clientList[clientIndex].price != productOnSite.clients[0].price)
							productVMObj.hasChanged = true;
						productVMObj.clientList[clientIndex].price = productOnSite.clients[0].price;

						if (productVMObj.clientList[clientIndex].specialPrice != productOnSite.clients[0].specialPrice)
							productVMObj.hasChanged = true;
						productVMObj.clientList[clientIndex].specialPrice = productOnSite.clients[0].specialPrice;

						if (productVMObj.clientList[clientIndex].url != productOnSite.clients[0].url)
							productVMObj.hasChanged = true;
						productVMObj.clientList[clientIndex].url = productOnSite.clients[0].url;

						productVMObj.clientList[clientIndex].productStatus = ClientServices.ItemType.Local;
					}
					else
					{
						productVMObj = LoadProductModel(clientProduct.product);
						var clientIndex = getClientIndex(productVMObj.clientList, catalogAddress.client);
						productVMObj.clientList[clientIndex].productStatus = ClientServices.ItemType.OnDB_NotOnSite;
					}
					if (sessionResponse.Where(prod => prod.productRef.ToLower() == clientProduct.product.productReference.ToLower()).Count() == 0)
						sessionResponse.Add(productVMObj);
				}

				// se verifica que productos estan en el sitio y no estan en nuestra base de datos
				foreach (var productOnSite in productsOnSite)
				{
					var productOnDB = clientProductsList.SingleOrDefault(onDB => onDB.productReference.ToLower() == productOnSite.productReference.ToLower());
					productVMObj = LoadProductModel(productOnSite);
					var ForceInclude = false;
					if (productOnDB == null)
					{
						var foundClientProducts = ProductsRelated(productOnSite.productReference, catalogAddress.client.Id);
						if (foundClientProducts.Count > 0)
						{
							productVMObj.clientList.AddRange(foundClientProducts);
							//ForceInclude = true;
						}
						var clientIndex = getClientIndex(productVMObj.clientList, catalogAddress.client);
						productVMObj.clientList[clientIndex].productStatus = ClientServices.ItemType.OnSite_NotOnDB;
					}

					if (sessionResponse.SelectMany(p => p.clientList).Where(cp => cp.reference.ToLower() == productOnSite.productReference.ToLower()).Count() == 0
						&& (sessionResponse.SelectMany(p => p.clientList).Where(cp => productOnSite.productReference.ToLower().Contains(cp.reference.ToLower()) && cp.reference.Length > 0).Count() == 0 || ForceInclude))
						sessionResponse.Add(productVMObj);
				}
			}
			else
			{
				var productObj = LoadEmptyProduct(ClientServices.ItemType.Local, catalogAddress.level_Id, catalogAddress.catalog_Id, catalogAddress.client);
				ViewData.Model = LoadProductModel(productObj);
				isEmpty = true;
				sessionResponse.Add(LoadProductModel(productObj));
			}
			// y se guarda la lista resultante en una sesion
			Session["CurrentProductList"] = sessionResponse;
			return sessionResponse;
		}

		private Product LoadEmptyProduct(ClientServices.ItemType type, int level_Id, int catalog_Id)
		{
			return LoadEmptyProduct(type, level_Id, catalog_Id, null);
		}

		private Product LoadEmptyProduct(ClientServices.ItemType type, int level_Id, int catalog_Id, Client client)
		{
			var productObj = new Product()
			{
				level_Id = level_Id,
				catalog_Id = catalog_Id,
				clients = new List<Client_Product>(),
				image = new Image() { url = "" },
				qualities = new List<Product_Quality>()
			};
			if (client != null)
			{
				var client_productObj = new Client_Product() { client = client, product = productObj, isActive = true };
				productObj.clients.Add(client_productObj);
			}
			return productObj;
		}

		private int getClientIndex(List<ClientViewModel> clientList, Client clientObj)
		{
			var index = 0;
			foreach (var client in clientList)
			{
				if (client.clientId == clientObj.Id)
					break;
				index++;
			}
			return index;
		}
		// Ajax
		public ActionResult GetProducts(int level, int catalog)
		{
			clientIdToBeShown = -1;
			var productList = new List<Product>();
			switch(level){
				case 0:
					productList = _productRepository.GetbyDepartment(catalog);
					break;
				case 1:
					productList = _productRepository.GetbyCategory(catalog);
					break;
				case 2:
					productList = _productRepository.GetbySubCategory(catalog);
					break;
				case 3:
					productList = _productRepository.GetbyProductType(catalog);
					break;
			}
			var productVMList = productList.Select(prod => LoadProductModel(prod)).OrderByDescending(prod => prod.clientList.Where(client => client.isActive).Count() > 0);
			Session["CurrentProductList"] = productVMList.ToList();

			var result = "success";
			if (productVMList.Count() > 0)
				ViewData.Model = productVMList.First();
			else
			{
				ViewData.Model = LoadProductModel(LoadEmptyProduct(ClientServices.ItemType.Local, level, catalog));
				result = "empty";
			}

			return Json(new
			{
				result = result,
				total = productVMList.Count(),
				client = 0,
				html = this.RenderViewToString("PartialViews/ProductsForm", ViewData)
			});

		}
		// Ajax
		public ActionResult GetCatalogClientList(int clientId, string type, string isShortVersion)
		{
			var clientObj = _clientRepository.Get(clientId);
			try
			{
				switch (type)
				{ 
					case "Local":
						return GetCatalogClientList(clientObj, ClientServices.ItemType.Local);
					case "OnDB":
						return GetCatalogClientList(clientObj, ClientServices.ItemType.OnDB_NotOnSite);
					default:
						return GetCatalogClientList(clientObj, ClientServices.ItemType.OnSite_NotOnDB);
				}
				
			}
			catch (Exception e)
			{
				return Json(new
				{
					result = "fail",
					msg = clientObj.name + " (" + clientObj.url + ") <br/>" + e.Message,
					html = ""
				});
			}
		}

		// Ajax
		public ActionResult GetCatalogList(string type, int id, bool loadNext)
		{
			// si id es igual a 0 trae todo los items del type
			var htmlResponse = "";
			var isEmpty = false;
			if (id == 0 && type == "Department")
			{
				var departmentItems = _departmentRepository.GetAll();
				var model = departmentItems.Select(dep => new DepartmentViewModel() { departmentId = dep.Id, departmentTitle = dep.name });
				ViewData.Model = model.ToList();
				htmlResponse = this.RenderViewToString("Catalogs/DepartmentForm", ViewData);
				isEmpty = model.Count() == 0;    
			}
			if ((loadNext && type == "Department") || (!loadNext && type == "Category"))
			{
				var categoryItems = _categoryRepository.GetByDepartament(_departmentRepository.Get(id));
				var model = categoryItems.Select(cat => new CategoryViewModel() { categoryId = cat.Id, categoryTitle = cat.name });
				ViewData.Model = model.ToList();
				htmlResponse = this.RenderViewToString("Catalogs/CategoryForm", ViewData);
				isEmpty = model.Count() == 0;
			}
			else if ((loadNext && type == "Category") || (!loadNext && type == "SubCategory"))
			{
				var subCategoryItems = _subCategoryRepository.GetByCategory(_categoryRepository.Get(id));
				var model = subCategoryItems.Select(subcat => new SubCategoryViewModel() { subCategoryId = subcat.Id, subCategoryTitle = subcat.name });
				ViewData.Model = model.ToList();
				htmlResponse = this.RenderViewToString("Catalogs/SubCategoryForm", ViewData);
				isEmpty = model.Count() == 0;
			}
			else if ((loadNext && type == "SubCategory") || (!loadNext && type == "ProductType"))
			{
				var productTypeItems = _productTypeRepository.GetBySubCategory(_subCategoryRepository.Get(id));
				var model = productTypeItems.Select(prodtype => new ProductTypeViewModel() { productTypeId = prodtype.Id, productTypeTitle = prodtype.name });
				ViewData.Model = model.ToList();
				htmlResponse = this.RenderViewToString("Catalogs/ProductTypeForm", ViewData);
				isEmpty = model.Count() == 0;
			}
			
			return Json(new
			{
				result = "success",
				html = htmlResponse,
				empty = isEmpty.ToString()
			});
		}

		// Ajax
		public ActionResult AddCatalogItem(string type, int parentId, string value)
		{
			try
			{
				if (type == "Department")
				{
					_departmentRepository.SaveOrUpdate(new Department() { name = value });
				}
				else if (type == "Category")
				{
					_categoryRepository.SaveOrUpdate(new Category() { name = value, department = _departmentRepository.Get(parentId) });
				}
				else if (type == "SubCategory")
				{
					_subCategoryRepository.SaveOrUpdate(new SubCategory() { name = value, category = _categoryRepository.Get(parentId) });
				}
				else
				{
					_productTypeRepository.SaveOrUpdate(new ProductType() { name = value, subCategory = _subCategoryRepository.Get(parentId) });
				}

				return Json(new
				{
					result = "success"
				});
			}
			catch (Exception e)
			{
				return Json(new
				{
					result = "fail",
					msg = e.Message
				});
			}
		}

		// Ajax
		public ActionResult DeleteCatalogItem(string type, int id)
		{
			try
			{
				if (type == "Department") {
					_departmentRepository.DeleteDepartmentItem(id);
				}
				else if (type == "Category")
				{
					_categoryRepository.DeleteCategoryItem(id);
				}
				else if (type == "SubCategory")
				{
					_subCategoryRepository.DeleteSubCategoryItem(id);
				}
				else
				{
					_productTypeRepository.DeleteProductTypeItem(id);
				}

				return Json(new
				{
					result = "success"
				});
			}
			catch (Exception e)
			{

				return Json(new
				{
					result = "fail",
					msg = e.Message
				});
			}
		}

		// Ajax
		public ActionResult FindQualityName(string v)
		{
			var quality = _qualityRepository.getByName(v);

			if (quality != null)
				return Json(new
				{
					qId = quality.Id,
					qName = quality.name
				});

			return Json(new
			{
				qId = 0,
				qName = v.Substring(0, 1).ToUpper() + v.Substring(1, v.Length - 1)
			});
		}

		#region CATALOGO

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

		public JsonResult FindCategoryByDepartmentId(int id)
		{
			return getJsonResult("una categoria", _categoryRepository.GetByDepartament(_departmentRepository.Get(id)).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.name }));
		}

		public JsonResult FindSubCategoryByCategoryId(int id)
		{
			return getJsonResult("una subcategoria", _subCategoryRepository.GetByCategory(_categoryRepository.Get(id)).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.name }));
		}

		public JsonResult FindProductTypeBySubCategoryId(int id)
		{
			return getJsonResult("un tipo de producto", _productTypeRepository.GetBySubCategory(_subCategoryRepository.Get(id)).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.name }));
		}

		#endregion
		#endregion

		#region Metodos

		private bool ChangeClientProduct(Client_Product clientProductObj, ClientViewModel client)
		{
			if (clientProductObj.price != client.price)
				return true;
			if (clientProductObj.specialPrice != client.specialPrice)
				return true;
			if (clientProductObj.productReference != client.reference)
				return true;
			if (clientProductObj.name != client.productName)
				return true;
			if (clientProductObj.url != client.url)
				return true;

			return false;
		}

		private ActionResult GetAllClientViewModel(bool forProducts)
		{
			ClientServices cService = new ClientServices();
			return View(_clientRepository.GetAll().Select(client => new ClientViewModel() { clientId = client.Id, clientName = client.name, domain = client.url, isActive = (forProducts ? cService.ClientHasProduct(client) : cService.ClientHasCatalog(client)) }).OrderByDescending(client => client.isActive).OrderBy(client => client.productName).ToList());
		}

		private ActionResult GetCatalogClientList(Client client, ClientServices.ItemType itemsType)
		{
			var defaultLevelId = 0;
			var defaultCatalogId = 1;
			if (itemsType == ClientServices.ItemType.Local)
			{
				ViewData.Model = _catalogAddressRepository.GetByClient(client).Select(item => new CatalogItemViewModel() {
					dbId = item.Id,
					url = item.url.UrlAddressFix(),
					clientId = client.Id,
					clientAddress = client.url,
					name = CatalogHelper.CatalogToString(false, CatalogHelper.FromLevelsToCatalog(item.level_Id, item.catalog_Id), false),
					catalogId = item.catalog_Id,
					levelId = item.level_Id,
					type = itemsType.ToString(),
					products = client.products.Where(client_product => client_product.product.catalog_Id == item.catalog_Id && client_product.product.level_Id == item.level_Id)
					.Select(cp => LoadProductModel(cp.product)).ToList()
				}).OrderBy(item => item.name).ToList();
			}
			else
			{
				ViewData.Model = new ClientServices(_clientRepository, _productRepository, _qualityRepository, _brandRepository)
					.GetMenuAddresses(client, itemsType)
					.Select(x => new CatalogItemViewModel()
					{
						dbId = x.id,
						url = x.address.UrlAddressFix(),
						clientId = client.Id,
						clientAddress = client.url,
						name = string.IsNullOrEmpty(x.name) ? CatalogHelper.CatalogToString(false, CatalogHelper.FromLevelsToCatalog(x.levelId, x.catalodId), false) : x.name,
						catalogId = defaultCatalogId,
						levelId = defaultLevelId,
						type = itemsType.ToString()
					}).OrderBy(item => item.name).ToList();
			}
			return Json(new
			{
				result = "success",
				msg = client.name + " (" + client.url + ")",
				html = this.RenderViewToString("PartialViews/CatalogForm", ViewData)
			});
		}

		private ProductViewModel LoadProductModel()
		{
			return LoadProductModel(null);
		}

		public ActionResult UpdateProduct(string param)
		{
			var p = new ProductViewModel();
			if (string.IsNullOrEmpty(param))
				p = LoadProductModel();
			else
			{
				p = LoadProductModel(_productRepository.GetbyReference(param));
			}

			return View(p);
		}

		private ProductViewModel LoadProductModel(Product pObj)
		{
			return LoadProductModel(pObj, new Dictionary<int, ClientServices.ItemType>());
		}

		private ProductViewModel LoadProductModel(Product pObj, Dictionary<int, Explora_Precios.ApplicationServices.ClientServices.ItemType> type)
		{
			var p = new ProductViewModel();
			var levelId = 0;
			var catalogId = 1;
			var clientProductList = new List<Client_Product>();
			if (pObj != null)
			{
				levelId = pObj.level_Id;
				catalogId = pObj.catalog_Id;
				clientProductList = pObj.clients == null ? new List<Client_Product>() : pObj.clients.ToList();
			}
			else
			{
				pObj = new Product();
				pObj.description = "";
				pObj.image = new Image();
				pObj.name = "";
				pObj.productReference = "";
				pObj.qualities = new List<Product_Quality>();
				pObj.clients = new List<Client_Product>();
			}
			p.catalogProduct = CatalogHelper.FromLevelsToCatalog(levelId, catalogId);
			p.catalogLevel = levelId;
			p.catalogId = catalogId;
			p.clientList = pObj.clients.Select(x => new ClientViewModel()
			{
				clientId = x.client.Id,
				clientName = x.client.name,
				price = x.price,
				isActive = x.isActive,
				specialPrice = x.specialPrice,
				url = x.url,
				productStatus = type.Count > 0 ? type.SingleOrDefault(status => status.Key == x.client.Id).Value : Explora_Precios.ApplicationServices.ClientServices.ItemType.Local,
				showMe = clientIdToBeShown > -1 ? clientIdToBeShown == x.client.Id : true,
				page = x.page, 
				reference = x.productReference,
				masterReference = pObj.productReference
			}).ToList();
				
			//    _clientRepository.GetAll().Select(x => new ClientViewModel()
			//{
			//    clientId = x.Id,
			//    clientName = x.name,
			//    price = clientProductList.SingleOrDefault(y => y.client.Id == x.Id) != null ? clientProductList.SingleOrDefault(y => y.client.Id == x.Id).price : 0,
			//    url = clientProductList.SingleOrDefault(cProduct => cProduct.client == x) != null ? clientProductList.SingleOrDefault(cProduct => cProduct.client == x).url : "",
			//    isActive = clientProductList.SingleOrDefault(y => y.client.Id == x.Id) != null ? clientProductList.SingleOrDefault(y => y.client.Id == x.Id).isActive : true,
			//    specialPrice = clientProductList.SingleOrDefault(y => y.client.Id == x.Id) != null ? clientProductList.SingleOrDefault(y => y.client.Id == x.Id).specialPrice : 0
			//}).ToList();

			pObj.brand = pObj.brand ?? new Brand();
			p.productBrand = pObj.brand.name;
			p.productBrandId = pObj.brand.Id;
			p.productDescription = pObj.description;
			p.productId = pObj.Id;
			p.productImageUrl = pObj.image.url;
			p.productName = pObj.name;
			p.productRef = pObj.productReference;
			p.qualities = pObj.qualities.Select(x => new QualityViewModel()
			{
				active = true,
				name = x.quality.name,
				value = x.value,
				Id = x.quality.Id
			}).ToList();
			return p;
		}

		private byte[] GetImage(string url)
		{

			var imageStream = CommonUtilities.GetImageFromUrl(url);

			byte[] buffer = new byte[1024];
			MemoryStream memStream = new MemoryStream();
			while (true)
			{
				int bytesRead = imageStream.Read(buffer, 0, buffer.Length);
				memStream.Write(buffer, 0, bytesRead);
				if (bytesRead == 0)
					break;
			}
			var response = memStream.ToArray();
			imageStream.Close();
			memStream.Close();
			return response;
		}

		private Quality GetQuality(QualityViewModel x)
		{

			var qualityResponse = _qualityRepository.getByName(x.name.Trim());
			if (qualityResponse == null)
				return _qualityRepository.SaveOrUpdate(new Quality() { name = x.name.Trim() });

			return qualityResponse;
		}

		private Client_Product GetClient(Product p, ClientViewModel x)
		{
			var client = _clientRepository.Get(x.clientId);
			var clientProductResponse = new Client_Product();

			clientProductResponse = new Client_Product()
			{
				name = "",
				client = client,
				price = x.price,
				url = x.url,
				isActive = x.isActive,
				product = p,
				dateCreated = DateTime.Now,
				dateModified = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue,
				dateReported = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue
			};

			if (x.specialPrice > 0)
				clientProductResponse.specialPrice = x.specialPrice;

			//_clientProductRepository.SaveOrUpdate(clientProductResponse);

			return clientProductResponse;
		}
		#endregion

	}
}
