using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using UserAuthentication.DTOs.MessageDTOs;
using UserAuthentication.Models;
using UserAuthentication.Utils;

namespace UserAuthentication.Services.ChatServices
{
    public class ChatAIService : MongoDBService, IChatAIService
    {
        private readonly IMongoCollection<Message> _messageCollection;
        private readonly IMapper _mapper;

        public ChatAIService(IOptions<MongoDBSettings> options, IMapper mapper) : base(options)
        {
            _messageCollection = GetCollection<Message>("Messages");
            _mapper = mapper;
        }
        private async Task<(int, string?, UsageMessageDTO?)> AddMessage(string userId, string message, MessageType messageType = MessageType.Human)
        {
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

        public async Task<(int, string?, UsageMessageDTO?)> AskAI(string userId, string message)
        {
            try
            {
                //TODO: Ask llm for response
                var addUserMessage = Task.Run(() => AddMessage(userId, message));


                var (status, statusMessage, answer) = await HttpPostRequest(message,"https://hakim-llm.onrender.com");
                var addAiMessage = Task.Run(() => AddMessage(userId, answer, MessageType.AI));

                var result = await Task.WhenAll(addUserMessage, addAiMessage);
                UsageMessageDTO? aiMessage = result[0].Item3;

                if(status == 1 && aiMessage != null)
                    return (1, "Asking llm successful", aiMessage);
                return (0, message, null);
            }
            catch (Exception ex)
            {

                return (0, ex.Message, null);
            }
        }

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