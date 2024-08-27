﻿using BookLib.DTOs;
using Microsoft.AspNetCore.Identity;

namespace BookLib.Services
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUser(UserForRegistrationDTO userForRegistrationDTO);
        Task<bool> ValidateUser(UserForAuthenticationDTO userForAuthenticationDTO);
        Task<string> CreateToken();
    }
}
