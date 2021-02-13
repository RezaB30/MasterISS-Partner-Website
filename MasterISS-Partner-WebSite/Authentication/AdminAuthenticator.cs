﻿using MasterISS_Partner_WebSite.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using RezaB.Web.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Web;

namespace MasterISS_Partner_WebSite.Authentication
{
    public static class AdminAuthenticator
    {
        public static bool AdminSignIn(this IOwinContext context, IEnumerable<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
            var authManager = context.Authentication;
            authManager.SignIn(identity);
            return true;
        }
    }
}