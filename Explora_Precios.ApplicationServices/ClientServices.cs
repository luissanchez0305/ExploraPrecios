using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Data;
using Explora_Precios.Core;
using System.Text.RegularExpressions;
using Explora_Precios.ApplicationServices.Interfaces;
using Explora_Precios.Core.DataInterfaces;
using System.Xml;
using Explora_Precios.Core.Helper;
//using System.Net;

namespace Explora_Precios.ApplicationServices
{
	public class CatalogItem
	{
		public int id { get; set; }
		public string name { get; set; }
		public string address { get; set; }
		public int levelId { get; set; }
		public int catalodId { get; set; }
	}

	public class ClientServices : IClientServices
	{
		private List<string> _productAddresses;
		private List<string> _productReferences;
		public List<string> productAddresses { get { return _productAddresses; } set { _productAddresses = value; } }
		public List<string> productReferences { get { return _productReferences; } set { _productReferences = value; } }
		CommonUtilities _commonObj = new CommonUtilities();
		IClientRepository _cRepository;
		IProductRepository _pRepository;
		IQualityRepository _qRepository;
		IBrandRepository _bRepository;

		public ClientServices() { }
		public ClientServices(IClientRepository cRepository, IProductRepository pRepository, IQualityRepository qRepository, IBrandRepository bRepository)
		{
			_cRepository = cRepository;
			_pRepository = pRepository;
			_qRepository = qRepository;
			_bRepository = bRepository;
		}

		#region Verificacion de catalogo

		public enum ItemType { OnSite_NotOnDB = 0, OnDB_NotOnSite = 1, Local = 2, Possible_Related = 3 }
		public enum PageStatus { PageNotSet = 0, PageChanged = 1, PageEqual = 2 }

		public bool ClientHasCatalog(Client client)
		{
			try
			{
				GetMenuAddresses(client, ItemType.Local, false);
				return true;
			}
			catch { return false; }
		}

		public List<CatalogItem> GetMenuAddresses(Client client, ItemType type)
		{
			return GetMenuAddresses(client, type, true);

		}

		public List<CatalogItem> GetMenuAddresses(Client client, ItemType type, bool getData)
		{
			var Items = new List<CatalogItem>();
			var _clientRepository = new ClientRepository();
			var masterAddress = "";
			var xhtmlProducts = "";
			if (getData)
			{
				masterAddress = "http://" + client.catalogAddress;
				xhtmlProducts = _commonObj.GetXHTML(masterAddress);
			}

			var catalogSite = new List<CatalogItem>();

			switch (client.Id)
			{
				case 1: // Tech and House
					if (getData)
					{
						var regExpression = "<a href=\"(?<address>.*)\"><span>(?<name>.*)</span></a>";
						var matchesTH = new Regex(regExpression).Matches(xhtmlProducts);
						foreach (Match match in matchesTH)
						{
							var matchValue = match.Groups["address"].Value;
							/*  solo en caso que el regular expression no separe 2 items del catalogo diferentes, se saca el address y el name del primero manualmente y se
							 * corre el regular expression nuevamente al segundo */
							var AnchorIndex = matchValue.LastIndexOf("<a");
							var name = "";
							var address = "";
							if (matchValue.Contains("<a"))
							{
								catalogSite.Add(new CatalogItem() { name = matchValue.Substring(matchValue.IndexOf("<span>") + 6, matchValue.IndexOf("</span>") - (matchValue.IndexOf("<span>") + 6)), address = matchValue.Substring(0, matchValue.IndexOf("\">")) });
								matchValue = matchValue.Substring(matchValue.IndexOf("</a>") + 4);
								var matchesTemp = new Regex("<a href=\"(?<address>.*)").Matches(matchValue);
								name = address = matchesTemp[0].Groups["address"].Value;
							}
							else
							{
								name = match.Groups["name"].Value;
								address = matchValue;
							}
							if (address.Length > 0 && !address.Contains("supermenu"))
								catalogSite.Add(new CatalogItem() { name = name, address = address.Replace("http://" + client.url, "") });
						}
					}
					break;
				case 3: // Panafoto
					if (getData)
					{
						var regExpression1 = "<a href=\"(?<address>.*)\" target=\"\".* alt=\"(?<name>.*)\">";
						var matchesPF = new Regex(regExpression1).Matches(xhtmlProducts);

						foreach (Match match in matchesPF)
						{
							var name = "";
							var address = "";
							if (match.Groups["address"].Value.Contains("cat/prods"))
							{
								name = match.Groups["name"].Value;
								address = match.Groups["address"].Value;
								address = address.Contains("class") ? address.Substring(0, address.IndexOf("\"")) : address;
							}
							if (address.Length > 0)
								catalogSite.Add(new CatalogItem() { name = name, address = address.Replace("http://" + client.url, "") });
						}
					}
					break;
				case 5: // Raenco
					if (getData)
					{
						catalogSite = RaencoReadCatalog(xhtmlProducts).Where(cat => !cat.address.Contains(client.url) && cat.address.Length > 1 && !cat.address.Contains("facebook")).ToList();
					}
					break;
				case 8: // Multimax
					if (getData)
					{
						catalogSite = MultimaxCatalog(xhtmlProducts);
					}
					break;
				case 11: // Tecnisoft
					if (getData)
					{
						catalogSite = TecniSoftCatalog(xhtmlProducts);
					}
					break;
				case 12: // Mac Store
					if (getData) 
					{
						catalogSite = MacStoreCatalog(xhtmlProducts).Where(cat => !cat.address.Contains("#") && cat.address.Contains("php")).ToList();
					}
					break;
				case 15: // Yoytec
					if (getData)
					{
						catalogSite = YoyTecCatalog(xhtmlProducts).Where(cat => cat.name.Length>0 && cat.address.Contains("cPath")).ToList();
					}
					break;
				default:
					NotSet();
					break;

			}

			if(type == ItemType.OnDB_NotOnSite)
			// Buscar items del catalogo de clientes en los items del catalogo del site
				foreach (var cc_item in client.catalog)
				{
					var temp = catalogSite.Where(site_item =>
						site_item.address.Length > cc_item.url.Length ?
							site_item.address.Contains(cc_item.url) :
							cc_item.url.Contains(site_item.address));
					if (temp.Count() == 0)
					{
						Items.Add(new CatalogItem() { address = cc_item.url, name = "", id = cc_item.Id, levelId = cc_item.level_Id, catalodId = cc_item.catalog_Id });
					}
				}
			else if (type == ItemType.OnSite_NotOnDB)
				foreach (var site_item in catalogSite)
				{
					var temp = client.catalog.Where(cc_item =>
						site_item.address.Length > cc_item.url.Length ?
							site_item.address.Contains(cc_item.url) :
							cc_item.url.Contains(site_item.address));
					if (temp.Count() == 0)
					{
						Items.Add(new CatalogItem() { address = site_item.address, name = site_item.name });
					}
				}
			return Items;
		}

