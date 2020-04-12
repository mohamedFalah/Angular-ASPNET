using System.Collections.Generic;
using System.Threading.Tasks;
using webapp.API.Models;

namespace webapp.API.Data
{
    public interface IFriendshipRepository
    {
        // T is user or photo till now T as entity pramater and T is type of class 
        // avoid two saperate methods for each class 
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;

         Task<bool> SaveAll();

         //help
         Task<IEnumerable<User>> GetUsers();
         Task<User> GetUser(int id);
    }
}