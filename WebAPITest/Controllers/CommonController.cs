using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Services;

namespace WebAPITest.Controllers
{
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly IJWTService _jwtService;

        public CommonController(IJWTService jwtService)
        {
            _jwtService = jwtService;
        }
        [Route("/email")]
        [HttpGet]
        public IActionResult GetEmailThroughJWT()
        {
            try
            {
                return new JsonResult(new { email = _jwtService.GetEmail(_jwtService.GetClaimList(HttpContext.Request.Headers["Authorization"].ToString())) });
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                return new JsonResult(new { email = "" });
            }
        }
    }
}