		private List<CatalogItem> MultimaxCatalog(string xhtmlProducts)
		{
			var catalogSite = new List<CatalogItem>();
			var readerProducts = System.Xml.XmlReader.Create(new System.IO.StringReader(xhtmlProducts));
			
			readerProducts.MoveToContent();
			while (readerProducts.Read())
			{
				if (readerProducts.AttributeExists("div", "id", "menu", true))
				{
					while (readerProducts.Read())
					{
						if (readerProducts.IsTagElement("a"))
						{
							var address = readerProducts.GetAttribute("href");
							readerProducts.Read();
							catalogSite.Add(new CatalogItem()
							{
								name = readerProducts.Value,
								address = address.Replace("sortci=price+asc","")
							});

						}

						if (readerProducts.IsTagEndElement("div")) { break; }
					}
					break;
				}
			}

			return catalogSite;
		}

		private List<CatalogItem> YoyTecCatalog(string xhtmlProducts)
		{
			var catalogSite = new List<CatalogItem>();
			var readerProducts = System.Xml.XmlReader.Create(new System.IO.StringReader(xhtmlProducts));

			readerProducts.MoveToContent();
			while (readerProducts.Read())
			{
				if (readerProducts.AttributeExists("td", "class", "boxText", true))
				{
					while (readerProducts.Read())
					{
						if (readerProducts.IsTagElement("a"))
						{
							var address = readerProducts.GetAttribute("href");
							readerProducts.Read();
							catalogSite.Add(new CatalogItem()
							{
								name = readerProducts.Value.Replace("->",""),
								address = address.IndexOf("?osCsid") > 0 ? address.Substring(0, address.IndexOf("?osCsid")) : address
							});

							if (readerProducts.Value.Contains("->"))
							{
								// leyendo la subcategoria 1
								var xhtmlSubCat1 = _commonObj.GetXHTML(address);
								var readerSubCat1 = System.Xml.XmlReader.Create(new System.IO.StringReader(xhtmlSubCat1));
								readerSubCat1.MoveToContent();
								var isInsideSubCat1 = false;
								while (readerSubCat1.Read())
								{
									if (readerSubCat1.IsTagElement("b"))
									{
										readerSubCat1.Read();
										if (readerSubCat1.NodeType == XmlNodeType.Text && readerSubCat1.Value == readerProducts.Value.Replace("->", ""))
										{
											isInsideSubCat1 = true;
											readerSubCat1.Read(); readerSubCat1.Read();
											while (readerSubCat1.Read())
											{
												if (readerSubCat1.IsTagElement("a"))
												{
													address = readerSubCat1.GetAttribute("href");
													readerSubCat1.Read();
													catalogSite.Add(new CatalogItem()
													{
														name = readerProducts.Value.Replace("->","") + " - " + readerSubCat1.Value.Replace("->", ""),
														address = address.IndexOf("?osCsid") > 0 ? address.Substring(0, address.IndexOf("?osCsid")) : address
													});
													if (!address.Contains("_"))
														break;
												}

												if (readerSubCat1.NodeType == XmlNodeType.Text && readerSubCat1.Value.Contains("->")) 
												{                                                
													// leyendo la subcategoria 2
													var xhtmlSubCat2 = _commonObj.GetXHTML(address);
													var readerSubCat2 = System.Xml.XmlReader.Create(new System.IO.StringReader(xhtmlSubCat2));
													readerSubCat2.MoveToContent();
													var isInsideSubCat2 = false;
													while (readerSubCat2.Read())
													{
														if (readerSubCat2.IsTagElement("b"))
														{
															readerSubCat2.Read();
															if (readerSubCat2.NodeType == XmlNodeType.Text && readerSubCat2.Value == readerSubCat1.Value.Replace("->", ""))
															{
																isInsideSubCat2 = true;
																readerSubCat2.Read(); readerSubCat2.Read();
																while (readerSubCat2.Read())
																{
																	if (readerSubCat2.IsTagElement("a"))
																	{
																		address = readerSubCat2.GetAttribute("href");
																		readerSubCat2.Read();
																		catalogSite.Add(new CatalogItem()
																		{
																			name = readerSubCat1.Value.Replace("->", "") + " - " + readerSubCat2.Value.Replace("->", ""),
																			address = address.IndexOf("?osCsid") > 0 ? address.Substring(0, address.IndexOf("?osCsid")) : address
																		});

																		if (address.IndexOf("_") == address.LastIndexOf("_"))
																			break;
																	}
																}
															}
														}
														if (isInsideSubCat2)
														{
															isInsideSubCat2 = false;
															break;
														}
													}
												}
											}
										}
									}
									if (isInsideSubCat1)
									{
										isInsideSubCat1 = false;
										break;
									}
								}
							}
						}

						if (readerProducts.IsTagEndElement("td"))
							break;
					}                    
				}
			}

			return catalogSite;
		}

		private List<CatalogItem> TecniSoftCatalog(string xhtmlProducts)
		{
			var catalogSite = new List<CatalogItem>();
			var readerProducts = System.Xml.XmlReader.Create(new System.IO.StringReader(xhtmlProducts));

			readerProducts.MoveToContent();
			while (readerProducts.Read())
			{
				if (readerProducts.AttributeExists("ul", "class", "CPcatDescList", true))
				{
					var ulIndex = 0;
					while (readerProducts.Read())
					{
						if (readerProducts.AttributeExists("ul", "class", "CPcatDescList", true))
							ulIndex++;

						if (readerProducts.AttributeExists("span", "class", "CPcatDescProd", true))
						{
							while (readerProducts.Read())
							{
								if (readerProducts.IsTagElement("a"))
								{
									var catalogItem = new CatalogItem() { address = "/" + readerProducts.GetAttribute("href") };
									readerProducts.Read();
									catalogItem.name = readerProducts.Value;
									catalogSite.Add(catalogItem);
									break;
								}
								if (readerProducts.IsTagEndElement("a"))
									break;
							}
						}

						if (readerProducts.IsTagEndElement("ul") && ulIndex == 0)
							break;

						if (readerProducts.IsTagEndElement("ul"))
							ulIndex--;
					}
				}
			}

			return catalogSite;
		}

		private List<CatalogItem> MacStoreCatalog(string xhtmlProducts)
		{
			var catalogSite = new List<CatalogItem>();
			var readerProducts = System.Xml.XmlReader.Create(new System.IO.StringReader(xhtmlProducts));

			readerProducts.MoveToContent();
			while (readerProducts.Read())
			{
				if (readerProducts.AttributeExists("ul", "class", "topnav", true))
				{
					// dentro de topnav
					while (readerProducts.Read())
					{
						if (readerProducts.IsTagElement("li")) 
						{
							while (readerProducts.Read())
							{ 
								if(readerProducts.IsTagElement("ul"))
								{
									// dentro de subnav
									while (readerProducts.Read())
									{
										if (readerProducts.IsTagElement("li"))
										{
											var catalogItem = new CatalogItem();
											while (readerProducts.Read())
											{
												if (readerProducts.IsTagElement("a"))
												{
													catalogItem.address = readerProducts.GetAttribute("href");
													readerProducts.Read();
													catalogItem.name = readerProducts.Value;
													catalogSite.Add(catalogItem);
													//2070385
												}

												if (readerProducts.IsTagEndElement("a"))
													break;
											}
										}
									}
								}
								if (readerProducts.IsTagEndElement("ul"))
									break;
							}
						}
						if (readerProducts.IsTagEndElement("li"))
							break;
					}
				}

				if (readerProducts.IsTagEndElement("ul"))
				{ break; }
			}
			return catalogSite;
		}

