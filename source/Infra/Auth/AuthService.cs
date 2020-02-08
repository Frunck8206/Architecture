using Architecture.CrossCutting;
using Architecture.CrossCutting.Resources;
using Architecture.Model;
using DotNetCore.Extensions;
using DotNetCore.Results;
using DotNetCore.Security;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Architecture.Infra
{
    public class AuthService : IAuthService
    {
        private readonly IHashService _hashService;
        private readonly IJsonWebTokenService _jsonWebTokenService;

        public AuthService
        (
            IHashService hashService,
            IJsonWebTokenService jsonWebTokenService
        )
        {
            _hashService = hashService;
            _jsonWebTokenService = jsonWebTokenService;
        }

        public AuthModel CreateAuth(SignInModel signInModel)
        {
            var salt = Guid.NewGuid().ToString();

            var password = _hashService.Create(signInModel.Password, salt);

            return new AuthModel(signInModel.Login, password, salt, Roles.User);
        }

        public TokenModel CreateToken(SignModel signModel)
        {
            var claims = new List<Claim>();

            claims.AddSub(signModel.UserId.ToString());

            claims.AddRoles(signModel.Roles.ToString().Split(", "));

            var token = _jsonWebTokenService.Encode(claims);

            return new TokenModel(token);
        }

        public IResult Validate(SignModel signModel, SignInModel signInModel)
        {
            if (signModel == default || signInModel == default)
            {
                return Result.Fail(Texts.LoginPasswordInvalid);
            }

            var password = _hashService.Create(signInModel.Password, signModel.Salt);

            if (signModel.Password != password)
            {
                return Result.Fail(Texts.LoginPasswordInvalid);
            }

            return Result.Success();
        }
    }
}
