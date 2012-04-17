using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web.Mvc;
using Explora_Precios.Core.DataInterfaces;
using System.Web.Security;
using System.Net.Mail;
using System.Net;
using SharpArch.Data.NHibernate;
using System.Web;
using Explora_Precios.Web.Controllers.Helpers;
using Explora_Precios.Web.Controllers.ViewModels;
using Explora_Precios.Web.Controllers.Validators;
using System.Configuration;
using Explora_Precios.Data;
using Facebook;

namespace Explora_Precios.Web.Controllers
{
	public class AccountController : PrimaryController
	{
		private const string redirectUrl = "/Home/ViewProduct?id={0}";

		/// <summary>
		/// The access token.
		/// </summary>
		private string FbTokenKey = ConfigurationManager.AppSettings["fbTokenKey"];

		public IFormsAuthentication FormsAuth
		{
			get;
			private set;
		}

		public IAccountMembership AccountMembership
		{
			get;
			private set;
		}

		public IUserRepository UserRepository;

		public AccountController(IUserRepository _uRepository)
			: this(null, null)
		{
			UserRepository = _uRepository;
		}

		public AccountController(IFormsAuthentication formsAuth, IAccountMembership membership)
		{
			FormsAuth = formsAuth ?? new FormsAuthenticationService();
			AccountMembership = membership ?? new AccountMembershipService();
		}

		public ActionResult Valid(string code, string email)
		{
			if (!string.IsNullOrEmpty(email))
			{
				var User = UserRepository.GetByEmail(email);
				if (User != null)
				{
					if (User.validationCode == code)
					{
						User.isApproved = true;
						User.validationCode = "";
						UserRepository.UpdateUser(User);
						FormsAuth.SignIn(User.username, true);
						return Redirect("/Account?valid=1");
					}
				}
			}
			return RedirectToAction("Index", "Home");
		}

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Login(string redirect)
		{
			try
			{
				ViewData["redirect"] = redirect;
				var html = this.RenderViewToString("Login", ViewData);
				return Json(new
				{
					result = "success",
					html = html
				});
			}
			catch (Exception ex)
			{

				return Json(new
				{
					result = "fail",
					html = "",
					msg = "Error: " + ex.Message
				});
			}
		}

		public ActionResult Register(string redirect)
		{
			try
			{
				ViewData["redirect"] = redirect;
				var UserModel = new UserViewModel();
				UserModel.IsNew = string.IsNullOrEmpty(HttpContext.User.Identity.Name);
				if (!UserModel.IsNew)
				{
					var user = UserRepository.GetByEmail(HttpContext.User.Identity.Name);
					UserModel.Birthdate = user.birthdate;
					UserModel.Email = user.email;
					UserModel.Gender = user.gender;
					UserModel.LastName = user.lastName;
					UserModel.Name = user.name;
				}
				ViewData.Model = UserModel;

				var html = this.RenderViewToString("Register", ViewData);
				return Json(new
				{
					result = "success",
					html = html
				});
			}
			catch (Exception ex)
			{

				return Json(new
				{
					result = "fail",
					html = "",
					msg = "Error: " + ex.Message
				});
			}
		}

		public ActionResult Forgot(string redirect)
		{
			ViewData["redirect"] = redirect;
			var html = this.RenderViewToString("Forgot", ViewData);
			return Json(new
			{
				result = "success",
				html = html
			});
		}

		public ActionResult Logout()
		{
			FormsAuth.SignOut();

			Request.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now;
			Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now;
			Response.Cookies[FbTokenKey].Expires = DateTime.Now;
			return RedirectToAction("Index", "Home");
		}

