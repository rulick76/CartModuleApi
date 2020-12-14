using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace CartModuleApi.Models
{
    public class CartItemBase
    {
        public CartItemBase()
        {
           
        }
        [Key]
        internal int Id { get; set; }
        public Product Product { get; set; }
        
    }

    public class CartItem:CartItemBase
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public bool Active { get; set; }
    }

    public class CartItemPost
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
