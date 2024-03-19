using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UserManagement.Models;

namespace UserManagement.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeInstitutionAccess : Attribute, IAsyncActionFilter
    {
        /*This Attribute is responsible for only verifying if the update or access 
        to certain private methods are only being accessed by the user itself 
        and no other user with similar role. It, however, doesn't catch any unauthorized access 
        by the super admin and that should be handled else where. */

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userIdFromToken = context.HttpContext.Items["InstitutionId"]?.ToString();
            var userRole = context.HttpContext.Items["Role"]?.ToString();
            var resourceId = context.RouteData.Values["id"]?.ToString();

            bool isSameAdmin = userIdFromToken == resourceId;
            bool isSuperAdmin = userRole == UserRole.SuperAdmin.ToString();
            bool isHealthCenterAdmin = userRole == UserRole.HealthCenterAdmin.ToString();

            if (isSameAdmin || isHealthCenterAdmin || isSuperAdmin)
            {
                await next();
                return;
            }

            context.Result = new ForbidResult();
        }
    }
}