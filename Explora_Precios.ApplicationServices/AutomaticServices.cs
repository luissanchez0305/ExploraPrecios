using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Core;
using Explora_Precios.Core.DataInterfaces;
using SharpArch.Core.PersistenceSupport;
using System.IO;

namespace Explora_Precios.ApplicationServices
{
	public class AddressesReferences
	{
		public Catalog_Address Catalog { get; set; }
		public List<string> Addresses { get; set; }
		public List<string> References { get; set; }
	}

	public class FollowerDetails
	{
		public User User { get; set; }
		public User_Product UserProduct { get; set; }
		public Client_Product ClientProduct { get; set; }
		public float BeforePrice { get; set; }
		public float AfterPrice { get; set; }
		public bool IsSale { get; set; }
	}

	public class AutomaticServices
	{
		ICatalog_AddressRepository _catalogAddressRepository;
		IClient_ProductRepository _clientProductRepository;
		IProductLogRepository _productLogRepository;
		IUser_ProductRepository _userProductRepository;
		IList<FollowerDetails> _followers;
		IList<ProductLog> _productLogs;
		IList<Client_Product> _clientProductUpdated;
		protected DateTime StartedAt { get; set; }

		public AutomaticServices(IClient_ProductRepository clientProductRepository, ICatalog_AddressRepository catalogAddressRepository, 
			IProductLogRepository productLogRepository, IUser_ProductRepository userProductRepository)
		{
			_clientProductRepository = clientProductRepository;
			_catalogAddressRepository = catalogAddressRepository;
			_productLogRepository = productLogRepository;
			_userProductRepository = userProductRepository;
			_followers = new List<FollowerDetails>();
			_productLogs = new List<ProductLog>();
			_clientProductUpdated = new List<Client_Product>();
			StartedAt = DateTime.Now;
		}

		/// <summary>
		/// Automaticamente actualiza los productos que estan amarrados a una direccion en internet
		/// </summary>
		/// <param name="clientProducts">Lista con todos los productos</param>
		/// <returns>true si el procedimiento termino satisfactoriamente</returns>
		public bool UpdateProducts(IEnumerable<Client_Product> clientProducts)
		{
			var doUpdate = System.Configuration.ConfigurationManager.AppSettings["productIdentity"] == "0";
			var clientService = new ClientServices();
			var Catalog_Addresses_References = new List<AddressesReferences>();
			var currentProductId = 0;
			foreach (var clientProduct in clientProducts.Where(cp => !string.IsNullOrEmpty(cp.url)))
			{
				if (System.Configuration.ConfigurationManager.AppSettings["productIdentity"] != "0" && !doUpdate)
				{
					doUpdate = System.Configuration.ConfigurationManager.AppSettings["productIdentity"] == clientProduct.product.Id.ToString();
				}
				if (doUpdate && clientProduct.client.isActive)
				{
					try
					{
						currentProductId = clientProduct.product.Id;
						var catalogAddresses = _catalogAddressRepository.GetByClientCatalogDetails(clientProduct.client, clientProduct.product.level_Id, clientProduct.product.catalog_Id);
						if (catalogAddresses.Count > 0)
						{
							foreach (var catalogAddress in catalogAddresses)
							{
								var siteAdrsRefs = Catalog_Addresses_References.SingleOrDefault(sar => sar.Catalog.Id == catalogAddress.Id);
								if (siteAdrsRefs == null)
								{
									clientService.GetClientItems(catalogAddress.client, catalogAddress.level_Id, catalogAddress.catalog_Id, false);
									siteAdrsRefs = new AddressesReferences() { Catalog = catalogAddress, Addresses = clientService.productAddresses, References = clientService.productReferences };
									Catalog_Addresses_References.Add(siteAdrsRefs);
								}

								var modified = false;
								var activated = clientProduct.isActive;
								var currentAddressesReferences = Catalog_Addresses_References.SingleOrDefault(address => address.Catalog == catalogAddress);
								var indexOfAddressesReferences = currentAddressesReferences.References.IndexOf(clientProduct.productReference);
								if (indexOfAddressesReferences > -1)
									activated = true;
								else
									activated = false;

								if (activated != clientProduct.isActive)
									modified = true;

								if (indexOfAddressesReferences > -1 && currentAddressesReferences.Addresses.ElementAt(indexOfAddressesReferences) != clientProduct.url)
								{
									modified = true;
									clientProduct.url = currentAddressesReferences.Addresses.ElementAt(indexOfAddressesReferences);
								}

								ModifyProduct(clientService, clientProduct, modified);
							}
						}
						else
						{
							ModifyProduct(clientService, clientProduct, false);
						}
					}
					catch (Exception e)
					{
						var errorException = new Exception(e.Message + "<b> product_Id: " + currentProductId.ToString() + "</b><br/>" + e.StackTrace);
						throw errorException;
					}
				}
			}

			// introducir a product logs lo ultimos cambios
			foreach (var productLog in _productLogs)
			{
				_productLogRepository.Insert(productLog);
			}
			
			// actualizar los client products modificados
			foreach (var clientProduct in _clientProductUpdated)
			{
				_clientProductRepository.Update(clientProduct);
			}

			// envia el correo a todos usuarios que desean ser notificados de los cambios encontrados
			var priceFormat = "{0:C}";
			foreach (var Follower in _followers.GroupBy(follow => follow.User))
			{
				var rows = "";
				var ProductIntroduced = new List<int>();
				foreach (var Details in Follower)
				{
					if (!ProductIntroduced.Contains(Details.ClientProduct.product.Id))
					{
						rows += "<tr><td>";
						// producto
						rows += "El producto " + (Details.ClientProduct.product.name.Length > 25 ? Details.ClientProduct.product.name.Substring(0, 25) + "..." : Details.ClientProduct.product.name) + " ";
						// tienda
						rows += "de la tienda " + Details.ClientProduct.client.name + " ";
						// precio de
						rows += (Details.IsSale ? "a sido puesto en <b>oferta</b> de " : "a sido rebajado de ") + string.Format(priceFormat, Details.BeforePrice) + " ";
						// precio a
						rows += "a " + string.Format(priceFormat, Details.AfterPrice) + " ";
						rows += "</td></tr>";
						ProductIntroduced.Add(Details.ClientProduct.product.Id);
					}
				}

				var Subject = "ExploraPrecios.com - Cambio de Precios y Ofertas";
				var FilePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "").Replace("/bin", "")) + "\\Content\\html\\follow.html";
				var Html = File.ReadAllText(FilePath);
				Html = Html.Replace("<name>", Follower.Key.name).Replace("<rows>", rows);

