using System;
using SharpArch.Core.DomainModel;

namespace Explora_Precios.Core
{
	public enum CounterType { Product = 0, Client = 1 }

	public class ProductCounter : Entity
	{
		[DomainSignature]
		public virtual CounterType Type { get; set; }
		public virtual Product product { get; set; }
		public virtual float weight { get; set; }
		public virtual DateTime date { get; set; }
	}

	public class ClientCounter : Entity
	{
		[DomainSignature]
		public virtual CounterType Type { get; set; }
		public virtual Client_Product client { get; set; }
		public virtual float weight { get; set; }
		public virtual DateTime date { get; set; }
	}
}
