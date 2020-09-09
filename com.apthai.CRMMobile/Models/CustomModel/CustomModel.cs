using Microsoft.AspNetCore.Mvc;
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
    public class AdsObject
    {
        public string AdsUrl { get; set; }
        public string Link { get; set; }
    }

    public class PersonalContact
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Remark { get; set; }
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
        public string ContactNo { get; set; }
    }

    public class GetUserPropotyObj
    {
        public Model.CRMWeb.Transfer transfer { get; set; }
        public Model.CRMWeb.TransferOwner TransferOwner { get; set; }
        public Model.CRMWeb.Unit Unit { get; set; }
        public Model.CRMWeb.Project Project { get; set; }
        public Model.CRMWeb.Floor Floor { get; set; }
    }
    public class SCBReturnObject
    {
        public string resCode { get; set; }
        public string resDesc { get; set; }
        public string transactionId { get; set; }
        public string confirmId { get; set; }
    }
    public class GetUserPropotyReturnObj
    {
        public List<GetUserPropotyObj> getUserPropotyObjs { get; set; }
    }
    public class GetBillingTrackingMobile
    {
        //public string AmountBalance { get; set; }
        public int UnitPriceStage { get; set; }
        public string ProjectNo { get; set; }
        public string UnitNo { get; set; }
        public Guid PaymentID { get; set; }
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
        public string FlagBookingReceipt { get; set; }
        public string BookingReceiptNo { get; set; }
        public string FlagAgreement { get; set; }
        public string AgreementAmount { get; set; }
        public string PayAgreementAmount { get; set; }
        public string AgreementPaymentDate { get; set; }
        public string FlagAgreementReceipt { get; set; }
        public string AgreementReceiptNo { get; set; }
        public string MasterPriceItemID { get; set; }
        public string Period { get; set; }
        public string ReceiptNo { get; set; }
        // ----------- เงินจอง : ใบเสร็จจริง ---------------
        public Guid BookingReceiptHeaderID { get; set; }
        public string BookingReceiptRemark { get; set; }
        public string BookingReceiptTempNo { get; set; }
        //------------ เงินจอง : ใบเสร็จชั่วคราว ---
        public string FlagBookingTemp { get; set; }
        public string BookingAmountTemp { get; set; }
        public string PayBookingAmountTemp { get; set; }
        public string BookingPaymentDateTemp { get; set; }
        public string FlagBookingReceiptTemp { get; set; }
        public string BookingReceiptNoTemp { get; set; }
        public Guid BookingReceiptHeaderIDTemp { get; set; }
        public string BookingReceiptRemarkTemp { get; set; }
        //----------- เงินสัญญา : ใบเสร็จจริง ---*/
        public Guid AgreementReceiptHeaderID { get; set; }
        public string AgreementReceiptRemark { get; set; }
        //------------ เงินสัญญา : ใบเสร็จชั่วคราว ---
        public string FlagAgreementTemp { get; set; }
        public string AgreementAmountTemp { get; set; }
        public string PayAgreementAmountTemp { get; set; }
        public string AgreementPaymentDateTemp { get; set; }
        public string FlagAgreementReceiptTemp { get; set; }
        public string AgreementReceiptNoTemp { get; set; }
        public Guid AgreementReceiptHeaderIDTemp { get; set; }
        public string AgreementReceiptRemarkTemp { get; set; }
        //----------- เงินงวดดาวน์ : ข้อมูล -------
        //----------- เงินงวดดาวน์ : ใบเสร็จจริง ---
        public Guid ReceiptHeaderID { get; set; }
        public string ReceiptRemark { get; set; }
        //----------- เงินงวดดาวน์ : ใบเสร็จชั่วคราว ---
        public string PaymentDateTemp { get; set; }
        public string AmountPaidTemp { get; set; }
        public string AmountBalanceTemp { get; set; }
        public string FlagReceiptTemp { get; set; }
        public string ReceiptNoTemp { get; set; }
        public string FlagOverDueTemp { get; set; }
        public Guid ReceiptHeaderIDTemp { get; set; }
        public string ReceiptRemarkTemp { get; set; }
        //------------ เงินโอน : ใบเสร็จจริง ---
        public string FlagTransfer { get; set; }
        public string TransferAmount { get; set; }
        public string PayTransferAmount { get; set; }
        public string TransferPaymentDate { get; set; }
        public string FlagTransferReceipt { get; set; }
        public string TransferReceiptNo { get; set; }
        public Guid TransferReceiptHeaderID { get; set; }
        public string TransferReceiptRemark { get; set; }
        //------------- เงินโอน : ใบเสร็จชั่วคราว ---
        public string FlagTransferTemp { get; set; }
        public string TransferAmountTemp { get; set; }
        public string PayTransferAmountTemp { get; set; }
        public string TransferPaymentDateTemp { get; set; }
        public string FlagTransferReceiptTemp { get; set; }
        public string TransferReceiptNoTemp { get; set; }
        public Guid TransferReceiptHeaderIDTemp { get; set; }
        public string TransferReceiptRemarkTemp { get; set; }
        public bool HaveFET { get; set; }
        public string AgreementReceiptTempNo { get; set; }
        public string DownReceiptNo { get; set; }
        public string DownReceiptTempNo { get; set; }
        public string TransferReceiptTempNo { get; set; }
        public string PaymentItemNameTH { get; set; }
        public string PaymentItemNameEN { get; set; }
        public string ContactAddressTH { get; set; }
        public string ContactAddressEN { get; set; }
    }
    public class BillingTrackingGroup

    {
        public double PaymentAmount { get; set; }
        public string PaymentDueDate { get; set; }
        public int DetailDownPayment { get; set; }
        public List<GetBillingTrackingMobile> GetBillingTrackingMobile { get; set; }
        public double PayRemain { get; set; }
        public bool IsOverDue { get; set; }
        public string DownPerInstallment { get; set; }
        public string NormalDownPerInstallment { get; set; }
        public string SpecialDownPaymentFlag { get; set; }
        public string SpecialDownPerInstallment { get; set; }
        public string FlagReceipt { get; set; }
        public string FlagOverDue { get; set; }
        public string FlagBooking { get; set; }
        public string BookingAmount { get; set; }
        public string BookingPaymentDate { get; set; }
        public string FlagBookingReceipt { get; set; }
        public string FlagAgreement { get; set; }
        public string AgreementAmount { get; set; }
        public string PayAgreementAmount { get; set; }
        public string FlagAgreementReceipt { get; set; }
        public string FlagTransfer { get; set; }
        public string TransferAmount { get; set; }
        public string TransferPaymentDate { get; set; }
        public string FlagTransferReceipt { get; set; }
        public string PaymentItemNameTH { get; set; }
        public string PaymentItemNameEN { get; set; }
    }
    public class BillingFinalTrackingGroup
    {
        public List<BillingTrackingGroup> BillingTrackingGroup { get; set; }
        public List<BillingTrackingGroup> BookingList { get; set; }
        public List<BillingTrackingGroup> ContractList { get; set; }
        public List<BillingTrackingGroup> TransferList { get; set; }
    }
    public class iCRMBooking
    {
        public string URLPicture { get; set; }
        public int ProjectID { get; set; }
        public string Project { get; set; }
        public string Unit { get; set; }
        public string ProjectShowName { get; set; }
        public string Brand { get; set; }
        public string UnitStatus { get; set; }
        public string UnitStatusCode { get; set; }
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
        public string FullnameMainAgreementEN { get; set; }
        public string NameMainAgreementEN { get; set; }
        public string LastNameMainAgreementEN { get; set; }
        public string AgreementOwnerNational { get; set; }
        public string CoAgreement { get; set; }
        public string TotalInstallments { get; set; }
        public string PaidInstallments { get; set; }
        public string AgreementNo { get; set; }
        public string FlagCreditDebit { get; set; }
        public string BookingOwnerAddressTH { get; set; }
        public string BookingOwnerAddressEN { get; set; }
        public string AgreementOwnerAddressTH { get; set; }
        public string AgreementOwnerAddressEN { get; set; }
        public string BookingOwnerEmail { get; set; }
        public string AgreementOwnerEmail { get; set; }
        public string ProjectAddressTH { get; set; }
        public string ProjectAddressEN { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactAddressTH { get; set; }
        public string ContactAddressEN { get; set; }
    }
    public class iCRMMyProperty
    {
        public string ProjectID { get; set; }
        public string Project { get; set; }
        public string ProjectEN { get; set; }
        public string Unit { get; set; }
        public string SellingPrice { get; set; }
        public string PayAmount { get; set; }
        public Guid ContactID { get; set; }
        public string ContactNo { get; set; }
        public string FirstNameTH { get; set; }
        public string LastNameTH { get; set; }
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
    public class CRMUserLoginWithContactID : Model.CRMMobile.UserLogin
    {
        public Guid CRMContactID { get; set; }
        public string ContactNo { get; set; }
    }
    public class GetUserCardReturnObj
    {
        public Guid BookingID { get; set; }
        public string ProjectID { get; set; }
        public string Project { get; set; }
        public string Unit { get; set; }
        public string UnitStatus { get; set; }
        public double SellingPrice { get; set; }
        public string UnitStatusCode { get; set; }
        public string FullnameMainAgreement { get; set; }
        public string NameMainAgreement { get; set; }
        public string LastNameMainAgreement { get; set; }
        public string FullnameMainAgreementEN { get; set; }
        public string NameMainAgreementEN { get; set; }
        public string LastNameMainAgreementEN { get; set; }
        public string ContactNo { get; set; }
        public string AgreementNo { get; set; }
        public string FlagCreditDebit { get; set; }
        public string Balance { get; set; }
        public string National { get; set; }
        public bool IsThaiNationality { get; set; }
        public double AgreementPrice { get; set; }
        public double ExtraAreaAmount { get; set; }
        public double TransferDiscount { get; set; }
        public double FreedownAmount { get; set; }
        public double BookingAmount { get; set; }
        public double AgreementAmount { get; set; }
        public double AgreementInstallmentAmount { get; set; }
        public double TotalInstallmentAmount { get; set; }
        //public FileContentResult QRCode { get; set; }
    }
    public class GetiCRMOwnerReturnObj
    {
        public string ContactID { get; set; }
        public string ContactNo { get; set; }
        public string ProjectID { get; set; }
        public string Project { get; set; }
        public string Unit { get; set; }
        public string UnitStatus { get; set; }
        public string UnitStatusCode { get; set; }
        public bool FlagMainBooker { get; set; }
        public string FullnameMainBooker { get; set; }
        public string NameMainBooker { get; set; }
        public string LastNameMainBooker { get; set; }
        public string CoBooker { get; set; }
        public string CoBookerEN { get; set; }
        public string BookingOwnerEmail { get; set; }
        public string BookingOwnerAddressTH { get; set; }
        public string BookingOwnerAddressEN { get; set; }
        public bool FlagMainAgreement { get; set; }
        public string FullnameMainAgreement { get; set; }
        public string NameMainAgreement { get; set; }
        public string LastNameMainAgreement { get; set; }
        public string FullnameMainAgreementEN { get; set; }
        public string NameMainAgreementEN { get; set; }
        public string LastNameMainAgreementEN { get; set; }
        public string AgreementOwnerNational { get; set; }
        public string CoAgreement { get; set; }
        public string CoAgreementEN { get; set; }
        public string AgreementOwnerEmail { get; set; }
        public string AgreementOwnerAddressTH { get; set; }
        public string AgreementOwnerAddressEN { get; set; }
        public bool   FlagMainTransfer { get; set; }
        public string FullnameMainTransfer { get; set; }
        public string NameMainTransfer { get; set; }
        public string LastNameMainTransfer { get; set; }
        public string TransferOwnerNational { get; set; }
        public string TransferOwnerEmail { get; set; }
        public string TransferOwnerAddress { get; set; }
        //public FileContentResult QRCode { get; set; }
    }
    public class GetUserCreditCardReturnObj
    {
        public string AccountNO { get; set; }
        public string CreditCardExpireMonth { get; set; }
        public string CreditCardExpireYear { get; set; }
        public string OwnerName { get; set; }
    }
    public class GetGetReceiptInfoReturnObj
    {
        public string ReceiptNo { get; set; }
        public string ReceiptTempNo { get; set; }
        public string PaymentMethodTH { get; set; }
        public string PaymentMethodEN { get; set; }
        public string ReceiveFromTH { get; set; }
        public string ReceiveFromEN { get; set; }
        public string ContactHouseNoTH { get; set; }
        public string ContactMooTH { get; set; }
        public string ContactVillageTH { get; set; }
        public string ContactSoiTH { get; set; }
        public string ContactRoadTH { get; set; }
        public string ContactCountryTH { get; set; }
        public string ContactSubDistrictTH { get; set; }
        public string ContactDistrictTH { get; set; }
        public string ContactProvinceTH { get; set; }
        public string ContactHouseNoEN { get; set; }
        public string ContactMooEN { get; set; }
        public string ContactVillageEN { get; set; }
        public string ContactSoiEN { get; set; }
        public string ContactRoadEN { get; set; }
        public string ContactCountryEN { get; set; }
        public string ContactSubDistrictEN { get; set; }
        public string ContactDistrictEN { get; set; }
        public string ContactProvinceEN { get; set; }
        public string ContactPostalCode { get; set; }
        public string URL { get; set; }
        public bool IsTemp { get; set; }
        public string ReceiveDate { get; set; }
    }
    public class SCBAuthenHeader
    {
        public string resourceOwnerId { get; set; }
        public string requestUId { get; set; }
        public string acceptlanguage { get; set; }
    }
    public class SCBAuthenstatus
    {
        public string code { get; set; }
        public string description { get; set; }
    }
    public class SCBAuthendata
    {
        public string accessToken { get; set; }
        public string tokenType { get; set; }
        public string expiresIn { get; set; }
        public string expiresAt { get; set; }
    }
    public class SCBDeepLinkResponddata
    {
        public string transactionId { get; set; }
        public string deeplinkUrl { get; set; }
        public string userRefId { get; set; }
    }
    public class SCBAuthObj
    {
        public string applicationKey { get; set; }
        public string applicationSecret { get; set; }

    }
    public class SCBAuthenRetrunObj
    {
        public SCBAuthenstatus status { get; set; }
        public SCBAuthendata data { get; set; }

    }
    public class SCBDeeplinkBillPaymentRetrunObj
    {
        public double paymentAmount { get; set; }
        public string accountTo { get; set; }
        public string accountFrom { get; set; }
        public string ref1 { get; set; }
        public string ref2 { get; set; }
        public string ref3 { get; set; }

    }
    public class SCBDeeplinkmerchantMetaData
    {
        public string callbackurl { get; set; }
        public string extraData { get; set; }
        public List<SCBDeeplinkpaymentInfo> paymentInfo { get; set; }
    }
    public class SCBDeeplinkpaymentInfo
    {
        public string type { get; set; }
        public string title { get; set; }
        public string header { get; set; }
        public string description { get; set; }
        public string imageUrl { get; set; }
    }
    public class SCBDeeplinkpaymentInfo2
    {
        public string type { get; set; }
        public string title { get; set; }
        public string header { get; set; }
        public string description { get; set; }
    }
    public class SCBDeeplinkBodyObj
    {
        public string transactionType { get; set; }
        public List<string> transactionSubType { get; set; }
        public int sessionValidityPeriod { get; set; }
        public string sessionValidUntil { get; set; }
        public SCBDeeplinkBillPaymentRetrunObj billPayment { get; set; }
        public SCBDeeplinkmerchantMetaData merchantMetaData { get; set; }
    }
    public class SCBDeepLinkRetrunObj
    {
        public SCBAuthenstatus status { get; set; }
        public SCBDeepLinkResponddata data { get; set; }
        public string SCBToken { get; set; }

    }
    //---------------- Get Transaction SCB -------------------------------
    public class SCBGetTransactionObj
    {
        public SCBAuthenstatus status { get; set; }
        public SCBGetTransactionData data { get; set; }
    }
    public class SCBGetTransactionData
    {
        public string partnerId { get; set; }
        public string transactionMethod { get; set; }
        public string updatedTimestamp { get; set; }
        public string statusCode { get; set; }
        public List<string> transactionSubType { get; set; }
        public string userRefId { get; set; }
        public string transactionId { get; set; }
        public string transactionType { get; set; }
        public string sessionValidityPeriod { get; set; }
        public SCBGetTransactionBillPayment billPayment { get; set; }
        public string partnerName { get; set; }
        public string errorMessage { get; set; }
        public SCBGetTransactionmerchantMetaData merchantMetaData { get; set; }
        public string paidAmount { get; set; }
        public string createdTimestamp { get; set; }
        public string accountFrom { get; set; }

    }
    public class SCBGetTransactionBillPayment
    {
        public string accountTo { get; set; }
        public string ref2 { get; set; }
        public string ref1 { get; set; }
        public string paymentAmount { get; set; }
        public string accountFrom { get; set; }
        public string ref3 { get; set; }
    }
    public class SCBGetTransactionmerchantMetaData
    {
        public string deeplinkUrl { get; set; }
        public string callbackurl { get; set; }
        public string extraData { get; set; }
        public string paymentAmount { get; set; }
        public string accountFrom { get; set; }
        public string ref3 { get; set; }
        public List<SCBGetTransactionpaymentInfo> paymentInfo { get; set; }
    }
    public class SCBGetTransactionpaymentInfo
    {
        public string header { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public string imageUrl { get; set; }
    }
    public class GetProjectInformation 
    {
        public string Name { get; set; }
        public string TaxID { get; set; }
        public string AccountType { get; set; }
        public string AccountNameTH { get; set; }
        public string AccountNameEN { get; set; }
        public string MerchantID { get; set; }
        public string IsPCard { get; set; }
        public string GLAccountNo { get; set; }
        public string ServiceCode { get; set; }
        public string BankAccountNo { get; set; }
        public string BillerID { get; set; }
        public Guid CompanyID { get; set; }
        public string ProjectID { get; set; }
        public string NameTH { get; set; }
        public string NameEN { get; set; }
        public string CompanyCode { get; set; }
    }
    public class FileUploadResult
    {
        public string Name { get; set; }
        public string Url { get; set; }
        //public string PublicUrl { get; set; }
        public string BucketName { get; set; }
    }
    public class DocumentDetailList : Model.CRMMobile.DocumentDetailLevel2
    {
        public List<Model.CRMMobile.SubDocumentDetailLevel3> SubDoct { get; set; }
    }
    public class DocumentDetailListResult 
    {
        public List<DocumentDetailList> DocumentDetailList { get; set; }
    }

    public class FileDTO
    {
        /// <summary>
        /// Url ของไฟล์
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// ชื่อไฟล์ (ที่เก็บบน DB)
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// ระบุว่าไฟล์อยู่ใน temp bucket
        /// </summary>
        /// <value><c>true</c> if is temp; otherwise, <c>false</c>.</value>
        public bool IsTemp { get; set; }

      
    }

}
