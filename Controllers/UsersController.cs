using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using netcore_webapi.Contracts;
using netcore_webapi.Models;

namespace netcore_webapi.Controllers
{
  [Authorize] // Authorize Attribute'ü ilgili Controller içerisinde [AllowAnonymous] attribute'üne sahip olmayan tüm Action'lara yetkisiz erişimi engeller.
  [ApiController] // ApiController Attribute'ü hem ilgili model için Validasyon kontrolü yapar hem de Actionlara gelen parametreler için ayrıca [FromBody] gibi tanımlamalar kullanılmasına gerek bıraktırmaz.
  [Route("api/[controller]")]
  public class UsersController : ControllerBase
  {
    private readonly IUsersService _userService;
    public UsersController(IUsersService userService)
    {
      _userService = userService;
    }

    [HttpGet]
    public async Task<IEnumerable<User>> Get()
    {
      // Token'ın barındırdığı değerlere erişme yöntemleri
      // Burada kullanımına gerek yok çünkü user based bir işlem yok ancak user based bir işlem olursa eriştiğimiz bilgileri kullanabiliriz.
      /* 
      1)
      HttpContext sınıfını kullanarak Action'a gelen isteğin hangi kullanıcıdan geldiğini handle edebiliriz. Burada HttpContext sınıfına erişmek için Startup.cs de DI tanımlamaya gerek yok.
      string userId = HttpContext.Items.FirstOrDefault(x => x.Key.ToString() == "UserId").Value as String; // Token decode edilerek içerisinde barındırdığu UserId'ye erişilebilir.
      2)
      HttpContext.Items.TryGetValue("decoded", out Object decoded);  // Bu şekilde tokenın barındırdığı bilgilere erişmek için request'in Middlewares klasöründeki DecodeToken() middleware'inden geçmiş olması gerekmektedir.
       */
      return await _userService.GetAll();
    }

    [HttpGet("{id}", Name = "GetUser")]
    public async Task<User> Get(string id)
    {
      return await _userService.Get(id);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody]User entity)
    {
      await _userService.Add(entity);
      return CreatedAtRoute("GetUser", new { id = entity.Id }, entity);
    }

    [HttpDelete("{id}")]
    public async Task<bool> RemoveUser(string id)
    {
      bool response = await _userService.Remove(id);
      return response;
    }

    [HttpPut("{id}")]
    public async Task<bool> UpdateUser(string id, [FromBody] User entity)
    {
      bool response = await _userService.Update(id, entity);
      return response;
    }
  }
}
