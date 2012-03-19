using System.Web.Mvc;
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
        IProductTypeRepository _productTypeRepository;
        ISubCategoryRepository _subCatRepository;
        ICategoryRepository _catRepository;
        IProductRepository _productRepository;
        IDepartmentRepository _departmentRepository;
        IClient_ProductRepository _cpRepository;
        IUserRepository _uRepository;
        IUser_ProductRepository _uProductRepository;
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
            IUser_ProductRepository uProductRepository)
        {
            _catRepository = catRepository;
            _productRepository = productRepository;
            _subCatRepository = subCatRepository;
            _productTypeRepository = productTypeRepository;
            _departmentRepository = departmentRepository;
            _cpRepository = cpRepository;
            _uRepository = uRepository;
            _uProductRepository = uProductRepository;
        }

        public ActionResult Index()
        {
            ViewData["IsMobile"] = false.ToString();
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
                if (goTo == "contact") Response.Redirect("/Contact"); else if (goTo == "manager") Response.Redirect("/ProductManager");
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
            
            // Get Ticker Items
            var result = new List<string>();

            var SubCats = ConfigurationManager.AppSettings["TickerSubCategories"].Split(',').RandomizeList<string>();
            var ProdutTypes = ConfigurationManager.AppSettings["TickerProductTypes"].Split(',').RandomizeList<string>();

            foreach (var subCat in SubCats)
            {
                var subCatObj = _subCatRepository.Get(int.Parse(subCat)).FromLevelsToCatalog();
                var productList =  _productRepository.GetbySubCategory(int.Parse(subCat)).Select(product => product.clients.OrderBy(client => client.price).First());
                if (productList.Count() > 0)
                {
                    var price = productList.OrderBy(product => product.price).First().price;
                    var li = "<li><span>" + subCatObj.subCategoryName + "</span><a href=\"/Home/Products/?catlev=2&id=" + subCat + "\" title=\"" + subCatObj.departmentName + " - " + subCatObj.categoryName + " - " + subCatObj.subCategoryName + "\" >desde $" + String.Format("{0:0.00}", price) + "</a></li>";
                    result.Add(li);
                }
            }

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

            var IntroModel = new IntroViewModel();
            IntroModel.TickerList = result;

            // Provisionalmente vamos a traer 20 productos en oferta para ser desplegados como destacados
            var offerProducts = _cpRepository.GetProductsOnSale().RandomizeList();
            var clientIds = offerProducts.Select(cp => cp.client.Id).Distinct();
            var highlightedProducts = new List<Client_Product>();
            foreach(var clientId in clientIds)
            {
                highlightedProducts.AddRange(offerProducts.Where(cp => cp.client.Id == clientId).Take(6));
            }
            var newProducts = _cpRepository.GetLastAdded();

            IntroModel.HighlightProducts = highlightedProducts.Select(client_product => new BannerProduct() { ProductId = client_product.product.Id, Name = client_product.product.name, Client = client_product.client.name, Image = client_product.product.image.imageObj, ClientId = client_product.client.Id, Price = client_product.specialPrice}).RandomizeList();
            IntroModel.OfferProducts = offerProducts.Select(client_product => new BannerProduct() { ProductId = client_product.product.Id, Name = client_product.product.name, Client = client_product.client.name, Image = client_product.product.image.imageObj, Price = client_product.specialPrice }).RandomizeList();
            IntroModel.NewProducts = newProducts.Select(client_product => new BannerProduct() { ProductId = client_product.product.Id, Name = client_product.product.name, Client = client_product.client.name, Image = client_product.product.image.imageObj, Price = client_product.price });
            
            return View(IntroModel);
        }

        public ActionResult Products(int? catLev, int? id, int? page)
        {
            // Get all the catalog items under the selected depatment. Default Electronicos = 1
            currentPage = page.HasValue ? page.Value - 1 : 0;
            catLev = catLev ?? 0;
            id = id ?? 1;
            var homeModel = LoadHomeModel((int)catLev, (int)id);

            LoadCounterValues();
            return View(homeModel);
        }

        public ActionResult Filter(string f, string filterData, string currentDisplay, int s, int? page)
        {
            var display = currentDisplay.Split(',');
            HomeViewModel homeModel;
            // check if the filter is being generated from a catalog click (s == 0)  
            // or a search (s == 1)
            if (s == 0)
            {
                homeModel = LoadHomeModel(int.Parse(display[0]), int.Parse(display[1]));
            }
            else
            {
                homeModel = LoadHomeModel(display[0], int.Parse(display[1]));
            }

            var data = filterData.Split(',');
            // check if the data to filter the products is from price (f == p)
            // or brand (f == b)
            var currentPage = page.HasValue ? page.Value - 1 : 0;
            var defaultPageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["DefaultPageSize"]);
            if (f == "p")
                homeModel.productsListViewModel.products = new PagedList<Explora_Precios.Core.Product>(homeModel.allProducts
                    .Where(x => x.clients
                        .Where(y => y.price >= int.Parse(data[0]) && 
                            y.price <= int.Parse(data[1]))
                        .Count() > 0), currentPage, defaultPageSize);
            else if (f == "b")
            {
                homeModel.productsListViewModel.products = new PagedList<Explora_Precios.Core.Product>(homeModel.allProducts.Where(x => x.brand.name == filterData), currentPage, defaultPageSize);
            }
            else
            {
                homeModel.productsListViewModel.products = new PagedList<Explora_Precios.Core.Product>(homeModel.allProducts.Where(x => x.clients.Where(y => y.isActive && y.specialPrice > 0).Count() > 0), currentPage, defaultPageSize);
            }

            var productVM = new ProductViewModel(_productTypeRepository, _subCatRepository, _catRepository, _departmentRepository);
            homeModel.productsListViewModel.productsList = homeModel.productsListViewModel.products.Select(product => productVM.LoadModel(product, false));
            homeModel.filterBackUrl = s == 0 ? "/?catlev=" + display[0] + "&id=" + display[1] : "/Home/Search?s=" + display[0] + "&d=" + display[1];

            LoadCounterValues();
            return View("Products",homeModel);
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
        public JsonResult GetFloatingProducts(string type, int? page, bool? ie8)
        {
            var productVM = new ProductViewModel(_productTypeRepository, _subCatRepository, _catRepository, _departmentRepository);
            var pageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["FloatingPageSize"]);
            var allProducts = new List<Explora_Precios.Core.Product>();
            if (type == "onsale")
            {
                allProducts = _cpRepository.GetProductsOnSale().Select(cp => cp.product).ToList();
            }
            else
            {
                allProducts = _cpRepository.GetLastAdded().Select(cp => cp.product).ToList();
            }

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
            var listIds = new List<int>();

            while (listIds.Count < 4)
            {
                var offerProduct = offerProducts[new Random().Next(offerProducts.Count - 1)];
                if (!listIds.Contains(offerProduct.Id))
                    listIds.Add(offerProduct.Id);
            }

            return Json(new
            {
                ids = listIds.ToArrayString()
            });

        }

        // AJAX
        public ActionResult GenerateImageCaptcha()
        {
            var key = System.Configuration.ConfigurationManager.AppSettings["PublicKey"];
            return Json(new
            {
                key = new Explora_Precios.ApplicationServices.CommonUtilities().CacheImage(GenerateImageBytesCaptcha()),
                text = IdEncrypter.EncryptStringAES(Session["CaptchaCode"].ToString(), key)
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
                text = IdEncrypter.EncryptStringAES(Session["CaptchaCode"].ToString(), key),
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
                    text = IdEncrypter.EncryptStringAES(Session["CaptchaCode"].ToString(), key)
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
			if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
			{

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
                                        avg = oProduct.ratings.Where(up => up.Type == User_Product.RelationType.Rating).Average(rat => rat.value).ToString("0.##")
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
        public ActionResult ProductDisplay(string _valId, string _highlight)
        {
            try
            {
                var productId = 0;
                if (!int.TryParse(_valId, out productId))
                {
                    productId = int.Parse(_valId.DecryptString());
                }
                var productVM = new ProductViewModel(_productTypeRepository, _subCatRepository, _catRepository, _departmentRepository);
                var productObj = _productRepository.Get(productId);
				var productVMObj = productVM.LoadModel(productObj, false);
				productVMObj.group.Grouped = productObj.groups.Count == 0 || CurrentUser == null ? GroupDisplay.Create : productObj.groups.Contains(CurrentUser) ? GroupDisplay.InGroup : GroupDisplay.IncludeMe;
				ViewData.Model = productVMObj;
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

        public ActionResult Search(string s, int d)
        {
            var homeViewModel = LoadHomeModel(s, d);
            LoadCounterValues();
            return View("Index", homeViewModel);
        }

        private void LoadCounterValues()
        {
            if (Session["NewProducts"] == null)
            {
                var newProducts = Session["NewProducts"] = _cpRepository.GetLastAdded();
                ViewData["newProductsCount"] = ((IList<Client_Product>)newProducts).Count;
            }
            else
                ViewData["newProductsCount"] = ((IList<Client_Product>)Session["NewProducts"]).Count;

            if (Session["DisplayOffers"] == null)
            {
                var onSaleProducts = Session["DisplayOffers"] = _cpRepository.GetProductsOnSale();
                ViewData["productsSaleCount"] = ((IList<Client_Product>)onSaleProducts).Count;
            }
            else
                ViewData["productsSaleCount"] = ((IList<Client_Product>)Session["DisplayOffers"]).Count;
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
            var homeModel = new HomeViewModel();
            var catalog = Catalog.Types.Department;
            switch (catLev)
            {
                case 0:
                    var depObj = new DepartmentRepository().Get((int)id);
                    homeModel = LoadPrimaryHomeViewModel(depObj);
                    homeModel.catalog = depObj.FromLevelsToCatalog();
                    break;
                case 1:
                    var catObj = _catRepository.Get((int)id);
                    catalog = Catalog.Types.Category;
                    homeModel = LoadPrimaryHomeViewModel(catObj.department);
                    homeModel.catalog = catObj.FromLevelsToCatalog();
                    break;
                case 2:
                    var subCatObj = _subCatRepository.Get((int)id);
                    catalog = Catalog.Types.SubCategory;
                    homeModel = LoadPrimaryHomeViewModel(subCatObj.category.department);
                    homeModel.catalog = subCatObj.FromLevelsToCatalog();
                    break;
                case 3:
                    var prodTypeObj = _productTypeRepository.Get((int)id);
                    catalog = Catalog.Types.ProductType;
                    homeModel = LoadPrimaryHomeViewModel(prodTypeObj.subCategory.category.department);
                    homeModel.catalog = prodTypeObj.FromLevelsToCatalog();
                    break;
            }

            var products = new List<Explora_Precios.Core.Product>();
            switch (catalog)
            {
                case Catalog.Types.Department:
                    products = _productRepository.GetbyDepartment((int)id); // TODO: Refactor de optimizacion
                    break;
                case Catalog.Types.Category:
                    products = _productRepository.GetbyCategory((int)id);
                    break;
                case Catalog.Types.SubCategory:
                    products = _productRepository.GetbySubCategory((int)id);
                    break;
                case Catalog.Types.ProductType:
                    products = _productRepository.GetbyProductType((int)id);
                    break;
            }
            products = products.Where(x => x.clients.Any(y => y.isActive)).ToList(); // TODO: Refactor de optimizacion
            homeModel = LoadProductsOnModel(homeModel, products);
            return homeModel;
        }

        /// <summary>
        /// Load home viewmodel from a search result
        /// </summary>
        /// <param name="s">search text</param>
        /// <param name="d">current department id</param>
        /// <returns>Homeviewmodel</returns>
        private HomeViewModel LoadHomeModel(string s, int d)
        {
            ViewData["search_text"] = s;
            var homeViewModel = new HomeViewModel();
            var departament_Obj = new DepartmentRepository().Get(d);
            homeViewModel = LoadPrimaryHomeViewModel(departament_Obj);
            homeViewModel = LoadProductsOnModel(homeViewModel, _productRepository.GetbySearchText(s, IsActivated.Yes).ToList());
            homeViewModel.catalog = departament_Obj.FromLevelsToCatalog();
            homeViewModel.isSearch = true;
            return homeViewModel;
        }

        private HomeViewModel LoadPrimaryHomeViewModel(Department department)
        {
            var homeViewModel = new HomeViewModel();
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
            var productsOrdered = products.OrderBy(p => p.clients.OrderBy(c => c.price).First().price);
            homeViewModel.productsListViewModel.products = new PagedList<Explora_Precios.Core.Product>(productsOrdered.AsEnumerable(), currentPage, defaultPageSize);
            homeViewModel.productsListViewModel.productsList = homeViewModel.productsListViewModel.products.Select(p => productVM.LoadModel(p, true));
            homeViewModel.allProducts = productsOrdered.ToList();
            return homeViewModel;
        }
    }
}
