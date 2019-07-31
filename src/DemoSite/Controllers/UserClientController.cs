using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DemoSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserClientController : ControllerBase
    {
		public UserClientController() {} //TODO: Inject something useful
		
        // GET api/UserClient
        [HttpGet]
        public ActionResult<IEnumerable<UserClient>> Get()
        {
            return new UserClient[] { default(UserClient), default(UserClient) };
        }

        // GET api/UserClient/5
        [HttpGet("{id}")]
        public ActionResult<UserClient> Get(int id)
        {
            return new UserClient();
        }

        // POST api/UserClient
        [HttpPost]
        public void Post([FromBody] UserClient value)
        {
        }

        // PUT api/UserClient/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] UserClient value)
        {
        }

        // DELETE api/UserClient/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}