using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
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
        private async Task<(int, string?, UsageMessageDTO?)> AddMessage(string userId, string message, MessageType messageType = MessageType.Human)
        {

            //Guard to prevent null message from being sent
            if (userId == null || message == null)
            {
                return (0, "Not all required fields are set", null);
            }

            Message newMessage = new() { UserId = userId, Type = messageType, Content = message };
            var createdMessage = _mapper.Map<UsageMessageDTO>(newMessage);
            try
            {
                await _messageCollection.InsertOneAsync(newMessage);
                return (1, "Message added successfully", createdMessage);
            }
            catch (Exception ex)
            {

                return (0, ex.Message, null);
            }
        }

        /// Sends an HTTP POST request to the AI server to ask a question.
        private static async Task<(int, string?, string?)> HttpPostRequest(string question, string url = "http://localhost:5000/ask")
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

                return (response.IsSuccessStatusCode ? 1 : 0, null, answer?.ToString());
            }
            catch (Exception ex)
            {
                return (0, ex.Message, null);
            }
        }

        /// Asks the AI a question and adds the user's and AI's messages to the chat log.
        public async Task<(int, string?, UsageMessageDTO?)> AskAI(string userId, string message)
        {
            try
            {
                var addUserMessage = Task.Run(() => AddMessage(userId, message));

                string llmUrl = _configuration["LLMUrl"]!;
                var (status, statusMessage, answer) = await HttpPostRequest(message, llmUrl);
                var addAiMessage = Task.Run(() => AddMessage(userId, answer ?? "", MessageType.AI));

                // Waits for both tasks to complete that are being executed in parallel.
                var result = await Task.WhenAll(addUserMessage, addAiMessage);
                UsageMessageDTO? aiMessage = result[1].Item3;

                if (status == 1 && aiMessage != null)
                    return (1, "Asking llm successful", aiMessage);
                return (0, message, null);
            }
            catch (Exception ex)
            {

                return (0, ex.Message, null);
            }
        }

        /// Retrieves all messages for a specific user from the chat log.
        public async Task<(int, string?, UsageMessageDTO[])> GetMessages(string userId)
        {
            try
            {
                var result = await _messageCollection.Find(m => m.UserId == userId).ToListAsync();
                UsageMessageDTO[] messages = _mapper.Map<UsageMessageDTO[]>(result);
                return (1, "Found messages", messages);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, Array.Empty<UsageMessageDTO>());
            }
        }
    }
}