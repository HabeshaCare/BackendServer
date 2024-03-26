using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UserManagement.DTOs.PatientDTOs;
using UserManagement.Models;
using UserManagement.Services.InstitutionService.HealthCenterService;

namespace UserManagement.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizePatientAccess : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var healthCenterService = (IHealthCenterService)context.HttpContext.RequestServices.GetService(typeof(IHealthCenterService))!;
            var institutionIdFromToken = context.HttpContext.Items["InstitutionId"]?.ToString() ?? "";
            var userRole = context.HttpContext.Items["Role"]?.ToString();
            var resourceId = context.RouteData.Values["id"]?.ToString();

            bool isSamePatient = institutionIdFromToken == resourceId;
            bool isSuperAdmin = userRole == UserRole.SuperAdmin.ToString();

            var response = await healthCenterService.GetSharedPatients(institutionIdFromToken);

            if (!response.Success)
            {
                context.Result = new ForbidResult("Error getting shared patients");
                return;
            }

            List<UsagePatientDTO> sharedPatients = response.Data!;

            bool healthCenterHasAccess = sharedPatients.Any(p => p.Id == resourceId);

            if (isSamePatient || healthCenterHasAccess || isSuperAdmin)
            {
                await next();
                return;
            }

            context.Result = new ForbidResult("You don't have access to this patient");
        }

    }
}