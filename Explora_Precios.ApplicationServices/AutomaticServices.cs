using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Core;
using Explora_Precios.Core.DataInterfaces;
using SharpArch.Core.PersistenceSupport;
using System.IO;
using System.Security.Cryptography;

namespace Explora_Precios.ApplicationServices
{
	public static class Helper
	{
		public static string RootFolder()
		{
			return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "").Replace("/bin", ""));
		}
		public static string Shorten(this string text, int lenght)
		{
			return text.Length >= lenght ? text.Substring(0, lenght) + "..." : text;
		}

		public static int CryptProductId(this string Id)
		{
			var productId = 0;
			if (!int.TryParse(Id, out productId))
			{
				productId = int.Parse(Id.DecryptString());
			}
			return productId;
		}

		public static string CryptProductId(this int Id)
		{
			return Id.ToString().EncryptString();
		}

		public static string EncryptString(this string val)
		{
			var key = System.Configuration.ConfigurationManager.AppSettings["PublicKey"];
			var encryptedValue = IdEncrypter.EncryptStringAES(val, key);
			return encryptedValue;
		}

		public static string DecryptString(this string val)
		{
			var key = System.Configuration.ConfigurationManager.AppSettings["PublicKey"];
			var decryptedValue = IdEncrypter.DecryptStringAES(val, key);
			return decryptedValue;
		}

		public static class IdEncrypter
		{
			private static byte[] _salt = Encoding.ASCII.GetBytes("o6806642kbM7c5");

			/// <summary>
			/// Encrypt the given string using AES.  The string can be decrypted using 
			/// DecryptStringAES().  The sharedSecret parameters must match.
			/// </summary>
			/// <param name="plainText">The text to encrypt.</param>
			/// <param name="sharedSecret">A password used to generate a key for encryption.</param>
			public static string EncryptStringAES(string value, string sharedSecret)
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value");
				if (string.IsNullOrEmpty(sharedSecret))
					throw new ArgumentNullException("sharedSecret");

				string outStr = null;                       // Encrypted string to return
				RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.

				try
				{
					// generate the key from the shared secret and the salt
					Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

					// Create a RijndaelManaged object
					// with the specified key and IV.
					aesAlg = new RijndaelManaged();
					aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
					aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

					// Create a decrytor to perform the stream transform.
					ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

					// Create the streams used for encryption.
					using (MemoryStream msEncrypt = new MemoryStream())
					{
						using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
						{
							using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
							{

								//Write all data to the stream.
								swEncrypt.Write(value);
							}
						}
						outStr = Convert.ToBase64String(msEncrypt.ToArray());
					}
				}
				finally
				{
					// Clear the RijndaelManaged object.
					if (aesAlg != null)
						aesAlg.Clear();
				}

				// Return the encrypted bytes from the memory stream.
				return outStr;
			}

			/// <summary>
			/// Decrypt the given string.  Assumes the string was encrypted using 
			/// EncryptStringAES(), using an identical sharedSecret.
			/// </summary>
			/// <param name="cipherText">The text to decrypt.</param>
			/// <param name="sharedSecret">A password used to generate a key for decryption.</param>
			public static string DecryptStringAES(string cipherText, string sharedSecret)
			{
				if (string.IsNullOrEmpty(cipherText))
					throw new ArgumentNullException("cipherText");
				if (string.IsNullOrEmpty(sharedSecret))
					throw new ArgumentNullException("sharedSecret");

				// Declare the RijndaelManaged object
				// used to decrypt the data.
				RijndaelManaged aesAlg = null;

				// Declare the string used to hold
				// the decrypted text.
				string plaintext = null;

				try
				{
					// generate the key from the shared secret and the salt
					Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

					// Create a RijndaelManaged object
					// with the specified key and IV.
					aesAlg = new RijndaelManaged();
					aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
					aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

					// Create a decrytor to perform the stream transform.
					ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
					// Create the streams used for decryption.                
					byte[] bytes = Convert.FromBase64String(cipherText);
					using (MemoryStream msDecrypt = new MemoryStream(bytes))
					{
						using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
						{
							using (StreamReader srDecrypt = new StreamReader(csDecrypt))

								// Read the decrypted bytes from the decrypting stream
								// and place them in a string.
								plaintext = srDecrypt.ReadToEnd();
						}
					}
				}
				finally
				{
					// Clear the RijndaelManaged object.
					if (aesAlg != null)
						aesAlg.Clear();
				}

				return plaintext;
			}
		}
	}

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

	public class CounterItem
	{
		public int Id { get; set; }
		public float Weight { get; set; }
		public DateTime Date { get; set; }
	}

	public class AutomaticServices
	{
		IProductRepository _productRepository;
		ICatalog_AddressRepository _catalogAddressRepository;
		IClient_ProductRepository _clientProductRepository;
		IProductLogRepository _productLogRepository;
		IUser_ProductRepository _userProductRepository;
		IList<FollowerDetails> _followers;
		IList<ProductLog> _productLogs;
		IList<Client_Product> _clientProductUpdated;
		IProductCounterRepository _productCounterRepository;
		IClientCounterRepository _clientCounterRepository;
		protected DateTime StartedAt { get; set; }

		public AutomaticServices(IClient_ProductRepository clientProductRepository, IProductRepository productRepository, IProductCounterRepository productCounterRepository,
			IClientCounterRepository clientCounterRepository)
		{
			_clientProductRepository = clientProductRepository;
			_productRepository = productRepository;
			_productCounterRepository = productCounterRepository;
			_clientCounterRepository = clientCounterRepository;
			StartedAt = DateTime.Now;
		}

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

		public bool UpdateCounters()
		{
			try
			{
				// Cargar informacion de archivos
				var ClientFilePath = Helper.RootFolder() + "\\Data\\clientcounter.txt";
				var fileLines = System.IO.File.ReadAllLines(ClientFilePath);
				var clientItems = fileLines.Length > 0 ? fileLines[0].Split(';').Select(i => new CounterItem { Id = int.Parse(i.Split(',')[0]), Weight = float.Parse(i.Split(',')[1]), Date = DateTime.Parse(i.Split(',')[2]) }) : null;
				var ProductFilePath = Helper.RootFolder() + "\\Data\\productcounter.txt";
				fileLines = System.IO.File.ReadAllLines(ProductFilePath);
				var productItems = fileLines.Length > 0 ? fileLines[0].Split(';').Select(i => new CounterItem { Id = int.Parse(i.Split(',')[0]), Weight = float.Parse(i.Split(',')[1]), Date = DateTime.Parse(i.Split(',')[2]) }) : null;

				if(clientItems != null)
				foreach (var counter in clientItems)
				{
					_clientCounterRepository.SaveOrUpdate(new ClientCounter
					{
						client = _clientProductRepository.Get(counter.Id),
						date = counter.Date,
						Type = CounterType.Client,
						weight = counter.Weight
					});
				}

				if (productItems != null)
				foreach (var counter in productItems)
				{
					_productCounterRepository.SaveOrUpdate(new ProductCounter
					{
						product = _productRepository.Get(counter.Id),
						date = counter.Date,
						Type = CounterType.Product,
						weight = counter.Weight
					});
				}

				System.IO.File.WriteAllText(ClientFilePath, "");
				System.IO.File.WriteAllText(ProductFilePath, "");
			}
			catch(Exception e)
			{
				var errorException = new Exception(e.Message + "<br/>" + e.StackTrace + "<br/>Duracion: <b>" + DateTime.Now.Subtract(StartedAt).TotalMinutes + " minutos</b>");
				throw errorException;
			}
			return true;
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
						var errorException = new Exception(e.Message + "<b> product_Id: " + currentProductId.ToString() + "</b><br/>" + e.StackTrace + "<br/>Duracion: <b>"+ DateTime.Now.Subtract(StartedAt).TotalMinutes + " minutos</b>");
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
						rows += "El producto <a href=\"http://www.exploraprecios.com?i=" + Details.ClientProduct.product.Id.CryptProductId() + "\">" + Details.ClientProduct.product.name.Shorten(25) + "</a> ";
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
				var FilePath = Helper.RootFolder() + "\\Content\\html\\follow.html";
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
