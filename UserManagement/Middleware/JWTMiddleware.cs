using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace UserManagement.Middleware
{
    /// Middleware for extracting and storing JWT claims in the HttpContext.
    public class JWTMiddleware
    {
        private readonly RequestDelegate _next;

        public JWTMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Extracts user Id and role claims from the JWT token.
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var InstitutionId = context.User.FindFirst("InstitutionId")?.Value;
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

            // Stores the extracted values in the HttpContext items for later use in controllers
            context.Items["UserId"] = userId;
            context.Items["Role"] = role;
            context.Items["InstitutionId"] = InstitutionId;

            await _next(context);
        }
    }
}