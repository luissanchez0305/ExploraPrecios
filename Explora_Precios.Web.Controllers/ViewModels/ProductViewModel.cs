using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Core;
using Explora_Precios.Core.DataInterfaces;
using Explora_Precios.Data;

namespace Explora_Precios.Web.Controllers.ViewModels
{
	public enum GroupDisplay { InGroup, Create, IncludeMe }

	public class GroupViewModel {
		public int GroupSize { get; set; }
		public GroupDisplay Grouped { get; set; }
		public bool DoPublish { get; set; }
		public string ProductId { get; set; }
		public string ProductName { get; set; }
		public byte[] Image { get; set; }
		public bool IsFacebooked { get; set; }
		public DateTime CreatedDate { get; set; }
	}

	public class GroupManagerViewModel
	{
		public IEnumerable<GroupViewModel> GroupsViewModel { get; set; }
		public int TotalPage { get; set; }
	}

	public class ProductClientViewModel {
		public ProductViewModel product { get; set; }
		public ClientViewModel client { get; set; }
	}
	public class ProductViewModel
	{
		IProductTypeRepository _productTypeRepository;
		ISubCategoryRepository _subCatRepository;
		ICategoryRepository _catRepository;
		IDepartmentRepository _departmentRepository;
		public ProductViewModel() { }

		public ProductViewModel(IProductTypeRepository productTypeRepository,
		ISubCategoryRepository subCatRepository,
		ICategoryRepository catRepository,
		IDepartmentRepository departmentRepository)
		{
			_productTypeRepository = productTypeRepository;
			_subCatRepository = subCatRepository;
			_catRepository = catRepository;
			_departmentRepository = departmentRepository;
			qualities = new List<QualityViewModel>();
			clientList = new List<ClientViewModel>();
			hasChanged = false;
		}
		public bool hasChanged { get; set; }
		public int productId { get; set; }
		public string productRef { get; set; }
		public string productName { get; set; }
		public CatalogViewModel catalogProduct { get; set; }
		public string productDescription { get; set; }
		public int productBrandId { get; set; }
		public string productBrand { get; set; }
		public List<ClientViewModel> clientList { get; set; }
		public float lowestPrice { get; set; }
		public float highestPrice { get; set; }
		public string productImageUrl { get; set; }
		public Byte[] productImage { get; set; }
		public int productImageWidth { get; set; }
		public int productImageHeight { get; set; }
		public List<string> qualityNames { get; set; }
		public List<QualityViewModel> qualities { get; set; }
		public int catalogLevel { get; set; }
		public int catalogId { get; set; }
		public string rating { get; set; }
		public GroupViewModel group { get; set; }
		public bool isLiked { get;set; }

		public ProductViewModel LoadModel(Product prod, bool resume)
		{
			var cpOrderedPriceList = prod.clients.Where(cp => cp.isActive).OrderBy(cp => cp.price).Select(cp => cp.price);
			var lowestPrice = prod.clients.SingleOrDefault(client => client.isActive && client.specialPrice > 0) != null ?
			   prod.clients.Where(client => client.isActive && client.specialPrice > 0).OrderBy(orderPrice => orderPrice.specialPrice).First().specialPrice : cpOrderedPriceList.First();
			var highestPrice = prod.clients.Count > 1 ? cpOrderedPriceList.Last() : lowestPrice;
			var maxQualityLength = 35;

			this.productBrand = prod.brand.name;
			this.productBrandId = prod.brand.Id;
			this.clientList = prod.clients.Where(cp => cp.isActive).Select(w => new ClientViewModel()
			{
				clientId = w.client.Id,
				clientName = w.client.name,
				price = w.price,
				specialPrice = w.specialPrice,
				url = w.url,
				domain = w.client.url.Replace("www.",""),
				modified = w.dateModified,
				reported = w.dateReported,
				clientProductId = w.Id
			}).ToList();
			this.catalogProduct = GetCatalogViewModel(prod.level_Id, prod.catalog_Id);
			this.productDescription = prod.description;
			this.productId = prod.Id;
			this.productImageUrl = prod.image.url;
			this.productImage = prod.image.imageObj;
			this.productName = prod.name;
			this.productRef = prod.productReference;
			this.lowestPrice = lowestPrice;
			this.highestPrice = highestPrice;
			this.qualities = prod.qualities.Select(quality => new QualityViewModel()
			{
				name = quality.quality.name,
				value = resume ?
					(quality.quality.name.Trim().Length + quality.value.Trim().Length > maxQualityLength ?
						quality.value.Substring(0, (maxQualityLength - quality.quality.name.Trim().Length) < 0 ? 0 :
							maxQualityLength - quality.quality.name.Trim().Length) + "..." : quality.value) :
					quality.value
			}).ToList();
			this.qualityNames = prod.qualities.Select(z => z.quality.name).ToList();

			var Ratings = prod.ratings.Where(up => up.Type == User_Product.RelationType.Rating);
			this.rating = !resume && Ratings.Count() > 0 ? Ratings.Average(rat => rat.value).ToString("0.##") : "0";
			this.group = new GroupViewModel { GroupSize = prod.groups.Count };
			this.isLiked = prod.ratings.SingleOrDefault(up => up.Type == User_Product.RelationType.Liked) != null;
			return this;
		}

		private CatalogViewModel GetCatalogViewModel(int levelId, int catalogId)
		{
			_productTypeRepository = _productTypeRepository ?? new ProductTypeRepository();
			_subCatRepository = _subCatRepository ?? new SubCategoryRepository();
			_catRepository = _catRepository ?? new CategoryRepository();
			_departmentRepository = _departmentRepository ?? new DepartmentRepository();

			var response = new CatalogViewModel();
			while (catalogId != 0)
			{
				if (levelId == 3)
				{
					var catalog = _productTypeRepository.Get(catalogId);
					response.productTypeId = catalogId;
					response.productTypeName = catalog.name;
					response.productTypes = _productTypeRepository.GetAll().Select(x => new ProductTypeViewModel() { productTypeId = x.Id, productTypeTitle = x.name }).ToList();
					catalogId = catalog.subCategory.Id;
				}
				else if (levelId == 2)
				{
					var catalog = _subCatRepository.Get(catalogId);
					response.subCategoryId = catalogId;
					response.subCategoryName = catalog.name;
					response.subCategories = _subCatRepository.GetAll().Select(x => new SubCategoryViewModel() { subCategoryId = x.Id, subCategoryTitle = x.name }).ToList();
					catalogId = catalog.category.Id;
				}
				else if (levelId == 1)
				{
					var catalog = _catRepository.Get(catalogId);
					response.categoryId = catalogId;
					response.categoryName = catalog.name;
					response.categories = _catRepository.GetAll().Select(x => new CategoryViewModel() { categoryId = x.Id, categoryTitle = x.name }).ToList();
					catalogId = catalog.department.Id;
				}
				else
				{
					var catalog = _departmentRepository.Get(catalogId);
					response.departmentId = catalogId;
					response.departmentName = catalog.name;
					response.departments = _departmentRepository.GetAll().Select(x => new DepartmentViewModel() { departmentId = x.Id, departmentTitle = x.name }).ToList();
					catalogId = 0;
				}
				levelId--;
			}
			return response;
		}
	}
}
