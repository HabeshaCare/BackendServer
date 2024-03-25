using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using UserManagement.DTOs;
using UserManagement.DTOs.MessageDTOs;
using UserManagement.Models;
using UserManagement.Utils;

namespace UserManagement.Services.ChatServices
{
    public class ChatAIService : MongoDBService, IChatAIService
    {
        private readonly IMongoCollection<Message> _messageCollection;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public ChatAIService(IOptions<MongoDBSettings> options, IMapper mapper, IConfiguration configuration) : base(options)
        {
            _messageCollection = GetCollection<Message>("Messages");
            _configuration = configuration;
            _mapper = mapper;
        }
        private async Task<SResponseDTO<UsageMessageDTO>> AddMessage(string userId, string message, MessageType messageType = MessageType.Human)
        {

            //Guard to prevent null message from being sent
            if (userId == null || message == null)
            {
                return new() { StatusCode = StatusCodes.Status400BadRequest, Errors = new() { "Not all required fields are set" } };
            }

            Message newMessage = new() { UserId = userId, Type = messageType, Content = message };
            var createdMessage = _mapper.Map<UsageMessageDTO>(newMessage);
            try
            {
                await _messageCollection.InsertOneAsync(newMessage);
                return new() { StatusCode = StatusCodes.Status201Created, Message = "Message added successfully", Data = createdMessage, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = StatusCodes.Status500InternalServerError, Errors = new() { ex.Message } };
            }
        }

        /// Sends an HTTP POST request to the AI server to ask a question.
        private static async Task<SResponseDTO<string>> HttpPostRequest(string question, string url = "http://localhost:5000/ask")
        {
            using HttpClient httpClient = new();
            try
            {
                var requestData = new
                {
                    query = question
                };

                string jsonBody = JsonSerializer.Serialize(requestData);

                StringContent content = new(jsonBody, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(url, content);

                string responseData = await response.Content.ReadAsStringAsync();
                var answer = JObject.Parse(responseData)["Response"];

                return new() { StatusCode = response.IsSuccessStatusCode ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable, Data = answer?.ToString(), Success = response.IsSuccessStatusCode };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = StatusCodes.Status500InternalServerError, Errors = new() { ex.Message } };
            }
        }

        /// Asks the AI a question and adds the user's and AI's messages to the chat log.
        public async Task<SResponseDTO<UsageMessageDTO>> AskAI(string userId, string message)
        {
            try
            {
                var addUserMessage = Task.Run(() => AddMessage(userId, message));

                string llmUrl = _configuration["LLMUrl"]!;
                var response = await HttpPostRequest(message, llmUrl);
                var addAiMessage = Task.Run(() => AddMessage(userId, response.Data ?? "", MessageType.AI));

                // Waits for both tasks to complete that are being executed in parallel.
                var result = await Task.WhenAll(addUserMessage, addAiMessage);
                UsageMessageDTO? aiMessage = result[1].Data;

                if (response.Success && aiMessage != null)
                    return new() { StatusCode = StatusCodes.Status200OK, Message = "Asking llm successful", Data = aiMessage, Success = true };
                return new() { StatusCode = response.StatusCode, Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = StatusCodes.Status500InternalServerError, Errors = new() { ex.Message } };
            }
        }

        /// Retrieves all messages for a specific user from the chat log.
        public async Task<SResponseDTO<List<UsageMessageDTO>>> GetMessages(string userId)
        {
            try
            {
                var result = await _messageCollection.Find(m => m.UserId == userId).ToListAsync();
                List<UsageMessageDTO> messages = _mapper.Map<List<UsageMessageDTO>>(result);
                return new() { StatusCode = StatusCodes.Status200OK, Message = "Found messages", Data = messages, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = StatusCodes.Status500InternalServerError, Errors = new() { ex.Message } };
            }
        }
    }
}