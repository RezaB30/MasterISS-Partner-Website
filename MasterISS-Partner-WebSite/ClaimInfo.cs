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
        public int PartnerId()
        {
            var partnerId = CurrentClaims().Where(c => c.Type == "PartnerId")
                  .Select(c => c.Value).SingleOrDefault();

            var convertedPartnerId = Convert.ToInt32(partnerId);

            return convertedPartnerId;
        }
        public int UserId()
        {
            var userId = CurrentClaims().Where(c => c.Type == ClaimTypes.NameIdentifier)
                  .Select(c => c.Value).SingleOrDefault();

            var convertedUserId = Convert.ToInt32(userId);

            return convertedUserId;
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
        public List<Claim> CurrentClaims()
        {
            var currentClaims= ClaimsPrincipal.Current.Identities.First().Claims.ToList();
            return currentClaims;
        }
        public IEnumerable<int> PartnerRoleId()
        {
            var partnerSetupServiceUser = CurrentClaims().Where(c => c.Type == "RoleId")
                  .Select(c => c.Value);

            List<int> roleIds = new List<int>();

            foreach (var item in partnerSetupServiceUser)
            {
                roleIds.Add(Convert.ToInt32(item));
            }
             return roleIds;
        }
    }
}