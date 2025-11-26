using Microsoft.IdentityModel.Tokens;
using Reservas_Laboratorio.Services;
using System.IdentityModel.Tokens.Jwt;

namespace Reservas_Laboratorio.Middleware
{
    public class JwtRefreshMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtRefreshMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService authService, ITokenService tokenService)
        {

            bool isApiRequest = context.Request.Path.StartsWithSegments("/api");
            var accessToken = context.Request.Cookies["access_token"];
            var refreshToken = context.Request.Cookies["refresh_token"];

            // Si no hay tokens → continuar 
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                await _next(context);
                return;
            }

            var handler = new JwtSecurityTokenHandler();

            try
            {
                //  VALIDACIÓN COMPLETA DEL TOKEN 
                handler.ValidateToken(accessToken, tokenService.GetValidationParameters(), out _);

                // Token válido → continuar 
                await _next(context);
                return;
            }
            catch (SecurityTokenExpiredException)
            {
                //  ACCESS TOKEN EXPIRADO → intentar refrescar 
                var newTokens = await authService.RefreshAccessTokenAsync(refreshToken);

                if (newTokens == null)
                {
                    //  Refresh falló → borrar tokens                   
                    context.Response.Cookies.Delete("access_token");
                    context.Response.Cookies.Delete("refresh_token");

                    if (isApiRequest)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(new { error = "token_expired" });
                        return;
                    }
                    context.Response.Redirect("/Auth/Login");
                    return;
                }

                //  Guardar AccessToken y RefreshToken NUEVOS     
                context.Response.Cookies.Append("access_token", newTokens.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = newTokens.ExpiresAt
                });

                context.Response.Cookies.Append("refresh_token", newTokens.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                await _next(context);
                return;
            }
            catch
            {
                //  TOKEN INVÁLIDO, MANIPULADO O CORRUPTO 
                context.Response.Cookies.Delete("access_token");
                context.Response.Cookies.Delete("refresh_token");

                if (isApiRequest)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { error = "invalid_token" });
                    return;
                }

                context.Response.Redirect("/Auth/Login");
                return;
            }
        }
    }

}