				var EmailService = new EmailServices(Follower.Key.email, Subject, Html, true);
				//EmailService.Send();
			}
			_userProductRepository.Update(_followers.Select(follow => follow.UserProduct));

			return true;
		}

		private void ModifyProduct(ClientServices clientService, Client_Product clientProduct, bool modified)
		{
			var siteProduct = clientService.GetClientProduct(clientProduct.url, clientProduct.productReference, clientProduct.client, clientProduct.product.level_Id, clientProduct.product.catalog_Id);
			if (siteProduct != null)
			{
				var siteClientProduct = siteProduct.clients.SingleOrDefault(client => client.client.Id == clientProduct.client.Id);
				//siteClientProduct = TestCases(siteClientProduct, clientProduct);
				if (siteClientProduct.price != clientProduct.price || siteClientProduct.specialPrice != clientProduct.specialPrice || clientProduct.dateReported > clientProduct.dateModified)
				{
					var OriginalPrice = clientProduct.price;
					modified = true;
					_productLogs.Add(new ProductLog()
					{
						changeDate = DateTime.Now,
						fromPrice = clientProduct.price,
						fromSpecialPrice = clientProduct.specialPrice,
						toPrice = siteClientProduct.price,
						toSpecialPrice = siteClientProduct.specialPrice,
						operation = 0,
						user_Id = 1,
						client_Id = clientProduct.client.Id,
						product_Id = clientProduct.product.Id
					});
					clientProduct.price = siteClientProduct.price;
					clientProduct.specialPrice = siteClientProduct.specialPrice;

					// recolecta la informacion de los usuarios que deben ser notificados de este cambio en el precio
					var userProducts = _userProductRepository.GetByProductAndActive(clientProduct.product);
					if (userProducts.Count > 0)
					{
						// si es oferta se le avisan a todos los followers
						if (siteClientProduct.specialPrice > 0)
						{
							foreach (var Follower in userProducts)
							{
								Follower.NotifiedValue = siteClientProduct.specialPrice;
								Follower.Notified = DateTime.Now;
								_followers.Add(new FollowerDetails()
								{
									ClientProduct = clientProduct,
									User = Follower.user,
									IsSale = true,
									BeforePrice = clientProduct.price,
									AfterPrice = siteClientProduct.specialPrice,
									UserProduct = Follower
								});
							}
						}
						// si solo cambio el precio solo se les notifica a los que quieren saber del cambio de precio
						else
						{
							var PriceFollowers = userProducts.Where(up => up.Type == User_Product.RelationType.FollowDoesPriceWentDown && up.value == 1 && up.Notified < StartedAt && (up.NotifiedValue == 0 || up.NotifiedValue > siteClientProduct.price));

							foreach (var priceFollower in PriceFollowers)
							{
								priceFollower.NotifiedValue = siteClientProduct.price;
								priceFollower.Notified = DateTime.Now;
								_followers.Add(new FollowerDetails()
								{
									ClientProduct = clientProduct,
									User = priceFollower.user,
									IsSale = false,
									BeforePrice = OriginalPrice,
									AfterPrice = siteClientProduct.price,
									UserProduct = priceFollower
								});
							}
						}
					}
				}
			}
			else if(clientProduct.isActive)
			{
				modified = true;
				clientProduct.isActive = false;
			}

			if (modified)
			{
				clientProduct.dateModified = DateTime.Now;
				_clientProductUpdated.Add(clientProduct);
			}
		}

		private Client_Product TestCases(Client_Product siteClientProduct, Client_Product clientProduct)
		{
			if (clientProduct.product.Id == 1224)
			{
				// entra en oferta
				siteClientProduct.price = clientProduct.price - 1;
			}
			if (clientProduct.product.Id == 1228)
			{
				// bajo de precio
				siteClientProduct.price = clientProduct.price - 2;
			}
			if (clientProduct.product.Id == 595)
			{
				// esta en oferta
				siteClientProduct.specialPrice = clientProduct.price - 5;
			}

			return siteClientProduct;
		}
	}
}
