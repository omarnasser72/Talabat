using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Models;
using Talabat.Core.Entities;

namespace Talabat.APIs.Controllers
{
    public class AuthenticationController : ApiBaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;

        public AuthenticationController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        #region Register
        [HttpPost]
        public async Task<ActionResult<UserDTO>> Register(RegisterModel registerModel)
        {
            if (IsEmailExist(registerModel.Email).Result.Value)
                return BadRequest(new ApiResponse(400, "Email already exist"));
            var user = new AppUser()
            {
                DisplayName = registerModel.DisplayName,
                Email = registerModel.Email,
                PhoneNumber = registerModel.PhoneNumber,
            };
            var res = await _userManager.CreateAsync(user, registerModel.Password);
            if (res.Succeeded)
            {
                var userDTO = new UserDTO()
                {
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    Token = "token"
                };
                return Ok(userDTO);
            }
            return BadRequest(new ApiResponse(400, "Couldn't create user."));
        }
        #endregion

        #region Login
        [HttpPost("Login")]
        public async Task<ActionResult<UserDTO>> Login(LoginModel loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);

            if (user == null)
                return Unauthorized(new ApiResponse(401, "Email doesn't exist"));

            var login = await _signInManager.CheckPasswordSignInAsync(user, loginModel.Password, false);

            if (login.Succeeded)
            {
                var userDTO = _mapper.Map<AppUser, UserDTO>(user);
                return Ok(userDTO);
            }
            return Unauthorized(new ApiResponse(401, "Wrong password"));
        }
        #endregion

        #region GetCurrentUser
        [Authorize]
        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<UserDTO?>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest(new ApiResponse(400, "Email doesn't exist"));
            var userDTO = _mapper.Map<AppUser, UserDTO>(user);
            //userDTO.Token = await
            return Ok(userDTO);
        }
        #endregion

        #region GetAddress
        [HttpGet("GetAddress")]
        public async Task<ActionResult<Address>> GetAddress()
        {
            var user = await _userManager.FindUserWithAddressAsync(User);
            if (user == null)
                return BadRequest(new ApiResponse(400, "Email doesn't exist"));
            var addressDTO = _mapper.Map<Address, AddressDTO>(user.Address);
            return Ok(new { addressDTO });
        }
        #endregion

        #region UpdateAddress
        [Authorize]
        [HttpPut("UpdateAddress")]
        public async Task<ActionResult<AddressDTO>> UpdateAddress(AddressModel addressModel)
        {
            var user = await _userManager.FindUserWithAddressAsync(User);
            if (user == null)
                return BadRequest(new ApiResponse(400, "Couldn't update user"));

            var address = _mapper.Map<AddressModel, Address>(addressModel);

            address.Id = user.Address.Id;
            user.Address = address;

            var userUpdate = await _userManager.UpdateAsync(user);
            if (userUpdate.Succeeded)
            {
                var addressDTO = _mapper.Map<Address, AddressDTO>(user.Address);
                return Ok(new { addressDTO });
            }
            return BadRequest(new ApiResponse(400, "Couldn't update user"));
        }
        #endregion

        #region Email Existence
        [HttpGet("EmailExist")]
        public async Task<ActionResult<bool>> IsEmailExist(string email)
            => await _userManager.FindByEmailAsync(email) != null;
        #endregion
    }
}