		private List<CatalogItem> RaencoReadCatalog(string xhtmlProducts)
		{
			var catalogSite = new List<CatalogItem>();
			var readerProducts = System.Xml.XmlReader.Create(new System.IO.StringReader(xhtmlProducts));

			readerProducts.MoveToContent();
			while (readerProducts.Read())
			{
				if (readerProducts.AttributeExists("ul", "class", "menu", true))
				{
					while (readerProducts.Read())
					{
						if (readerProducts.AttributeExists("li", "class", "item", false))
						{
							// primer nivel (parent)
							while (readerProducts.Read())
							{
								if (readerProducts.IsTagElement("a"))
								{
									var catalogItem = new CatalogItem() { address = readerProducts.GetAttribute("href") };
									while (readerProducts.Read())
									{
										if (readerProducts.NodeType == XmlNodeType.Text)
										{
											catalogItem.name = readerProducts.Value;
											break;
										}
									}
									catalogSite.Add(catalogItem);
								}

								if (readerProducts.IsTagElement("ul"))
								{
									// segundo nivel
									while (readerProducts.Read())
									{
										if (readerProducts.AttributeExists("li", "class", "item", false))
										{
											while (readerProducts.Read())
											{
												if (readerProducts.IsTagElement("a"))
												{
													var catalogItem = new CatalogItem() { address = readerProducts.GetAttribute("href") };
													while (readerProducts.Read())
													{
														if (readerProducts.NodeType == XmlNodeType.Text)
														{
															catalogItem.name = readerProducts.Value;
															break;
														}
													}
													catalogSite.Add(catalogItem);
												}
											}
										}

										if (readerProducts.IsTagElement("ul"))
										{
											// tercer nivel
											while (readerProducts.Read())
											{
												if (readerProducts.AttributeExists("li", "class", "item", false))
												{
													while (readerProducts.Read())
													{
														if (readerProducts.IsTagElement("a"))
														{
															var catalogItem = new CatalogItem() { address = readerProducts.GetAttribute("href") };
															while (readerProducts.Read())
															{
																if (readerProducts.NodeType == XmlNodeType.Text)
																{
																	catalogItem.name = readerProducts.Value;
																	break;
																}
															}
															catalogSite.Add(catalogItem);
														}
													}
												}

												if (readerProducts.IsTagElement("ul"))
												{
													// cuarto nivel
													while (readerProducts.Read())
													{
														if (readerProducts.AttributeExists("li", "class", "item", false))
														{
															while (readerProducts.Read())
															{
																if (readerProducts.IsTagElement("a"))
																{
																	var catalogItem = new CatalogItem() { address = readerProducts.GetAttribute("href") };
																	while (readerProducts.Read())
																	{
																		if (readerProducts.NodeType == XmlNodeType.Text)
																		{
																			catalogItem.name = readerProducts.Value;
																			break;
																		}
																	}
																	catalogSite.Add(catalogItem);
																}
															}
														}

														if (readerProducts.IsTagEndElement("li"))
														{
															break;
														}
													}
												}

												if (readerProducts.IsTagEndElement("li"))
												{
													break;
												}
											}
										}

										if (readerProducts.IsTagEndElement("li"))
										{
											break;
										}
									}
								}

								if (readerProducts.IsTagEndElement("li"))
								{
									break;
								}
							}
						}

						if (readerProducts.IsTagEndElement("li"))
						{
							break;
						}
					}
				}

				if (readerProducts.IsTagEndElement("ul"))
				{
					break;
				}
			}
			return catalogSite;
		}

		private void NotSet()
		{
			throw new Exception("No ha sido configurado aun");
		}

		#endregion

