using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAuthentication.Utils
{
    public class MongoDBSettings
    {
        public string ConnectionUrl {get; set;} = "";
        public string DBName {get; set; } = "";
    }
}