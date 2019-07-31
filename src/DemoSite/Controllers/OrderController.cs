using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DemoSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
		public OrderController() {} //TODO: Inject something useful
		
        // GET api/Order
        [HttpGet]
        public ActionResult<IEnumerable<Order>> Get()
        {
            return new Order[] { default(Order), default(Order) };
        }

        // GET api/Order/5
        [HttpGet("{id}")]
        public ActionResult<Order> Get(int id)
        {
            return new Order();
        }

        // POST api/Order
        [HttpPost]
        public void Post([FromBody] Order value)
        {
        }

        // PUT api/Order/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Order value)
        {
        }

        // DELETE api/Order/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}