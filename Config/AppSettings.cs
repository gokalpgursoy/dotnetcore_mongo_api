namespace netcore_webapi
{
  public class AppSettings
  {
    public string Secret { get; set; }  // Token generate ederken kullanılır
    public string ConnectionString { get; set; }  // Database bağlantısı için kullanılır (username, password ve ip)
    public string Database { get; set; }  // Database bağlantısı için kullanılır (database name)
  }
}