using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using MasterISS_Partner_WebSite.Models;
using RezaB.Web.Authentication;

namespace MasterISS_Partner_WebSite.Authentication
{
    public class SubUserAuthenticator : Authenticator<PartnerWebSiteEntities, User, SHA256>
    {
        public SubUserAuthenticator() : base(u => u.Id, u => u.NameSurname, u => u.UserSubMail, u => u.Password, u => u.IsEnabled)
        {
        }

    }
}