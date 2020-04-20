
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace com.apthai.CRMMobile.HttpRestModel
{
    public partial class Register
    {
        public string PhoneNumber { get; set; }
        public int PINCode { get; set; }
        public string CitizenIdentityNo { get; set; }
    }
    public partial class CheckPinParam
    {
        public string PINCode { get; set; }
        public string AccessKey { get; set; }
        public string DeviceID { get; set; }
    }
    public partial class ChangePINParam
    {
        public string OldPIN { get; set; }
        public string NewPIN { get; set; }
        public string AccessKey { get; set; }
        public string DeviceID { get; set; }
    }
    public partial class GetUserPhoneParam
    {
        public string CitizenIdentityNo { get; set; }
    }
    public partial class GetUserPropotyParam
    {
        public string CustometID { get; set; }
    }
    public class AutorizeDataJWT
    {
        public bool LoginResult { get; set; }
        public string LoginResultMessage { get; set; }
        public string UserPrincipalName { get; set; }
        public string DomainUserName { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string EmployeeID { get; set; }
        public string Email { get; set; }
        public string Division { get; set; }

        public string Token { get; set; }

        public DateTime? AccountExpirationDate { get; set; }
        public DateTime? LastLogon { get; set; }

        public string AuthenticationProvider { get; set; }
        public string SysUserId { get; set; }
        public string SysUserData { get; set; }
        public string SysUserRoles { get; set; }
        public string SysAppCode { get; set; }
        public string AppUserRole { get; set; }
        public string UserProject { get; set; }
        public string UserApp { get; set; }
        
    }
    public class GetCAllArea
    {
        public string ProductTypeCate { get; set; }
        public string AccessKey { get; set; }
        public string EmpCode { get; set; }
    }
    public class callTDefectObj
    {
        public int TDefectID { get; set; }
        public string AccessKey { get; set; }
        public string EmpCode { get; set; }
    }
    public class CreateDefectTransactionParam
    {
        public string DefectType { get; set; }
        public string ProductID { get; set; }
        public string ItemID { get; set; }
        public string Description { get; set; }
        public string DeviceId { get; set; }
        public string AccessKey { get; set; }
        public string EmpCode { get; set; }
    }
    public class GetCAllType
    {
        public string AccessKey { get; set; }
        public string EmpCode { get; set; }
    }
    public class RegisterData
    {
        public string CitizenIdentityNo { get; set; }
        public string PhoneNumber { get; set; }
        public string DeviceID { get; set; }
        public string DeviceType { get; set; }
        public string PINCode { get; set; }
    }
    public class RequestOTPParam
    {
        public string PhoneNumber { get; set; }
    }

    public partial class GetUserBookingTrackingByUserIDParam
    {
        public string UserID { get; set; }
    }
    public partial class GetUserBillingTrackingParam
    {
        public string UserID { get; set; }
        public string Project { get; set; }
        public string Unit { get; set; }
    }
    public partial class GetUserICRMContactParam
    {
        public string ContactNo { get; set; }
    }
    public partial class GetUserCardParam
    {
        public string ContactNo { get; set; }
        //public string ProjectNo { get; set; }
        //public string UnitNo { get; set; }
    }
    public partial class GetUserCreditCardParam
    {
        public string Key { get; set; }
        public string ProjectNo { get; set; }
        public string UnitNo { get; set; }
    }
}