		#region Obtencion de productos individuales
		public Product GetProduct(string url, out bool isNew)
		{
			isNew = true;
			var clientDomain = "";
			var clientObj = _cRepository.GetByUrl(url, out clientDomain);
			var productObj = new Product();

			if (clientObj != null)
			{
				var xhtmlProducts = _commonObj.GetXHTML(url);
				var regex_name = " ";
				var regex_reference = "";
				var regex_price = "";
				var regex_qualities = "";
				var regex_image = "";
				var regex_brand = "";
				// Dependiendo de que cliente es usa un regular expression diferente
				switch (clientObj.Id)
				{
					// Tech and house
					case 1:
						regex_name = "<h3\\sclass=\"product-name\">(?<name>.*)</h3>";
						regex_reference = "SKU:\\s(?<reference>.*)(<br\\s/><br\\s/>)";
						regex_price = "<span\\sclass=\"price\">US\\x24\\s(?<price>.*)(</span></span>)";
						regex_qualities = "<tr>\\n\\s*<td\\s.*>(?<name>.*)</td>\\n\\s*<td\\s.*>(?<quality>.*)</td>";
						regex_image = "<img\\ssrc=\"(?<image>.*.jpg)\"\\salt.*/>";
						regex_brand = "<img\\sid='brandimage'\\s.*alt='(?<brand>.*)'></a>";
						break;
					// Panafoto
					case 3:
						regex_name = "<div\\sclass=\"titulo\"><h1>(?<name>.*)<br.*";
						regex_reference = "<br.*><h3>\\x28(?<reference>.*)\\x29</h3>";
						regex_price = "<h2>.*\\n\\s*\\x24(?<price>.*)\\sUSD";
						regex_qualities = "<img\\ssrc=[\"-'].*bullet.gif[\"-']\\s.*>(?<name>\\s)\\s*(?<quality>\\s.*)\\s<br\\s.*";
						regex_image = "<div\\sclass=\"imagenes\"\\s.*>\\s*\\n\\s*<img\\ssrc=\"(?<image>.*)\"\\salign.*/>";
						break;
					// Photura
					case 4:
						regex_name = "<b><strong>(?<name>.*)[:-]\\s.*<br />|<b><strong>(?<name>.*)[:-]\\s.*</strong>";
						regex_reference = "(<b><strong>.*[-:]\\s)(?<reference>[a-zA-Z0-9\x2d]*)<br\\s/>|(<b><strong>.*[-:]\\s)(?<reference>[a-zA-Z0-9\\x2d]*)</strong>";
						regex_price = "<span>\\x24(?<price>.*)</span>";
						regex_qualities = "�(?<name>\\s)(?<quality>.*)";
						regex_image = "<img src=\"(?<image>.*)\"\\sborder=.*\\s*hspace.*\"";
						break;
				}

				// se obtienen todos los valores de la pagina
				var referenceCodeMatch = new Regex(regex_reference).Matches(xhtmlProducts);
				var productReference = referenceCodeMatch.Count > 0 ? referenceCodeMatch[0].Groups["reference"].Value : "";

				var priceMatch = new Regex(regex_price).Matches(xhtmlProducts);
				// al traer precios con valores mayores a mil es posible que tengan . para separar los miles y , para separar los decimales
				// float acepta coma para separar los miles y . para los decimales, CheckPrice hace esto
				var price = priceMatch.Count > 0 ? float.Parse(CheckPrice(priceMatch[0].Groups["price"].Value)) : 0;

				var nameMatch = new Regex(regex_name).Matches(xhtmlProducts);
				var productName = nameMatch.Count > 0 ? nameMatch[0].Groups["name"].Value : "";

				var brandMatch = new Regex(regex_brand).Matches(xhtmlProducts);
				var brandName = brandMatch.Count > 0 ? brandMatch[0].Groups["brand"].Value : "";

				var imageMatch = new Regex(regex_image).Matches(xhtmlProducts);
				var imageUrl = imageMatch.Count > 0 ? imageMatch[0].Groups["image"].Value : "";
				// se refina el url de la imagen en caso que venga incompleta se le agrega el dominio a que pertenece
				imageUrl = !imageUrl.Contains("http") ? "http://" + clientDomain + (imageUrl.Substring(0,1) != "/" ? "/" : "") + imageUrl : imageUrl;

				// se busca el producto por si no ha sido ingresado con anterioridad
				//if (string.IsNullOrEmpty(productReference))
				//    throw new Exception("No reference retrieved");

				if (!string.IsNullOrEmpty(productReference))
					productObj = _pRepository.GetbyReference(productReference);

				if (productObj != null)
				{
					isNew = false;
					var clientFound = false;
					foreach (var item in productObj.clients)
					{
						if (item.client == clientObj)
						{
							clientFound = true;
							item.price = price;
							break;
						}
					}
					if (!clientFound)
					{
						productObj.clients.Add(new Client_Product()
						{
							client = clientObj,
							price = price,
							url = url,
							isActive = true
						});
					}
				}
				// producto nuevo
				else
				{
					// crear producto nuevo
					productObj = new Product()
					{
						productReference = productReference,
						name = productName,
						clients = new List<Client_Product>(),
						qualities = new List<Product_Quality>()
					};

					// ingresar cliente, precio y direccion del producto
					productObj.clients.Add(new Client_Product()
					{
						url = url,
						price = price,
						client = clientObj,
						isActive = true
					});

					// ingresar las caracteristicas del producto
					var qualities = new Regex(regex_qualities).Matches(xhtmlProducts);
					foreach (Match quality in qualities)
					{
						var qualityName = quality.Groups["name"].Value;
						var qualityValue = quality.Groups["quality"].Value;
						var qualityObj = _qRepository.getByName(qualityName);
						productObj.qualities.Add(new Product_Quality()
						{
							quality = qualityObj != null ? qualityObj : new Quality() { name = qualityName },
							value = qualityValue
						});
					}

					if (brandName.Length > 0)
					{
						var brand = _bRepository.GetByBrandName(brandName);
						productObj.brand = brand != null ? brand : new Brand() { name = brandName };
					}

					productObj.image = new Image()
					{
						url = imageUrl
					};
				}
			}

			return productObj;
		}

		private string CheckPrice(string price)
		{
			if (price.Contains(',') && price.Contains('.') && price.IndexOf(',') > price.IndexOf('.'))
			{
				var pointIndex = price.IndexOf('.');
				price = price.Replace(',', '.');
				price = price.Remove(pointIndex, 1);
				price = price.Insert(pointIndex, ",");
			}
			else if (price.Contains(',') && !price.Contains('.'))
				price = price.Replace(',', '.');
			return price;
		}
		#endregion

		#region Obtencion masiva de productos

		public bool ClientHasProduct(Client client)
		{
			try
			{
				GetClientItems(client);
				return true;
			}
			catch { return false; }
		}

		public List<Product> GetClientItems(Client client)
		{
			return GetClientItems(client, null, null, false);
		}

		public List<Product> GetClientItems(Client client, int? levelId, int? catalogId, bool getProductObjects)
		{
			var _catAddresRepository = new Catalog_AddressRepository();
			var _catalog_addressObj = levelId.HasValue ? _catAddresRepository.GetByClientCatalogDetails(client, levelId.Value, catalogId.Value) : 
				new List<Catalog_Address>();
			switch (client.Id)
			{
				case 1:
					if (levelId.HasValue)
						return GetTechAndHouseItems(_catalog_addressObj, getProductObjects).ToList();
					break;
				case 3:
					if (levelId.HasValue)
						return GetPanaFotoItems(_catalog_addressObj, getProductObjects).ToList();
					break;
				case 8:
					if (levelId.HasValue)
						return GetMultimaxItems(_catalog_addressObj, getProductObjects).ToList();
						break;
				case 15:
					if (levelId.HasValue)
						return GetYoyTecItems(_catalog_addressObj, getProductObjects).ToList();
					break;
				case 12:
					if (levelId.HasValue)
						return GetMacStoreItems(_catalog_addressObj, getProductObjects).ToList();
					break;
				default:
					NotSet();
					break;
			}

			return null;
		}

		public Product GetClientProduct(string url, string reference, Client client, int level_Id, int catalog_Id) 
		{

			switch (client.Id)
			{
				case 1:
					return GetTechAndHouseProduct(url, client, level_Id, catalog_Id);
				case 3:
					return GetPanaFotoProduct(url, client, level_Id, catalog_Id);
				case 15:
					return GetYoyTecProduct(url, client, level_Id, catalog_Id);
				case 12:
					var _catAddresRepository = new Catalog_AddressRepository();
					var _catalog_addressObj = _catAddresRepository.GetByClientCatalogDetails(client, level_Id, catalog_Id);
					return GetMacStoreItems(_catalog_addressObj, false).SingleOrDefault(product => product.productReference == reference);
				default:
					NotSet();
					break;
			}
			return null;
		}

