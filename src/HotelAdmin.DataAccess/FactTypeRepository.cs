using HotelAdmin.Domain;
using Petite;

namespace HotelAdmin.DataAccess
{
    public class FactTypeRepository : RepositoryBase<FactType>, IFactTypeRepository
    {
        public FactTypeRepository(IDbSetProvider objectSetProvider) : base(objectSetProvider)
        {
        }
    }
}