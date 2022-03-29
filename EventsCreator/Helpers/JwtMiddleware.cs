using EventsCreator.EfStuff.Repository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace EventsCreator.Helpers
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly ILogger _logger;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context, UserRepository userRepository)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                AttachUserToContext(context, userRepository, token);

            await _next(context);
        }
        public void AttachUserToContext(HttpContext context, UserRepository userRepository, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                // min 16 characters
                var key = Encoding.ASCII.GetBytes(AuthOptions.KEY);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                context.Items["User"] = userRepository.Get(userId);
            }
            catch
            {
                // todo: need to add logger
            }
        }
    }
}
