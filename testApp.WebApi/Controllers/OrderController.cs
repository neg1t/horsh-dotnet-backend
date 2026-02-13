using Microsoft.AspNetCore.Mvc;
using testApp.Data;
using testApp.Data.Models;

namespace testApp.WebApi.Controllers
{
    [ApiController]
    [Route("order")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Метод создания заказа
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Order), Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderModel order)
        {
            // Базовая валидация входа
            if (order is null)
            {
                return BadRequest("Request body is null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderFromDb = await _context.Orders.FindAsync(order.Id);

            if (orderFromDb is not null)
            {
                return BadRequest(new BadRequestMessage { Message = "Already have the same order" });
            }

            var newOrder = new Order
            { 
                Id = order.Id,
                Name = order.Name,
                Description = order.Description,
            };
            await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync();

            return Ok(newOrder);
        }

        /// <summary>
        /// Метод получения всех заказов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(Order[]), Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrders([FromQuery] GetOrdersQueryParams query)
        {
            var orders = _context.Orders.Where(x => x.Name.Equals(query.SearchQuery)).ToList();

            return Ok(orders);
        }

        public class CreateOrderModel
        {
            public int Id { get; set; }
            public required string Name { get; set; }
            public string? Description { get; set; } = "???";
        }

        public class BadRequestMessage
        {
            public required string Message { get; set; }
        }

        public class GetOrdersQueryParams
        {
            public string? SearchQuery { get; set; }
        }
    }

}