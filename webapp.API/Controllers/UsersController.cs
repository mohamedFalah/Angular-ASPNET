using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapp.API.Data;
using webapp.API.Dtos;

namespace webapp.API.Controllers
{
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
        public async Task<IActionResult> GetUsers()
        {
            var users = await repo.GetUsers();
            var userToReturn = mapper.Map<IEnumerable<UserForListDto>>(users);
            return Ok(userToReturn);
        }
        [HttpGet("{id}")]

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

    }
}