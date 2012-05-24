using System.Data.Entity;
using System.Linq;
using HotelAdmin.Domain;
using Petite;

namespace HotelAdmin.DataAccess
{
    public class HistoryItemRepository : RepositoryBase<HistoryItem>, IHistoryItemRepository
    {
        public HistoryItemRepository(IDbSetProvider objectSetProvider) : base(objectSetProvider)
        {
        }

        protected override IQueryable<HistoryItem> Query
        {
            get
            {
                return base.Query.Include("Hotel").OrderByDescending(h => h.Timestamp);
            }
        }

        public HistoryItem GetLastItem(int id)
        {
            return Query.FirstOrDefault(h => h.HotelId == id);
        }
    }
}