		#region Posts
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Login()
		{
			var LoginViewModel = new LoginViewModel();
			TryUpdateModel(LoginViewModel);
			var email = LoginViewModel.Email;
			var password = LoginViewModel.Password;
			var remember = LoginViewModel.Remember;
			email = string.IsNullOrEmpty(email) ? "" : email.Trim();
			password = string.IsNullOrEmpty(password) ? "" : password.Trim();
			var persistent = remember;
			if (!AccountMembership.ValidateUser(email, password))
			{
				ViewData.ModelState.AddModelError("ErrorMessage", "El email o contraseña proporcionados son incorrectos.");
				var html = this.RenderViewToString("Login", ViewData);
				ViewData["redirect"] = LoginViewModel.Redirect;

				return Json(new
				{
					result = "fail",
					html = html
				});
			}

			// Validar si ya ha sido aprobado
			var User = UserRepository.GetByEmail(email);
			if (!User.isApproved)
			{
				ViewData.ModelState.AddModelError("ErrorMessage", "Su cuenta no ha sido verificada aún. Por favor revise su correo y siga las instrucciones para que su cuenta sea habilitada.");
				ViewData["redirect"] = LoginViewModel.Redirect;

				return Json(new
				{
					result = "fail",
					html = this.RenderViewToString("Login", ViewData)
				});            
			}

			FormsAuth.SignIn(email, persistent);
			if (!string.IsNullOrEmpty(LoginViewModel.Redirect))
			{
				var ProductRepository = new Explora_Precios.Data.ProductRepository();
				var Product = ProductRepository.Get(int.Parse(LoginViewModel.Redirect.DecryptString()));
				var ProductViewModel = new ProductViewModel().LoadModel(Product, false);
				ProductViewModel.catalogProduct = GetCatalog(Product.level_Id, Product.catalog_Id);
				ViewData.Model = ProductViewModel;

				return Json(new
				{
					result = "redirect",
					html = this.RenderViewToString("~/Views/Home/PartialViews/ProductDetails.ascx", ViewData)
				});
			}

			return Json(new
			{
				result = "success",
				html = this.RenderViewToString("Login", ViewData)
			});
		}

