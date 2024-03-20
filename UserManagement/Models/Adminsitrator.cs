using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UserManagement.Models
{
    public class Administrator : User
    {
        /* 
        Currently, this class is only needed to keep because 
        administrators are kept in a separate collection and 
        a different type is need to handle it
        */
        //The list of institution ids managed by the administrator
        public string InstitutionId { get; set; } = string.Empty;
    }
}