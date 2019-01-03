using System.Collections.Generic;
using netcore_webapi.Contracts;
using netcore_webapi.Models;
using System.Linq;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace netcore_webapi
{
  public class AuthService : IAuthService
  {
    private readonly AppSettings _appSettings;
    private readonly DbContext _db = null;

    public AuthService(IOptions<AppSettings> settings)
    {
      _db = new DbContext(settings);
      _appSettings = settings.Value;
    }
    public async Task<string> Authenticate(string username, string password)
    {
      // User user = this._collection.Find(new BsonDocument { { "_id", new ObjectId(id) } }).FirstAsync().Result;
      User user = await _db.Users.Find(x => x.UserName == username && x.Password == password).FirstOrDefaultAsync();
      if (user == null)
        return null;  // Eğer böyle bir kullanıcı yoksa boş dönülür.

      // Böyle bir kullanıcı varsa Token generate edilir.
      JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler(); // Token oluşturmak için JwtSecurityTokenHandler sınıfı kullanılır
      byte[] key = Encoding.ASCII.GetBytes(_appSettings.Secret); // AppSettings'de ki Secret prop'unun value'su byte dizisine dönüştürüldü
      SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[] {
          // Token içerisinde hangi bilgileri barındıracak ona göre doldurulur
          new Claim("UserId", user.Id.ToString()),
          new Claim("UserName", user.UserName),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        }),
        Expires = DateTime.UtcNow.AddHours(1),  // Token'ın expire olma süresi ne kadar olacak
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) // Token hangi key ve hangi algoritma kullanılarak üretilecek
      };
      SecurityToken tokenObject = tokenHandler.CreateToken(tokenDescriptor);
      string token = tokenHandler.WriteToken(tokenObject);
      return token;
    }
  }
}