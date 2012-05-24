using System.Runtime.Serialization;

namespace HotelAdmin.Service
{
    [DataContract]
    public class FactTypeModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Code { get; set; }
        
        [DataMember]
        public string Name { get; set; }
    }
}