		private List<Product> GetMultimaxItems(IList<Catalog_Address> _catalog_addressesObj, bool getProductObjects)
		{
			productAddresses = new List<string>();
			productReferences = new List<string>();
			var productList = new List<Product>();
			
			var pageLimit = 25;
			if (_catalog_addressesObj != null)
			{
				foreach (var _catalog_addressObj in _catalog_addressesObj)
				{
					var goOn = true;
					var page = 1;
					var masterAddress = "http://"+_catalog_addressObj.client.url + _catalog_addressObj.url;
					var regex_products = "<div class=\"item\">";
					while (goOn)
					{
						List<string> listAddresses = new List<string>();
						var xhtmlProducts = _commonObj.GetXHTML(masterAddress + "&mpv=childItemsDTO&mpvp=" + page.ToString() + "&pageItemsCount=" + pageLimit);
						int productsCount = new Regex(regex_products).Matches(xhtmlProducts).Count;

						var readerProducts = System.Xml.XmlReader.Create(new System.IO.StringReader(xhtmlProducts));
						
						// Primero recopilamos todas las direcciones de los productos
						readerProducts.MoveToContent();
						while (readerProducts.Read())
						{
							switch (readerProducts.NodeType)
							{
								case XmlNodeType.Element:
									if (readerProducts.AttributeExists("div", "class", "item", true))
									{
										// sacar el xhtml del producto
										var productUrl = "";
										var productRef = "";
										var productPrice = "";
										var productImageUrl = "";
										while (readerProducts.Read())
										{
											if (readerProducts.AttributeExists("div", "class", "image", true))
											{
												while (readerProducts.Read())
												{
													if (readerProducts.NodeType == XmlNodeType.Element && readerProducts.Name == "a" && string.IsNullOrEmpty(productUrl))
													{
														productUrl = "http://" + _catalog_addressObj.client.url + "/" + readerProducts.GetAttribute("href");
													}
													if (readerProducts.IsTagEndElement("div"))
														break;
												}
											}
											if (readerProducts.AttributeExists("div", "class", "title", true))
											{
												while (readerProducts.Read())
												{
													if (readerProducts.NodeType == XmlNodeType.Element && readerProducts.Name == "a" && string.IsNullOrEmpty(productRef))
													{
														readerProducts.Read();
														productRef = readerProducts.Value;
													}

													if (readerProducts.IsTagEndElement("div"))
														break;
												}
											}
											if (readerProducts.AttributeExists("div", "class", "actions", true))
											{
												while (readerProducts.Read())
												{
													if (readerProducts.AttributeExists("div", "class", "price", true))
													{
														while (readerProducts.Read())
														{
															if (readerProducts.AttributeExists("div", "class", "integer", true))
															{
																readerProducts.Read();
																productPrice = readerProducts.Value;
															}
															if (readerProducts.IsTagEndElement("div"))
																break;
														}
														break;
													}
												}
												break;
											}
										}

										var xhtmlProduct = _commonObj.GetXHTML(productUrl);
										var readerProduct = System.Xml.XmlReader.Create(new System.IO.StringReader(xhtmlProduct));
										readerProduct.MoveToContent();
										while (readerProduct.Read() && productImageUrl.Length == 0)
										{
											if (readerProduct.AttributeExists("div", "class", "preview", true))
											{
												while (readerProduct.Read())
												{
													if (readerProduct.NodeType == XmlNodeType.Element && readerProduct.Name == "img")
													{
														var src = readerProduct.GetAttribute("src");
														productImageUrl = src != "store$item.itemimage" ? "http://" + _catalog_addressObj.client.url + "/" + src : "";
														break;
													}
												}
												break;
											}
										}

										var product = new Product
										{
											productReference = productRef,
											catalog_Id = _catalog_addressObj.catalog_Id,
											level_Id = _catalog_addressObj.level_Id,
											image = new Image { url = productImageUrl },
											name = productRef
										};
										product.clients.Add(new Client_Product { client = _catalog_addressObj.client, price = float.Parse(productPrice), url = productUrl, productReference = "", isActive = true });
										productList.Add(product);
									}
									break;
							}
						}
						
						if (productsCount < pageLimit)
							goOn = false;
						else
							page += 1;
					}// end while goOn
				}
			}

			return productList;
		}

		private List<Product> GetMacStoreItems(IList<Catalog_Address> _catalog_addressesObj, bool getProductObjects)
		{
			productAddresses = new List<string>();
			productReferences = new List<string>();
			var productList = new List<Product>();

			if (_catalog_addressesObj != null)
			{
				foreach (var _catalog_addressObj in _catalog_addressesObj)
				{
					var masterAddress = "http://" + _catalog_addressObj.client.url + _catalog_addressObj.url;

					string regex_tables = "<div\\sid=\"inline\">";
					var xhtmlProducts = _commonObj.GetXHTML(masterAddress);
					var htmlMatches = new Regex(regex_tables).Matches(xhtmlProducts);

					var readerProducts = System.Xml.XmlReader.Create(new System.IO.StringReader(xhtmlProducts));
					
					readerProducts.MoveToContent();
					// Parse the file and display each of the nodes.
					while (readerProducts.Read())
					{
						var imageUrl = "";
						// Found the image div
						if (readerProducts.AttributeExists("div", "class", "box", true))
						{
							while (readerProducts.Read())
							{
								if (readerProducts.IsTagElement("img"))
								{
									imageUrl = _catalog_addressObj.client.url + readerProducts.GetAttribute("src");
									break;
								}
							}
						}   

						// Found the products div
						if (readerProducts.AttributeExists("div", "id", "specs", true))
						{
							while (readerProducts.Read())
							{
								// Found one product
								if (readerProducts.IsTagElement("li"))
								{
									var product = new Product();
									
									var name = "";
									var url = masterAddress;
									var reference = "";
									var addProduct = true;

									var client_product = new Client_Product()
									{
										client = _catalog_addressObj.client,
										isActive = true,
										url = masterAddress,
										price = 0
									};

									while (readerProducts.Read())
									{
										var qualities = new List<Product_Quality>();  
										if (readerProducts.AttributeExists("span", "class", "specs-title", true))
										{
											readerProducts.Read();
											name = readerProducts.Value;
											readerProducts.Read();
											readerProducts.Read();
											if (readerProducts.NodeType == XmlNodeType.Text)
												reference = readerProducts.Value;
											// search for reference and qualitites
											while (readerProducts.Read())
											{
												if (readerProducts.IsTagElement("strong") && string.IsNullOrEmpty(reference))
												{
													while (readerProducts.Read())
													{
														if (readerProducts.IsTagEndElement("strong"))
														{
															readerProducts.Read();
															reference = readerProducts.Value;
															break;
														}
													}
												}

												if (!string.IsNullOrEmpty(reference))
												{
													var isQuality = true;       
													while (readerProducts.Read())
													{
														if (readerProducts.NodeType == XmlNodeType.Text)
														{
															if (isQuality)
																qualities.Add(new Product_Quality() { quality = new Quality() { name = "" }, value = readerProducts.Value.Replace("»", "").Trim() });
														}
														if (readerProducts.AttributeExists("span", "class", "price", true))
														{ 
															readerProducts.Read();
															addProduct = readerProducts.Value.Contains("$");
															if (addProduct)
															{
																client_product.price = float.Parse(readerProducts.Value.Substring(readerProducts.Value.IndexOf("$") + 1));
																product.qualities = qualities;
															}
															client_product.productReference = reference.Trim();
															break;
														}
														else if (readerProducts.IsTagElement("span"))
														{
															isQuality = false;
														}
														else if (readerProducts.IsTagEndElement("span"))
														{
															isQuality = true;
														}
													}
												}
												if (client_product.price > 0 || !addProduct)
													break;
											}
										}

										if (readerProducts.IsTagEndElement("li") && addProduct)
										{
											product.level_Id = _catalog_addressObj.level_Id;
											product.catalog_Id = _catalog_addressObj.catalog_Id;
											product.brand = new Brand() { name = "Apple" };
											product.productReference = reference.Trim();
											product.name = name;
											product.image = new Image() { url = imageUrl };
											product.clients.Add(client_product);
											productList.Add(product);
											break;
										}
									}

								}
								if (readerProducts.IsTagEndElement("ul"))
									break;
							}
						}
						if (readerProducts.IsTagEndElement("div"))
							break;
					}
				}
			}

			return productList;
		}