		private CatalogViewModel GetCatalog(int levelId, int catalogId)
		{
			var response = new CatalogViewModel();
			while (catalogId != 0)
			{
				if (levelId == 3)
				{
					var ProductTypeRepository = new Explora_Precios.Data.ProductTypeRepository();
					var catalog = ProductTypeRepository.Get(catalogId);
					response.productTypeId = catalogId;
					response.productTypeName = catalog.name;
					response.productTypes = ProductTypeRepository.GetAll().Select(x => new ProductTypeViewModel() { productTypeId = x.Id, productTypeTitle = x.name }).ToList();
					catalogId = catalog.subCategory.Id;
				}
				else if (levelId == 2)
				{
					var SubCategoryRepository = new Explora_Precios.Data.SubCategoryRepository();
					var catalog = SubCategoryRepository.Get(catalogId);
					response.subCategoryId = catalogId;
					response.subCategoryName = catalog.name;
					response.subCategories = SubCategoryRepository.GetAll().Select(x => new SubCategoryViewModel() { subCategoryId = x.Id, subCategoryTitle = x.name }).ToList();
					catalogId = catalog.category.Id;
				}
				else if (levelId == 1)
				{
					var CategoryRepository = new Explora_Precios.Data.CategoryRepository();
					var catalog = CategoryRepository.Get(catalogId);
					response.categoryId = catalogId;
					response.categoryName = catalog.name;
					response.categories = CategoryRepository.GetAll().Select(x => new CategoryViewModel() { categoryId = x.Id, categoryTitle = x.name }).ToList();
					catalogId = catalog.department.Id;
				}
				else
				{
					var DepartmentRepository = new Explora_Precios.Data.DepartmentRepository();
					var catalog = DepartmentRepository.Get(catalogId);
					response.departmentId = catalogId;
					response.departmentName = catalog.name;
					response.departments = DepartmentRepository.GetAll().Select(x => new DepartmentViewModel() { departmentId = x.Id, departmentTitle = x.name }).ToList();
					catalogId = 0;
				}
				levelId--;
			}
			return response;
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Register()
		{
			ViewData["PasswordLength"] = AccountMembership.MinPasswrodLength;
			var result = "";
			var html = "";
			var msg = "";
			var UserModel = new UserViewModel();
			var ValidModel = TryUpdateModel(UserModel);
			ViewData.Model = UserModel;
			var ValidationResults = new RegisterValidator().Validate(UserModel.Email, UserModel.Name, UserModel.LastName, UserModel.Birthdate);
			if (!ValidModel || !ValidationResults.isValid())
			{
				ViewData.ModelState.AddRuleViolations(ValidationResults);
				ValidModel = false;
			}
			if (UserModel.IsNew)
			{
				var ValidRegistration = true;
				if (ValidateRegistration(UserModel.Email, UserModel.NewPassword, UserModel.ConfirmPassword))
				{
					if (ValidModel)
					{
						MembershipCreateStatus status = AccountMembership.CreateUser(UserModel.Email, UserModel.NewPassword, UserModel.Email);
						if (status == MembershipCreateStatus.Success)
						{
							var Code = "";
							var emailSent = AccountHelper.SendEmail(AccountHelper.EmailType.NewUser, UserModel.Email, UserModel.Name, out Code);
							var NewUser = UserRepository.GetByEmail(UserModel.Email);
							NewUser.email = UserModel.Email;
							NewUser.username = UserModel.Email;
							NewUser.name = UserModel.Name;
							NewUser.lastName = UserModel.LastName;
							NewUser.gender = UserModel.Gender;
							NewUser.birthdate = UserModel.Birthdate;
							NewUser.validationCode = Code;
							UserRepository.UpdateUser(NewUser);
							var Message = "Su inscripción ha sido ingresada con éxito! <br />En breve recibirá un correo para verificar su dirección de correo electrónico.";
							if (!string.IsNullOrEmpty(UserModel.Redirect))
							{
								var ProductRepository = new Explora_Precios.Data.ProductRepository();
								var Product = ProductRepository.Get(int.Parse(UserModel.Redirect.DecryptString()));
								var ProductViewModel = new ProductViewModel().LoadModel(Product, false);
								ProductViewModel.catalogProduct = GetCatalog(Product.level_Id, Product.catalog_Id);
								ViewData.Model = ProductViewModel;

								return Json(new
								{
									result = "redirect",
									html = this.RenderViewToString("/Home/PartialViews/ProductDetails", ViewData),
									msg = Message
								});
							}
							else
							{

								result = "success";
								html = "";
								msg = Message;
							}
						}
						else
						{
							ViewData.ModelState.AddModelError("ErrorData", ErrorCodeToString(status));
							result = "fail";
							html = this.RenderViewToString("Register", ViewData);
						}
					}
				}
				else
				{
					ValidRegistration = false;
				}

				if (!ValidRegistration || !ValidModel)
				{
					ViewData["redirect"] = UserModel.Redirect;
					if (ValidRegistration)
						ViewData.ModelState.AddModelError("ErrorData", "Error al introducir su email, su password o su confirmación. Intente nuevamente por favor.");
					result = "fail";
					html = this.RenderViewToString("Register", ViewData);
				}
			}
			else
			{
				var UpdateSuccess = true;
				if (!string.IsNullOrEmpty(UserModel.Password))
				{
					try
					{
						if (!ValidateChangePassword(UserModel.Password, UserModel.NewPassword, UserModel.ConfirmPassword))
						{
							throw new Exception("El password es incorrecto o el nuevo password es invalido. Por favor corriga e intente nuevamente.");
						}
						else
						{
							if (!AccountMembership.ChangePassword(User.Identity.Name, UserModel.Password, UserModel.NewPassword))
							{
								ModelState.AddModelError("password", "La contraseña actual o la nueva contraseña son incorrectas");
								ModelState.AddModelError("newPassword", "");
								throw new Exception();
							}
						}
					}
					catch
					{
						UpdateSuccess = false;
					}
				}
				if (ValidModel && UpdateSuccess)
				{
					var User = UserRepository.GetByEmail(HttpContext.User.Identity.Name);
					try
					{
						User.email = UserModel.Email;
						User.username = UserModel.Email;
						User.name = UserModel.Name;
						User.lastName = UserModel.LastName;
						User.gender = UserModel.Gender;
						User.birthdate = UserModel.Birthdate;
						if (HttpContext.User.Identity.Name != UserModel.Email)
						{
							var Code = "";
							AccountHelper.SendEmail(AccountHelper.EmailType.ChangeEmail, UserModel.Email, UserModel.Name, out Code);
							User.isApproved = false;
							User.validationCode = Code;
							FormsAuth.SignOut();
						}
						UserRepository.UpdateUser(User);

						msg = "Su Perfil ha sido actualizado con éxito";
						html = "";
						result = "success";
					}
					catch
					{
						UpdateSuccess = false;
						ModelState.AddModelError("ErrorData", "No se pudieron guardar los cambios. Intente nuevamente mas tarde.");
					}
				}

				if (!UpdateSuccess || !ValidModel)
				{
					result = "fail";
					html = this.RenderViewToString("Register", ViewData);
				}
			}

			return CreateJSon(msg, html, result);
		}
		
		public ActionResult FacebookRegister(string _token, string _email, string _last_name, string _first_name, string _gender, string _redirect)
		{
			try
			{
				var existed = false;
				Session[FbTokenKey] = _token;
				Response.Cookies[FbTokenKey].Value = _token;
				MembershipCreateStatus status = AccountMembership.CreateUser(_email, ConfigurationManager.AppSettings["fbPass"], _email);
				System.Threading.Thread.Sleep(2000);
				if (status == MembershipCreateStatus.DuplicateUserName)
					existed = true;
				var NewUser = UserRepository.GetByEmail(_email);
				NewUser.email = _email;
				NewUser.username = _email;
				NewUser.name = _first_name;
				NewUser.lastName = _last_name;
				NewUser.gender = _gender.First();
				NewUser.birthdate = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
				NewUser.isApproved = true;
				NewUser.facebookToken = _token;
				UserRepository.UpdateUser(NewUser);

				FormsAuth.SignIn(_email, true);
				return Json(new
				{
					result = "success",
					existed = existed,
					redirect = _redirect
				});
			}
			catch (Exception ex)
			{
				return Json(new
				{
					result = "fail",
					msg = ex.Message + "<br/>" + ex.StackTrace
				});
			}
		}

		private IDictionary<TKey, TValue> Merge<TKey, TValue>(IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
		{
			//Contract.Ensures(Contract.Result<IDictionary<TKey, TValue>>() != null);

			first = first ?? new Dictionary<TKey, TValue>();
			second = second ?? new Dictionary<TKey, TValue>();
			var merged = new Dictionary<TKey, TValue>();

			foreach (var kvp in second)
			{
				merged.Add(kvp.Key, kvp.Value);
			}

			foreach (var kvp in first)
			{
				if (!merged.ContainsKey(kvp.Key))
				{
					merged.Add(kvp.Key, kvp.Value);
				}
			}

			return merged;
		}

		private IDictionary<string, object> ParseUrlQueryString(string query)
		{
			//Contract.Ensures(Contract.Result<IDictionary<string, object>>() != null);

			var result = new Dictionary<string, object>();

			// if string is null, empty or whitespace
			if (string.IsNullOrEmpty(query) || query.Trim().Length == 0)
			{
				return result;
			}

			string decoded = Server.HtmlDecode(query);
			int decodedLength = decoded.Length;
			int namePos = 0;
			bool first = true;

			while (namePos <= decodedLength)
			{
				int valuePos = -1, valueEnd = -1;
				for (int q = namePos; q < decodedLength; q++)
				{
					if (valuePos == -1 && decoded[q] == '=')
					{
						valuePos = q + 1;
					}
					else if (decoded[q] == '&')
					{
						valueEnd = q;
						break;
					}
				}

				if (first)
				{
					first = false;
					if (decoded[namePos] == '?')
					{
						namePos++;
					}
				}

				string name, value;
				if (valuePos == -1)
				{
					name = null;
					valuePos = namePos;
				}
				else
				{
					name = Server.UrlDecode(decoded.Substring(namePos, valuePos - namePos - 1));
				}

				if (valueEnd < 0)
				{
					namePos = -1;
					valueEnd = decoded.Length;
				}
				else
				{
					namePos = valueEnd + 1;
				}

				value = Server.UrlDecode(decoded.Substring(valuePos, valueEnd - valuePos));

				if (!string.IsNullOrEmpty(name))
				{
					result[name] = value;
				}

				if (namePos == -1)
				{
					break;
				}
			}

			return result;
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Forgot(string email, string Redirect)
		{
			var emailSent = false;
			if (!string.IsNullOrEmpty(email))
			{
				var User = UserRepository.GetByEmail(email);
				if (User != null)
					try
					{
						emailSent = AccountHelper.SendEmail(AccountHelper.EmailType.ForgotPassword, User.name, email);
					}
					catch(Exception ex)
					{
						ModelState.AddModelError("_FORM", ex.Message + " " + ex.StackTrace);
						return CreateJSon("", this.RenderViewToString("Forgot", ViewData), "fail");
					}
			}
			if (!emailSent)
			{
				ModelState.AddModelError("_FORM", "El correo proporcionado no esta bajo ninguna cuenta conocida. Por favor intente nuevamente");
				return CreateJSon("", this.RenderViewToString("Forgot", ViewData), "fail");
			}
			var Message = "Su nueva contraseña a sido enviada a su correo";
			if (!string.IsNullOrEmpty(Redirect))
			{
				var ProductRepository = new Explora_Precios.Data.ProductRepository();
				var Product = ProductRepository.Get(int.Parse(Redirect.DecryptString()));
				var ProductViewModel = new ProductViewModel().LoadModel(Product, false);
				ProductViewModel.catalogProduct = GetCatalog(Product.level_Id, Product.catalog_Id);
				ViewData.Model = ProductViewModel;

				return Json(new
				{
					result = "redirect",
					html = this.RenderViewToString("/Home/PartialViews/ProductDetails", ViewData),
					msg = Message
				});
			}
			return CreateJSon(Message, this.RenderViewToString("Forgot", ViewData), "success");
		}
		#endregion        

		#region Validations         

		private bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword)
		{
			var IsValid = true;
			if (string.IsNullOrEmpty(currentPassword))
			{
				ModelState.AddModelError("currentPassword", "Por favor especifique un password.");
				IsValid = false;
			}
			if (string.IsNullOrEmpty(newPassword) || newPassword.Length < AccountMembership.MinPasswrodLength)
			{
				ModelState.AddModelError("password", string.Format("El password debe ser de al menos {0} caracteres.", AccountMembership.MinPasswrodLength));
				IsValid = false;
			}

			if (!string.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
			{
				ModelState.AddModelError("ConfirmPassword", "El password y el password de confirmacion no coinciden.");
				IsValid = false;
			}

			return IsValid;
		}

		private bool ValidateRegistration(string email, string password, string confirmPassword)
		{
			if (string.IsNullOrEmpty(email))
			{
				ModelState.AddModelError("email", "Proporcione el email.");
			}
			if (string.IsNullOrEmpty(password) || password.Length < AccountMembership.MinPasswrodLength)
			{
				ModelState.AddModelError("password", string.Format("La contraseña debe ser de al menos {0} caracteres.", AccountMembership.MinPasswrodLength));
			}
			if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
			{
				ModelState.AddModelError("_Form", "La contraseña y la contraseña de confirmación no coinciden.");
			}

			return ModelState.IsValid;
		}

		private bool ValidateLogOn(string email, string password)
		{
			if (!AccountMembership.ValidateUser(email, password))
			{
				ModelState.AddModelError("_FORM", "El email o contraseña proporcionados son incorrectos.");
			}
			return ModelState.IsValid;
		}

		#endregion

		private string ErrorCodeToString(MembershipCreateStatus status)
		{ 
			switch(status)
			{
				case MembershipCreateStatus.DuplicateUserName:
					return "El nombre de usuario ya existe en la base de datos de la aplicación.";
				case MembershipCreateStatus.DuplicateEmail:
					return "La dirección de correo electrónico ya existe en la base de datos de la aplicación.";
				case MembershipCreateStatus.InvalidPassword:
					return "La contraseña no tiene el formato correcto.";
				case MembershipCreateStatus.InvalidAnswer:
					return "La respuesta de la contraseña no tiene el formato correcto.";
				case MembershipCreateStatus.InvalidQuestion:
					return "La pregunta de la contraseña no tiene el formato correcto.";
				case MembershipCreateStatus.InvalidUserName:
					return "No se encontró el nombre de usuario en la base de datos.";
				case MembershipCreateStatus.ProviderError:
					return "El proveedor devolvió un error no descrito por otros valores de la enumeración MembershipCreateStatus.";
				case MembershipCreateStatus.UserRejected:
					return "El usuario no se ha creado, por un motivo definido por el proveedor.";
				default:
					return "Un error desconocido ha ocurrido. Por favor verifique los datos enviados e intente nuevamente. Si el problema persiste por favor comunicarse con su administrador.";
			}
		}

		private JsonResult CreateJSon(string msg, string html, string result)
		{
			return Json(new
			{
				result = result,
				html = html,
				msg = msg
			});
		}
	}

	public interface IFormsAuthentication
	{
		void SignIn(string userName, bool createPersistentCookie);
		void SignOut();
	}

	public interface IAccountMembership
	{
		int MinPasswrodLength { get; }
		bool ValidateUser(string userName, string password);
		MembershipCreateStatus CreateUser(string userName, string password, string email);
		bool ChangePassword(string userName, string oldPassword, string newPasswrod);
	}

	public class AccountMembershipService : IAccountMembership
	{
		private MembershipProvider _provider;

		public int MinPasswrodLength
		{
			get
			{
				return _provider.MinRequiredPasswordLength;
			}
		}

		public AccountMembershipService() : this(null)
		{ }

		public AccountMembershipService(MembershipProvider provider)
		{
			_provider = provider ?? Membership.Provider;
		}

		public int MinPasswordLength
		{
			get {return _provider.MinRequiredPasswordLength;}
		}

		public bool ValidateUser(string userName, string password)
		{
			try
			{
				return _provider.ValidateUser(userName, password);
			}
			catch { return false; }
		}

		public MembershipCreateStatus CreateUser(string userName, string password, string email)
		{
			MembershipCreateStatus status;
			_provider.CreateUser(userName, password, email, null, null, false, null, out status);
			return status;
		}

		public bool ChangePassword(string userName, string oldPassword, string newPassword)
		{
			MembershipUser user = _provider.GetUser(userName, true);
			return user.ChangePassword(oldPassword, newPassword);
		}
	}

	public class FormsAuthenticationService : IFormsAuthentication
	{
		public void SignIn(string userName, bool createPersistentCookie)
		{
			FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
		}
		public void SignOut()
		{
			FormsAuthentication.SignOut();
		}

	}
}
