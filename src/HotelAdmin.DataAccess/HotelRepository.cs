using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using HotelAdmin.Domain;
using Petite;

namespace HotelAdmin.DataAccess
{
    public class HotelRepository : RepositoryBase<Hotel>, IHotelRepository
    {
        public HotelRepository(IDbSetProvider objectSetProvider) : base(objectSetProvider)
        {
        }

        protected override System.Linq.IQueryable<Hotel> Query
        {
            get
            {
                return base.Query.Include("Facts.FactType");
            }
        }

        public IEnumerable<Hotel> FindByFacts(string query, IEnumerable<string> facts)
        {
            var mainQuery = string.IsNullOrWhiteSpace(query) ? Query : Query.Where(h => h.Name.Contains(query));
            foreach(var fact in facts)
            {
                var fa = fact;
                if (!string.IsNullOrWhiteSpace(fa))
                {
                    mainQuery = mainQuery.Where(h => h.Facts.Where(f=>f.Value).Select(f => f.FactType.Code).Contains(fa));
                }
            }
            return mainQuery.ToArray();
        }
    }

    public class HistoryItemRepository : RepositoryBase<HistoryItem>, IHistoryItemRepository
    {
        public HistoryItemRepository(IDbSetProvider objectSetProvider) : base(objectSetProvider)
        {
        }

        protected override IQueryable<HistoryItem> Query
        {
            get
            {
                return base.Query.Include("Hotel");
            }
        }
    }
}