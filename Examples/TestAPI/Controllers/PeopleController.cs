using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sdotcode.DataLib.Examples;
using sdotcode.Repository;

namespace TestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PeopleController : ExtendedControllerBase<IPersonModel>
    {
        private readonly Service<IPersonModel> service;

        public PeopleController(Service<IPersonModel> service)
        {
            this.service = service;
        }

        [HttpGet]
        [Route("find/{propertyName}/{value}")]
        public override async Task<ActionResult> Get([FromRoute]string propertyName, [FromRoute]string value)
        {
            return Ok(await service.GetAsync(propertyName, value));
        }

        [HttpGet]
        public override async Task<ActionResult> Get(int page=0, int pageSize=10)
        {
            return Ok(await service.GetAsync(page, pageSize));
        }

        [HttpDelete]
        public override async Task<ActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);
            if (result)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPut]
        public override async Task<ActionResult> Upsert([FromBody] IEnumerable<IPersonModel> people)
        {
            if (people == null || !people.Any())
            {
                return BadRequest("Invalid item passed");
            }
            return Ok(await service.AddOrUpdateAsync(people));
        }
    }
}
