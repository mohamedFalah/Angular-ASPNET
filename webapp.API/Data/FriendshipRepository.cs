using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using webapp.API.Helpers;
using webapp.API.Models;

namespace webapp.API.Data
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly DataContext context;
        public FriendshipRepository(DataContext context)
        {
            this.context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            context.Remove(entity);
        }

        public async Task<Add> GetAdd(int userId, int recipientId)
        {
            return await context.Adds.FirstOrDefaultAsync(u => 
                        u.AdderId == userId && u.AddedId == recipientId);
                    
        }

        public async Task<Photo> getMainPhoto(int userId)
        {
            return await context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            //asQueryable to use Where clouse 
            var users = context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();

            //fillter the current user 
            users = users.Where(u => u.Id != userParams.UserId);
            
            //filter by gender
            if(!string.IsNullOrEmpty(userParams.Gender))
                users = users.Where(u => u.Gender == userParams.Gender);

            //users follows and followee
            if(userParams.adders)
            {
                var userAdders = await GetUsersAdds(userParams.UserId, userParams.adders);
                users = users.Where(u => userAdders.Contains(u.Id));
            }
            if(userParams.addeds)
            {
                var userAddeds = await GetUsersAdds(userParams.UserId, userParams.adders);
                users = users.Where(u => userAddeds.Contains(u.Id));
            }
            //filter by age
            if (userParams.MinAge != 18 || userParams.MaxAge != 99 )
            {
                //birth date age range 
                var minDoB = DateTime.Today.AddYears(-userParams.MaxAge - 1);  
                var maxDoB = DateTime.Today.AddYears(-userParams.MinAge );

                users = users.Where(u => u.DateOfBirth >= minDoB && u.DateOfBirth <= maxDoB);
            }

            //order 
            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch(userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;

                }
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<int>> GetUsersAdds(int id, bool adders){
            var user = await context.Users
                             .Include(x => x.Adders).Include(x => x.Addeds)
                             .FirstOrDefaultAsync(u => u.Id == id);

            if(adders)
            {
                return user.Adders.Where(u => u.AddedId == id).Select(i => i.AdderId);
            }
            else
            {
                return user.Addeds.Where( u => u.AdderId ==id).Select(i => i.AddedId);
            }
        }

        public async Task<bool> SaveAll()
        {
           return await context.SaveChangesAsync()> 0;
        }
    }
}