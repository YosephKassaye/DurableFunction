using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
   public class RequestObject
    {
        [DataMember]
        public int RequestId { get; set; }
        [DataMember]
        public string RequestTypeId { get; set; }
        [DataMember]
        public string SystemTypeId { get; set; }
        [DataMember]
        public string CreatedDate { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
