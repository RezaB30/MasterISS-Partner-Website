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
        public int BillingPeriodId { get; set; }

        public string ReferenceNo { get; set; }

        public int ServiceId { get; set; }

        [Required]
        public int? SelectedBillingPeriod { get; set; }

        [Display(Name = "PartnerTariffID", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public int PartnerTariffID { get; set; }

        [Required]
        public int? SelectedPartnerTariff { get; set; }

        [Display(Name = "SetupAddress", ResourceType = typeof(Localization.Model))]
        public AdresInfoViewModel SetupAddress { get; set; }

        [Display(Name = "PSTN", ResourceType = typeof(Localization.Model))]
        public string PSTN { get; set; }
    }

    public class IndividualViewModel
    {
        [Display(Name = "BirthPlace", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Model))]
        public string BirthPlace { get; set; }

        [Display(Name = "FathersName", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Model))]
        public string FathersName { get; set; }

        [Display(Name = "MothersMaidenName", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Model))]
        public string MothersMaidenName { get; set; }

        [Display(Name = "MothersName", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Model))]
        public string MothersName { get; set; }

        [Display(Name = "NationalityId", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public int NationalityId { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Model))]
        public int? SelectedNationality { get; set; }

        [Display(Name = "ProfessionId", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public int ProfessionId { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Model))]
        public int? SelectedProfession { get; set; }

        [Display(Name = "ResidencyAddress", ResourceType = typeof(Localization.Model))]
        public AdresInfoViewModel ResidencyAddress { get; set; }

        [Display(Name = "SexId", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public int SexId { get; set; }

        public bool IndividualSameAddress { get; set; }
    }

    public class IDCardViewModel
    {
        [Display(Name = "BirthDate", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Model))]
        public string BirthDate { get; set; }

        [Display(Name = "CardTypeId", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public int CardTypeId { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Model))]
        public short? SelectedCardType { get; set; }

        [Display(Name = "DateOfIssue", ResourceType = typeof(Localization.Model))]
        public string DateOfIssue { get; set; }

        [Display(Name = "District", ResourceType = typeof(Localization.Model))]
        public string District { get; set; }

        [Display(Name = "FirstName", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Model))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Model))]
        [Display(Name = "LastName", ResourceType = typeof(Localization.Model))]
        public string LastName { get; set; }

        [Display(Name = "Neighbourhood", ResourceType = typeof(Localization.Model))]
        public string Neighbourhood { get; set; }

        [Display(Name = "PageNo", ResourceType = typeof(Localization.Model))]
        public string PageNo { get; set; }

        [Display(Name = "PassportNo", ResourceType = typeof(Localization.Model))]
        public string PassportNo { get; set; }

        [Display(Name = "PlaceOfIssue", ResourceType = typeof(Localization.Model))]
        public string PlaceOfIssue { get; set; }

        [Display(Name = "Province", ResourceType = typeof(Localization.Model))]
        public string Province { get; set; }

        [Display(Name = "RowNo", ResourceType = typeof(Localization.Model))]
        public string RowNo { get; set; }

        [Display(Name = "SerialNo", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Model))]
        public string SerialNo { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Model))]
        [Display(Name = "TCKNo", ResourceType = typeof(Localization.Model))]
        public string TCKNo { get; set; }

        [Display(Name = "VolumeNo", ResourceType = typeof(Localization.Model))]
        public string VolumeNo { get; set; }
    }

    public class GeneralInfoViewModel
    {
        [Display(Name = "BillingAddress", ResourceType = typeof(Localization.Model))]
        public AdresInfoViewModel BillingAddress { get; set; }

        [Display(Name = "ContactPhoneNo", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string ContactPhoneNo { get; set; }

        [Display(Name = "CustomerTypeId", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public int CustomerTypeId { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        [Display(Name = "Email", ResourceType = typeof(Localization.Model))]
        public string Email { get; set; }

        [Display(Name = "OtherPhoneNos", ResourceType = typeof(Localization.Model))]
        public string[] OtherPhoneNos { get; set; }

        [Display(Name = "CultureCustomer", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string Culture { get; set; }

        public string SelectedCulture { get; set; }

        public bool GeneralSameAddress { get; set; }
    }

    public class CorporateInfoViewModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        [Display(Name = "CentralSystemNo", ResourceType = typeof(Localization.Model))]
        public string CentralSystemNo { get; set; }

        [Display(Name = "CompanyAddress", ResourceType = typeof(Localization.Model))]
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
        public int ExecutiveNationalityId { get; set; }

        [Display(Name = "ExecutiveProfessionId", ResourceType = typeof(Localization.Model))]
        public int ExecutiveProfessionId { get; set; }

        [Display(Name = "ExecutiveResidencyAddress", ResourceType = typeof(Localization.Model))]
        public AdresInfoViewModel ExecutiveResidencyAddress { get; set; }

        [Display(Name = "BBK", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string ExecutiveResidencyBBK { get; set; }

        [Display(Name = "ExecutiveSexId", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public short ExecutiveSexId { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        [Display(Name = "Title", ResourceType = typeof(Localization.Model))]
        public string Title { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        [Display(Name = "TradeRegistrationNo", ResourceType = typeof(Localization.Model))]
        public string TradeRegistrationNo { get; set; }
    }
}