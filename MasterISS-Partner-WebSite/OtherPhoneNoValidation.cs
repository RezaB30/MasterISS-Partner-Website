using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MasterISS_Partner_WebSite
{
    public class OtherPhoneNoValidation : ValidationAttribute
    {
        public override bool IsValid(object numbers)
        {
            var regex = new Regex(@"^((\d{3})(\d{3})(\d{2})(\d{2}))$");

            foreach (string number in numbers as IEnumerable<string>)
            {
                if (!regex.IsMatch(number))
                {
                    return false;
                }
            }
            return true;
        }
    }
}