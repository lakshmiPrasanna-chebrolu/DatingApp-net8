using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey= config["TokenKey"] ?? throw new Exception("Cannot access tokenKey from appSettings");
        if(tokenKey.Length<64) throw new Exception("Your tokenKey needs to be longer");
        // the token key must have length more than 64. 
        // this token is encrypted in to symmetric security key and used further for validating the token 
        var key= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        // we need to claim the token with the user name, role and date of birth 
        // Here we are claiming with NameIdentifier (that is user name)

        var claims= new List<Claim>{
            new Claim(ClaimTypes.NameIdentifier,user.UserName)
        };

        //sigining the credentials with the encrypted key and security algorithms 
        //Here we are using the hmac sha 512 algorithm
        var creds= new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        //we can consider the tokenDescriptor to be creating the payload with the claims,expiry date, signing credentials

        var tokenDescriptor= new SecurityTokenDescriptor{
            Subject= new ClaimsIdentity(claims),
            Expires=DateTime.UtcNow.AddDays(7),
            SigningCredentials=creds
        };

        //Token Handler is the one , which is going to create the token with all the details and 
        //store it in the server for that session.
        var tokenHandler= new JwtSecurityTokenHandler();
        var token=tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
