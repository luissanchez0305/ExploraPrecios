using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.DomainModel;
using System.Diagnostics;

namespace Explora_Precios.Core
{
	[DebuggerDisplay("{name}")]
	public class Department : Entity
	{
		[DomainSignature]
		public virtual string name { get; set; }
		public virtual int index_order { get; set; }
		public virtual IList<Category> categories { get; set; }
	}
}
