using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using webapp.API.Data;
using webapp.API.Dtos;
using webapp.API.Helpers;
using webapp.API.Models;

namespace webapp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userID}/photos")]
    public class PhotosController : ControllerBase
    {
        private readonly IFriendshipRepository repo;
        private readonly IMapper mapper;
        private readonly IOptions<CloudinarySettings> cloudinaryConfig;
        private Cloudinary cloudinary;

        public PhotosController(IFriendshipRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this.cloudinaryConfig = cloudinaryConfig;
            this.mapper = mapper;
            this.repo = repo;

            Account account = new Account(
                this.cloudinaryConfig.Value.CloudName,
                this.cloudinaryConfig.Value.ApiKey,
                this.cloudinaryConfig.Value.ApiSecret
            );

            this.cloudinary = new Cloudinary(account);

        }


        //delete photo 
        [HttpDelete("{id}")]

        public async Task<IActionResult> DeletePhoto(int userId, int id){

            //check the user 
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            //get the user 
            var userFromRepo = await repo.GetUser(userId);

            //check the photo exits 
            if(!userFromRepo.Photos.Any(p => p.Id == id)){
                return Unauthorized();
            }

            //get the phto 
            Photo PhotoFromRepo = await repo.GetPhoto(id);

            //is the main photo 
            if(PhotoFromRepo.IsMain){
                return BadRequest("do not delete the main photo");
            }
            if(PhotoFromRepo.PublicId != null){
                //delete the photo from cloudinary  
                var deletionParams = new DeletionParams(PhotoFromRepo.PublicId) {};
                var deletionResult = cloudinary.Destroy(deletionParams);

                if(deletionResult.Result == "ok"){
                    repo.Delete(PhotoFromRepo);
                }
            }

            if(PhotoFromRepo.PublicId == null){
                repo.Delete(PhotoFromRepo);
            }
            


            //save change 
            if(await repo.SaveAll()){
                return Ok();
            }

            return BadRequest("faild to delete photo");

        }
        
        //method created for createdAtRoute response 
        // which has name and route of photo id
        [HttpGet("{id}", Name ="GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id){
            //get the photo from repository
            var photoFromRepo = await repo.GetPhoto(id);
            // map the photo to photo for return 
            var photo = mapper.Map<PhotoForReturnDto>(photoFromRepo);
            // return ok response with photo as parameter
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotosForUser( int userId, PhotoForCreationDto photoForCreationDto){
            //check the user verification 
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            //get the user
            var userFromRepo = await repo.GetUser(userId);
            //create file for the photo
            var file = photoForCreationDto.File;
            //create car for uploading the file 
            var uploadResult = new ImageUploadResult();

            //if the file is not empty 
            if(file.Length >0) {

                using(var stream = file.OpenReadStream()) {
                    //fill the uploading parameters the description and transform method from cloudinary to create square picture around the face
                    var uploadParams = new ImageUploadParams() {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    // uplaod the photo to the cloudinary via .Upload method
                    uploadResult = cloudinary.Upload(uploadParams);
                }
            }
            // get the url of the photo and assign to dto 
            photoForCreationDto.Url = uploadResult.Uri.ToString();
            // get the public id of the photo from cloudinary 
            photoForCreationDto.PublicId = uploadResult.PublicId;

            // create var photo and map to the creation dto 
            var photo = mapper.Map<Photo>(photoForCreationDto);

            // if the user has no photos assign the uploaded photo as main 
            if(!userFromRepo.Photos.Any(u => u.IsMain))
                photo.IsMain = true;
            // add the photo to the user set of photos 
            userFromRepo.Photos.Add(photo);
            
            // if finished save the photo to db
            if( await repo.SaveAll()) {
                // create photo to return with public id and map it with created photo 
                var photoToReturn = mapper.Map<PhotoForReturnDto>(photo);
                //return createdAtRoute which takes three parameters
                // one is HttpGet Method "GetPhoto"
                // second create new photo with user id and photo id
                // third the photo must be returned
                return CreatedAtRoute("GetPhoto", new Photo{ UserId = userId, Id = photo.Id }, photoToReturn);
            }

            return BadRequest("Could not add");

        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id){
             //check the user verification 
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            //check the id exits 
            var user = await repo.GetUser(userId);
            if(!user.Photos.Any(p=>p.Id == id)){
                return Unauthorized();
            }

            var photoFromRepo = await repo.GetPhoto(id);
            if(photoFromRepo.IsMain){
                return BadRequest("this is the main photo");
            }
            var currentMainPhoto = await repo.getMainPhoto(userId);
            currentMainPhoto.IsMain = false;
            photoFromRepo.IsMain = true;

            if(await repo.SaveAll()){
                return NoContent();
            }

            return BadRequest("could not set the main photo");

        }


    }
}