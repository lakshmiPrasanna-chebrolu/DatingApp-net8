using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController(IUserRepository userRepository, IMapper mapper,
        IPhotoService photoService) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await userRepository.GetMembersAsync();
            //var usersToReturn=mapper.Map<IEnumerable<MemberDto>>(users);
            return Ok(users);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<MemberDto>> GetUserById(int id)
        {
            var user = await userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return mapper.Map<MemberDto>(user);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await userRepository.GetMemberByUserNameAsync(username);
            if (user == null) return NotFound();
            return user;//mapper.Map<MemberDto>(user);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user= await userRepository.GetUserByUserNameAsync(User.GetUserName());
            if(user==null) return BadRequest("Could not find the user");
            mapper.Map(memberUpdateDto,user);
            //userRepository.Update(user);
            if(await userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update the user");
        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file){
            var user=await userRepository.GetUserByUserNameAsync(User.GetUserName());
            if(user==null) return BadRequest("Could not update the user");
            var result=await photoService.AddPhotoAsync(file);
            if(result.Error!=null) return BadRequest(result.Error.Message);
            var photo= new Photo{
                Url=result.SecureUrl.AbsoluteUri,
                PublicId=result.PublicId,
            };
            user.Photos.Add(photo);
            if(await userRepository.SaveAllAsync()) 
                return CreatedAtAction(nameof(GetUser),
                    new {username=user.UserName},mapper.Map<PhotoDto>(photo));
            return BadRequest("Failed to Add the photo to database");
        }

        [HttpPut("set-main-photo/{photoId:int}")]
        public async Task<ActionResult> SetMainPhoto(int photoId){
            var user = await userRepository.GetUserByUserNameAsync(User.GetUserName());
            if(user==null) return BadRequest("User cannot be found");
            var photo=user.Photos.FirstOrDefault(x=>x.Id==photoId);
            if(photo==null || photo.IsMain) return BadRequest("Cannot use this photo as main");
            var currentMain=user.Photos.FirstOrDefault(x=>x.IsMain);
            if(currentMain!=null) currentMain.IsMain=false;
            photo.IsMain=true;
            if(await userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Problem setting the main photo");
        }
        [HttpDelete("delete-photo/{photoId:int}")]
        public async Task<ActionResult> DeletePhoto(int photoId){
            var user=await userRepository.GetUserByUserNameAsync(User.GetUserName());
            if(user==null) return BadRequest("Could not find the user");
            var photo=user.Photos.FirstOrDefault(x=>x.Id==photoId);
            if(photo==null || photo.IsMain) return BadRequest("This photo cannot be deleted.");
            if(photo.PublicId!=null){
                var result=await photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error!=null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if(await userRepository.SaveAllAsync()) return Ok();
            return BadRequest("Problem Deleting the photo");
        }
    }
}
