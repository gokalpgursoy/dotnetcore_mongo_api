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
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.AspNetCore.Http;

namespace netcore_webapi
{
  public class UsersService : IUsersService
  {
    private readonly DbContext _db = null;
    private readonly IHttpContextAccessor _httpContextAccessor; // (Service katmanında erişemek için Startup.cs de DI tanımlanmalı

    public UsersService(IOptions<AppSettings> settings, IHttpContextAccessor httpContextAccessor)
    {
      _db = new DbContext(settings);
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
      try
      {
        // decoded objesine Controller'da erişilip Service'e parametre olarak da geçilebilir ya da IHttpContextAccessor sayesinde direk olarak burada da erişilebilir.
        _httpContextAccessor.HttpContext.Items.TryGetValue("decoded", out Object decoded);
        return await _db.Users.Find(_ => true).ToListAsync();
      }
      catch (Exception ex)
      {
        throw new Exception("An error occured UserService.GetAll()", ex);
      }
    }

    public async Task<User> Get(string id)
    {
      try
      {
        // var filter = Builders<User>.Filter.Eq("Id", id);
        // return await _db.Users.Find(filter).FirstOrDefaultAsync();
        return await _db.Users.Find(x => x.Id == ObjectId.Parse(id)).FirstOrDefaultAsync();
      }
      catch (Exception ex)
      {
        throw new Exception("An error occured UserService.Get()", ex);
      }
    }

    public async Task Add(User entity)
    {
      try
      {
        await _db.Users.InsertOneAsync(entity);
      }
      catch (Exception ex)
      {
        throw new Exception("An error occured UserService.Add()", ex);
      }
    }

    public async Task<bool> Remove(string id)
    {
      try
      {
        // DeleteResult actionResult = await _db.Users.DeleteOneAsync(Builders<User>.Filter.Eq("Id", id));
        DeleteResult actionResult = await _db.Users.DeleteOneAsync(x => x.Id == ObjectId.Parse(id));
        return actionResult.IsAcknowledged && actionResult.DeletedCount > 0;
      }
      catch (System.Exception ex)
      {
        throw new Exception("An error occured UserService.Remove()", ex);
      }
    }

    public async Task<bool> Update(string id, User entity)
    {
      var filter = Builders<User>.Filter.Eq(x => x.Id, ObjectId.Parse(id));
      var update = Builders<User>.Update
                                  .Set(x => x.FirstName, entity.FirstName)
                                  .Set(x => x.LastName, entity.LastName)
                                  .Set(x => x.UserName, entity.UserName)
                                  .Set(x => x.Password, entity.Password);
      try
      {
        UpdateResult actionResult = await _db.Users.UpdateOneAsync(filter, update);
        return actionResult.IsAcknowledged
            && actionResult.ModifiedCount > 0;
      }
      catch (System.Exception ex)
      {
        throw new Exception("An error occured UserService.Update()", ex);
      }
    }

    public async Task<List<User>> GetByField(string fieldName, string fieldValue)
    {
      var filter = Builders<User>.Filter.Eq(fieldName, fieldValue);
      var result = await _db.Users.Find(filter).ToListAsync();
      return result;
    }
  }
}