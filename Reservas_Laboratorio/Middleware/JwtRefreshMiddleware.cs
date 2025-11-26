using System.IdentityModel.Tokens.Jwt;
using Reservas_Laboratorio.Services;

namespace Reservas_Laboratorio.Middleware
{
    public class JwtRefreshMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtRefreshMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService authService)
        {
            bool isApiRequest = context.Request.Path.StartsWithSegments("/api");

            var accessToken = context.Request.Cookies["access_token"];
            var refreshToken = context.Request.Cookies["refresh_token"];

            if (!string.IsNullOrEmpty(accessToken))
            {
                var handler = new JwtSecurityTokenHandler();

                try
                {
                    var jwtToken = handler.ReadJwtToken(accessToken);

                    // Token expirado
                    if (jwtToken.ValidTo < DateTime.UtcNow)
                    {
                        if (!string.IsNullOrEmpty(refreshToken))
                        {
                            var newTokens = await authService.RefreshAccessTokenAsync(refreshToken);

                            if (newTokens != null)
                            {
                                var accessOptions = new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = true,
                                    SameSite = SameSiteMode.Strict,
                                    Expires = newTokens.ExpiresAt
                                };

                                var refreshOptions = new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = true,
                                    SameSite = SameSiteMode.Strict,
                                    Expires = DateTime.UtcNow.AddDays(7)
                                };

                                context.Response.Cookies.Append("access_token", newTokens.AccessToken, accessOptions);
                                context.Response.Cookies.Append("refresh_token", newTokens.RefreshToken, refreshOptions);
                            }
                            else
                            {
                                //  Refresh falló: limpiar tokens
                                context.Response.Cookies.Delete("access_token");
                                context.Response.Cookies.Delete("refresh_token");

                                if (isApiRequest)
                                {
                                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                    await context.Response.WriteAsJsonAsync(new { error = "token_refresh_failed" });
                                    return;
                                }

                                context.Response.Redirect("/Auth/Login");
                                return;
                            }
                        }
                    }
                }
                catch
                {
                    // Token dañado o inválido
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

            await _next(context);
        }
    }

}
