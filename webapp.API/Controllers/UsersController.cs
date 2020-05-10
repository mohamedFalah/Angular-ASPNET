using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapp.API.Data;
using webapp.API.Dtos;
using webapp.API.Helpers;
using webapp.API.Models;

namespace webapp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IFriendshipRepository repo;
        private readonly IMapper mapper;
        public UsersController(IFriendshipRepository repo, IMapper mapper)
        {
            this.mapper = mapper;
            this.repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            //get the current user ID 
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            //get the user from repo 
            var userFromRepo =  await repo.GetUser(currentUserId);
            //add the user to the userParams property
            userParams.UserId = currentUserId;

            //fiter the user by gender  to display the opposite gender
            // if(!string.IsNullOrEmpty(userParams.Gender))
            // {
            //     userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";

            // }

            var users = await repo.GetUsers(userParams);
            var userToReturn = mapper.Map<IEnumerable<UserForListDto>>(users);
            Response.AddPagination(users.CurrentPage, users.Pagesize, users.TotalCount, users.TotalPages);
            return Ok(userToReturn);
        }


        [HttpGet("{id}", Name= "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await repo.GetUser(id);
            var userToReturn = mapper.Map<UserForDetailedDto>(user);
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto){
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await repo.GetUser(id);

            mapper.Map(userForUpdateDto, userFromRepo);

            if(await repo.SaveAll()){
                return NoContent();
            }

            throw new Exception($"updating user {id} faild with save");
        }

        [HttpPost("{id}/add/{recipientId}")]

        public async Task<IActionResult> AddUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var add = await repo.GetAdd(id, recipientId);

            if(add != null)
                return BadRequest(" you added this user");
            if (await repo.GetUser(recipientId) == null)
                return NotFound();
            add = new Add 
            {   
                AdderId = id,
                AddedId = recipientId
            };

            repo.Add<Add>(add);

            if (await repo.SaveAll())
                return Ok();

            return BadRequest("failed to add user");
        } 

    }
}