		private List<Product> GetYoyTecItems(IList<Catalog_Address> _catalog_addressesObj, bool getProductObjects)
		{
			productAddresses = new List<string>();
			productReferences = new List<string>();
			var productList = new List<Product>();
			var pageLimit = 100;

			if (_catalog_addressesObj != null)
			{
				foreach (var _catalog_addressObj in _catalog_addressesObj)
				{
					var goOn = true;
					var page = 1;
					string regex_products = "<td\\sclass=\"productListingPrice\".*>";
					while (goOn)
					{
						List<string> listAddresses = new List<string>();
						var xhtmlProducts = _commonObj.GetXHTML(_catalog_addressObj.url + "/page/" + page.ToString());
						int productsCount = new Regex(regex_products).Matches(xhtmlProducts).Count;

						var readerProducts = System.Xml.XmlReader.Create(new System.IO.StringReader(xhtmlProducts));

						// Primero recopilamos todas las direcciones de los productos
						readerProducts.MoveToContent();
						while (readerProducts.Read())
						{
							if (readerProducts.AttributeExists("td", "class", "productListing-small", true))
							{
								while (readerProducts.Read())
								{
									if (readerProducts.IsTagElement("a") && !readerProducts.GetAttribute("href").Contains("buy"))
									{
										var address = readerProducts.GetAttribute("href");
										address = address.Substring(0, address.IndexOf("?"));
										if (!listAddresses.Contains(address))
											listAddresses.Add(address);
										break;
									}
									if (readerProducts.IsTagEndElement("td"))
										break;
								}

							}
						}

						// recorremos las direcciones de direcciones y sacamos la informacion de todos los productos
						foreach (var address in listAddresses)
						{
							try
							{
								var product = GetYoyTecProduct(address, _catalog_addressObj.client, _catalog_addressObj.level_Id, _catalog_addressObj.catalog_Id);

								if (product != null)
								{
									if (!string.IsNullOrEmpty(product.clients[0].productReference) && !productAddresses.Contains(product.clients[0].productReference))
									{
										productReferences.Add(product.clients[0].productReference);
										productAddresses.Add(product.clients[0].url);
									}
									else if (!string.IsNullOrEmpty(product.clients[0].url))
									{
										goOn = false; break;
									}

									productList.Add(product);
									System.Threading.Thread.Sleep(10000);
								}
							}
							catch (Exception e)
							{
								throw e;
							}
						}
						
						if (productsCount < pageLimit)
							goOn = false;
						else
							page += 1;
					}
				}             
			}

			return productList;
		}

		private Product GetYoyTecProduct(string address, Client client, int level_Id, int catalog_Id)
		{
			var xhtmlProduct = _commonObj.GetXHTML(address);
			if (xhtmlProduct.Contains("No encontrado!"))
				return null;
			var readerProduct = System.Xml.XmlReader.Create(new System.IO.StringReader(xhtmlProduct));

			var title = "";
			var reference = "";
			var price = new List<Client_Product>();
			var qualities = new List<Product_Quality>();
			var imageUrl = "";
			var brand = "";

			readerProduct.MoveToContent();
			while (readerProduct.Read())
			{
				if (readerProduct.AttributeExists("form", "name", "cart_quantity", true))
				{
					while (readerProduct.Read())
					{
						if (readerProduct.AttributeExists("td", "class", "pageHeading", true))
						{
							readerProduct.Read();
							title = readerProduct.Value;
							qualities = (!title.Contains("-") ? title : title.Substring(title.IndexOf("-"))).Split(',').Select(quality => new Product_Quality()
							{
								quality = new Quality() { name = "" },
								value = quality.Trim()
							}).ToList();
							while (readerProduct.Read())
							{
								if (readerProduct.IsTagElement("span"))
								{
									while (readerProduct.Read())
									{
										if (readerProduct.NodeType == XmlNodeType.Text)
										{
											reference = readerProduct.Value.Replace("Modelo: ", "").Replace("[", "").Replace("]", "");
											break;
										}
									}
								}
								if (readerProduct.IsTagEndElement("span"))
									break;
							}
						}
						if (readerProduct.AttributeExists("td", "class", "pagePrice", true))
						{
							var clientProductObj = new Client_Product();
							clientProductObj.url = address;
							clientProductObj.client = client;
							clientProductObj.isActive = true;
							clientProductObj.name = title;
							clientProductObj.productReference = reference;
							clientProductObj.price = 0;

							while (readerProduct.Read())
							{
								if (readerProduct.IsTagElement("s"))
								{
									while (readerProduct.Read())
									{
										if (readerProduct.NodeType == XmlNodeType.Text && readerProduct.Value.Length > 0 && readerProduct.Value.Contains("$"))
										{
											clientProductObj.price = float.Parse(readerProduct.Value.Substring(readerProduct.Value.IndexOf("$") + 1));
											break;
										}
									}
								}
								else if (readerProduct.NodeType == XmlNodeType.Text && readerProduct.Value.Length > 0 && readerProduct.Value.Contains("$"))
								{
									var priceValue = float.Parse(readerProduct.Value.Substring(readerProduct.Value.IndexOf("$") + 1));
									if (clientProductObj.price > 0)
										clientProductObj.specialPrice = priceValue;
									else
										clientProductObj.price = priceValue;
									break;
								}
							}
							price.Add(clientProductObj);
						}
						if (readerProduct.AttributeExists("a", "href", "_resize", false))
						{
							imageUrl = readerProduct.GetAttribute("href");
							imageUrl = imageUrl.Replace("_resize", "").Substring(0, imageUrl.IndexOf('?') - 7);
						}
						if (readerProduct.IsTagEndElement("form"))
							break;
					}
				}

				if (readerProduct.AttributeExists("img", "src", "images/manufacturer_", false))
				{
					brand = readerProduct.GetAttribute("alt");
				}
			}

			return new Product()
			{
				brand = new Brand()
				{
					name = brand
				},
				description = "",
				name = title,
				image = new Image()
				{
					//imageObj = client.DownloadData(match.Groups["image"].Value),
					url = imageUrl
				},
				clients = price,
				productReference = reference,
				level_Id = level_Id,
				catalog_Id = catalog_Id,
				qualities = qualities
			};
		}

