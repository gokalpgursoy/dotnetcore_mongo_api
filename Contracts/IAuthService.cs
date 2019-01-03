using System.Collections.Generic;
using System.Threading.Tasks;
using netcore_webapi.Models;

namespace netcore_webapi.Contracts
{
  public interface IAuthService
  {
    Task<string> Authenticate(string username, string password);
  }
}