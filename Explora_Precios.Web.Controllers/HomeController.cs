﻿using System.Web.Mvc;
using System;
using Explora_Precios.Data;
using SharpArch.Data;
using System.Linq;
using Explora_Precios.Data.NHibernateMaps;
using System.IO;
using Explora_Precios.Web.Controllers.ViewModels;
using Explora_Precios.Core.DataInterfaces;
using Explora_Precios.Core;
using Explora_Precios.Core.Helper;
using System.Collections.Generic;
using Explora_Precios.Web.Controllers.Helpers;
using Explora_Precios.ApplicationServices;
using System.Drawing.Imaging;
using System.Web;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Explora_Precios.Web.Controllers
{
	[HandleError]
	public class HomeController : PrimaryController
	{
		IProductTypeRepository	 _productTypeRepository;
		ISubCategoryRepository _subCatRepository;
		ICategoryRepository _catRepository;
		IProductRepository _productRepository;
		IDepartmentRepository _departmentRepository;
		IClient_ProductRepository _cpRepository;
		IUserRepository _uRepository;
		IUser_ProductRepository _uProductRepository;
		IGroup_UserRepository _groupRepository;
		private int currentPage = 0;
		private const string RegisterPage = @"<div style=""background-color:#F6F5EA;"">Para poder {0} debes<br />registrarte a <b>ExploraPrecios.com</b>.<br />Que esperas suscribete ya!</div>
						<div class='loginbox' style=""color:#D84018;""><input type='hidden' id='Redirect' value='{1}' /><a class='register' style=""color:#D84018;"" href='javascript:void(0)'>Registrarse</a> / <a class='login' style=""color:#D84018;"" href='javascript:void(0)'>Entrar</a></div>";

		public HomeController(ICategoryRepository catRepository, 
			IProductRepository productRepository, 
			ISubCategoryRepository subCatRepository, 
			IProductTypeRepository productTypeRepository, 
			IDepartmentRepository departmentRepository, 
			IClient_ProductRepository cpRepository,
			IUserRepository uRepository,
			IUser_ProductRepository uProductRepository,
			IGroup_UserRepository groupRepository)
		{
			_catRepository = catRepository;
			_productRepository = productRepository;
			_subCatRepository = subCatRepository;
			_productTypeRepository = productTypeRepository;
			_departmentRepository = departmentRepository;
			_cpRepository = cpRepository;
			_uRepository = uRepository;
			_uProductRepository = uProductRepository;
			_groupRepository = groupRepository;
		}

		public ActionResult Offline()	
		{ 
			return View();
		}

		public ActionResult Index()
		{
			return View("Offline");
			/*ViewData["IsMobile"] = false.ToString();
			string u = Request.ServerVariables["HTTP_USER_AGENT"];
			Regex b = new Regex(@"android.+mobile|avantgo|bada\\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\\/|plucker|pocket|psp|symbian|treo|up\\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\\-(n|u)|c55\\/|capi|ccwa|cdm\\-|cell|chtm|cldc|cmd\\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\\-s|devi|dica|dmob|do(c|p)o|ds(12|\\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\\-|_)|g1 u|g560|gene|gf\\-5|g\\-mo|go(\\.w|od)|gr(ad|un)|haie|hcit|hd\\-(m|p|t)|hei\\-|hi(pt|ta)|hp( i|ip)|hs\\-c|ht(c(\\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\\-(20|go|ma)|i230|iac( |\\-|\\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\\/)|klon|kpt |kwc\\-|kyo(c|k)|le(no|xi)|lg( g|\\/(k|l|u)|50|54|e\\-|e\\/|\\-[a-w])|libw|lynx|m1\\-w|m3ga|m50\\/|ma(te|ui|xo)|mc(01|21|ca)|m\\-cr|me(di|rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\\-2|po(ck|rt|se)|prox|psio|pt\\-g|qa\\-a|qc(07|12|21|32|60|\\-[2-7]|i\\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\\-|oo|p\\-)|sdk\\/|se(c(\\-|0|1)|47|mc|nd|ri)|sgh\\-|shar|sie(\\-|m)|sk\\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\\-|v\\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\\-|tdg\\-|tel(i|m)|tim\\-|t\\-mo|to(pl|sh)|ts(70|m\\-|m3|m5)|tx\\-9|up(\\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|xda(\\-|2|g)|yas\\-|your|zeto|zte\\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			if ((b.IsMatch(u) || v.IsMatch(u.Substring(0, 4))))
			{
				ViewData["IsMobile"] = true.ToString();
			}

			var goTo = System.Configuration.ConfigurationManager.AppSettings["OnLoadGoTo"];
			if (!string.IsNullOrEmpty(goTo) && goTo != "main")
			{
				if (goTo == "contact") Response.Redirect("/Contact"); else if (goTo == "manager") Response.Redirect("/Manager");
			}
			var RenderMobile = Request.QueryString["status"];
			var RenderCookie = this.ControllerContext.HttpContext.Request.Cookies["RenderMobile"];
			if (RenderCookie == null)
			{
				HttpCookie cookie = new HttpCookie("RenderMobile");
				cookie.Value = RenderMobile ?? "normal";

				this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
			}
			else if (string.IsNullOrEmpty(RenderMobile))
			{
				RenderMobile = RenderCookie.Value;
			}

			if (!(RenderMobile ?? "normal").Contains("normal"))
				Response.Redirect("http://mobile.exploraprecios.com");

			var IntroModel = new IntroViewModel();

			//Productos de las lista de destacados, en ofertas y nuevos
			if (System.Web.HttpRuntime.Cache.Get("HighlightProducts") == null ||
				System.Web.HttpRuntime.Cache.Get("OfferProducts") == null ||
				System.Web.HttpRuntime.Cache.Get("NewProducts") == null ||
				System.Web.HttpRuntime.Cache.Get("GroupedProducts") == null)
			{
				var RawLists = LoadBannerLists();

				// Rellenamos el modelo con bannerproduct models vacios
				IntroModel.HighlightProducts = LoadBannerProductModelList(RawLists[0]); // Highlighted
				IntroModel.OfferProducts = LoadBannerProductModelList(RawLists[1]); // Offers
				IntroModel.NewProducts = LoadBannerProductModelList(RawLists[2]); // News
				// Llenar el listado de grupos se hace aparte por los datos desplegados, no es igual a los otros banners
				var GroupedProductsVM = LoadGroupedBannerList();
				IntroModel.GroupedProducts = GroupedProductsVM;
			}
			else
			{
				// Rellenamos el modelo con bannerproduct models vacios
				IntroModel.HighlightProducts = LoadBannerProductModelList((IEnumerable<Client_Product>)System.Web.HttpRuntime.Cache.Get("HighlightProducts"));
				IntroModel.OfferProducts = LoadBannerProductModelList((IEnumerable<Client_Product>)System.Web.HttpRuntime.Cache.Get("OfferProducts"));
				IntroModel.NewProducts = LoadBannerProductModelList((IEnumerable<Client_Product>)System.Web.HttpRuntime.Cache.Get("NewProducts"));
				IntroModel.GroupedProducts = LoadGroupedBannerList((IEnumerable<Group_User>)System.Web.HttpRuntime.Cache.Get("GroupedProducts"));
			}
			return View(IntroModel);*/
		}

		public void SendIt(string browser, string where)
		{
			var email = new EmailServices("info@exploraprecios.com", "Error en: " + where, "Browser: " + browser);
			email.Send();
		}

		private IEnumerable<GroupViewModel> LoadGroupedBannerList(IEnumerable<Group_User> GroupUserList)
		{
			var productCount = 5;
			var productList = GroupUserList.Take(productCount);
			// Actualiza conteo de productos
			UpdateCounterFile(productList.Select(cp => cp.product.Id.ToString() + "," + ((float)(1 / productCount)).TwoDecimals() + "," + DateTime.Now.ToShortDateString()).ToArray(), CounterType.product);
			return GroupUserList.Take(5).Select(group => LoadGroupViewModel(group, true)).Concat(GroupUserList.Skip(5).Select(group => LoadGroupViewModel(group, false)));
		}

		private IEnumerable<GroupViewModel> LoadGroupedBannerList()
		{
			var RawGroupedProducts = _groupRepository.GetLatest();

			System.Web.HttpRuntime.Cache.Insert("GroupedProducts", RawGroupedProducts,
				null,
				DateTime.Now.AddHours(2),
				System.Web.Caching.Cache.NoSlidingExpiration);

			return LoadGroupedBannerList(RawGroupedProducts);
		}

		private GroupViewModel LoadGroupViewModel(Group_User group, bool loadImage)
		{
			return new GroupViewModel
			{
				CreatedDate = group.created,
				ProductId = group.product.Id.CryptProductId(),
				ProductName = group.product.name,
				Image = loadImage ? group.product.image.imageObj : null,
				GroupSize = group.product.groups.Count()
			};
		}

		private List<IEnumerable<Client_Product>> LoadBannerLists()
		{
			var lists = new List<IEnumerable<Client_Product>>();
			var offerProducts = _cpRepository.GetProductsOnSale().RandomizeList(); // sacamos al azar los productos en oferta
			var clientIds = offerProducts.Select(cp => cp.client.Id).Distinct(); // sacamos los clientes que tienen productos en oferta
			var highlightedProducts = new List<Client_Product>();
			foreach (var clientId in clientIds) // tomamos maximo 6 productos de cada cliente y lo ponemos en la lista de destacados
			{
				highlightedProducts.AddRange(offerProducts.Where(cp => cp.client.Id == clientId).Take(6));
			}
			if (highlightedProducts.Count < 10)
			{
				highlightedProducts.AddRange(_cpRepository.GetLastUpdated());
			}
			var newProducts = _cpRepository.GetLastAdded(); // sacamos los nuevos productos

			// se generan los objetos de banner, se revuelven y se guardan una variable
			var Raw_HighlightProducts = highlightedProducts.RandomizeList();
			var Raw_OfferProducts = offerProducts.RandomizeList();

			// se guardan en memoria en ese orden
			System.Web.HttpRuntime.Cache.Insert("HighlightProducts", Raw_HighlightProducts,
				null,
				DateTime.Now.AddHours(2),
				System.Web.Caching.Cache.NoSlidingExpiration);
			System.Web.HttpRuntime.Cache.Insert("OfferProducts", Raw_OfferProducts,
			 null,
			 DateTime.Now.AddHours(2),
			 System.Web.Caching.Cache.NoSlidingExpiration);
			System.Web.HttpRuntime.Cache.Insert("NewProducts", newProducts,
				null,
				DateTime.Now.AddHours(2),
				System.Web.Caching.Cache.NoSlidingExpiration);

			lists.Add(Raw_HighlightProducts);
			lists.Add(Raw_OfferProducts);
			lists.Add(newProducts);
			return lists;
		}

		private IEnumerable<BannerProduct> LoadBannerProductModelList(IEnumerable<Client_Product> cpList)
		{
			var productCount = 5;
			var productList = cpList.Take(productCount);
			// Actualiza conteo de productos
			UpdateCounterFile(productList.Select(cp => cp.product.Id.ToString() + "," + (1 / (float)productCount).TwoDecimals() + "," + DateTime.Now.ToShortDateString()).ToArray(), CounterType.product);
			return productList.Select(cp => LoadBannerProduct(cp, true)).Concat(cpList.Skip(5).Select(cp => LoadBannerProduct(cp, false)));
		}

		private BannerProduct LoadBannerProduct(Client_Product cp, bool loadImage)
		{
			byte[] image = { };
			try
			{
				image = loadImage && cp.product.image != null ? cp.product.image.imageObj : null;
			}
			catch (NHibernate.LazyInitializationException)
			{
				var product = _productRepository.Get(cp.product.Id);
				image = product.image.imageObj;
			}

			return new BannerProduct()
			{
				ProductId = cp.product.Id,
				Name = cp.product.name,
				Client = cp.client.name,
				Image = image,
				ClientId = cp.client.Id,
				Price = cp.specialPrice > 0 ? cp.specialPrice : cp.price
			};
		}

		public ActionResult PageBanner(int toPage, string banner)
		{
			var pageSize = 5;
			var lists = new List<IEnumerable<Client_Product>>();
			var width = 120;
			var height = 80;
			IEnumerable<Group_User> grouped;
			if (System.Web.HttpRuntime.Cache.Get("HighlightProducts") == null ||
				System.Web.HttpRuntime.Cache.Get("OfferProducts") == null ||
				System.Web.HttpRuntime.Cache.Get("NewProducts") == null ||
				System.Web.HttpRuntime.Cache.Get("GroupedProducts") == null)
			{
				lists = LoadBannerLists();
				grouped = _groupRepository.GetLatest();
			}
			else
			{
				lists.Add((IEnumerable<Client_Product>)System.Web.HttpRuntime.Cache.Get("HighlightProducts"));
				lists.Add((IEnumerable<Client_Product>)System.Web.HttpRuntime.Cache.Get("OfferProducts"));
				lists.Add((IEnumerable<Client_Product>)System.Web.HttpRuntime.Cache.Get("NewProducts"));
				grouped = (IEnumerable<Group_User>)System.Web.HttpRuntime.Cache.Get("GroupedProducts");
			}

			var guids = new List<string>();
			IEnumerable<int> ids;
			var listCount = 0;
			switch (banner)
			{
				case "Highlighted":
					listCount = lists[0].Count();
					ids = lists[0].Select(cp => cp.product.Id);
					width = 90;
					height = 75;
					break;
				case "Offers":
					listCount = lists[1].Count();
					ids = lists[1].Select(cp => cp.product.Id);
					break;
				case "News":
					listCount = lists[2].Count();
					ids = lists[2].Select(cp => cp.product.Id);
					break;
				default:
					listCount = grouped.Count();
					ids = grouped.Select(g => g.product.Id);
					break;
			}
			var pageItemsCount = toPage * pageSize < listCount ? pageSize : pageSize - ((toPage * pageSize) - listCount);
			// Actualiza conteo de productos
			UpdateCounterFile(ids.Take(pageItemsCount).Select(id => id.ToString() + "," + (1 / (float)pageItemsCount).TwoDecimals() + "," + DateTime.Now.ToShortDateString()).ToArray(), CounterType.product);
			for (int i = 0; i < pageItemsCount; i++)
			{
				var image = _productRepository.Get(ids.ElementAt(i + ((toPage - 1) * pageSize))).image.imageObj;
				var sizes = image.FitImage(width, height);
				guids.Add(CommonUtilities.CacheImage(image) + "," + sizes[0] + "," + sizes[1]);
			}



			return Json(new { guids = string.Join(";", guids.ToArray()), count = pageItemsCount });
		}

		public ActionResult Products(int? catLev, int? id)
		{
			// Get all the catalog items under the selected depatment. Default Electronicos = 1
			catLev = catLev ?? 0;
			id = id ?? 1;
			var homeModel = LoadHomeModel((int)catLev, (int)id);
			homeModel.LoadFilters((int)catLev, (int)id);
			ViewData["depId"] = homeModel.departmentId;
			return View(homeModel);
		}

		public ActionResult AccessLink(int cpId, string url)
		{
			UpdateCounterFile(cpId.ToString() + ",1," + DateTime.Now.ToShortDateString(), CounterType.client);
			return Redirect(url);
		}

		public ActionResult LoadMainMenu(int depId)
		{
			var newProductsCount = 0;
			if (Session["NewProducts"] == null)
			{
				var newProducts = Session["NewProducts"] = _cpRepository.GetLastAdded();
				newProductsCount = ((IList<Client_Product>)newProducts).Count;
			}
			else
				newProductsCount = ((IList<Client_Product>)Session["NewProducts"]).Count;

			var mainMenuModel = new MainMenuModel { DepId = depId, DepList = new DepartmentRepository().GetAll().OrderBy(d => d.index_order), DisplayNewProducts = depId >= 0 && newProductsCount > 0 };
			ViewData.Model = mainMenuModel;
			return Json(new { 
				html = this.RenderViewToString("PartialViews/MainMenu",ViewData)
			});
		}

		public ActionResult LoadTickerList()
		{
			// Get Ticker Items
			var result = new List<string>();

			var SubCats = ConfigurationManager.AppSettings["TickerSubCategories"].Split(',').RandomizeList<string>();

			if (System.Web.HttpRuntime.Cache.Get("SubCategorySpinnerList") == null)
			{
				foreach (var subCat in SubCats)
				{
					var subCatObj = _subCatRepository.Get(int.Parse(subCat)).FromLevelsToCatalog();
					var productList = _productRepository.GetbySubCategory(int.Parse(subCat)).Select(product => product.clients.OrderBy(client => client.price).First());
					if (productList.Count() > 0)
					{
						var price = productList.OrderBy(product => product.price).First().price;
						var li = "<li><span>" + subCatObj.subCategoryName + "</span><a href=\"/Home/Products/?catlev=2&id=" + subCat + "\" title=\"" + subCatObj.departmentName + " - " + subCatObj.categoryName + " - " + subCatObj.subCategoryName + "\" >desde $" + String.Format("{0:0.00}", price) + "</a></li>";
						result.Add(li);
					}
				}
				System.Web.HttpRuntime.Cache.Insert("SubCategorySpinnerList", result,
					null,
					DateTime.Now.AddHours(23),
					System.Web.Caching.Cache.NoSlidingExpiration);
			}
			else
			{
				result = (List<string>)System.Web.HttpRuntime.Cache.Get("SubCategorySpinnerList");
			}

			var ProdutTypes = ConfigurationManager.AppSettings["TickerProductTypes"].Split(',').RandomizeList<string>();

			foreach (var prodType in ProdutTypes)
			{
				var prodTypeObj = _productTypeRepository.Get(int.Parse(prodType)).FromLevelsToCatalog();
				var productList = _productRepository.GetbyProductType(int.Parse(prodType)).Select(product => product.clients.OrderBy(client => client.price).First());
				if (productList.Count() > 0)
				{
					var price = productList.OrderBy(product => product.price).First().price;
					var li = "<li><span>" + prodTypeObj.subCategoryName + "</span><a href=\"/Home/Products/?catlev=2&id=" + prodType + "\" title=\"" + prodTypeObj.departmentName + " - " + prodTypeObj.categoryName + " - " + prodTypeObj.subCategoryName + " - " + prodTypeObj.productTypeName + "\" >desde $" + String.Format("{0:0.00}", price) + "</a></li>";
					result.Add(li);
				}
			}

			ViewData.Model = result;
			return Json(new { html = this.RenderViewToString("PartialViews/ProductTicker", ViewData) });
		}

		public ActionResult ScrollProducts(int catLev, int id, int page)
		{
			currentPage = page;
			var homeModel = LoadHomeModel(catLev, id);
			ViewData.Model = homeModel.productsListViewModel;
			return Json(new
			{
				html = this.RenderViewToString("PartialViews/ProductsList", ViewData),
				hasNext = ViewData.Model = homeModel.productsListViewModel.products.HasNextPage
			});
		}

		public ActionResult ScrollFilter(int? catLev, int? id, string s, int page, string p, string b, bool o)
		{
			var defaultPageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["DefaultPageSize"]);
			currentPage = page;
			var HomeModel = s != null ? LoadHomeModel(s) : LoadHomeModel(catLev.Value, id.Value);
			var products = FilterProduct(HomeModel.allProducts, p, b, o);
			HomeModel.productsListViewModel.products = new PagedList<Explora_Precios.Core.Product>(products.OrderBy(product => product.clients.Select(c => c.price).First()), currentPage, defaultPageSize);

			var productVM = new ProductViewModel(_productTypeRepository, _subCatRepository, _catRepository, _departmentRepository);
			HomeModel.productsListViewModel.productsList = HomeModel.productsListViewModel.products.Select(product => productVM.LoadModel(product, false));
			ViewData.Model = HomeModel.productsListViewModel;
			
			return Json(new
			{
				html = this.RenderViewToString("PartialViews/ProductsList", ViewData),
				hasNext = ViewData.Model = HomeModel.productsListViewModel.products.HasNextPage
			});
		}

		public ActionResult Filter(string p, string b, bool o, string s, int? cl, int? ci)
		{
			var HomeModel = s != null ? LoadHomeModel(s) : LoadHomeModel(cl.Value, ci.Value);
			HomeModel.isFilter = true;
			var defaultPageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["DefaultPageSize"]);
			IEnumerable<Product> products = FilterProduct(HomeModel.allProducts, p, b, o);
			var OrderedProducts = products.OrderBy(product => product.clients.Select(c => c.price).First());

			if (p != null)
			{
				var rangePrices = p.Split(',');
				HomeModel.Filter.CurrentMinPrice = float.Parse(rangePrices[0]);
				HomeModel.Filter.CurrentMaxPrice = float.Parse(rangePrices[1]);
			}
			else if (products.Count() > 1)
			{
				HomeModel.Filter.CurrentMaxPrice = OrderedProducts.Last().clients.First().price;
			}
			if (b != null)
			{
				HomeModel.Filter.CurrentBrand = b;
			}
			HomeModel.Filter.FilterType = (p != null && b != null) ? FilterViewModel.ItemFilterTypes.PriceBrand :
									(o && b != null) ? FilterViewModel.ItemFilterTypes.SaleBrand :
									(o) ? FilterViewModel.ItemFilterTypes.Sale :
									(p != null) ? FilterViewModel.ItemFilterTypes.Price :
									(b != null) ? FilterViewModel.ItemFilterTypes.Brand :
									FilterViewModel.ItemFilterTypes.None;

			HomeModel.productsListViewModel.products = new PagedList<Explora_Precios.Core.Product>(OrderedProducts, currentPage, defaultPageSize);
			HomeModel.allProducts = products.ToList();
			var productVM = new ProductViewModel(_productTypeRepository, _subCatRepository, _catRepository, _departmentRepository);
			HomeModel.productsListViewModel.productsList = HomeModel.productsListViewModel.products.Select(product => productVM.LoadModel(product, true));
			if (s != null)
				HomeModel.LoadFilters(search: s);
			else
				HomeModel.LoadFilters(cl.Value, ci.Value);
			ViewData["depId"] = HomeModel.departmentId;
			return View("Products", HomeModel);
		}

		public ActionResult GetPromoVideo()
		{
			ViewData.Model = HttpContext.User.Identity.Name;
			return Json(new { html = this.RenderViewToString("PartialViews/PromoVideo", ViewData) });
		}

		public ActionResult GenerateBannerProduct(string side, int position, int id)
		{
			var offerProduct = _cpRepository.Get(id);

			ViewData.Model = new ClientViewModel()
			{
				brandName = offerProduct.product.brand.name,
				clientName = offerProduct.client.name,
				image = offerProduct.product.image.imageObj,
				price = offerProduct.price,
				specialPrice = offerProduct.specialPrice,
				productName = offerProduct.product.name,
				productId = offerProduct.product.Id
			};
			ViewData["LeftRight"] = side;
			ViewData["Position"] = position;
			return Json(new
			{
				html = this.RenderViewToString("PartialViews/BannerProduct", ViewData)
			});
		}

		public ActionResult ViewProduct(string id)
		{
			var productVM = new ProductViewModel(_productTypeRepository, _subCatRepository, _catRepository, _departmentRepository);
			System.Web.Security.MembershipCreateStatus status = System.Web.Security.MembershipCreateStatus.Success;
			var membership = new NHCustomProviders.NHCustomMembershipProvider();
			membership.Initialize("NHCustomMembershipProvider", (System.Collections.Specialized.NameValueCollection)System.Configuration.ConfigurationManager.GetSection("NHCustomMembershipProvider"));
			var user = membership.CreateUser("prueba", "test", "info@exploraprecios.com", "fonos?", "fonos en ingles", true, null, out status);
			return View(productVM.LoadModel(_productRepository.Get(int.Parse(id.DecryptString())), false));
		}

		// AJAX
		public JsonResult GetFloatingProducts(int? page, bool? ie8)
		{
			var productVM = new ProductViewModel(_productTypeRepository, _subCatRepository, _catRepository, _departmentRepository);
			var pageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["FloatingPageSize"]);
			var allProducts =  _cpRepository.GetLastAdded().Select(cp => cp.product).ToList();

			var pagedList = new PagedList<Explora_Precios.Core.Product>(allProducts, page.HasValue ? page.Value - 1 : 0, pageSize);
			ViewData.Model = new ProductsListViewModel()
			{
				productsList = pagedList.Select(p => productVM.LoadModel(p, true)),
				products = pagedList
			};
			return Json(new
			{
				html = this.RenderViewToString(ie8.HasValue && ie8.Value ? "PartialViews/ProductsFloatingIE8" : "PartialViews/ProductsList", ViewData)
			});
		}

		// AJAX
		public ActionResult GetBannerProducts()
		{
			var offerProducts = (IList<Client_Product>)Session["DisplayOffers"];
			if (offerProducts == null)
			{
				offerProducts = _cpRepository.GetProductsOnSale();

			}
			var listClientProductIds = new List<Client_Product>();

			while (listClientProductIds.Count < 4)
			{
				if (offerProducts.Count <= 4)
				{
					foreach (var offerProduct in offerProducts)
						listClientProductIds.Add(offerProduct);
					break;
				}

				var clientProduct = offerProducts[new Random().Next(offerProducts.Count - 1)];
				if (!listClientProductIds.Contains(clientProduct))
					listClientProductIds.Add(clientProduct);
			}

			// Actualiza conteo de productos
			var productList = listClientProductIds.Select(cp => cp.product.Id);
			UpdateCounterFile(productList.Select(p => p.ToString() + "," + (1 / (float)productList.Count()).TwoDecimals() + "," + DateTime.Now.ToShortDateString()).ToArray(), CounterType.product);

			return Json(new
			{
				ids = listClientProductIds.Select(cp => cp.Id).ToArrayString()
			});
		}

		// AJAX
		public ActionResult GenerateImageCaptcha()
		{
			var key = System.Configuration.ConfigurationManager.AppSettings["PublicKey"];
			return Json(new
			{
				key = Explora_Precios.ApplicationServices.CommonUtilities.CacheImage(GenerateImageBytesCaptcha()),
				text = Explora_Precios.ApplicationServices.Helper.IdEncrypter.EncryptStringAES(Session["CaptchaCode"].ToString(), key)
			});
		}
		
		// AJAX
		public ActionResult GenerateCaptcha()
		{
			//cImage.Image.Save(Server.MapPath("~\\CaptchaImages\\" + Convert.ToString(Session["CaptchaCode"]) + ".jpg"), ImageFormat.Jpeg);
			//CaptachaImage.ImageUrl = "~\\CaptchaImages\\" + Convert.ToString(Session["CaptchaCode"]) + ".jpg";
			ViewData.Model = GenerateImageBytesCaptcha();
			var key = System.Configuration.ConfigurationManager.AppSettings["PublicKey"];
			return Json(new
			{
				text = Explora_Precios.ApplicationServices.Helper.IdEncrypter.EncryptStringAES(Session["CaptchaCode"].ToString(), key),
				html = this.RenderViewToString("PartialViews/ProductReport", ViewData)
			});
		}

		// AJAX
		public ActionResult ValidateCaptcha(string _textGened, string _text, string _current)
		{
			var productVM = new ProductViewModel(_productTypeRepository, _subCatRepository, _catRepository, _departmentRepository);
			if (_textGened.DecryptString().ToLower() == _text.ToLower())
			{
				try
				{
					var clientProductObj = _cpRepository.Get(int.Parse(_current.DecryptString()));

					clientProductObj.dateReported = DateTime.Now;
					_cpRepository.Update(clientProductObj);
					ViewData.Model = productVM.LoadModel(clientProductObj.product, false);
					return Json(new
					{
						result = "valid",
						msg = clientProductObj.product.name + " (" + clientProductObj.product.productReference + ")",
						html = this.RenderViewToString("PartialViews/ProductDetails", ViewData)
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

				//ClientScript.RegisterClientScriptBlock(typeof(Page), "ValidateMsg", "<script>alert('You entered Correct CAPTCHA Code!');</script>");
			}
			else
			{
				var key = System.Configuration.ConfigurationManager.AppSettings["PublicKey"];
				ViewData.Model = GenerateImageBytesCaptcha();
				return Json(new
				{
					result = "novalid",
					html = this.RenderViewToString("PartialViews/ProductReport", ViewData),
					text = Explora_Precios.ApplicationServices.Helper.IdEncrypter.EncryptStringAES(Session["CaptchaCode"].ToString(), key)
				});
				//ClientScript.RegisterClientScriptBlock(typeof(Page), "ValidateMsg", "<script>alert('You entered INCORRECT CAPTCHA Code!');</script>");
			}
		}

		//AJAX 
		public ActionResult GetFollow(string redirect)
		{
			ViewData["product"] = redirect;
			if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
			{
				var FollowOfferResults = _uProductRepository.GetByUserAndProductAndType(CurrentUser, _productRepository.Get(int.Parse(redirect.DecryptString())), User_Product.RelationType.FollowIsOnSale);
				var FollowPriceResults = _uProductRepository.GetByUserAndProductAndType(CurrentUser, _productRepository.Get(int.Parse(redirect.DecryptString())), User_Product.RelationType.FollowDoesPriceWentDown);

				var followViewModel = new FollowViewModel();
				ViewData.Model = followViewModel;

				followViewModel.Offer = FollowOfferResults != null ? FollowOfferResults.value == 1 ? true : false : false;
				followViewModel.Price = FollowPriceResults != null ? FollowPriceResults.value == 1 ? true : false : false;

				return Json(new { html = this.RenderViewToString("PartialViews/FollowProduct", ViewData) });
			}

			return Json(new {
				html = string.Format(RegisterPage, "seguir los productos", redirect)
			});
		}

		//AJAX
		public ActionResult GetGroup(string redirect)
		{
			ViewData["product"] = redirect;
			if (CurrentUser != null)
			{
				var GroupVM = new GroupViewModel();
				var ProductObj = _productRepository.Get(int.Parse(redirect.DecryptString()));
				if (ProductObj.groups.Where(x => x.user == CurrentUser).Count() > 0)
					GroupVM.Grouped = GroupDisplay.InGroup;
				else if (ProductObj.groups.Count > 0)
					GroupVM.Grouped = GroupDisplay.IncludeMe;
				else
					GroupVM.Grouped = GroupDisplay.Create;

				GroupVM.GroupSize = ProductObj.groups.Count;
				GroupVM.IsFacebooked = !string.IsNullOrEmpty(CurrentUser.facebookToken);
				GroupVM.DoPublish = GroupVM.Grouped == GroupDisplay.InGroup; // si esta en un grupo que solo pueda hacer publish sin tener que presionar el checkbox
				ViewData.Model = GroupVM;
				return Json(new { html = this.RenderViewToString("PartialViews/GroupProduct", ViewData) });
			}

			return Json(new
			{
				html = string.Format(RegisterPage, "entrar al grupo", redirect)
			});
		}
		
		//AJAX 
		public ActionResult SetFollow()
		{
			var FollowModel = new FollowViewModel();
			TryUpdateModel(FollowModel);

			var ProductObj = _productRepository.Get(int.Parse(FollowModel.Product.DecryptString()));

			var OfferResult = _uProductRepository.GetByUserAndProductAndType(CurrentUser, ProductObj, User_Product.RelationType.FollowIsOnSale); 
			var PriceResult = _uProductRepository.GetByUserAndProductAndType(CurrentUser, ProductObj, User_Product.RelationType.FollowDoesPriceWentDown); 
			
			if (OfferResult == null)
				OfferResult = new User_Product() { product = ProductObj, Type = User_Product.RelationType.FollowIsOnSale, user = CurrentUser, value = FollowModel.Offer ? 1 : 0, NotifiedValue = ProductObj.clients.Select(client => client.specialPrice).Min(), Notified = DateTime.Now };
			else
				OfferResult.value = FollowModel.Offer ? 1 : 0;

			if (PriceResult == null)
				PriceResult = new User_Product() { product = ProductObj, Type = User_Product.RelationType.FollowDoesPriceWentDown, user = CurrentUser, value = FollowModel.Price ? 1 : 0, NotifiedValue = ProductObj.clients.Select(client => client.price).Min(), Notified = DateTime.Now };
			else
				PriceResult.value = FollowModel.Price ? 1 : 0;

			_uProductRepository.Update(OfferResult, PriceResult);

			return Json(new { result = "success", msg = "" });
		}

		public ActionResult SetLike(string _id)
		{
			var ProductObj = _productRepository.Get(_id.CryptProductId());
			var FBclient = new Facebook.FacebookClient();
			// Publicacion en muro de usuario
			try
			{
				FBclient.AccessToken = CurrentUser.facebookToken;
				var userMsg = string.Format("Encontré un(a) \"{0}\" en ExploraPrecios.com y me gusto esto. Regístrate y encontrarás muchos otros productos más! " +
										"Haz click aquí: http://www.exploraprecios.com?i={1}", ProductObj.name, Server.UrlEncode(_id));

				var description = string.Join(", ", ProductObj.clients.Select(client => client.client.name).ToArray());
				var userParameters = new Dictionary<string, object>
				{
					{"access_token",  CurrentUser.facebookToken},
					{"appId", "285146028212857"},
					{"message", userMsg},
					{"caption", "Desde $" + String.Format("{0:0.00}",ProductObj.clients.OrderBy(client => client.price).First().price)},
					{"description", "Puedes encontrarlo en la" + (ProductObj.clients.Count > 1 ? "s siguientes tiendas: " : " tienda ") + description },
					{"name", ProductObj.name},
					{"picture", ProductObj.image.url},
					{"link", string.Format("http://www.exploraprecios.com?i={0}", _id)}
				};
				var userResult = FBclient.Post("me/feed", userParameters);
				var upObj = new User_Product { Type = User_Product.RelationType.Liked, product = ProductObj, user = CurrentUser };
				_uProductRepository.SaveOrUpdate(upObj);
			}
			catch (NullReferenceException ex)
			{
				return Json(new { result = "fail", msg = "Necesitas estar conectado con Facebook." });
			}
			catch (Facebook.FacebookApiException ex)
			{
				if (ex.Message.Contains("authorized"))
					return Json(new { result = "fail", msg = "ExploraPrecios.com no está autorizado para poner mensajes en Facebook.\nSi desea autorizarnos vuelva presionar el boton \"Conectar con Facebook\" y asegúrese de autorizar el permiso." });
				if (ex.Message.Contains("Duplicate"))
					return Json(new { result = "fail", msg = "Mensaje duplicado" });
			}

			// Publicacion en paginas de facebook de clientes
			FBclient.AccessToken = CurrentUser.facebookToken;
			foreach (var client in ProductObj.clients)
			{
				if (client.client.facebookPublish)
				{

					try
					{
						var clientResult = FBclient.Post(client.client.facebookId + "/feed", SetFacebookParameters(client));
					}
					catch (NullReferenceException ex)
					{
						return Json(new { result = "fail", msg = "Necesitas estar conectado con Facebook." });
					}
					catch (Facebook.FacebookApiException ex)
					{
						if (ex.Message.Contains("authorized"))
							return Json(new { result = "fail", msg = "ExploraPrecios.com no está autorizado para poner mensajes en Facebook.\nSi desea autorizarnos vuelva presionar el boton \"Conectar con Facebook\" y asegúrese de autorizar el permiso." });
						if (ex.Message.Contains("Duplicate"))
							return Json(new { result = "fail", msg = "Mensaje duplicado" });
					}
				}
			}

			return Json(new { result = "success", msg = "KUDOS!!" });
		}

		// AJAX
		public ActionResult SetGroup()
		{
			var GroupModel = new GroupViewModel();
			TryUpdateModel(GroupModel);

			var ProductObj = _productRepository.Get(GroupModel.ProductId.CryptProductId());
			if (GroupModel.DoPublish)
			{
				var FBclient = new Facebook.FacebookClient();
				FBclient.AccessToken = CurrentUser.facebookToken;
				var msg = string.Format("He creado un grupo de compra de \"{0}\" en www.ExploraPrecios.com. Registrate, entra al grupo y podras recibir grandes descuentos! "+
										"Haz click en el siguiente link: http://www.exploraprecios.com?i={1}", ProductObj.name, Server.UrlEncode(GroupModel.ProductId));
				var description = string.Join(", ", ProductObj.clients.Select(client => client.name).ToArray());

				var parameters = new Dictionary<string, object>
				{
					{"access_token",  CurrentUser.facebookToken},
					{"appId", "285146028212857"},
					{"message", msg},
					{"caption", "Lo puedes encontrar desde $" + String.Format("{0:0.00}",ProductObj.clients.OrderBy(client => client.price).First().price)},
					{"description", "Puedes encontrarlo en la" + (ProductObj.clients.Count > 1 ? "s siguientes tiendas: " : " tienda ") + description },
					{"name", ProductObj.name},
					{"picture", ProductObj.image.url},
					{"link", string.Format("http://www.exploraprecios.com?i={0}", Server.UrlEncode(GroupModel.ProductId))}
				};
				try
				{
					var result = FBclient.Post("me/feed", parameters);
				}
				catch (Facebook.FacebookApiException ex)
				{
					if (ex.Message.Contains("authorized"))
						return Json(new { result = "fail", msg = "ExploraPrecios.com no está autorizado para poner mensajes en su Facebook.\nSi desea autorizarnos vuelva presionar el boton \"Conectar con Facebook\" y asegúrese de autorizar el permiso." });
					if (ex.Message.Contains("Duplicate"))
						return Json(new { result = "fail", msg = "Mensaje duplicado" });
				}

				
				FBclient.AccessToken = CurrentUser.facebookToken;
				foreach (var client in ProductObj.clients)
				{
					if (client.client.facebookPublish)
					{
						try
						{
							var clientResult = FBclient.Post(client.client.facebookId + "/feed", SetFacebookParameters(client));
						}
						catch (NullReferenceException ex)
						{
							return Json(new { result = "fail", msg = "Necesitas estar conectado con Facebook." });
						}
						catch (Facebook.FacebookApiException ex)
						{
							if (ex.Message.Contains("authorized"))
								return Json(new { result = "fail", msg = "ExploraPrecios.com no está autorizado para poner mensajes en Facebook.\nSi desea autorizarnos vuelva presionar el boton \"Conectar con Facebook\" y asegúrese de autorizar el permiso." });
							if (ex.Message.Contains("Duplicate"))
								return Json(new { result = "fail", msg = "Mensaje duplicado" });
						}
					}
				}
			}

			var dbModified = false;
			if (ProductObj.groups.Count == 0 || ProductObj.groups.SingleOrDefault(group => group.user == CurrentUser) == null)
			{
				var newGroup_User = new Group_User { user = CurrentUser, product = ProductObj, created = DateTime.Now };
				_groupRepository.Update(newGroup_User);
				ProductObj.groups.Add(newGroup_User);
				dbModified = true;
			}

			return Json(new { result = "success", msg = "", groupSize = ProductObj.groups.Count, showWarning = dbModified });
		}

		public ActionResult GetGroupManager()
		{
			if (CurrentUser != null)
			{
				var GroupsList = _groupRepository.GetByUser(CurrentUser).OrderByDescending(group => group.created).Select(group => new GroupViewModel
				{
					CreatedDate = group.created,
					ProductName = group.product.name,
					ProductId = group.product.Id.CryptProductId()
				});
				var GroupManagerVM = new GroupManagerViewModel { TotalPage = (int)(GroupsList.Count() / 5) };

				GroupManagerVM.GroupsViewModel = GroupsList.Take(5);
				ViewData.Model = GroupManagerVM;
				return Json(new { html = this.RenderViewToString("PartialViews/GroupManager", ViewData) });
			}
			else
				return Json(new { html = "Por favor regístrese o ingrese antes de utilizar grupos de compra" });

		}

		public ActionResult GetGroupManagerList(int _toPage)
		{
			var GroupsList = _groupRepository.GetByUser(CurrentUser).OrderByDescending(group => group.created).Select(group => new GroupViewModel
			{
				CreatedDate = group.created,
				ProductName = group.product.name,
				ProductId = group.product.Id.CryptProductId()
			});
			if (GroupsList.Count() > 0)
			{
				var TotalPage = (int)(GroupsList.Count() / 5);
				for (int i = 0; i < _toPage; i++)
				{
					GroupsList = GroupsList.Skip(5);
				}

				ViewData.Model = GroupsList.Take(5); ;
				return Json(new { html = this.RenderViewToString("PartialViews/GroupManagerList", ViewData), totalPage = TotalPage });
			}
			else
				return Json(new { html = "No ha creado o no ha ingresado a ningún grupo aún. Qué esperas? Crea un grupo y empieza a ahorrar!", totalPage = 0 });
		}

		public ActionResult SetRate(string _product, int _value)
		{
			try
			{
				var oProduct = _productRepository.Get(int.Parse(_product.DecryptString()));
				var newUserProduct = new User_Product() { product = oProduct, user = _uRepository.Get(2), value = _value, Type = User_Product.RelationType.Rating };
				oProduct.ratings.Add(newUserProduct);
				_uProductRepository.SaveOrUpdate(newUserProduct);
				var response = Json(new
									{
										result = "valid",
										avg = oProduct.ratings.Where(up => up.Type == User_Product.RelationType.Rating).Average(rat => rat.value).TwoDecimals()
									});
				return response;
			}
			catch (Exception ex)
			{
				return Json(new
				{
					result = "fail",
					msg = "Error: " + ex.Message
				});
				
			}
		}

		// AJAX
		public ActionResult ProductDisplay(string _valId, string _hightlight)
		{
			try
			{
				var productVM = new ProductViewModel(_productTypeRepository, _subCatRepository, _catRepository, _departmentRepository);
				var productObj = _productRepository.Get(_valId.Replace(' ', '+').CryptProductId());
				var productVMObj = productVM.LoadModel(productObj, false);
				productVMObj.group.Grouped = productObj.groups.Count == 0 || CurrentUser == null ? GroupDisplay.Create : productObj.groups.Where(x => x.user == CurrentUser).Count() > 0 ? GroupDisplay.InGroup : GroupDisplay.IncludeMe;
				productVMObj.group.IsFacebooked = CurrentUser != null && !string.IsNullOrEmpty(CurrentUser.facebookToken);
				ViewData.Model = productVMObj;
				// Actualiza el archivo contador con el producto
				UpdateCounterFile(productObj.Id.ToString() + ",1," + DateTime.Now.ToShortDateString(), CounterType.product);
				return Json(new
				{
					result = "success",
					msg = productObj.name + " (" + productObj.productReference + ")",
					html = this.RenderViewToString("PartialViews/ProductDetails", ViewData)
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

		public ActionResult Search(string s)
		{
			var homeViewModel = LoadHomeModel(s);
			homeViewModel.LoadFilters(search: s);
			return View(homeViewModel);
		}

		public ActionResult ScrollSearch(string s, int page)
		{
			currentPage = page;
			var homeViewModel = LoadHomeModel(s);

			ViewData.Model = homeViewModel.productsListViewModel;
			return Json(new
			{
				html = this.RenderViewToString("PartialViews/ProductsList", ViewData),
				hasNext = ViewData.Model = homeViewModel.productsListViewModel.products.HasNextPage
			});
		}

		private Dictionary<string, object> SetFacebookParameters(Client_Product client)
		{
			var offer = client.specialPrice > 0;
			var haveProductUrl = client.url.Length > 0;
			var haveUrl = client.client.url.Length > 0;
			var clientMsg = string.Format("Encontré un(a) \"{0}\" en ExploraPrecios.com " + (offer ? "y está en OFERTA!!" : ".") +
				" Regístrate y encontrarás muchos otros productos más! " +
				"Haz click aquí: " + (haveProductUrl ? "{1}" : " http://www.exploraprecios.com?i={1}"), client.product.name,
				haveProductUrl ? client.url : Server.UrlEncode(client.product.Id.CryptProductId()));
			return new Dictionary<string, object>
							{
								{"access_token",  CurrentUser.facebookToken},
								{"appId", "285146028212857"},
								{"message", clientMsg},
								{"caption", "$" + String.Format("{0:0.00}", offer ? client.specialPrice : client.price)},
								{"description", "Puedes encontrar otros de nuestros productos en " + (haveUrl ? "nuestra pagina: " + client.client.url : "http://www.exploraprecios.com?catlev=" + client.product.level_Id + "&id="+client.product.catalog_Id) },
								{"name", client.product.name},
								{"picture", client.product.image.url},
								{"link", string.Format("http://www.exploraprecios.com?i={0}", client.product.Id.CryptProductId())}
							};
		}

		private IEnumerable<Product> FilterProduct(IEnumerable<Product> products, string p, string b, bool o)
		{
			// Filtrar por precios o oferta
			if (p != null)
			{
				var rangePrices = p.Split(',');
				products = products
						.Where(x => x.clients
							.Where(y => y.price >= float.Parse(rangePrices[0]) &&
								y.price <= float.Parse(rangePrices[1]))
							.Count() > 0);
			}

			// Filtrar por oferta
			if (o)
			{
				products = products.Where(x => x.clients.Where(y => y.isActive && y.specialPrice > 0).Count() > 0);
			}
			// Filtrar por marca
			if (b != null)
			{
				products = products.Where(x => x.brand.name == (b));
			};
			return products;
		}

		private byte[] GenerateImageBytesCaptcha()
		{
			CaptchaImage cImage = new CaptchaImage(CaptchaImage.generateRandomCode(), 140, 40);
			System.Drawing.Bitmap bmp = cImage.Image;
			System.IO.MemoryStream stream = new System.IO.MemoryStream();
			bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
			stream.Position = 0;
			byte[] data = new byte[stream.Length];
			stream.Read(data, 0, (int)stream.Length);
			cImage.Dispose();
			return data;
		}

		/// <summary>
		/// Load Home viewmodel from de catalog level and catalog id
		/// </summary>
		/// <param name="catLev">catalog level (0,1,2,3)</param>
		/// <param name="id">catalog id</param>
		/// <returns>Homeviewmodel</returns>
		private HomeViewModel LoadHomeModel(int catLev, int id)
		{
			var homeModel = new HomeViewModel(catLev, id);
			var catalog = Catalog.Types.Department;
			switch (catLev)
			{
				case 0:
					var depObj = new DepartmentRepository().Get(id);
					homeModel = LoadPrimaryHomeViewModel(depObj);
					homeModel.catalog = depObj.FromLevelsToCatalog();
					break;
				case 1:
					var catObj = _catRepository.Get(id);
					catalog = Catalog.Types.Category;
					homeModel = LoadPrimaryHomeViewModel(catObj.department);
					homeModel.catalog = catObj.FromLevelsToCatalog();
					break;
				case 2:
					var subCatObj = _subCatRepository.Get(id);
					catalog = Catalog.Types.SubCategory;
					homeModel = LoadPrimaryHomeViewModel(subCatObj.category.department);
					homeModel.catalog = subCatObj.FromLevelsToCatalog();
					break;
				case 3:
					var prodTypeObj = _productTypeRepository.Get(id);
					catalog = Catalog.Types.ProductType;
					homeModel = LoadPrimaryHomeViewModel(prodTypeObj.subCategory.category.department);
					homeModel.catalog = prodTypeObj.FromLevelsToCatalog();
					break;
			}
			var products = new List<Explora_Precios.Core.Product>();
			EdgePrices EdgePrices;
			switch (catalog)
			{
				case Catalog.Types.Department:
					products = _productRepository.GetbyDepartment(id);
					EdgePrices = _productRepository.GetbyDepartmentEdgePrices(id);
					break;
				case Catalog.Types.Category:
					products = _productRepository.GetbyCategory(id);
					EdgePrices = _productRepository.GetbyCategoryEdgePrices(id);
					break;
				case Catalog.Types.SubCategory:
					products = _productRepository.GetbySubCategory(id);
					EdgePrices = _productRepository.GetbySubCategoryEdgePrices(id);
					break;
				default:
					products = _productRepository.GetbyProductType(id);
					EdgePrices = _productRepository.GetbyProductTypeEdgePrices(id);
					break;
			}

			homeModel.Filter.CurrentMinPrice = 0;
			homeModel.Filter.CurrentMaxPrice = EdgePrices.Max;

			homeModel = LoadProductsOnModel(homeModel, products);
			homeModel.currentCatalogId = id;
			homeModel.currentCatalogLevel = catLev;
			return homeModel;
		}

		/// <summary>
		/// Load home viewmodel from a search result
		/// </summary>
		/// <param name="s">search text</param>
		/// <param name="d">current department id</param>
		/// <returns>Homeviewmodel</returns>
		private HomeViewModel LoadHomeModel(string s)
		{
			ViewData["search_text"] = s;
			var homeViewModel = new HomeViewModel();
			var products = _productRepository.GetbySearchText(s, IsActivated.Yes).ToList();
			homeViewModel.Filter.CurrentMinPrice = products.Count > 0 ? products.First().clients.OrderBy(c => c.price).First().price : 0;
			homeViewModel.Filter.CurrentMaxPrice = products.Count > 0 ? products.Last().clients.OrderByDescending(c => c.price).First().price : 0;
			homeViewModel = LoadProductsOnModel(homeViewModel, products);
			homeViewModel.catalog = null; 
			homeViewModel.isSearch = true;
			homeViewModel.currentSearch = s;
			return homeViewModel;
		}

		private HomeViewModel LoadPrimaryHomeViewModel(Department department)
		{
			var homeViewModel = new HomeViewModel(0, department.Id);
			homeViewModel.departmentId = department.Id;
			homeViewModel.departmentTitle = department.name;
			homeViewModel.categories = department.categories.OrderBy(x => x.name).Select(x => new CategoryViewModel()
			{
				categoryId = x.Id,
				categoryTitle = x.name,
				subCategories = x.subCategories.Select(y => new SubCategoryViewModel()
				{
					subCategoryId = y.Id,
					subCategoryTitle = y.name,
					productTypes = y.productTypes.Select(z => new ProductTypeViewModel()
					{
						productTypeId = z.Id,
						productTypeTitle = z.name
					}).ToList()
				}).ToList()
			}).ToList();

			return homeViewModel;
		}

		private HomeViewModel LoadProductsOnModel(HomeViewModel homeViewModel, List<Explora_Precios.Core.Product> products)
		{
			var productVM = new ProductViewModel(_productTypeRepository, _subCatRepository, _catRepository, _departmentRepository);
			var defaultPageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["DefaultPageSize"]);
			//var productsOrdered = products.OrderBy(p => p.clients.OrderBy(c => c.price).First().price);
			homeViewModel.productsListViewModel.products = new PagedList<Explora_Precios.Core.Product>(products.AsEnumerable(), currentPage, defaultPageSize);
			homeViewModel.productsListViewModel.productsList = homeViewModel.productsListViewModel.products.Select(p => productVM.LoadModel(p, true));
			homeViewModel.allProducts = products;
			// Actualiza el archivo contador de productos
			UpdateCounterFile(homeViewModel.productsListViewModel.products.Select(p => p.Id.ToString() + "," + ((1 / (float)homeViewModel.productsListViewModel.products.Count).TwoDecimals()) + "," + DateTime.Now.ToShortDateString()).ToArray(), CounterType.product);
			return homeViewModel;
		}

		private enum CounterType { product, client }

		private void UpdateCounterFile(string productWeight, CounterType type)
		{
			var array = new string[]{ productWeight };
			UpdateCounterFile(array, type);
		}

		/// <summary>
		/// Actualiza el archivo contador de productos visualizados
		/// </summary>
		/// <param name="productsWeights">Id de producto y peso separado por una coma</param>
		private void UpdateCounterFile(string[] productsWeights, CounterType type)
		{
			if (productsWeights.Length > 0)
			{
				var FilePath = Helper.RootFolder() + "\\Data\\" + type.ToString() + "counter.txt";
				var sb = new System.Text.StringBuilder();
				var fileLines = System.IO.File.ReadAllLines(FilePath);
				foreach (string line in fileLines)
				{ sb.Append(line); }
				if (fileLines.Length > 0)
					sb.Append(";");
				sb.Append(string.Join(";", productsWeights));
				System.IO.File.WriteAllText(FilePath, sb.ToString());
			}
		}
	}
}