		private List<Product> GetPanaFotoItems(IList<Catalog_Address> _catalog_addressesObj, bool getProductObjects)
		{
			productAddresses = new List<string>();
			productReferences = new List<string>();
			var productList = new List<Product>();
			var pageLimit = 9;

			if (_catalog_addressesObj != null)
			{
				foreach (var _catalog_addressObj in _catalog_addressesObj)
				{
					var masterAddress = "http://" + _catalog_addressObj.client.url + _catalog_addressObj.url + (_catalog_addressObj.url.Contains("?") ? "&" : "?");
					var goOn = true;
					var page = 1;
					string regex_tables = "<div\\sclass=\"marca\">";

					while (goOn)
					{
						var pageVal = page > 1 ? page.ToString() : "";
						var xhtmlProducts = _commonObj.GetXHTML(masterAddress + "pagina=" + pageVal);
						var htmlMatches = new Regex(regex_tables).Matches(xhtmlProducts);

						var readerProducts = System.Xml.XmlReader.Create(new System.IO.StringReader(xhtmlProducts));

						readerProducts.MoveToContent();
						// Parse the file and display each of the nodes.
						while (readerProducts.Read())
						{
							switch (readerProducts.NodeType)
							{
								case XmlNodeType.Element:
									if (readerProducts.AttributeExists("div", "class", "imagenProducto", false))
									{
										// sacar el xhtml del producto
										var productUrl = "";
										var productRef = "";
										while (readerProducts.Read())
										{
											if (readerProducts.NodeType == XmlNodeType.Element && readerProducts.Name == "a")
											{
												productUrl = readerProducts.GetAttribute("href");
											}
											if (readerProducts.AttributeExists("div", "class", "serieProducto", true))
											{
												while (readerProducts.Read())
												{
													if (readerProducts.IsTagElement("br"))
													{
														readerProducts.Read();
														productRef = readerProducts.Value.Trim();
														break;
													}
												}
											}

											if (readerProducts.IsTagEndElement("div") && !string.IsNullOrEmpty(productRef))
												break;
										}
										if (!string.IsNullOrEmpty(productRef) && !productReferences.Contains(productRef))
										{
											productAddresses.Add(productUrl);
											productReferences.Add(productRef);
										}
										else if (!string.IsNullOrEmpty(productUrl))
											goOn = false;

										if (getProductObjects)
										{
											var product = GetPanaFotoProduct(productUrl, _catalog_addressObj.client, _catalog_addressObj.level_Id, _catalog_addressObj.catalog_Id);
											if (product != null)
												productList.Add(product);
										}
									}
									break;
							}
						}
						if (htmlMatches.Count < pageLimit)
							goOn = false;
						else
							page += 1;
					}
				}
				
			}

			return productList.ToList();
		}

		private Product GetPanaFotoProduct(string address, Client client, int level_Id, int catalog_Id)
		{
			var xhtmlProductFeatures = _commonObj.GetXHTML(address);
			// sacarle todos los elementos al producto
			var readerProduct = System.Xml.XmlReader.Create(new System.IO.StringReader(xhtmlProductFeatures));
			readerProduct.MoveToContent();

			var title = "";
			var reference = "";
			var price = new List<Client_Product>();
			var qualities = new List<Product_Quality>();
			var imageUrl = "";
			while (readerProduct.Read())
			{
				try
				{
					switch (readerProduct.NodeType)
					{
						case XmlNodeType.Element:
							if (readerProduct.AttributeExists("div", "class", "titulo", false))
							{
								while (readerProduct.Read())
								{
									if (readerProduct.NodeType == XmlNodeType.Text && readerProduct.Value.Length > 0)
									{
										title = readerProduct.Value;
									}
									if (readerProduct.NodeType == XmlNodeType.Element && readerProduct.Name == "h3")
									{
										readerProduct.Read();
										reference = readerProduct.Value.Replace("(", "").Replace(")", "");
										if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(reference))
											return null;
										break;
									}
								}
							}
							if (readerProduct.AttributeExists("div", "class", "precio", false))
							{
								var clientProductObj = new Client_Product();
								clientProductObj.url = address;
								clientProductObj.client = client;
								clientProductObj.isActive = true;
								clientProductObj.name = title;
								clientProductObj.productReference = reference;

								while (readerProduct.Read())
								{
									if (readerProduct.NodeType == XmlNodeType.Text && readerProduct.Value.Length > 0 && readerProduct.Value.Contains("$"))
									{
										clientProductObj.price = float.Parse(readerProduct.Value.Substring(readerProduct.Value.IndexOf("$") + 1).Replace("USD", ""));
										break;
									}
								}
								price.Add(clientProductObj);
							}
							if (readerProduct.AttributeExists("div", "class", "imagenes", false))
							{
								while (readerProduct.Read())
								{
									if (readerProduct.NodeType == XmlNodeType.Element && readerProduct.Name == "img")
									{
										imageUrl = readerProduct.GetAttribute("src").Replace("./", "");
										break;
									}
								}
							}
							if (readerProduct.AttributeExists("div", "class", "carasteristicas", true))
							{
								while (readerProduct.Read())
								{
									if (readerProduct.NodeType == XmlNodeType.Element && readerProduct.Name == "img" && readerProduct.GetAttribute("src") != null && readerProduct.GetAttribute("src").IndexOf("bullet") > -1)
									{
										readerProduct.Read();
										if (readerProduct.NodeType == XmlNodeType.Text && readerProduct.Value.Length > 0)
										{
											qualities.Add(new Product_Quality()
											{
												quality = new Quality() { name = "" },
												value = readerProduct.Value.Trim()
											});
										}
									}
									if (readerProduct.NodeType == XmlNodeType.EndElement && readerProduct.Name == "p")
										break;
								}
							}
							break;
					}
				}
				catch { throw; }
			}

			return new Product()
			{
				brand = new Brand()
				{
					name = title.Length > 0 ? title.Substring(0, title.IndexOf('-') != -1 ? title.IndexOf('-') : title.Length - 1).Trim() : ""
				},
				description = "",
				image = new Image()
				{
					//imageObj = client.DownloadData(match.Groups["image"].Value),
					url = imageUrl
				},
				name = title,
				clients = price,
				productReference = reference,
				level_Id = level_Id,
				catalog_Id = catalog_Id,
				qualities = qualities
			};
		}

