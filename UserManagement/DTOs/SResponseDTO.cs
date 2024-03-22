using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.DTOs
{
    public class SResponseDTO<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string[] Errors { get; set; } = Array.Empty<string>();
        public string Token { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}