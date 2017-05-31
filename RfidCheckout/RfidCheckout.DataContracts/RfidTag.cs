using System;
using System.Runtime.Serialization;

namespace RfidCheckout.DataContracts
{
    [DataContract]
    public class RfidTag
    {
        [DataMember]
        public string TagId { get; set; }

        [DataMember]
        public string Product { get; set; }

        [DataMember]
        public double Price { get; set; }

        public RfidTag()
        {
            TagId = Guid.NewGuid().ToString();
        }
    }
}
