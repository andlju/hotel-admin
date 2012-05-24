using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HotelAdmin.Domain;

namespace HotelAdmin.DataAccess
{
    public class DummyHistoryItemRepository : IHistoryItemRepository
    {
        public HistoryItem Get(Expression<Func<HistoryItem, bool>> whereClause)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HistoryItem> List()
        {
            yield return new HistoryItem()
                             {
                                 ActionType = 1,
                                 HotelId = 5,
                                 HotelName = "Sunwing Resort Alcudia",
                                 Timestamp = new DateTime(2012, 05, 03, 13, 45, 0)
                             };

            yield return new HistoryItem()
                             {
                                 ActionType = 0,
                                 HotelId = 5,
                                 HotelName = "Sunwing Resort Alcudia",
                                 Timestamp = new DateTime(2012, 05, 03, 13, 37, 0)
                             };
            yield return new HistoryItem()
                             {
                                 ActionType = 1,
                                 HotelId = 2,
                                 HotelName = "Dorado Beach",
                                 Timestamp = new DateTime(2012, 05, 02, 12, 28, 0)
                             };
            yield return new HistoryItem()
                             {
                                 ActionType = 2,
                                 HotelId = 3,
                                 HotelName = "Test Beach Garden",
                                 Timestamp = new DateTime(2012, 05, 02, 10, 28, 0)
                             };
            yield return new HistoryItem()
                             {
                                 ActionType = 1,
                                 HotelId = 2,
                                 HotelName = "Test Beach Garden",
                                 Timestamp = new DateTime(2012, 05, 02, 8, 28, 0)
                             };
        }

        public IEnumerable<HistoryItem> Find(Expression<Func<HistoryItem, bool>> whereClause)
        {
            throw new NotImplementedException();
        }

        public void Add(HistoryItem entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(HistoryItem entity)
        {
            throw new NotImplementedException();
        }
    }
}