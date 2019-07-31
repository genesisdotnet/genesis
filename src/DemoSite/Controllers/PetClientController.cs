using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DemoSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetClientController : ControllerBase
    {
		public PetClientController() {} //TODO: Inject something useful
		
        // GET api/PetClient
        [HttpGet]
        public ActionResult<IEnumerable<PetClient>> Get()
        {
            return new PetClient[] { default(PetClient), default(PetClient) };
        }

        // GET api/PetClient/5
        [HttpGet("{id}")]
        public ActionResult<PetClient> Get(int id)
        {
            return new PetClient();
        }

        // POST api/PetClient
        [HttpPost]
        public void Post([FromBody] PetClient value)
        {
        }

        // PUT api/PetClient/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] PetClient value)
        {
        }

        // DELETE api/PetClient/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}