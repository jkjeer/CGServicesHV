using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CGServicesHV.Common.Types.E1
{
    [CollectionDataContract(Namespace = "")]
    public class F58002Xs : List<F58002X>
    {
    }

    [DataContract(Namespace = "")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class F58002X
    {
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public F580021 F580021 { get; set; }
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public F580024 F580024 { get; set; }
    }
}
