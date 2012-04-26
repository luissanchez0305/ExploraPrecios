using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.DomainModel;
using System.Diagnostics;

namespace Explora_Precios.Core
{
	[DebuggerDisplay("{name} - {url} - {catalogAddress}")]
	public class Client : Entity
	{
		[DomainSignature]
		public virtual string name { get; set; }
		public virtual string url { get; set; }
		public virtual bool isActive { get; set; }
		public virtual string catalogAddress { get; set; }
		public virtual IList<Client_Product> products { get; set; }
		public virtual IList<Catalog_Address> catalog { get; set; }
		public virtual string facebookId { get; set; }
		public virtual bool facebookPublish { get; set; }
	}
}
