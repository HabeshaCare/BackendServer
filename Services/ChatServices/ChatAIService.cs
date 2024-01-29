using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
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

        private async Task<(int, string?, string?)> HttpPostRequest(string question, string url = "http://localhost:5000/ask")
        {
            using (HttpClient httpClient = new HttpClient())
            {
                // Specify the API endpoint URL

                try
                {
                    var requestData = new
                    {
                        query = question
                    };

                    string jsonBody = JsonSerializer.Serialize(requestData);

                    StringContent content = new(jsonBody, System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("API Response: " + responseData);
                    }
                    else
                    {
                        Console.WriteLine($"API Request Failed: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                    return (1, "Message success", null);
                }
                catch (Exception ex)
                {
                    return (0, ex.Message, null);
                }
            }
        }

        public async Task<(int, string?, UsageMessageDTO?)> AskAI(string userId, string message)
        {
            try
            {
                //TODO: Ask llm for response
                var addUserMessage = Task.Run(() => AddMessage(userId, message));
                var addAiMessage = Task.Run(() => AddMessage(userId, message, MessageType.AI));
                await Task.WhenAll(addUserMessage, addAiMessage);
                await HttpPostRequest("What am I sick about?");
                return (1, "Asking llm successful", null);
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