using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.apthai.CRMMobile.CustomModel
{
    public class GetUserCRMPhoneNumber
    {
        public List<Model.CRMWeb.ContactPhone> contactPhones { get; set; }
        public string FirstNameTH { get; set; }
        public string LastNameTH { get; set; }
        public string CitizenIdentityNo { get; set; }
        public bool IsVIP { get; set; }
    }
    public class ThaiBulkOTPRequest
    {
        public string key { get; set; }
        public string secret { get; set; }
        public string msisdn { get; set; }
    }
    public class ThaiBulkVerifyOTP
    {
        public string token { get; set; }
        public string pin { get; set; }
    }
    public class thaiBulkOTPRequestReturnObj
    {
        public ThaiBulkOTPRequestReturn data { get; set; }
    }
    public class ThaiBulkOTPRequestReturn
    {
        public string status { get; set; }
        public string token { get; set; }
    }
    public class thaiBulkOTPVerifyReturnObj
    {
        public ThaiBulkOTPRequestReturn data { get; set; }
    }
    public class thaiBulkOTPVerifyReturn
    {
        public string status { get; set; }
        public string token { get; set; }
    }
    public class VerifyPINReturnObj : Model.CRMMobile.UserLogin
    {
        public string CRMContactID { get; set; }
        public string TitleExtTH { get; set; }
        public string FirstNameTH { get; set; }
        public string MiddleNameTH { get; set; }
        public string LastNameTH { get; set; }
        public string Nickname { get; set; }
        public string TitleExtEN { get; set; }
        public string FirstNameEN { get; set; }
        public string MiddleNameEN { get; set; }
        public string LastNameEN { get; set; }
        public string CitizenIdentityNo { get; set; }
        public string Created { get; set; }
        public string CreatedBy { get; set; }
        public string Updated { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public string PINCode { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class GetUserPropotyObj
    {
        public Model.CRMWeb.Transfer transfer { get; set; }
        public Model.CRMWeb.TransferOwner TransferOwner { get; set; }
        public Model.CRMWeb.Unit Unit { get; set; }
        public Model.CRMWeb.Project Project { get; set; }
        public Model.CRMWeb.Floor Floor { get; set; }
    }
    public class GetUserPropotyReturnObj
    {
        public List<GetUserPropotyObj> getUserPropotyObjs { get; set; }
    }
    public class GetBillingTrackingMobile
    {
        public string ProjectNo { get; set; }
        public string UnitNo { get; set; }
        public string TotalInstallment { get; set; }
        public string NormalDownPayment { get; set; }
        public string SpecialDownPayment { get; set; }
        public string TotalInstallmentAmount { get; set; }
        public string UnitPriceStageName { get; set; }
        public string DetailDownPayment { get; set; }
        public string DownPerInstallment { get; set; }
        public string NormalDownPerInstallment { get; set; }
        public string SpecialDownPaymentFlag { get; set; }
        public string SpecialDownPerInstallment { get; set; }
        public string PaymentDueDate { get; set; }
        public string PaymentDate { get; set; }
        public string AmountPaid { get; set; }
        public string AmountBalance { get; set; }
        public string FlagReceipt { get; set; }
        public string FlagOverDue { get; set; }
        public string FlagBooking { get; set; }
        public string BookingAmount { get; set; }
        public string PayBookingAmount { get; set; }
        public string BookingPaymentDate { get; set; }
        public string FlagAgreement { get; set; }
        public string AgreementAmount { get; set; }
        public string PayAgreementAmount { get; set; }
        public string AgreementPaymentDate { get; set; }
    }
    public class iCRMBooking
    {
        public string URLPicture { get; set; }
        public int ProjectID { get; set; }
        public string Project { get; set; }
        public string Unit { get; set; }
        public string Brand { get; set; }
        public string UnitStatus { get; set; }
        public string HouseNumber { get; set; }
        public string Building { get; set; }
        public string floor { get; set; }
        public string RoomCodeFloorCode { get; set; }
        public string RoomPlanFloorPlan { get; set; }
        public string TypeHouse { get; set; }
        public string SaleArea { get; set; }
        public string LivingSpace { get; set; }
        public string BookingDate { get; set; }
        public string AgreementDate { get; set; }
        public string SalePrice { get; set; }
        public string HouseReservationPrice { get; set; }
        public string CashDiscount { get; set; }
        public string SalesPriceDiscount { get; set; }
        public string FlagMainBooker { get; set; }
        public string FullnameMainBooker { get; set; }
        public string NameMainBooker { get; set; }
        public string LastNameMainBooker { get; set; }
        public string CoBooker { get; set; }
        public string FlagMainAgreement { get; set; }
        public string FullnameMainAgreement { get; set; }
        public string NameMainAgreement { get; set; }
        public string LastNameMainAgreement { get; set; }
        public string CoAgreement { get; set; }
        public string TotalInstallments { get; set; }
        public string PaidInstallments { get; set; }
        public string AgreementNo { get; set; }
        public string FlagCreditDebit { get; set; }
    }
    public class iCRMContact
    {
        public string ID { get; set; }
        public int ContactNo { get; set; }
        public string FullnameTH { get; set; }
        public string ContactTitleTH { get; set; }
        public string FirstNameTH { get; set; }
        public string MiddleNameTH { get; set; }
        public string LastNameTH { get; set; }
        public string FullnameEN { get; set; }
        public string ContactTitleEN { get; set; }
        public string FirstNameEN { get; set; }
        public string MiddleNameEN { get; set; }
        public string LastNameEN { get; set; }
        public string Email { get; set; }
        public string CitizenIdentityNo { get; set; }
        public string ContactAddressFullTH { get; set; }
        public string HouseNoTH { get; set; }
        public string MooTH { get; set; }
        public string VillageTH { get; set; }
        public string SoiTH { get; set; }
        public string RoadTH { get; set; }
        public string SubDistrictTH { get; set; }
        public string ProvinceTH { get; set; }
        public string CountryTH { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string HouseNoEN { get; set; }
        public string MooEN { get; set; }
        public string VillageEN { get; set; }
        public string SoiEN { get; set; }
        public string RoadEN { get; set; }
        public string SubDistrictEN { get; set; }
        public string DistrictEN { get; set; }
        public string ProcinceEN { get; set; }
        public string CountryEN { get; set; }
    }

}
