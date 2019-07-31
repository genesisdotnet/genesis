using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DemoSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiExceptionController : ControllerBase
    {
		public ApiExceptionController() {} //TODO: Inject something useful
		
        // GET api/ApiException
        [HttpGet]
        public ActionResult<IEnumerable<ApiException>> Get()
        {
            return new ApiException[] { default(ApiException), default(ApiException) };
        }

        // GET api/ApiException/5
        [HttpGet("{id}")]
        public ActionResult<ApiException> Get(int id)
        {
            return new ApiException();
        }

        // POST api/ApiException
        [HttpPost]
        public void Post([FromBody] ApiException value)
        {
        }

        // PUT api/ApiException/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] ApiException value)
        {
        }

        // DELETE api/ApiException/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}