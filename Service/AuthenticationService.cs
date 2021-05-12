using Core.Configuration;
using Core.Dtos;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Client> _client;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenservice;

        public AuthenticationService(IOptions<List<Client>> optionsClient,ITokenService tokenService,UserManager<UserApp> userManager,IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenservice)

        {
            _client = optionsClient.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshTokenservice = userRefreshTokenservice;
        }
        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return Response<TokenDto>.Fail("Email or Password is wrong",400,true);

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password)) return Response<TokenDto>.Fail("Email or Password is wrong", 400, true);

            var token = _tokenService.CreateToken(user);
            var userRefreshToken = await _userRefreshTokenservice.Where(x => x.UserID == user.Id).SingleOrDefaultAsync();
            if (userRefreshToken == null)
            {
                await _userRefreshTokenservice.AddAsync(new UserRefreshToken { UserID = user.Id, Code = token.RefreshToken, Expiration = token.RefreshTokenExpiration });
            }
            else
            {
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
            }

            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(token, 200);
        }

        public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var client = _client.SingleOrDefault(x=>x.Id==clientLoginDto.ClientId && x.Secret==clientLoginDto.ClientSecret);
            if (client == null)
            {
                return Response<ClientTokenDto>.Fail("ClientId or ClientSecret not found", 404, true);
            }

            var token = _tokenService.CreteTokenByClient(client);
            return Response<ClientTokenDto>.Success(token, 200);
        }

        public async Task<Response<TokenDto>> CreteTokenByRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenservice.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken == null)
            {
                return Response<TokenDto>.Fail("Refresh Token not found", 404, true);
            }
            var user = await _userManager.FindByIdAsync(existRefreshToken.UserID);
            if (user == null)
            {
                return Response<TokenDto>.Fail("UserId not found", 404, true);
            }

            var tokenDto = _tokenService.CreateToken(user);
            existRefreshToken.Code = tokenDto.RefreshToken;
            existRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;
            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(tokenDto, 200);
        }

        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenservice.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken == null)
            {
                return Response<NoDataDto>.Fail("Refresh Token not found", 404, true);
            }
            _userRefreshTokenservice.Remove(existRefreshToken);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(200);
        }
    }
}
