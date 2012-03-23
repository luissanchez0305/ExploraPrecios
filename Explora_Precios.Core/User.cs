using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.DomainModel;
using System.Diagnostics;

namespace Explora_Precios.Core
{
    [DebuggerDisplay("{email}")]
    public class User : Entity
    {
        public User() { }

        [DomainSignature]
        public virtual string email { get; set; }
        public virtual string username { get; set; }
        public virtual City city { get; set; }
        public virtual char gender { get; set; }
        public virtual string name { get; set; }
        public virtual string lastName { get; set; }
        public virtual string validationCode { get; set; }
        public virtual DateTime birthdate { get; set; }
        public virtual bool isApproved { get; set; }
        public virtual string facebookToken { get; set; }
        public virtual IList<Group_User> groups { get; set; }
    }
}
