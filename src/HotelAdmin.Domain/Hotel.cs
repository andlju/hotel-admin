using System.Collections.ObjectModel;

namespace HotelAdmin.Domain
{
    public class Hotel
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        public string ResortName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Collection<Fact> Facts { get; set; }
    }
}