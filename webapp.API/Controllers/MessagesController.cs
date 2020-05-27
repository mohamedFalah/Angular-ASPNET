using System;
using System.Collections.Generic;
using System.Linq;
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
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IFriendshipRepository repository;
        private readonly IMapper mapper ;

        public MessagesController(IFriendshipRepository repository, IMapper mapper)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        [HttpGet("{id}", Name="GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await repository.GetMessage(id);

            if(messageFromRepo == null)
                return NotFound();

            return Ok(messageFromRepo);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userID, MessageForCreationDto messageForCreationDto)
        {
            if (userID != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreationDto.SenderId = userID;

            var recipient = await repository.GetUser(messageForCreationDto.RecipientId);
            if(recipient == null)
                return BadRequest("no user found");

            var message = mapper.Map<Message>(messageForCreationDto);

            repository.Add(message);

            if( await repository.SaveAll())
            {
                var messageToReturn = mapper.Map<MessageForCreationDto>(message);
                return CreatedAtRoute("GetMessage", new {userID, id = message.Id}, messageToReturn);
            }
            
            throw new Exception("creating new message failed to save");

        }

        [HttpGet]
        public  async Task<IActionResult> GetMessagesForUser(int userId,[FromQuery]MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageParams.UserId = userId;
            
            var messageFromRepo = await repository.GetMessagesForUsers(messageParams);

            var messages = mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);

            Response.AddPagination(
                    messageFromRepo.CurrentPage,
                    messageFromRepo.Pagesize,
                    messageFromRepo.TotalCount,
                    messageFromRepo.TotalPages);

            return Ok(messages);

        }

        [HttpGet("thread/{recipientID}")]
        public async Task<IActionResult> getMessageThread(int userId, int recipientID) {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var messagesFromRepo = await repository.GetMessageThread(userId, recipientID);
            var messageThread = mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            return Ok(messageThread);

        }
   
   
        [HttpPost("{id}")]
        public async Task<IActionResult> deleteMessage(int userId, int id){
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
           // var userFromRepo = await repository.GetUser(userId);
            var messageFromRepo = await repository.GetMessage(id);
            
            //check if the user exits 
            if(messageFromRepo.SenderId == userId) {
                messageFromRepo.SenderDeleted = true;
            }
            if(messageFromRepo.RecipientId == userId) {
                messageFromRepo.RecipientDeleted = true;
            }

            if(messageFromRepo.RecipientDeleted && messageFromRepo.SenderDeleted){
                repository.Delete(messageFromRepo);
            }

            if(await repository.SaveAll()) {
                return NoContent();
            }



        throw new Exception("error deleting the message");
           
            
        }
    
    
        [HttpPost("{id}/read")]
        public async Task<IActionResult> isReadMessage(int userId, int id){
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var messageFromRepo = await repository.GetMessage(id);

            if(messageFromRepo.RecipientId != userId){
                return Unauthorized();
            }
            messageFromRepo.IsRead = true;
            messageFromRepo.DateRead = DateTime.Now;

            await repository.SaveAll();

            return NoContent();

        }
    }
}