using HotelAdmin.Domain;
using Petite;

namespace HotelAdmin.DataAccess
{
    public class IdentityMapRepository : RepositoryBase<IdentityMap>, IIdentityMapRepository
    {
        public IdentityMapRepository(IDbSetProvider objectSetProvider) : base(objectSetProvider)
        {
        }
    }
}