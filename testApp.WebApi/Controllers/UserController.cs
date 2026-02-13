using Microsoft.AspNetCore.Mvc;
using testApp.Data;
using testApp.Data.Models;

namespace testApp.WebApi.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Метод создания пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(User),200)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel user)
        {
            var userFromDb = _context.Users.Where(x => x.Id == user.Id).SingleOrDefault();
            if (userFromDb is not null)
            {
                return BadRequest();
            }

            var newUser = new User
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Age = user.Age,
            };


            await _context.Users.AddAsync(newUser);

            await _context.SaveChangesAsync();

            return Ok(newUser);
        }

    }

    public class CreateUserModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public int? Age { get; set; }
    }
}
