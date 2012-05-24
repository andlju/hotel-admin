using System.Data.Entity;
using HotelAdmin.Domain;

namespace HotelAdmin.DataAccess
{
    public class HotelAdminContext : DbContext
    {
        public HotelAdminContext() : base("HotelAdminContext")
        {
        }

        public IDbSet<Hotel> Hotels { get; set; }
        public IDbSet<FactType> FactTypes { get; set; }
        public IDbSet<HistoryItem> HistoryItems { get; set; } 

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fact>().HasKey(f => new {f.HotelId, f.FactTypeId});
        }

    }
}