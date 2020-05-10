using System.Collections.Generic;
using System.Threading.Tasks;
using webapp.API.Helpers;
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
         Task<PagedList<User>> GetUsers(UserParams userParams);
         Task<User> GetUser(int id);
         Task<Photo> GetPhoto(int id);

         Task<Photo> getMainPhoto(int userId);

         Task<Add> GetAdd(int userId, int recipientId);
    }
}