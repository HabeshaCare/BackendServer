using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.DTOs;
using UserManagement.DTOs.MessageDTOs;
using UserManagement.Models;

namespace UserManagement.Services.ChatServices
{
    public interface IChatAIService
    {
        Task<SResponseDTO<UsageMessageDTO[]>> GetMessages(string userId);
        Task<SResponseDTO<UsageMessageDTO>> AskAI(string userId, string message);
    }
}