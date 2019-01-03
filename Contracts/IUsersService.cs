using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using netcore_webapi.Models;

namespace netcore_webapi.Contracts
{
    public interface IUsersService
    {
        Task<IEnumerable<User>> GetAll();
        Task<User> Get(string id);
        Task Add(User entity);
        Task<bool> Update(string id , User entity);
        Task<bool> Remove(string id);

    }
}