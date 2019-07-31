using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DemoSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
		public CategoryController() {} //TODO: Inject something useful
		
        // GET api/Category
        [HttpGet]
        public ActionResult<IEnumerable<Category>> Get()
        {
            return new Category[] { default(Category), default(Category) };
        }

        // GET api/Category/5
        [HttpGet("{id}")]
        public ActionResult<Category> Get(int id)
        {
            return new Category();
        }

        // POST api/Category
        [HttpPost]
        public void Post([FromBody] Category value)
        {
        }

        // PUT api/Category/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Category value)
        {
        }

        // DELETE api/Category/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}