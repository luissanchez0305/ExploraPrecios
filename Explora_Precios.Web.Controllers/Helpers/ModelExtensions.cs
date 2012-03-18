using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Web.Controllers.ViewModels;

namespace Explora_Precios.Web.Controllers.Helpers
{
    public static class ModelExtensions
    {
        public static bool IsMinDate(this DateTime date)
        {
            return date.Year == 1;
        }
        public static List<ProductViewModel> GetProductsOfType(this List<ProductViewModel> list, string display, int clientId)
        {
            IEnumerable<ProductViewModel> productsResponse = new List<ProductViewModel>();
            if (display == "new")
                productsResponse = list.Where(x => x.clientList.SingleOrDefault(y => y.productStatus == ApplicationServices.ClientServices.ItemType.OnSite_NotOnDB && y.clientId == clientId) != null);
            else if (display == "deleted")
                productsResponse = list.Where(x => x.clientList.SingleOrDefault(y => y.productStatus == ApplicationServices.ClientServices.ItemType.OnDB_NotOnSite && y.clientId == clientId) != null);
            else
                productsResponse = list.Where(x => x.clientList.SingleOrDefault(y => y.productStatus == ApplicationServices.ClientServices.ItemType.Local && y.clientId == clientId) != null);

            return productsResponse.OrderByDescending(prod => prod.clientList.ElementAt(GetClientIndex(prod.clientList, clientId)).isActive).ToList();
        }

        private static int GetClientIndex(List<ClientViewModel> list, int clientId)
        {
            var index = 0;
            foreach (var clientVM in list)
            {
                if (clientVM.clientId == clientId)
                    break;
                index++;
            }
            return index;
        }
    }
}
