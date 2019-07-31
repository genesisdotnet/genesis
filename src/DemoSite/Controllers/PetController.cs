using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DemoSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetController : ControllerBase
    {
		public PetController() {} //TODO: Inject something useful
		
        // GET api/Pet
        [HttpGet]
        public ActionResult<IEnumerable<Pet>> Get()
        {
            return new Pet[] { default(Pet), default(Pet) };
        }

        // GET api/Pet/5
        [HttpGet("{id}")]
        public ActionResult<Pet> Get(int id)
        {
            return new Pet();
        }

        // POST api/Pet
        [HttpPost]
        public void Post([FromBody] Pet value)
        {
        }

        // PUT api/Pet/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Pet value)
        {
        }

        // DELETE api/Pet/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}