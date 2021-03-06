﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.PersistenceSupport;
using Explora_Precios.Core;

namespace Explora_Precios.Core.DataInterfaces
{
	public interface IProductCounterRepository : IRepository<ProductCounter>
	{
		IEnumerable<ProductCounterDepartment> GetChartGeneralData();
		IEnumerable<ProductCounterDepartment> GetChartIndividualData();
	}
	public interface IClientCounterRepository : IRepository<ClientCounter>
	{
		IEnumerable<ClientCounter> GetChartData();
		IEnumerable<ProductCounterDepartment> GetChartClientData();
	}
}
