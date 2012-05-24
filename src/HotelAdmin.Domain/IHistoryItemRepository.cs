using Petite;

namespace HotelAdmin.Domain
{
    public interface IHistoryItemRepository : IRepository<HistoryItem>
    {
        HistoryItem GetLastItem(int id);
    }
}