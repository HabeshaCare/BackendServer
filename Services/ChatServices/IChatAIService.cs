using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAuthentication.DTOs.MessageDTOs;
using UserAuthentication.Models;

namespace UserAuthentication.Services.ChatServices
{
    public interface IChatAIService
    {
        Task<(int, string?, UsageMessageDTO?)> AddMessage(string userId, string message, MessageType messageType = MessageType.Human);
        Task<(int, string?, UsageMessageDTO[])> GetMessages(string userId);
    }
}