using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAuthentication.DTOs.MessageDTOs
{
    public class UsageMessageDTO : MessageDTO
    {
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}