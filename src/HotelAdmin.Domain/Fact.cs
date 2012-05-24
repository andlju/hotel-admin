using System.ComponentModel.DataAnnotations;

namespace HotelAdmin.Domain
{
    public class Fact
    {
        public int HotelId { get; set; }
        public int FactTypeId { get; set; }

        public FactType FactType { get; set; }
        public bool Value { get; set; }
        public string Details { get; set; }
    }
}