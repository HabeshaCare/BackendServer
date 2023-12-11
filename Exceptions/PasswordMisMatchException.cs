using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAuthentication.Exceptions
{
    public class PasswordMisMatchException : Exception
    {
        public PasswordMisMatchException() :base("Passwords Don't match")
        {
            
        }
        
    }
}