		private List<Product> GetTechAndHouseItems(IList<Catalog_Address> _catalog_addressesObj, bool getProductObjects)
		{
			productAddresses = new List<string>();
			productReferences = new List<string>();
			var productList = new List<Product>();
			var pageLimit = 48;
			if (_catalog_addressesObj != null)
			{
				foreach (var _catalog_addressObj in _catalog_addressesObj)
				{
					var masterAddress = "http://" + _catalog_addressObj.client.url + _catalog_addressObj.url + (_catalog_addressObj.url.Contains("?") ? "&" : "?") + "limit=" + pageLimit.ToString();

					string regex_products = "<li class=\"item.*>";
					var goOn = true;
					var page = 1;
					while (goOn)
					{
						var xhtmlProducts = _commonObj.GetXHTML(masterAddress + "&p=" + page.ToString());
						var readerProducts = System.Xml.XmlReader.Create(new System.IO.StringReader(xhtmlProducts));

						var htmlMatches = new Regex(regex_products).Matches(xhtmlProducts);

						readerProducts.MoveToContent();
						// Parse the file and display each of the nodes.
						while (readerProducts.Read())
						{
							switch (readerProducts.NodeType)
							{
								case XmlNodeType.Element:
									if (readerProducts.AttributeExists("li", "class", "item", false))
									{
										// sacar el xhtml del producto
										var productUrl = "";
										var productRef = "";
										while (readerProducts.Read())
										{
											if (readerProducts.NodeType == XmlNodeType.Element && readerProducts.Name == "a")
											{
												productUrl = readerProducts.GetAttribute("href");
											}
											if (readerProducts.AttributeExists("span", "class", "model", true))
											{
												readerProducts.Read();
												productRef = readerProducts.Value;
												break;
											}
										}
										if (!string.IsNullOrEmpty(productRef) && !productReferences.Contains(productRef))
										{
											productAddresses.Add(productUrl);
											productReferences.Add(productRef);
										}
										else if (!string.IsNullOrEmpty(productUrl))
											goOn = false;

										if (getProductObjects)
										{
											var product = GetTechAndHouseProduct(productUrl, _catalog_addressObj.client, _catalog_addressObj.level_Id, _catalog_addressObj.catalog_Id);
											if (product != null)
												productList.Add(product);
										}
									}
									break;
							}
						}

						if (htmlMatches.Count < pageLimit)
							goOn = false;
						else
							page++;
					}
				}
			}

			return productList.ToList();
		}

		private Product GetTechAndHouseProduct(string address, Client client, int level_Id, int catalog_Id)
		{
			var xhtmlProductFeatures = _commonObj.GetXHTML(address);
			if (string.IsNullOrEmpty(xhtmlProductFeatures))
				return null;
			// sacarle todos los elementos al producto
			var readerProduct = System.Xml.XmlReader.Create(new System.IO.StringReader(xhtmlProductFeatures));
			readerProduct.MoveToContent();

			var title = "";
			var reference = "";
			var price = new List<Client_Product>();
			var qualities = new List<Product_Quality>();
			var imageUrl = "";

			while (readerProduct.Read())
			{
				try
				{
					switch (readerProduct.NodeType)
					{
						case XmlNodeType.Element:
							if(readerProduct.IsTagElement("title"))
							{
								while (readerProduct.Read())
								{
									if (readerProduct.NodeType == XmlNodeType.Text && readerProduct.Value.Contains("404 "))
										return null;
									if (readerProduct.IsTagEndElement("title"))
										break;
								}

							}
							if (readerProduct.AttributeExists("div", "class", "product-name", false))
							{
								while (readerProduct.Read())
								{
									if (readerProduct.NodeType == XmlNodeType.Text && readerProduct.Value.Length > 0)
										title = readerProduct.Value;

									if (readerProduct.NodeType == XmlNodeType.EndElement && readerProduct.Name == "h1")
										break;
								}
							}
							else if (readerProduct.AttributeExists("div", "class", "price-box", false))
							{
								var clientProductObj = new Client_Product();
								clientProductObj.client = client;
								clientProductObj.isActive = true;
								clientProductObj.url = address;
								clientProductObj.productReference = reference;
								while (readerProduct.Read())
								{
									if (readerProduct.AttributeExists("span", "class", "price", true) && !(readerProduct.AttributeExists("span", "id", "product-price", false)))
									{
										readerProduct.Read();
										if (readerProduct.NodeType == XmlNodeType.Text && readerProduct.Value.Length > 0)
										{
											clientProductObj.price = float.Parse(readerProduct.Value.Substring(readerProduct.Value.IndexOf("US") + 4).Replace(".", "").Replace(",", ".").Trim(), System.Globalization.NumberStyles.Currency);
										}
									}

									if (readerProduct.AttributeExists("span", "class", "price", true) && readerProduct.AttributeExists("span", "id", "product-price", false))
									{
										readerProduct.Read();
										if (readerProduct.NodeType == XmlNodeType.Text && readerProduct.Value.Length > 0)
										{
											clientProductObj.specialPrice = float.Parse(readerProduct.Value.Substring(readerProduct.Value.IndexOf("US") + 4).Replace(".", "").Replace(",", ".").Trim(), System.Globalization.NumberStyles.Currency);
										}
									}

									if (readerProduct.NodeType == XmlNodeType.EndElement && readerProduct.Name == "div")
									{
										if (price.Count == 0)
											price.Add(clientProductObj);
										break;
									}
								}
							}
							else if (readerProduct.AttributeExists("div", "class", "product-img-box", false))
							{
								while (readerProduct.Read())
								{
									if (readerProduct.NodeType == XmlNodeType.Element && readerProduct.Name == "img")
										imageUrl = readerProduct.GetAttribute("src");

									if (readerProduct.NodeType == XmlNodeType.EndElement && readerProduct.Name == "a")
										break;
								}
							}
							else if (readerProduct.AttributeExists("table", "id", "product-attribute-specs-table", false))
							{
								var qualityName = "";
								while (readerProduct.Read())
								{
									if (readerProduct.AttributeExists("th", "class", "label", false) && readerProduct.NodeType == XmlNodeType.Element)
									{
										readerProduct.Read();
										qualityName = readerProduct.Value;
									}
									if (readerProduct.AttributeExists("td", "class", "data", false) && readerProduct.NodeType == XmlNodeType.Element)
									{
										readerProduct.Read();
										qualities.Add(new Product_Quality() { quality = new Quality() { name = qualityName }, value = readerProduct.Value });
									}
									if (readerProduct.NodeType == XmlNodeType.EndElement && readerProduct.Name == "table")
										break;
								}
							}
							else if (readerProduct.AttributeExists("span", "class", "model", true))
							{
								while (readerProduct.Read())
								{
									if (readerProduct.NodeType == XmlNodeType.Text && readerProduct.Value.Length > 0)
										reference = readerProduct.Value;
									if (readerProduct.IsTagEndElement("span"))
										break;

								}
							}
							break;

					}

				}
				catch (Exception e)
				{
					throw e;
				}
			}

			return new Product()
			{
				brand = new Brand()
				{
					name = title.Substring(0, title.IndexOf('-') != -1 ? title.IndexOf('-') : title.Length - 1)
				},
				description = "",
				name = title,
				image = new Image()
				{
					//imageObj = client.DownloadData(match.Groups["image"].Value),
					url = imageUrl
				},
				clients = price,
				productReference = reference.Trim(),
				level_Id = level_Id,
				catalog_Id = catalog_Id,
				qualities = qualities.Select(x => new Product_Quality() { quality = x.quality, value = x.value }).ToList()
			};
		}

		#endregion
	}
}
