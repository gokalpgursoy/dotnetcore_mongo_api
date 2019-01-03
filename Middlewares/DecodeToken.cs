using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class DecodeToken
{
  private readonly RequestDelegate _next;

  public DecodeToken(RequestDelegate next)
  {
    _next = next;
  }

  public Task Invoke(HttpContext context)
  {
    // Request controller'a gitmeden önce DecodeToken  middleware'ından geçirilerek token decode edilir ve bu değerler sonrasında Actionlar içinde HttpContext sınıfı ile kullanılabilir.
    ClaimsIdentity claimsIdentity = context.User.Identity as ClaimsIdentity;
    string userId = claimsIdentity.FindFirst("UserId")?.Value;
    string userName = claimsIdentity.FindFirst("UserName")?.Value;
    // context.Items.Add("UserId", userId);
    // context.Items.Add("UserName", userName);
    context.Items.Add("decoded", new
    {
      UserId = userId,
      UserName = userName,
    });
    return this._next(context);
  }
}