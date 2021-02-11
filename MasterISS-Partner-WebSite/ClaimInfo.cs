using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace MasterISS_Partner_WebSite
{
    public class ClaimInfo
    {
        public string PartnerSetupServiceUser()
        {
            var partnerSetupServiceUser = CurrentClaims().Where(c => c.Type == "SetupServiceUser")
                  .Select(c => c.Value).SingleOrDefault();
            return partnerSetupServiceUser;
        }
        public string PartnerSetupServiceHash()
        {
            var partnerSetupServiceHash = CurrentClaims().Where(c => c.Type == "SetupServiceHash")
                  .Select(c => c.Value).SingleOrDefault();
            return partnerSetupServiceHash;
        }
        public string PartnerId()
        {
            var partnerId = CurrentClaims().Where(c => c.Type == "UserId")
                  .Select(c => c.Value).SingleOrDefault();
            return partnerId;
        }
        public string GetPartnerName()
        {
            var partnerName = CurrentClaims().Where(c => c.Type == "PartnerName")
                   .Select(c => c.Value).SingleOrDefault();
            return partnerName;
        }
        public string GetUserPassword()
        {
            var userPassword = CurrentClaims().Where(c => c.Type == "UserPassword")
                 .Select(c => c.Value).SingleOrDefault();
            return userPassword;
        }
        private List<Claim> CurrentClaims()
        {
            return ClaimsPrincipal.Current.Identities.First().Claims.ToList();
        }
        public IEnumerable<string> PartnerRoleId()
        {
            var partnerSetupServiceUser = CurrentClaims().Where(c => c.Type == "RoleId")
                  .Select(c => c.Value);
             return partnerSetupServiceUser;
        }
    }
}