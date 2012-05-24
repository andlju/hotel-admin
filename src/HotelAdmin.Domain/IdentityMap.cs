using System;
using System.ComponentModel.DataAnnotations;

namespace HotelAdmin.Domain
{
    public class IdentityMap
    {
        [Key]
        public Guid AggregateId { get; set; }

        public string TypeName { get; set; }
        public long ModelId { get; set; }
    }
}