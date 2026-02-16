using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            var q = query ?? new GetOrdersQueryParams();

            IQueryable<Order> ordersQuery = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(q.SearchQuery))
            {
                var searchQueryLower = q.SearchQuery.Trim().ToLower();
                ordersQuery = ordersQuery.Where(o => o.Name != null && o.Name.ToLower().Contains(searchQueryLower) || o.Description != null && o.Description.ToLower().Contains(searchQueryLower));
            }

            if (q.HasDescription.HasValue)
            {
                if (q.HasDescription.Value)
                {
                    ordersQuery = ordersQuery.Where(o => !string.IsNullOrEmpty(o.Description));
                }
                else
                {
                    ordersQuery = ordersQuery.Where(o => string.IsNullOrEmpty(o.Description));
                }
            }

            var orders = ordersQuery.ToList();

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
            public bool? HasDescription { get; set; }
        }
    }

}