﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Shelfy.Core.Domain;
using Shelfy.Core.Repositories;
using Shelfy.Infrastructure.DTO;
using Shelfy.Infrastructure.DTO.Jwt;
using Shelfy.Infrastructure.DTO.User;
using Shelfy.Infrastructure.Extensions;

namespace Shelfy.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncrypterService _encrypterService;
        private readonly IJwtHandler _jwtHandler;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IEncrypterService encrypterService,
            IJwtHandler jwtHandler, IMapper mapper)
        {
            _userRepository = userRepository;
            _encrypterService = encrypterService;
            _jwtHandler = jwtHandler;
            _mapper = mapper;
        }

        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetOrFailAsync(id);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetByUserNameAsync(string username)
        {
            var user = await _userRepository.GetOrFailAsync(username);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.BrowseAsync();

            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task RegisterAsync(Guid userid, string email, string username,
            string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null)
            {
                throw new Exception($"User with email '{email}' already exist.");
            }

            user = await _userRepository.GetByUsernameAsync(username);
            if (user != null)
            {
                throw new Exception($"User with username '{username}' already exist.");
            }

            password.PasswordValidation();

            var salt = _encrypterService.GetSalt();
            var hash = _encrypterService.GetHash(password, salt);
            var defaultAvatar = "https://www.stolarstate.pl/avatar/user/default.png";

            user = new User(userid, email, username, hash, salt, Role.User, defaultAvatar);

            await _userRepository.AddAsync(user);
        }

        public async Task<TokenDto> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("Invalid credentials, try again.");
            }

            var hash = _encrypterService.GetHash(password, user.Salt);
            if (user.Password != hash)
            {
                throw new Exception("Invalid credentials, try again.");
            }

            var jwt = _jwtHandler.CreateToken(user.UserId, user.Role);

            return new TokenDto
            {
                Token = jwt.Token,
                Role = user.Role,
                Expires = jwt.Expires
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetOrFailAsync(id);

            await _userRepository.RemoveAsync(user.UserId);
        }

        public async Task ChangePassword(Guid id, string oldPassword, string newPassword)
        {
            var user = await _userRepository.GetOrFailAsync(id);

            var salt = user.Salt;
            var hash = _encrypterService.GetHash(oldPassword, salt);

            if (user.Password != hash)
            {
                throw new Exception("Your old password is incorrect, try again.");
            }

            newPassword.PasswordValidation();

            user.SetPassword(newPassword);
        }

        public async Task SetAvatar(Guid id, string avatar)
        {
            var user = await _userRepository.GetOrFailAsync(id);

            user.SetAvatar(avatar);
        }
    }
}