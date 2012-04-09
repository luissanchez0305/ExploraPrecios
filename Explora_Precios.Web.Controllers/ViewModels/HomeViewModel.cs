using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Core;

namespace Explora_Precios.Web.Controllers.ViewModels
{
	public class HomeViewModel
	{
		public HomeViewModel(int CatalogLevel, int CatalogId)
		{
			productsListViewModel = new ProductsListViewModel();
			categories = new List<CategoryViewModel>();
			isSearch = false;
			Filter = new FilterViewModel(CatalogLevel, CatalogId);
		}

		public HomeViewModel()
		{
			productsListViewModel = new ProductsListViewModel();
			categories = new List<CategoryViewModel>();
			isSearch = false;
			Filter = new FilterViewModel();
		}

		public CatalogViewModel catalog { get; set; }
		public bool isSearch { get; set; }
		public string departmentTitle { get; set; }
		public int departmentId { get; set; }
		public IList<CategoryViewModel> categories { get; set; }
		public ProductsListViewModel productsListViewModel { get; set; }
		public IList<Product> allProducts { get; set; }
		public FilterViewModel Filter { get; set; }
	}

	public class IntroViewModel
	{
		public IntroViewModel()
		{
			TickerList = new List<string>();
			HighlightProducts = new List<BannerProduct>();
			OfferProducts = new List<BannerProduct>();
			NewProducts = new List<BannerProduct>();
		}

		public List<string> TickerList { get; set; }
		public IEnumerable<BannerProduct> HighlightProducts { get; set; }
		public IEnumerable<BannerProduct> OfferProducts { get; set; }
		public IEnumerable<BannerProduct> NewProducts { get; set; }
		public IEnumerable<GroupViewModel> GroupedProducts { get; set; }
	}

	public class BannerProduct
	{
		public int ProductId { get; set; }
		public string Name { get; set; }
		public float Price { get; set; }
		public int ClientId { get; set; }
		public string Client { get; set; }
		public byte[] Image { get; set; }
	}
}
