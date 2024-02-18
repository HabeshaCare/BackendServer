using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.DTOs.MessageDTOs;
using UserManagement.Models;

namespace UserManagement.Services.ChatServices
{
    public interface IChatAIService
    {
        Task<(int, string?, UsageMessageDTO[])> GetMessages(string userId);
        Task<(int, string?, UsageMessageDTO?)> AskAI(string userId, string message);
    }
}