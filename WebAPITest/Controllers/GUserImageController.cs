using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Model;
using WebAPITest.Services;

namespace WebAPITest.Controllers
{
    [Route("/guserImage")]
    [ApiController]
    public class GUserImageController : ControllerBase
    {
        private readonly GardenUserContext _context;
        private readonly IUserService _userService;
        private IWebHostEnvironment _environment;
        private readonly IJWTService _jwtService;

        public GUserImageController(GardenUserContext context, IUserService userService, IWebHostEnvironment environment, IJWTService jwtService)
        {
            _context = context;
            _userService = userService;
            _environment = environment;
            _jwtService = jwtService;
        }

        [HttpGet]
        [Route("/profile_img")]
        public IActionResult GetProfileImg()
        {
            try
            {
                return Ok(new { token = true });
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                return Ok(new { token = false });
            }
        }

        [HttpPost]
        [Route("/profile_img")]
        public async Task<IActionResult> SaveProfileImg([FromForm] UserProfileImg file)
        {
            try
            {
                string jwt = GetJWT();
                if (string.IsNullOrEmpty(jwt)) { return Ok(new { token = false }); }

                GardenUser guser = _context.GardenUser.FirstOrDefault(g => g.Email == _jwtService.GetEmail(_jwtService.GetClaimList(jwt)));

                var uploadFile = file.ProfileImg;
                long size = uploadFile.Length;
                if (size > 0)
                {
                   
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmss_") + Path.GetFileName(uploadFile.FileName);

                    //string wwwPath = this._environment.WebRootPath;
                    string path = Path.Combine($"uploads");
                    
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string filePath = Path.Combine(path, fileName);
                    var tt = Path.GetFullPath(filePath);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await uploadFile.CopyToAsync(stream);
                    }

                    guser.FilePath = filePath.Replace("/app/wwwroot", "");

                    _context.Update(guser);
                    await _context.SaveChangesAsync();
                }
               
                return Ok(new { token = true });
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                return Ok(new { token = false });
            }
        }

        /// <summary>
        /// Authorization 값 가져오기 - jwt
        /// </summary>
        /// <returns></returns>
        private string GetJWT()
        {
            return HttpContext.Request.Headers["Authorization"].ToString();
        }
    }
}
