using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace UserManagement.Models
{
    public enum UserRole
    {
        Patient,
        Doctor,
        SuperAdmin,
        HealthCenterAdmin,
        PharmacyAdmin,
        LaboratoryAdmin,
        Reception
    }
}