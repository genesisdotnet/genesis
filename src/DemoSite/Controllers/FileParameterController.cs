using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DemoSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileParameterController : ControllerBase
    {
		public FileParameterController() {} //TODO: Inject something useful
		
        // GET api/FileParameter
        [HttpGet]
        public ActionResult<IEnumerable<FileParameter>> Get()
        {
            return new FileParameter[] { default(FileParameter), default(FileParameter) };
        }

        // GET api/FileParameter/5
        [HttpGet("{id}")]
        public ActionResult<FileParameter> Get(int id)
        {
            return new FileParameter();
        }

        // POST api/FileParameter
        [HttpPost]
        public void Post([FromBody] FileParameter value)
        {
        }

        // PUT api/FileParameter/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] FileParameter value)
        {
        }

        // DELETE api/FileParameter/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}