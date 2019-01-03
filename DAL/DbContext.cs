using Microsoft.Extensions.Options;
using MongoDB.Driver;
using netcore_webapi.Models;

namespace netcore_webapi
{
  public class DbContext
  {
    private readonly IMongoDatabase _db = null;

    public DbContext(IOptions<AppSettings> settings)
    {
      //appsetting.json'da tanımlanan sabit değerlere erişilerek ilgili database'e bağlantısı oluşturulur
      MongoClient client = new MongoClient(settings.Value.ConnectionString);
      if (client != null)
        _db = client.GetDatabase(settings.Value.Database);
    }
    public IMongoCollection<User> Users { get { return _db.GetCollection<User>("User"); } }
    
  }
}