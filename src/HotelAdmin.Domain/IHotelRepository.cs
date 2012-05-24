using System.Collections.Generic;
using Petite;

namespace HotelAdmin.Domain
{
    public interface IHotelRepository : IRepository<Hotel>
    {
        IEnumerable<Hotel> FindByFacts(string query, IEnumerable<string> facts);
    }
}