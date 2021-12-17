using System;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers {
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase {
        public PlatformsController()
        {
            
        }

        [HttpPost]
        public ActionResult TestInboundConnection() {
            return Ok("Inbound test from Platforms Controller");
        }
    }
}