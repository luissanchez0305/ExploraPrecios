using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explora_Precios.Web.Controllers.ViewModels
{

	public class ClientViewModel
	{
		public int clientId { get; set; }
		public int productId { get; set; }
		public int clientProductId { get; set; }
		public string clientName { get; set; }
		public string productName { get; set; }
		public string brandName { get; set; }
		public byte[] image { get; set; }
		public string domain { get; set; }
		public string url { get; set; }
		public bool isActive { get; set; }
		public bool showMe { get; set; }
		public bool isMain { get; set; }
		public Explora_Precios.ApplicationServices.ClientServices.ItemType productStatus { get; set; }
		public Explora_Precios.ApplicationServices.ClientServices.PageStatus pageStatus { get; set; }
		public float price { get; set; }
		public float specialPrice { get; set; }
		public float oldPrice { get; set; }
		public string page { get; set; }
		public string reference { get; set; }
		public DateTime modified { get; set; }
		public DateTime reported { get; set; }
		public string facebookId { get; set; }
		public bool facebookPublish { get; set; }
		public bool choosen { get; set; }
		public string catalogAddress { get; set; }
		public string masterReference { get; set; }
	}
}
