using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DemoSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreClientController : ControllerBase
    {
		public StoreClientController() {} //TODO: Inject something useful
		
        // GET api/StoreClient
        [HttpGet]
        public ActionResult<IEnumerable<StoreClient>> Get()
        {
            return new StoreClient[] { default(StoreClient), default(StoreClient) };
        }

        // GET api/StoreClient/5
        [HttpGet("{id}")]
        public ActionResult<StoreClient> Get(int id)
        {
            return new StoreClient();
        }

        // POST api/StoreClient
        [HttpPost]
        public void Post([FromBody] StoreClient value)
        {
        }

        // PUT api/StoreClient/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] StoreClient value)
        {
        }

        // DELETE api/StoreClient/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}