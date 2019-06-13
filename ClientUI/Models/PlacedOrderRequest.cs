using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ClientUI.Models
{
    public enum OrderStatus
    {
        [Display(Name = "Submitted")]
        Submitted,

        [Display(Name = "Received")]
        Received,

        [Display(Name = "Shipped")]
        Shipped
    }

    public class PlacedOrderRequest : IEntity
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public string OrderId { get; set; }

        public string ItemName { get; set; }

        public OrderStatus OrderStatus { get; set; }
    }
}