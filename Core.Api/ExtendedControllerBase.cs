using Microsoft.AspNetCore.Mvc;

namespace sdotcode.Repository
{
    public abstract class ExtendedControllerBase<T> : ControllerBase
    {
        [HttpGet]
        [Route("{propertyName}/{value}")]
        public abstract Task<ActionResult> Get([FromRoute] string propertyName, [FromRoute] string value);

        [HttpGet]
        public abstract Task<ActionResult> Get(int page=0, int pageSize=10);
        
        [HttpDelete]
        public abstract Task<ActionResult> Delete(int id);
        
        [HttpPut]
        public abstract Task<ActionResult> Upsert([FromBody] IEnumerable<T> items);
    }
}
