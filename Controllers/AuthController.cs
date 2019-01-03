using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using netcore_webapi.Contracts;
using netcore_webapi.Models;

namespace netcore_webapi.Controllers
{
  // [Authorize]  // AuthController sadece kullanıcıların diğer Controller'da kullanacağı token'ı generate eden Controller olduğu için burada [Authorize] kullanılmamıştır.
  [ApiController]
  [Route("[controller]")]
  public class AuthController : ControllerBase
  {
    private IAuthService _authService;
    public AuthController(IAuthService authService)
    {
      _authService = authService;
    }

    // [AllowAnonymous] // Controller [Authorize] olarak taglenirse [AllowAnonymous] olarak taglenen Action'lar token kontrolünden muaf tutulur
    [HttpPost]
    public ActionResult Post(User entity)
    {
      try
      {
        string token = (_authService.Authenticate(entity.UserName, entity.Password)).Result;
        if (token == null) {
          return Unauthorized();
        }
        return Ok(new {
          success = true,
          token = token
        });
      }
      catch (System.Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
  }
}
