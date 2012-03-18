using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Core;
using SharpArch.Data.NHibernate;
using Explora_Precios.Core.DataInterfaces;
using NHibernate.Criterion;

namespace Explora_Precios.Data
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public Product GetbyReference(string reference)
        {
            var productsList = GetbyReference(reference, Precision.High);
            return productsList.Count > 0 ? productsList[0] : null;
        }

        /// <summary>
        /// Get the product by it's reference
        /// </summary>
        /// <param name="reference">reference string to look for</param>
        /// <param name="precision">the precision that it's going to look for (high - equal, medium - partition of reference in 1 stage, low - anyone that can find)</param>
        /// <returns></returns>
        public IList<Product> GetbyReference(string reference, Precision precision)
        {
            reference = reference.Replace("/", "").Replace("-", "").Replace(" ", "");
            var productsResponse = NHibernateSession.Current.CreateCriteria(typeof(Product))
                .Add(Expression.Like("productReference", reference, precision == Precision.High ? MatchMode.Exact : MatchMode.Anywhere))
                .List<Product>().ToList();

            if (precision == Precision.Medium || precision == Precision.Low)
            {
                var productsResponseReverse = NHibernateSession.Current.CreateCriteria(typeof(Product))
                    //.Add(Expression.Sql("'" + reference + "' LIKE '%' + productReference + '%'"))
                    .Add(Expression.And(Expression.Sql("'" + reference + "' LIKE '%' + productReference + '%'"), Expression.Not(Expression.In("Id", productsResponse.Select(x => x.Id).ToArray()))))
                    .List<Product>().ToList();

                productsResponse.AddRange(productsResponseReverse);
            }

            if (precision == Precision.Low && productsResponse.Count > 0)
            {
                productsResponse = FindDinamicModifiedReference(reference).ToList();
            }

            return productsResponse;
        }

        private IList<Product> FindDinamicModifiedReference(string reference)
        {
            for (int i = reference.Length - 1; i > reference.Length - 4; i--)
            {
                var productsResponse = NHibernateSession.Current.CreateCriteria(typeof(Product))
                .Add(Expression.Eq("productReference", reference.Substring(0, i)))
                .List<Product>();
                if (productsResponse != null) return productsResponse;
            }
            return null;
        }

        public List<Product> GetbyProductType(int productTypeId)
        {
            var producttypeList = new List<int>();
            producttypeList.Add(productTypeId);
            return GetData(null, null, producttypeList.AsEnumerable());
        }

        public List<Product> GetbySubCategory(int subCategoryId)
        {
            var subcategories = new SubCategoryRepository().Get(subCategoryId);
            var producttypes = subcategories.productTypes.Select(x => x.Id);
            var subcategoryList = new List<int>();
            subcategoryList.Add(subCategoryId);
            return GetData(null, subcategoryList.AsEnumerable(), producttypes);
        }

        public List<Product> GetbyCategory(int categoryId)
        {
            var categories = new CategoryRepository().Get(categoryId);
            var subcategories = categories.subCategories.Select(x => x.Id);
            var producttypes = categories.subCategories.SelectMany(x => x.productTypes).Select(y => y.Id);
            var categoryList = new List<int>();
            categoryList.Add(categoryId);
            return GetData(categoryList.AsEnumerable(), subcategories, producttypes);
        }

        public List<Product> GetbyDepartment(int departmentId)
        { 
            var department = new DepartmentRepository().Get(departmentId);
            var categories = department.categories.Select(x => x.Id);   
            var subcategories = department.categories.SelectMany(x => x.subCategories).Select(y => y.Id).Distinct();
            var producttypes = department.categories.SelectMany(x => x.subCategories).SelectMany(y => y.productTypes).Select(z => z.Id).Distinct();

            return GetData(categories, subcategories, producttypes);;
        }

        public IList<Product> GetbySearchText(string text)
        {
            return GetbySearchText(text, IsActivated.NoMatter);
        }

        public IList<Product> GetbySearchText(string text, IsActivated isActivated)
        {
            var result = NHibernateSession.Current.CreateCriteria(typeof(Product))
                             .Add(Expression.Like("name", text, MatchMode.Anywhere).IgnoreCase())
                             .List<Product>().ToList();

            // Search by brand name
            result.AddRange(NHibernateSession.Current.CreateCriteria(typeof(Product))
                .CreateCriteria("brand","b")
                             .Add(Expression.Like("b.name", text, MatchMode.Anywhere).IgnoreCase())
                             .List<Product>().ToList());

            // Search by client name
            result.AddRange(NHibernateSession.Current.CreateCriteria(typeof(Product))
                .CreateCriteria("clients", "c")
                .CreateCriteria("client", "c1")
                             .Add(Expression.Like("c1.name", text, MatchMode.Anywhere).IgnoreCase())
                             .List<Product>().ToList());

            // Search by product qualities
            result.AddRange(NHibernateSession.Current.CreateCriteria(typeof(Product_Quality))
                             .Add(Expression.Like("value", text, MatchMode.Anywhere).IgnoreCase())
                             .List<Product_Quality>().Select(x => x.product));

            // Take out all the products with no clients activated
            if (isActivated != IsActivated.NoMatter)
            {
                result = (from products in result
                          where products.clients.Where(client => isActivated == IsActivated.Yes ? client.isActive : !client.isActive).Count() > 0
                          select products).ToList();
                          
            }
            return result.Distinct().ToList();
        }

        private List<Product> GetData(IEnumerable<int> categories, IEnumerable<int> subcategories, IEnumerable<int> producttypes)
        {
            var response = new List<Product>();

            if (producttypes != null)
            {
                response.AddRange(GetCatalogData(3, producttypes));

                if (subcategories != null)
                {
                    response.AddRange(GetCatalogData(2, subcategories));

                    if (categories != null)
                    {
                        response.AddRange(GetCatalogData(1, categories));
                    }
                }
            }
            return response;
        }

        private IEnumerable<Product> GetCatalogData(int level, IEnumerable<int> catalog_Ids)
        {
            var producList = NHibernateSession.Current.CreateCriteria(typeof(Product))
                             .Add(Expression.And(Expression.Eq("level_Id", level),
                             Expression.In("catalog_Id", catalog_Ids.ToArray())))
                             .List<Product>();

            // Quita todos los productos que tengan el despliegue de sus clientes asignados desactivados
            return producList;//.Where(x => x.clients.Any(y => y.isActive)).ToList();
        }
    }
}
