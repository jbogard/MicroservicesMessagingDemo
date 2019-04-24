using System;

namespace Sales.Models
{
    public class SubmittedOrder : IEntity
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public Color Color { get; set; }
        public string OrderId { get; set; }
    }
}