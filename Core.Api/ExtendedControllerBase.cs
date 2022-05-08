using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sdotcode.DataLib.Core;

namespace sdotcode.DataLib.Core.Api
{
    public abstract class ExtendedControllerBase<T> : ControllerBase
        where T : IStoredItem, new()
    {
        protected readonly Service<T> Service;
        public ExtendedControllerBase(Service<T> service)
        {
            this.Service = service;
        }

        [HttpGet]
        [Route("find/{propertyName}/{value}")]
        public virtual async Task<ActionResult> Get([FromRoute] string propertyName, [FromRoute] string value)
        {
            return Ok(await Service.GetAsync(propertyName, value));
        }

        [HttpGet]
        public virtual async Task<ActionResult> Get(int page = 0, int pageSize = 10)
        {
            return Ok(await Service.GetAsync(page, pageSize));
        }

        [HttpDelete]
        public virtual async Task<ActionResult> Delete(int id)
        {
            var result = await Service.DeleteAsync(id);
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
        public virtual async Task<ActionResult> Upsert([FromBody] IEnumerable<T> items)
        {
            if (items == null || !items.Any())
            {
                return BadRequest("Invalid item passed");
            }
            return Ok(await Service.AddOrUpdateAsync(items));
        }
    }
}
