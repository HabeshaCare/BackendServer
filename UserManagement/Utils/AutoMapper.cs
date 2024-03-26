using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using UserManagement.Models;
using UserManagement.DTOs.MessageDTOs;
using UserManagement.DTOs.ScheduleDTOs;
using UserManagement.Models.DTOs;
using UserManagement.Models.DTOs.UserDTOs;
using UserManagement.DTOs.PatientDTOs;
using UserManagement.DTOs.AdminDTOs;
using UserManagement.DTOs.InstitutionDTOs;
using UserManagement.DTOs.HealthCenterDTOs;
using UserManagement.DTOs.PharmacyDTOs;
using UserManagement.DTOs.LaboratoryDTOs;

namespace UserManagement.Utils
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            //Used to check if a property is not null before trying to map.
            CreateMap<RegistrationDTO, Administrator>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<RegistrationDTO, Doctor>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<RegistrationDTO, Patient>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<User, UserDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<User, Doctor>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<User, Patient>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<User, Administrator>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<User, LoginDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<User, UsageUserDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<User, UsagePatientDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<User, UsageAdminDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<User, UpdatePatientDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<User, UpdateAdminDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<User, UpdateUserDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<User, RegistrationDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<User, SchedulerUserDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));

            CreateMap<Doctor, UpdateDoctorDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<Doctor, UsageDoctorDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<Doctor, ScheduledDoctorDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));

            CreateMap<Patient, UsagePatientDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<Patient, UpdatePatientDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));

            CreateMap<Schedule, ScheduleDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<Schedule, CreateScheduleDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));

            CreateMap<Institution, HealthCenter>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<Institution, Pharmacy>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<Institution, Laboratory>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<Institution, InstitutionDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));

            CreateMap<HealthCenter, HealthCenterDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<HealthCenter, UpdateHealthCenterDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));

            CreateMap<Administrator, UsageAdminDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<Administrator, UpdateAdminDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));

            CreateMap<Pharmacy, PharmacyDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));

            CreateMap<Laboratory, LaboratoryDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));
            CreateMap<Laboratory, UpdateLaboratoryDTO>().ReverseMap().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && (!string.IsNullOrEmpty(srcMember.ToString()) || (srcMember is Array array && array.Length > 0))));

            CreateMap<Message, MessageDTO>().ReverseMap();
            CreateMap<Message, UsageMessageDTO>().ReverseMap();
        }
    }
}