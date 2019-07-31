using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DemoSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiResponseController : ControllerBase
    {
		public ApiResponseController() {} //TODO: Inject something useful
		
        // GET api/ApiResponse
        [HttpGet]
        public ActionResult<IEnumerable<ApiResponse>> Get()
        {
            return new ApiResponse[] { default(ApiResponse), default(ApiResponse) };
        }

        // GET api/ApiResponse/5
        [HttpGet("{id}")]
        public ActionResult<ApiResponse> Get(int id)
        {
            return new ApiResponse();
        }

        // POST api/ApiResponse
        [HttpPost]
        public void Post([FromBody] ApiResponse value)
        {
        }

        // PUT api/ApiResponse/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] ApiResponse value)
        {
        }

        // DELETE api/ApiResponse/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}