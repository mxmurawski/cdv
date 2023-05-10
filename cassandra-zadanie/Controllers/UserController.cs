using app3.Models;
using Microsoft.AspNetCore.Mvc;

namespace app3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DBC _dbc;

        public UserController(DBC dbc)
        {
            _dbc = dbc;
        }
        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(_dbc.GetUsers());
        }

        [HttpPost()]
        public IActionResult Register([FromBody] User user)
        {
            _dbc.AddUser(user);
            return Ok();
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO dto)
        {
            bool check = _dbc.Login(dto);
            return Ok(check == true ? "Logged In" : "Unauthorized");
        }

        [HttpGet("api/{email}/favourites")]
        public ActionResult<List<University>> GetFavourites(string email)
        {
            return Ok(_dbc.GetFavourites(email));
        }

        [HttpPost("api/{email}/favourites")]
        public ActionResult AddToFavourities(string email, [FromBody]string universityName)
        {
            _dbc.AddToFavourities(email, universityName);
            return Ok();
        }
        
    }
}