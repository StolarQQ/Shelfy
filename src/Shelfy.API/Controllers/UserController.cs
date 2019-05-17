﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Shelfy.Infrastructure.Commands;
using Shelfy.Infrastructure.DTO.Jwt;
using Shelfy.Infrastructure.Services;

namespace Shelfy.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMemoryCache _cache;

        public UserController(IUserService userService, IMemoryCache cache)
        {
            _userService = userService;
            _cache = cache;
        }

        [HttpGet("{id}", Name = "GetUserById")]
        [Authorize(Policy = "HasUserRole")]
        public async Task<IActionResult> Get(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with id '{id}' not found");
            }

            return Ok(user);
        }

        [HttpGet("username/{username}")]
        [Authorize(Policy = "HasUserRole")]
        public async Task<IActionResult> Get(string username)
        {
            var user = await _userService.GetByUserNameAsync(username);
            if (user == null)
            {
                return NotFound($"User with username'{username}' not found");
            }

            return Ok(user);
        }

        [HttpGet]
        //[Authorize(Policy = "HasUserRole")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int pageNumber, int pageSize)
        {
            var user = await _userService.BrowseAsync(pageNumber, pageSize);
           
            return Ok(user);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post(CreateUser command)
        {
            var userId = Guid.NewGuid();
            await _userService.RegisterAsync(userId, command.Email,
                command.Username, command.Password);

            return CreatedAtRoute("GetUserById", new {Id = userId}, command);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Login command)
        {
            await _userService.LoginAsync(command.Email, command.Password);
            var jwt = _cache.Get<TokenDto>(command.Email);

            return Ok(jwt);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                NotFound($"User with id '{id}' was not found");
            }

            await _userService.DeleteAsync(user.UserId);

            return NoContent();
        }
    }
}
