using System.Collections;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthMicroservice.API.ApplicationOptions;
using AuthMicroservice.API.Dtos;
using AuthMicroservice.API.Repositories;
using AuthMicroservice.Data.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace AuthMicroservice.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UserController(IUserRepo userRepo, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]UserDto userDto)
        {
            var user = _userRepo.Authenticate(userDto.UserName, userDto.Password);

            if(user == null)
                return BadRequest(new {message = "Username or password is incorrect"});

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("_appSettings.Secret");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserID.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                                            SecurityAlgorithms.HmacSha256Signature) 
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            //return basic user info (without password) and token to store client side
            return Ok(new {
                Id = user.UserID,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]UserDto userDto)
        {
            //map dto to entity
            var user = _mapper.Map<User>(userDto);

            try{
                //save
                _userRepo.Create(user, userDto.Password);
                return Ok();
            }
            catch(AppException ex)
            {
                //return error message if there was an exception
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userRepo.GetAll();
            var userDto = _mapper.Map<IList<UserDto>>(users);
            return Ok(userDto);
        }   

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]UserDto userDto)
        {
            //map to entity and set id
            var user = _mapper.Map<User>(userDto);
            
            user.UserID = id;

            try{
                //save
                _userRepo.Update(user, userDto.Password);
                return Ok();
            }
            catch(AppException ex)
            {
                //returns error message if there is an exceptiion
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userRepo.Delete(id);
            return Ok();
        }
    }
}