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
using Explora_Precios.ApplicationServices;
using System.Configuration;
using Explora_Precios.Data;
using Facebook;
using Explora_Precios.Core;

namespace Explora_Precios.Web.Controllers
{
	public class ServicesController : PrimaryController
	{
		IClientCounterRepository _clientCounterRepository;
		IProductCounterRepository _productCounterRepository;
		IUserRepository _userRepository;

		public ServicesController(IClientCounterRepository clientCounterRepository, IProductCounterRepository productCounterRepository, IUserRepository userRepository)
		{
			_clientCounterRepository = clientCounterRepository;
			_productCounterRepository = productCounterRepository;
			_userRepository = userRepository;
		}

		public ActionResult LoadUsersCount()
		{
			return Json(new
			{
				data = _userRepository.GetAll().Count()
			});
			
		}

		public ActionResult LoadClientChartData() 
		{
			var productData = _productCounterRepository.GetChartIndividualData();
			var clientData = _clientCounterRepository.GetChartClientData();

			var productsArray = LoadArray(productData);
			var clientsArray = LoadArray(clientData);

			var eleClientCount = clientsArray.Where(c => c.departmentId == 1).Count();
			var eleProductCount = productsArray.Where(p => p.departmentId == 1).Count() + eleClientCount;

			var hogClientCount = clientsArray.Where(c => c.departmentId == 2).Count();
			var hogProductCount = productsArray.Where(p => p.departmentId == 2).Count() + hogClientCount;

			var jugClientCount = clientsArray.Where(c => c.departmentId == 5).Count();
			var jugProductCount = productsArray.Where(p => p.departmentId == 5).Count() + jugClientCount;

			return Json(new
			{
				data = "Electronicos," + eleProductCount + "," + eleClientCount + ";Hogar," + hogProductCount + "," + hogClientCount + ";Juguetes," + jugProductCount + "," + jugClientCount
			});
		}

		public ActionResult LoadProductsChartData()
		{
			var productData = _productCounterRepository.GetChartGeneralData();

			var array = LoadArray(productData);

			var result = "Mes,Electronicos,Hogar,Juguetes;";
			var fromDate = DateTime.Now >= DateTime.Parse("06/01/2012").AddDays(-1) ? DateTime.Now.Subtract(DateTime.Parse("06/01/2012").AddDays(-1)).Days < 150 ? DateTime.Parse("05/01/2012") : DateTime.Now.AddMonths(-4) : DateTime.Parse("05/01/2012");

			float max = 0;
			for (int i = 0; i <= 4; i++)
			{
				var compareDate = fromDate.AddMonths(i);
				var weights = "";
				var data = array.Where(item => item.date.Month == compareDate.Month && item.date.Year == compareDate.Year);
				// Electronicos
				var value = data.Where(item => item.departmentId == 1).Sum(item => item.weight);
				if (value > max)
					max = value;
				weights += value.TwoDecimals() + ",";
				// Hogar
				value = data.Where(item => item.departmentId == 2).Sum(item => item.weight);
				if (value > max)
					max = value;
				weights += value.TwoDecimals() + ",";
				// Juguetes
				value = data.Where(item => item.departmentId == 5).Sum(item => item.weight);
				if (value > max)
					max = value;
				weights += value.TwoDecimals() + ",";
				
				if(weights.Length > 0)
				result += compareDate.ToString("MMM-yyyy").FirstCharacterUpper() + "," + weights.Substring(0, weights.Length - 1) + ";";
			}
			return Json(new
			{
				data = result.Substring(0, result.Length - 1),
				max = max
			});
		}

		private IEnumerable<ProductCounterDepartment> LoadArray(IEnumerable<ProductCounterDepartment> data)
		{
			return from p in data
				   where p.departmentId != 3 && p.departmentId != 4
				   group p by new { p.date, catalog = p.departmentId } into newp
				   select new ProductCounterDepartment { date = newp.Key.date, departmentId = newp.Key.catalog, weight = newp.Sum(product => product.weight) };
		}
	}

}
