using MasterISS_Partner_WebSite.Models;
using RezaB.Web.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace MasterISS_Partner_WebSite.Authentication
{
    public class AdminAuthenticator: Authenticator<PartnerWebSiteEntities, User, SHA256>
    {
        public AdminAuthenticator() : base(u => u.Id, u => u.NameSurname, u => u.UserSubMail, u => u.Password, u => u.IsEnabled)
        {
        }
    }
}