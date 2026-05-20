using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenCarWash.Api.Helpers
{
    public class EmailSettings
    {
        public string Host{get;set;} = string.Empty;
        public int Port{get;set;}
        public string SenderEmail{get;set;} = string.Empty;
        public string SenderName{get;set;} = string.Empty;
        public string Username{get;set;} = string.Empty;
        public string Password{get;set;} = string.Empty;
        public bool EnableSsl{get;set;}
    }
}