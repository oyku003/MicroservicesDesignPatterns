using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.Dtos;
using Order.API.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPublishEndpoint publishEndpoint;
        public OrdersController(AppDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDto orderCreateDto)
        {
            var newOrder = new Models.Order {
            
            BuyerId = orderCreateDto.BuyerId,
            Status= OrderStatus.Suspend,
            Address = new Address { Line = orderCreateDto.AddressDto.Line, District = orderCreateDto.AddressDto.District, Province = orderCreateDto.AddressDto.Province},
            CreatedDate = DateTime.Now
            };

            orderCreateDto.OrderItemDtos.ForEach(x =>
            {
                newOrder.Items.Add(new OrderItem { Price = x.Price, ProductId = x.ProductId, Count = x.Count });
            });


            await _context.AddAsync(newOrder);
            await _context.SaveChangesAsync();

            var orderCreatedEvent = new OrderCreatedEvent()
            {
                BuyerId = orderCreateDto.BuyerId,
                OrderId = newOrder.Id,
                Payment = new PaymentMessage { CardName = orderCreateDto.PaymentDto.CardName, CardNumber = orderCreateDto.PaymentDto.CardNumber, CVV = orderCreateDto.PaymentDto.CVV, TotalPrice = orderCreateDto.OrderItemDtos.Sum(x=>x.Price*x.Count), Expiration = orderCreateDto.PaymentDto.Expiration}
            };

            orderCreateDto.OrderItemDtos.ForEach(item =>
            {
                orderCreatedEvent.OrderItems.Add(new OrderItemMessage { ProductId = item.ProductId, Count = item.Count });

            });

            await publishEndpoint.Publish(orderCreatedEvent);//send metodu direkt kuyruga gönderir.publish ile gönderilen kuyruklara herhangi bir kuyruk subscribe olabilir. Send ile gönderilende sadece kuyruga subscribe olundugunda alınır.Send metodu genelde dinleyen instance oldugunda kullanılır. publish ile exchange gider .Farklı servislerin dinleyecegi bi sistem için publish etmemiz gerekir.Kısacası, birden fazla miksorsevrisin dinleyecegi yapılarda publish yapılır (birdern fazla subs. olunabilmesi için). Bir mikroservis dinyeleceyse send yollanır (kuyruk ismi verilip kuyrukta kaydedilip bir servisin o kuyrugu dinlemesi saglanır.)

            return Ok();
        }
    }
}
