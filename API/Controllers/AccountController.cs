using API.DTOs;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {

        //Below usermanage and signinmanager are required for login in and user creation/update
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        //Injection ItokenService for tokens
        private readonly ITokenService tokenService;
        //Injecting Imapper for DTO conversion
        private readonly IMapper mapper;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager
            , ITokenService tokenService,IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        //with no route paramter, when we hit the Account this method will be called
        //Only authorized(logged in and user should supply the token) user can access this 
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            //client will send a token to get current user
            //getting the email address out of HTTPContext
            //HttpContext will have user data only if user is already logged in
            //var email =HttpContext.User?.Claims?.FirstOrDefault(x=>x.Type==ClaimTypes.Email)?.Value;
            //var user = await userManager.FindByNameAsync(email);

            //using userManager extension method(simple way)
            var user = await userManager.FindUserByEmailFromClaimPrinciple(User);

                return new UserDto
                {
                    Email = user.Email,
                    Token = tokenService.CreateToken(user),
                    DisplayName = user.DisplayName
                };
        }

        //To check if email address already exists
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery]string email)
        {
            //return true if email is not exists
            return await userManager.FindByEmailAsync(email)!=null;
        }

        //need to apply authorize as we retrieving user in the code
        [Authorize]
        //To get the user address
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            //HttpContext will have user data only if user is already logged in
            //var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

            //using userManager extension method(simple way)
            var user =await userManager.FindUserByClaimsPrincipleWithAddress(User);

            //user and address entities are having 1 to 1 relationship,
            //this lead to an error while returning user.Adddress directly ,A possible object cycle was detected.
            //This can either be due to a cycle or if the object depth is larger than the maximum allowed depth of 32
            //to avoid this using AddressDTO

            var addressdto=mapper.Map<Address,AddressDto>(user.Address);
            return addressdto;
        }

        [Authorize]
        //Updating address
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto address)
        {
            var user = await userManager.FindUserByClaimsPrincipleWithAddress(HttpContext.User);
            user.Address=mapper.Map<AddressDto,Address>(address);
            var result= await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(mapper.Map<Address,AddressDto>(user.Address));
            }
            return BadRequest("Problem Updating User Address!");
        }

        [HttpPost("login")]
        //using DTO object to use only required details for login 
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            //get the user details based on provided login details
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new ApiResponse(401));
            }
            //passing user, user shared password and setting accountlockout to false
            //Below will return identity signInResult of signin attempt
            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new ApiResponse(401));
            }
            return new UserDto
            {
                Email = user.Email,
                Token = tokenService.CreateToken(user),
                DisplayName = user.DisplayName
            };
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            //to check if provided email address already exists
            if(CheckEmailExistsAsync(registerDto.Email).Result.Value)
            {
                return new BadRequestObjectResult(new ApiValidationErrorResponse
                {
                    Errors = new[] { "Email address is in use" }
                });
            }

            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                //UserName will be a required value in Identity
                UserName = registerDto.Email
            };
            //passing identity user object and a password for adding new user
            var result = await userManager.CreateAsync(user, registerDto.Password);
            if(!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400));
            }
            return new UserDto
            {
                Email = user.Email,
                Token = tokenService.CreateToken(user),
                DisplayName = user.DisplayName
            };
        }
    }
}
