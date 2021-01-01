
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UptimeAPI.Messaging
{
    public enum AuthErrors
    {
        BadUserName,
        BadPassword,
        BadLogin,
        NoResourceAccess,
        KeysNoMatch, 
        NoMatchFound
    }
    public class Error 
    {
        public static Dictionary<AuthErrors, string> HttpRequest { get;  } = new Dictionary<AuthErrors, string>()
        {
            { AuthErrors.BadPassword, "Incorrect password"  },
            { AuthErrors.BadUserName, "Incorrect username" },
            { AuthErrors.BadLogin, "Incorrect username or password" },
            { AuthErrors.NoResourceAccess, "This user does not have access to this data resource" },
            { AuthErrors.KeysNoMatch, "Supplied keys do not match" },
                        { AuthErrors.NoMatchFound, "No match was found" },

        };
    }
}
