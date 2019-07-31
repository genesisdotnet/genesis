using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DemoSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
		public TagController() {} //TODO: Inject something useful
		
        // GET api/Tag
        [HttpGet]
        public ActionResult<IEnumerable<Tag>> Get()
        {
            return new Tag[] { default(Tag), default(Tag) };
        }

        // GET api/Tag/5
        [HttpGet("{id}")]
        public ActionResult<Tag> Get(int id)
        {
            return new Tag();
        }

        // POST api/Tag
        [HttpPost]
        public void Post([FromBody] Tag value)
        {
        }

        // PUT api/Tag/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Tag value)
        {
        }

        // DELETE api/Tag/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}