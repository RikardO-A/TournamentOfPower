using Microsoft.AspNetCore.Mvc;
using ToP.Application.Interfaces;

namespace ToP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CacheController : ControllerBase
    {
        private readonly IRoundRobinService _roundRobinService;

        public CacheController(IRoundRobinService roundRobinService)
        {
            _roundRobinService = roundRobinService;
        }

        /// <summary>
        /// POST /api/cache/invalidate - Manually invalidate the tournament cache
        /// </summary>
        [HttpPost("invalidate")]
        public ActionResult InvalidateCache()
        {
            _roundRobinService.InvalidateCache();
            return Ok(new { message = "Cache invalidated successfully" });
        }
    }
}
