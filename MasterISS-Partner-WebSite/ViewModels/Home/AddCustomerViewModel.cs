using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.ViewModels.Home
{
    public class AddCustomerViewModel
    {
        public long ID { get; set; }
        public CorporateInfoViewModel CorporateInfo { get; set; }
        public GeneralInfoViewModel GeneralInfo { get; set; }
        public IDCardViewModel IDCard { get; set; }
        public IndividualViewModel Individual { get; set; }
        public SubscriptionInfoViewModel SubscriptionInfo { get; set; }
    }
  
    public class SubscriptionInfoViewModel
    {
        [Display(Name = "BillingPeriodId", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public int BillingPeriodId { get; set; }

        [Display(Name = "PartnerTariffID", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public int PartnerTariffID { get; set; }

        public AdresInfoViewModel SetupAddress { get; set; }
    }

    public class IndividualViewModel
    {
        [Display(Name = "BirthPlace", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string BirthPlace { get; set; }

        [Display(Name = "FathersName", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string FathersName { get; set; }

        [Display(Name = "MothersMaidenName", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string MothersMaidenName { get; set; }

        [Display(Name = "MothersName", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string MothersName { get; set; }

        [Display(Name = "NationalityId", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public int NationalityId { get; set; }

        [Display(Name = "ProfessionId", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public int? ProfessionId { get; set; }

        public AdresInfoViewModel ResidencyAddress { get; set; }

        [Display(Name = "SexId", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public int? SexId { get; set; }

        [Display(Name = "SameSetupAddress", ResourceType = typeof(Localization.Model))]
        public bool SameSetupAddressByIndividual { get; set; }
    }

    public class CorporateInfoViewModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        [Display(Name = "CentralSystemNo", ResourceType = typeof(Localization.Model))]
        public string CentralSystemNo { get; set; }

        public AdresInfoViewModel CompanyAddress { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        [Display(Name = "ExecutiveBirthPlace", ResourceType = typeof(Localization.Model))]
        public string ExecutiveBirthPlace { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        [Display(Name = "TaxNo", ResourceType = typeof(Localization.Model))]
        public string TaxNo { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        [Display(Name = "TaxOffice", ResourceType = typeof(Localization.Model))]
        public string TaxOffice { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        [Display(Name = "ExecutiveFathersName", ResourceType = typeof(Localization.Model))]
        public string ExecutiveFathersName { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        [Display(Name = "ExecutiveMothersMaidenName", ResourceType = typeof(Localization.Model))]
        public string ExecutiveMothersMaidenName { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        [Display(Name = "ExecutiveMothersName", ResourceType = typeof(Localization.Model))]
        public string ExecutiveMothersName { get; set; }

        [Display(Name = "ExecutiveNationalityId", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public int? ExecutiveNationalityId { get; set; }

        [Display(Name = "ExecutiveProfessionId", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public int? ExecutiveProfessionId { get; set; }

        public AdresInfoViewModel ExecutiveResidencyAddress { get; set; }

        [Display(Name = "ExecutiveSexId", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public short? ExecutiveSexId { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        [Display(Name = "Title", ResourceType = typeof(Localization.Model))]
        public string Title { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        [Display(Name = "TradeRegistrationNo", ResourceType = typeof(Localization.Model))]
        public string TradeRegistrationNo { get; set; }

        [Display(Name = "SameSetupAddress", ResourceType = typeof(Localization.Model))]
        public bool SameSetupAddressByCorporativeCompanyAddress { get; set; }
        
        [Display(Name = "SameSetupAddress", ResourceType = typeof(Localization.Model))]
        public bool SameSetupAddressByCorporativeResidencyAddress { get; set; }
    }

    public class IDCardViewModel
    {
        [Display(Name = "BirthDate", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string BirthDate { get; set; }

        [Display(Name = "CardTypeId", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public int? CardTypeId { get; set; }

        [Display(Name = "DateOfIssue", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string DateOfIssue { get; set; }

        [Display(Name = "District", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string District { get; set; }

        [Display(Name = "FirstName", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        [Display(Name = "LastName", ResourceType = typeof(Localization.Model))]
        public string LastName { get; set; }

        [Display(Name = "Neighbourhood", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string Neighbourhood { get; set; }

        [Display(Name = "PageNo", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string PageNo { get; set; }

        [Display(Name = "PassportNo", ResourceType = typeof(Localization.Model))]
        public string PassportNo { get; set; }

        [Display(Name = "PlaceOfIssue", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string PlaceOfIssue { get; set; }

        [Display(Name = "Province", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string Province { get; set; }

        [Display(Name = "RowNo", ResourceType = typeof(Localization.Model))]
        public string RowNo { get; set; }

        [Display(Name = "SerialNo", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string SerialNo { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        [MaxLength(11, ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "MaxTCKValid")]
        [MinLength(11, ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "MinTCKValid")]
        [Display(Name = "TCKNo", ResourceType = typeof(Localization.Model))]
        public string TCKNo { get; set; }

        [Display(Name = "VolumeNo", ResourceType = typeof(Localization.Model))]
        public string VolumeNo { get; set; }
    }

    public class GeneralInfoViewModel
    {
        public GeneralInfoViewModel()
        {
            OtherPhoneNos = new List<string>();
        }

        public AdresInfoViewModel BillingAddress { get; set; }

        [Display(Name = "ContactPhoneNo", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        [RegularExpression(@"^((\d{3})(\d{3})(\d{2})(\d{2}))$", ErrorMessageResourceName = "ValidPhoneNumber", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string ContactPhoneNo { get; set; }

        [Display(Name = "CustomerTypeId", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public int? CustomerTypeId { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        [MaxLength(300, ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "EmailMaxLenght")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "EmailFormat")]
        [Display(Name = "Email", ResourceType = typeof(Localization.Model))]
        public string Email { get; set; }

        [Display(Name = "OtherPhoneNos", ResourceType = typeof(Localization.Model))]
        [OtherPhoneNoValidation(ErrorMessageResourceName = "ValidPhoneNumber", ErrorMessageResourceType = typeof(Localization.Validation))]
        public List<string> OtherPhoneNos { get; set; }

        [Display(Name = "CultureCustomer", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string Culture { get; set; }

        [Display(Name = "SameSetupAddress", ResourceType = typeof(Localization.Model))]
        public bool SameSetupAddressByBilling { get; set; }

    }

}