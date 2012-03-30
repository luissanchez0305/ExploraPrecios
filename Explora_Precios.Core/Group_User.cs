using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using SharpArch.Core.DomainModel;

namespace Explora_Precios.Core
{
	[DebuggerDisplay("{product.name} - {user.email}")]
	public class Group_User : Entity
	{
		[DomainSignature]
		public virtual Product product { get; set; }
		public virtual User user { get; set; }
		public virtual DateTime created { get; set; }
	}
}
