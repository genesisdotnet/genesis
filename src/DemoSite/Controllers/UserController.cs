using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DemoSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
		public UserController() {} //TODO: Inject something useful
		
        // GET api/User
        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            return new User[] { default(User), default(User) };
        }

        // GET api/User/5
        [HttpGet("{id}")]
        public ActionResult<User> Get(int id)
        {
            return new User();
        }

        // POST api/User
        [HttpPost]
        public void Post([FromBody] User value)
        {
        }

        // PUT api/User/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] User value)
        {
        }

        // DELETE api/User/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}