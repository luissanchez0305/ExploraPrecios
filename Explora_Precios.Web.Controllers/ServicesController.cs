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
		IProductCounterRepository _productCounterRepository;

		public ServicesController(IProductCounterRepository productCounterRepository)
		{
			_productCounterRepository = productCounterRepository;
		}

		public ActionResult LoadChartData()
		{
			var productData = _productCounterRepository.GetChartData();

			var array = from p in productData
						where p.product.GetProductDepartment().Id != 3 && p.product.GetProductDepartment().Id != 4
						group p by new { p.date, catalog = p.product.GetProductDepartment() } into newp
						select new { date = newp.Key.date, department = newp.Key.catalog, weight = newp.Sum(product => product.weight) };

			var result = "Mes,Electronicos,Hogar,Juguetes;";
			var fromDate = DateTime.Now > DateTime.Parse("06/01/2012").AddDays(-1) ? DateTime.Now.Subtract(DateTime.Parse("06/01/2012").AddDays(-1)).Days > 150 ? DateTime.Parse("05/01/2012") : DateTime.Now.AddMonths(-4) : DateTime.Parse("05/01/2012");

			float max = 0;
			for (int i = 0; i <= 4; i++)
			{
				var compareDate = fromDate.AddMonths(i);
				var weights = "";
				var data = array.Where(item => item.date.Month == compareDate.Month && item.date.Year == compareDate.Year);
				// Electronicos
				var value = data.Where(item => item.department.Id == 1).Sum(item => item.weight);
				if (value > max)
					max = value;
				weights += value.TwoDecimals() + ",";
				// Hogar
				value = data.Where(item => item.department.Id == 2).Sum(item => item.weight);
				if (value > max)
					max = value;
				weights += value.TwoDecimals() + ","; 
				value = data.Where(item => item.department.Id == 5).Sum(item => item.weight);
				// Juguetes
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
	}

}
