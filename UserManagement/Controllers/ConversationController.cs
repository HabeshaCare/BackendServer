using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Attributes;
using UserManagement.Services.ChatServices;

namespace UserManagement.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/conversation")]
    public class ConversationController : ControllerBase
    {
        private readonly IChatAIService _chatAIService;
        public ConversationController(IChatAIService chatAIService)
        {
            _chatAIService = chatAIService;
        }

        /// <summary>
        /// Get a list of messages between the user and the Chat bot. This implementation doesn't have different conversation but stores all messages in a single collection.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns>ActionResult containing the list of messages.</returns>
        [HttpGet("{id}/chat/")]
        [AuthorizeAccess]
        public async Task<IActionResult> GetUserMessages(string id)
        {
            var response = await _chatAIService.GetMessages(id);
            return new ObjectResult(response);
        }

        [HttpPost("{id}/chat/")]
        [AuthorizeAccess]
        public async Task<IActionResult> AskAI([FromBody] string message, string id)
        {
            var response = await _chatAIService.AskAI(id, message);
            return new ObjectResult(response);
        }
    }
}