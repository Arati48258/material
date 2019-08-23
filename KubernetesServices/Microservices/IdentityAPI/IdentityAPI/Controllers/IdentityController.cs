using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityAPI.Infrastructure;
using IdentityAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IdentityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {

        private IdentityDbContext db;
        private IConfiguration configuration;

        public IdentityController(IdentityDbContext dbContext, IConfiguration configuration)
        {
            this.db = dbContext;
            this.configuration = configuration;
        }

        //POST /api/identity/auth/register
        [HttpPost("auth/register", Name ="Register")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult<dynamic>> RegisterAsync(UserInfo model)
        {
            TryValidateModel(model);
            if(ModelState.IsValid)
            {
                var result = await db.Users.AddAsync(model);
                await db.SaveChangesAsync();
                return Created("", new
                {
                    result.Entity.Id, result.Entity.FirstName,result.Entity.LastName,result.Entity.Email
                });
            }
            else
            {
                return BadRequest(ModelState);
            }
        }


        //POST /api/identity/auth/token
        [HttpPost("auth/token", Name ="GetToken")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult<dynamic> GetToken(LoginModel model)
        {
            TryValidateModel(model);
            if(ModelState.IsValid)
            {
                var user = db.Users.SingleOrDefault(u => u.Email == model.Email && u.Password == model.Password);
                if(user==null)
                {
                    return Unauthorized();
                }
                else
                {
                    return Ok(GenerateToken(user));
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        private dynamic GenerateToken(UserInfo user)
        {
            var claims = new List<Claim> {
                    new Claim(JwtRegisteredClaimNames.Sub, user.FirstName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, configuration.GetValue<string>("Jwt:Audience")));
            //claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "catalog"));
            //claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "payment"));
            //claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "cart"));

            //claims.Add(new Claim(ClaimTypes.Role, "manager"));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Secret")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("Jwt:Issuer"),
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );
            var tokenString= new JwtSecurityTokenHandler().WriteToken(token);
            return new
            {
                user.FirstName,
                user.LastName,
                token = tokenString
            };
        }
    }
}