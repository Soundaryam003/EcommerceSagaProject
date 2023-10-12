using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Order.API.Database;
using Order.API.Models;
using Plain.RabbitMQ;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly OrderingContext _orderingContext;
        private readonly IPublisher _publisher;

        public OrderItemsController(OrderingContext orderingContext, IPublisher publisher)
        {
            _orderingContext = orderingContext;
            _publisher = publisher;

        }

        [HttpGet("{id}")]
        int idpublic async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItem(int id) 
        
       {
            var orderItem = await orderingContext.orderItems.FindAsync(id);
            if(orderItem == null)
            {
                return NotFound();
            }
            return Ok(orderItem);
        }

        [HttpPost]
        public async Task<ActionResult<OrderItem>> PostOrderItem(OrderItem orderItem)

        {
            orderingContext.OrderItems.Add(orderItem);
            await orderingContext.SaveChangesAsync();

            //new inserted identity

            _publisher.Publish(JsonConvert.SerializeObject(new orderrequest
            {

            }),
            "Order_Created_routingkey",
            null);

        }
    }
}
