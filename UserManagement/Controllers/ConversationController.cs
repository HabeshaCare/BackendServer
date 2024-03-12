using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Services.ChatServices;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/conversation")]
    public class ConversationController : ControllerBase
    {
        private readonly IChatAIService _chatAIService;
        public ConversationController(IChatAIService chatAIService)
        {
            _chatAIService = chatAIService;
        }
        [HttpGet("{id}/chat/")]
        public async Task<IActionResult> GetUserMessages(string id)
        {
            var (status, message, messages) = await _chatAIService.GetMessages(id);
            if (status == 0)
                return BadRequest(new { error = message });

            return Ok(new { successMessage = message, messages });
        }

        [HttpPost("{id}/chat/")]
        public async Task<IActionResult> AskAI([FromBody] string message, string id)
        {
            var (status, statusMessage, response) = await _chatAIService.AskAI(id, message);
            if (status == 0 || response == null)
                return NotFound(new { error = "AI server not found" });
            return Ok(new { response, message = statusMessage });
        }
    }
}