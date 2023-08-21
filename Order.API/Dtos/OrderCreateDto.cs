using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.API.Dtos
{
    public class OrderCreateDto
    {
        public List<OrderItemDto> OrderItemDtos { get; set; }
        public PaymentDto  PaymentDto { get; set; }
        public string BuyerId { get; set; }
        public AddressDto  AddressDto { get; set; }

    }
}
