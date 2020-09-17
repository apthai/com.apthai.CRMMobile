using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using com.apthai.CoreApp.Data.Services;
using com.apthai.CRMMobile.CustomModel;
using com.apthai.CRMMobile.HttpRestModel;
using com.apthai.CRMMobile.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using com.apthai.CRMMobile.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Primitives;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using QRCoder;
using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore.Internal;
using MoreLinq;
using Microsoft.AspNetCore.Hosting;
using Minio;
using Minio.DataModel;
using System.Text;

namespace com.apthai.CRMMobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {


        private readonly IMasterRepository _masterRepo;
        private readonly IAuthorizeService _authorizeService;
        private readonly IUserRepository _UserRepository;
        private readonly IMobileMessagingClient _mobileMessagingClient;
        private readonly IHostingEnvironment _hostingEnvironment;
        public UserController(IMasterRepository masterRepo, IAuthorizeService authorizeService, IUserRepository userRepository, IMobileMessagingClient mobileMessagingClient, IHostingEnvironment hostingEnvironment)
        {

            _masterRepo = masterRepo;
            _authorizeService = authorizeService;
            _UserRepository = userRepository;
            _mobileMessagingClient = mobileMessagingClient;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Route("PaymentTransaction")]
        [SwaggerOperation(Summary = "เปลี่ยนภาษาของบุคคลนั้นๆ",
      Description = "เปลี่ยนภาษาของบุคคลนั้นๆ")]
        public async Task<object> ChangeLanguage([FromBody]PaymentTransactionParam data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;
                //if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
                //{
                //    string AccessKey = api_key.First();
                //    string EmpCodeKey = EmpCode.First();

                //    if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                //    {
                //        return new
                //        {
                //            success = false,
                //            data = new AutorizeDataJWT(),
                //            message = "Require Key to Access the Function"
                //        };
                //    }
                //    else
                //    {
                //        string APApiKey = Environment.GetEnvironmentVariable("API_Key");
                //        if (APApiKey == null)
                //        {
                //            APApiKey = UtilsProvider.AppSetting.ApiKey;
                //        }
                //        if (api_key != APApiKey)
                //        {
                //            return new
                //            {
                //                success = false,
                //                data = new AutorizeDataJWT(),
                //                message = "Incorrect API KEY !!"
                //            };
                //        }
                //    }
                //}
                Model.CRMMobile.PaymentTransaction paymentTransaction = _UserRepository.GetUserPaymentTransactionByUserID(data.transactionId);
                //Model.CRMMobile.UserProfile userProfile = _UserRepository.GetUserProfileByCRMContactID_Mobile(data.CRMContactID);
                paymentTransaction.Status = "Success";
                paymentTransaction.CurrencyCode = data.currencyCode;
                paymentTransaction.TransactionType = data.transactionType;
                paymentTransaction.TransactionAmount = Convert.ToDecimal(data.amount);

                bool result = _UserRepository.UpdatePaymentTransaction(paymentTransaction);
                SCBReturnObject obj = new SCBReturnObject();
                obj.resCode = "00";
                obj.resDesc = "Success";
                obj.transactionId = paymentTransaction.TransactionID;

                return new
                {
                    success = true,
                    data = obj,
                    message = "change Laguage Success !"
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }
        [HttpPost]
        [Route("ChangeLanguage")]
        [SwaggerOperation(Summary = "เปลี่ยนภาษาของบุคคลนั้นๆ",
       Description = "เปลี่ยนภาษาของบุคคลนั้นๆ")]
        public async Task<object> ChangeLanguage([FromBody]ChangeLanguageParam data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;
                //if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
                //{
                //    string AccessKey = api_key.First();
                //    string EmpCodeKey = EmpCode.First();

                //    if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                //    {
                //        return new
                //        {
                //            success = false,
                //            data = new AutorizeDataJWT(),
                //            message = "Require Key to Access the Function"
                //        };
                //    }
                //    else
                //    {
                //        string APApiKey = Environment.GetEnvironmentVariable("API_Key");
                //        if (APApiKey == null)
                //        {
                //            APApiKey = UtilsProvider.AppSetting.ApiKey;
                //        }
                //        if (api_key != APApiKey)
                //        {
                //            return new
                //            {
                //                success = false,
                //                data = new AutorizeDataJWT(),
                //                message = "Incorrect API KEY !!"
                //            };
                //        }
                //    }
                //}
                Model.CRMMobile.UserProfile userProfile = _UserRepository.GetUserProfileByCRMContactID_Mobile(data.CRMContactID);
                userProfile.Language = data.Language.ToLower();

                bool result = _UserRepository.UpdateCSUserProfile(userProfile);

                return new
                {
                    success = true,
                    data = userProfile,
                    message = "change Laguage Success !"
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("Register")]
        [SwaggerOperation(Summary = "Log In เข้าสู้ระบบเพื่อรับ Access Key ",
        Description = "Access Key ใช้ในการเรียหใช้ Function ต่างๆ เพื่อไม่ให้ User Login หลายเครื่องในเวลาเดียวกัน")]
        public async Task<object> Register([FromBody]RegisterData data)
        {
            try
            {
                //#region VerifyHeader
                //string ErrorHeader = "";
                //if (!VerifyHeader(out ErrorHeader))
                //{
                //    return new
                //    {
                //        success = false,
                //        data = "Invalid AccessKey!!. ",
                //        valid = false
                //    };
                //}
                //#endregion

                //bool asd = SHAHelper.VerifyHash("verify", "SHA512", GenerateAccessToken);
                Model.CRMWeb.Contact contact = _UserRepository.GetCRMContactByIDCardNO(data.CitizenIdentityNo);
                if (contact == null)
                {
                    return new
                    {
                        success = false,
                        data = "Invalid ID Card No. Or there is no user with this IDCard on CRM System ",
                        valid = false
                    };
                }
                Model.CRMWeb.ContactPhone contactPhone = _UserRepository.GetSingleContactPhoneNumberByContactID_Web(contact.ID.ToString(), data.PhoneNumber);
                if (contactPhone == null)
                {
                    return new
                    {
                        success = false,
                        data = "Invalid Phone Number that assosiate with the IDCard !! ",
                        valid = false
                    };
                }
                Model.CRMMobile.UserProfile ExistData = _UserRepository.GetUserProfileByCRMContactID_Mobile(contact.ID.ToString());
                if (ExistData == null)
                {
                    Model.CRMMobile.UserProfile cSUserProfile = new Model.CRMMobile.UserProfile();
                    cSUserProfile.CRMContactID = contact.ID.ToString();
                    cSUserProfile.TitleExtEN = contact.TitleExtTH;
                    cSUserProfile.FirstNameTH = contact.FirstNameTH;
                    cSUserProfile.LastNameTH = contact.LastNameTH;
                    cSUserProfile.Nickname = contact.Nickname;
                    cSUserProfile.TitleExtEN = contact.TitleExtEN;
                    cSUserProfile.FirstNameEN = contact.FirstNameEN;
                    cSUserProfile.MiddleNameEN = contact.MiddleNameEN;
                    cSUserProfile.LastNameEN = contact.LastNameEN;
                    cSUserProfile.CitizenIdentityNo = contact.CitizenIdentityNo;
                    cSUserProfile.Created = DateTime.Now.ToShortDateString();
                    cSUserProfile.CreatedBy = "System-Register";
                    cSUserProfile.Updated = null;
                    cSUserProfile.UpdatedBy = null;
                    cSUserProfile.IsActive = true;
                    cSUserProfile.ContactNo = contact.ContactNo;
                    cSUserProfile.PINCode = SHAHelper.ComputeHash(data.PINCode, "SHA512", null);
                    cSUserProfile.Language = data.Language.ToLower();
                    cSUserProfile.ContactNo = contact.ContactNo;

                    long ProfileID = 0;
                    bool insert = _UserRepository.InsertCSUserProfile(cSUserProfile, out ProfileID);

                    string GenerateAccessToken = SHAHelper.ComputeHash(data.DeviceID, "SHA512", null);
                    //------------------ Regis FireBase For FireBase's Token -----------------------

                    var claims = new Dictionary<string, object>()
                    {
                      { "CRMContactID", contact.ID.ToString()},
                      { "UserToken", GenerateAccessToken },
                    };

                    //-------------------------------------------------------------------------------
                    CRMUserLoginWithContactID cSUserLogin = new CRMUserLoginWithContactID();
                    cSUserLogin.UserPhoneNumber = data.PhoneNumber;
                    cSUserLogin.LoginDate = DateTime.Now.ToShortDateString();
                    cSUserLogin.DeviceID = data.DeviceID;
                    cSUserLogin.DeviceType = data.DeviceType;
                    cSUserLogin.UserToken = GenerateAccessToken;
                    cSUserLogin.UserProfileID = Convert.ToInt32(ProfileID);
                    cSUserLogin.CRMContactID = contact.ID;
                    cSUserLogin.FireBaseToken = data.FireBaseToken;
                    cSUserLogin.Notification = true;
                    cSUserLogin.ContactNo = contact.ContactNo;
                    bool insertUserLogin = _UserRepository.InsertCSUserLogin(cSUserLogin);
                    return new
                    {
                        success = true,
                        data = cSUserLogin,
                        Message = "Register Complete!"
                    };
                }
                else
                {
                    Model.CRMMobile.UserLogin userLogin = _UserRepository.GetUserLoginByPhoneNumbandDeviceandUserProfileID_Mobile(data.DeviceID, data.PhoneNumber, ExistData.UserProfileID);
                    if (userLogin == null)
                    {
                        ExistData.PINCode = SHAHelper.ComputeHash(data.PINCode, "SHA512", null);
                        bool insert = _UserRepository.UpdateCSUserProfile(ExistData);

                        string GenerateAccessToken = SHAHelper.ComputeHash(data.DeviceID, "SHA512", null);
                        CRMUserLoginWithContactID cSUserLogin = new CRMUserLoginWithContactID();
                        cSUserLogin.UserPhoneNumber = data.PhoneNumber;
                        cSUserLogin.LoginDate = DateTime.Now.ToShortDateString();
                        cSUserLogin.DeviceID = data.DeviceID;
                        cSUserLogin.DeviceType = data.DeviceType;
                        cSUserLogin.UserToken = GenerateAccessToken;
                        cSUserLogin.UserProfileID = ExistData.UserProfileID;
                        cSUserLogin.CRMContactID = contact.ID;
                        cSUserLogin.Notification = true;
                        cSUserLogin.ContactNo = contact.ContactNo;
                        bool insertUserLogin = _UserRepository.InsertCSUserLogin(cSUserLogin);

                        return new
                        {
                            success = true,
                            data = cSUserLogin,
                            Message = "Register Complete!"
                        };
                    }
                    else
                    {
                        ExistData.PINCode = SHAHelper.ComputeHash(data.PINCode, "SHA512", null);
                        bool insert = _UserRepository.UpdateCSUserProfile(ExistData);

                        string GenerateAccessToken = SHAHelper.ComputeHash(data.DeviceID, "SHA512", null);
                        userLogin.UserToken = GenerateAccessToken;
                        userLogin.LoginDate = DateTime.Now.ToShortDateString();
                        CRMUserLoginWithContactID cSUserLogin = new CRMUserLoginWithContactID();
                        cSUserLogin.UserPhoneNumber = data.PhoneNumber;
                        cSUserLogin.LoginDate = userLogin.LoginDate;
                        cSUserLogin.DeviceID = userLogin.DeviceID;
                        cSUserLogin.DeviceType = userLogin.DeviceType;
                        cSUserLogin.UserToken = userLogin.UserToken;
                        cSUserLogin.UserProfileID = userLogin.UserProfileID;
                        cSUserLogin.CRMContactID = contact.ID;
                        cSUserLogin.Notification = true;
                        cSUserLogin.ContactNo = contact.ContactNo;
                        bool insertUserLogin = _UserRepository.UpdateCSUserLogin(userLogin);

                        return new
                        {
                            success = true,
                            data = cSUserLogin,
                            Message = "Register Complete!"
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("RequestOTP")]
        [SwaggerOperation(Summary = "Request OTP From ThaiBulk เพื่อมสเพื่อยืนยันตัวตน เพื่อใช่งานระบบ ",
        Description = "เพื่อทำการ ยืนยันตัวตนของผู้ใช้งาน ซึ่ง 1 คนสามารถมีมากกว่า 1 UserLogin ได้")]
        public async Task<object> RequestOTP([FromBody]RequestOTPParam data)
        {
            try
            {
                //#region VerifyHeader
                //string ErrorHeader = "";
                //if (!VerifyHeader(out ErrorHeader))
                //{
                //    return new
                //    {
                //        success = false,
                //        data = "Invalid AccessKey!!. ",
                //        valid = false
                //    };
                //}
                //#endregion

                var client = new HttpClient();
                ThaiBulkOTPRequest thaiBulkOTPRequest = new ThaiBulkOTPRequest();
                thaiBulkOTPRequest.key = UtilsProvider.AppSetting.ThaiBulkApiKey;
                thaiBulkOTPRequest.secret = UtilsProvider.AppSetting.ThaiBulkSecret;
                thaiBulkOTPRequest.msisdn = data.PhoneNumber;

                var Content = new StringContent(JsonConvert.SerializeObject(thaiBulkOTPRequest));
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                string PostURL = Environment.GetEnvironmentVariable("ThaiBulkRequestOTPURL");
                if (PostURL == null)
                {
                    PostURL = UtilsProvider.AppSetting.ThaiBulkRequestOTPURL;
                }
                string RequestParam = "?key=" + thaiBulkOTPRequest.key + "&secret=" + thaiBulkOTPRequest.secret + "&msisdn=" + thaiBulkOTPRequest.msisdn;
                PostURL = PostURL + RequestParam;
                var Respond = await client.PostAsync(PostURL + RequestParam, Content);
                if (Respond.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new
                    {
                        success = false, 
                        data = new AutorizeDataJWT(),
                        valid = false
                    };
                }
                var RespondData = await Respond.Content.ReadAsStringAsync();
                RespondData = RespondData.Replace(@"\", "");
                thaiBulkOTPRequestReturnObj returnObj = new thaiBulkOTPRequestReturnObj();
                returnObj = JsonConvert.DeserializeObject<thaiBulkOTPRequestReturnObj>(RespondData);


                return new
                {
                    success = true,
                    data = returnObj,
                    Message = "Request OTP Successfully!. Please Check OTP Number sended to your mobile !"
                };


            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("VerifyOTP")]
        [SwaggerOperation(Summary = "Request OTP From ThaiBulk เพื่อมสเพื่อยืนยันตัวตน เพื่อใช่งานระบบ ",
            Description = "เพื่อทำการ ยืนยันตัวตนของผู้ใช้งาน ซึ่ง 1 คนสามารถมีมากกว่า 1 UserLogin ได้")]
        public async Task<object> VerifyOTP([FromBody]ThaiBulkVerifyOTP data)
        {
            try
            {

                //#region VerifyHeader
                //string ErrorHeader = "";
                //if (!VerifyHeader(out ErrorHeader))
                //{
                //    return new
                //    {
                //        success = false,
                //        data = "Invalid AccessKey!!. ",
                //        valid = false
                //    };
                //}
                //#endregion
                string key = Environment.GetEnvironmentVariable("ThaiBulkVerifyOTPURL");
                if (key == null)
                {
                    key = UtilsProvider.AppSetting.ThaiBulkApiKey;
                }
                string secret = Environment.GetEnvironmentVariable("ThaiBulkVerifyOTPURL");
                if (secret == null)
                {
                    secret = UtilsProvider.AppSetting.ThaiBulkSecret;
                }
                var client = new HttpClient();
                ThaiBulkOTPRequest thaiBulkOTPRequest = new ThaiBulkOTPRequest();
                var Content = new StringContent(JsonConvert.SerializeObject(thaiBulkOTPRequest));
                string PostURL = Environment.GetEnvironmentVariable("ThaiBulkVerifyOTPURL");
                if (PostURL == null)
                {
                    PostURL = UtilsProvider.AppSetting.ThaiBulkVerifyOTPURL;
                }
                string RequestParam = "?key=" + key + "&secret=" + secret + "&token=" + data.token + "&pin=" + data.pin;
                PostURL = PostURL + RequestParam;
                var Respond = await client.PostAsync(PostURL, Content);
                if (Respond.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new
                    {
                        success = false,
                        data = new AutorizeDataJWT(),
                        valid = false
                    };
                }
                var RespondData = await Respond.Content.ReadAsStringAsync();
                RespondData = RespondData.Replace(@"\", "");
                thaiBulkOTPVerifyReturnObj returnObj = new thaiBulkOTPVerifyReturnObj();
                returnObj = JsonConvert.DeserializeObject<thaiBulkOTPVerifyReturnObj>(RespondData);


                return new
                {
                    success = true,
                    data = returnObj,
                    Message = "Verify OTP Successfully!. The Pin is Match"
                };


            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("GetUserPhoneNumberByIDCardNo")]
        [SwaggerOperation(Summary = "เรียกดูเบอร์โทรศัพท์ของลูกค้าจากระบบ CRM ทั้งหมด",
      Description = "Access Key ใช้ในการเรียหใช้ Function ถึงจะเรียกใช้ Function ได้")]
        public async Task<object> GetUserPhoneNumberByIDCardNo([FromBody]GetUserPhoneParam data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;
                //if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
                //{
                //    string AccessKey = api_key.First();
                //    string EmpCodeKey = EmpCode.First();

                //    if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                //    {
                //        return new
                //        {
                //            success = false,
                //            data = new AutorizeDataJWT(),
                //            message = "Require Key to Access the Function"
                //        };
                //    }
                //    else
                //    {
                //        string APApiKey = Environment.GetEnvironmentVariable("API_Key");
                //        if (APApiKey == null)
                //        {
                //            APApiKey = UtilsProvider.AppSetting.ApiKey;
                //        }
                //        if (api_key != APApiKey)
                //        {
                //            return new
                //            {
                //                success = false,
                //                data = new AutorizeDataJWT(),
                //                message = "Incorrect API KEY !!"
                //            };
                //        }
                //    }
                //}

                Model.CRMWeb.Contact contact = _UserRepository.GetCRMContactByIDCardNO(data.CitizenIdentityNo);
                if (contact == null)
                {
                    return new
                    {
                        success = false,
                        data = new AutorizeDataJWT(),
                        message = "Only AP Customer Can Regist to the System !!"
                    };
                }
                List<Model.CRMWeb.ContactPhone> contactPhone = _UserRepository.GetContactPhoneNumberByContactID_Web(contact.ID.ToString());
                if (contactPhone == null)
                {
                    return new
                    {
                        success = false,
                        data = new AutorizeDataJWT(),
                        message = "There is no Assosiate Phone Number with this IDCard Number!!"
                    };
                }
                GetUserCRMPhoneNumber cRMContact = new GetUserCRMPhoneNumber();
                cRMContact.contactPhones = contactPhone;
                cRMContact.FirstNameTH = contact.FirstNameTH;
                cRMContact.LastNameTH = contact.LastNameTH;
                cRMContact.IsVIP = contact.IsVIP;
                cRMContact.CitizenIdentityNo = contact.CitizenIdentityNo;

                return new
                {
                    success = true,
                    data = cRMContact,
                    message = "Get User Phone Success !"
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("GetUserBookingByUserID")]
        [SwaggerOperation(Summary = "ดึงข้อมูล การจองและสถานะการจองของลูกค้า",
         Description = "ส่ง UserID (GUID) มาเพื่อดึงข้อมูลการจองของลูกค้านั้นๆ")]
        public async Task<object> GetUserBookingByUserID([FromBody]GetUserBookingTrackingByUserIDParam data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;
                //if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
                //{
                //    string AccessKey = api_key.First();
                //    string EmpCodeKey = EmpCode.First();

                //    if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                //    {
                //        return new
                //        {
                //            success = false,
                //            data = new AutorizeDataJWT(),
                //            message = "Require Key to Access the Function"
                //        };
                //    }
                //    else
                //    {
                //        string APApiKey = Environment.GetEnvironmentVariable("API_Key");
                //        if (APApiKey == null)
                //        {
                //            APApiKey = UtilsProvider.AppSetting.ApiKey;
                //        }
                //        if (api_key != APApiKey)
                //        {
                //            return new
                //            {
                //                success = false,
                //                data = new AutorizeDataJWT(),
                //                message = "Incorrect API KEY !!"
                //            };
                //        }
                //    }
                //}

                Model.CRMWeb.Contact contact = _UserRepository.GetCRMContactByID(data.UserID);
                if (contact == null)
                {
                    return new
                    {
                        success = false,
                        data = new AutorizeDataJWT(),
                        message = "Only AP Customer Can Regist to the System !!"
                    };
                }
                List<iCRMBooking> getBilling = _UserRepository.GetUseriBookingByUserID(data.UserID);
                for (int i = 0; i < getBilling.Count(); i++)
                {
                    if (getBilling[i].Project == null || getBilling[i].Project == "")
                    {
                        getBilling.Remove(getBilling[i]);
                        continue;
                    }
                    var Project = getBilling[i].Project.Split(" ");
                    string ProjectShowName = "";
                    for (int ii = 1; ii < Project.Count(); ii++)
                    {
                        if (ProjectShowName == "")
                        {
                            ProjectShowName = Project[ii];
                        }
                        else
                        {
                            ProjectShowName = ProjectShowName + " " + Project[ii];
                        }
                    }
                    getBilling[i].ProjectShowName = ProjectShowName;
                    if (getBilling[i].ProjectAddressTH != "" && getBilling[i].ProjectAddressTH != null)
                    {
                        byte[] bytes = Encoding.Default.GetBytes(getBilling[i].ProjectAddressTH);
                        getBilling[i].ProjectAddressTH = Encoding.UTF8.GetString(bytes);
                    }
                    if (getBilling[i].AgreementOwnerAddressTH != "" && getBilling[i].AgreementOwnerAddressTH != null)
                    {
                        byte[] bytes2 = Encoding.Default.GetBytes(getBilling[i].AgreementOwnerAddressTH);
                        getBilling[i].AgreementOwnerAddressTH = Encoding.UTF8.GetString(bytes2);
                    }
                    if (getBilling[i].BookingOwnerAddressTH != "" && getBilling[i].BookingOwnerAddressTH != null)
                    {
                        byte[] bytes3 = Encoding.Default.GetBytes(getBilling[i].BookingOwnerAddressTH);
                        getBilling[i].BookingOwnerAddressTH = Encoding.UTF8.GetString(bytes3);
                    }
                }
                if (getBilling.Count == null)
                {
                    return new
                    {
                        success = false,
                        data = new AutorizeDataJWT(),
                        message = "There is no Assosiate Phone Number with this IDCard Number!!"
                    };
                }
                return new
                {
                    success = true,
                    data = getBilling,
                    message = "Get User iBooking Success !"
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }
        [HttpPost]
        [Route("iCRMMyProperty")]
        [SwaggerOperation(Summary = "ดึงข้อมูล การจองและสถานะการจองของลูกค้า",
 Description = "ส่ง UserID (GUID) มาเพื่อดึงข้อมูลการจองของลูกค้านั้นๆ")]
        public async Task<object> iCRMMyProperty([FromBody]iCRMMyPropertyParam data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;
                //if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
                //{
                //    string AccessKey = api_key.First();
                //    string EmpCodeKey = EmpCode.First();

                //    if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                //    {
                //        return new
                //        {
                //            success = false,
                //            data = new AutorizeDataJWT(),
                //            message = "Require Key to Access the Function"
                //        };
                //    }
                //    else
                //    {
                //        string APApiKey = Environment.GetEnvironmentVariable("API_Key");
                //        if (APApiKey == null)
                //        {
                //            APApiKey = UtilsProvider.AppSetting.ApiKey;
                //        }
                //        if (api_key != APApiKey)
                //        {
                //            return new
                //            {
                //                success = false,
                //                data = new AutorizeDataJWT(),
                //                message = "Incorrect API KEY !!"
                //            };
                //        }
                //    }
                //}

                Model.CRMWeb.Contact contact = _UserRepository.GetCRMContactByID(data.CRMContactID);
                if (contact == null)
                {
                    return new
                    {
                        success = false,
                        data = new AutorizeDataJWT(),
                        message = "Only AP Customer Can Regist to the System !!"
                    };
                }
                List<iCRMMyProperty> cRMMyProperties = _UserRepository.GetUseriCRMMyPropoty(data.ContactNo);

                if (cRMMyProperties.Count == null)
                {
                    return new
                    {
                        success = false,
                        data = new AutorizeDataJWT(),
                        message = "There is no Assosiate Phone Number with this IDCard Number!!"
                    };
                }
                return new
                {
                    success = true,
                    data = cRMMyProperties,
                    message = "Get User iBooking Success !"
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }
        [HttpPost]
        [Route("GetUserBillingTrackingold")]
        [SwaggerOperation(Summary = "เรียกดูเบอร์โทรศัพท์ของลูกค้าจากระบบ CRM ทั้งหมด",
Description = "Access Key ใช้ในการเรียหใช้ Function ถึงจะเรียกใช้ Function ได้")]
        public async Task<object> GetUserBillingTrackingold([FromBody]GetUserBillingTrackingParam data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;
                //if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
                //{
                //    string AccessKey = api_key.First();
                //    string EmpCodeKey = EmpCode.First();

                //    if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                //    {
                //        return new
                //        {
                //            success = false,
                //            data = new AutorizeDataJWT(),
                //            message = "Require Key to Access the Function"
                //        };
                //    }
                //    else
                //    {
                //        string APApiKey = Environment.GetEnvironmentVariable("API_Key");
                //        if (APApiKey == null)
                //        {
                //            APApiKey = UtilsProvider.AppSetting.ApiKey;
                //        }
                //        if (api_key != APApiKey)
                //        {
                //            return new
                //            {
                //                success = false,
                //                data = new AutorizeDataJWT(),
                //                message = "Incorrect API KEY !!"
                //            };
                //        }
                //    }
                //}

                //Model.CRMWeb.Contact contact = _UserRepository.GetCRMContactByID(data.UserID);
                //if (contact == null)
                //{
                //    return new
                //    {
                //        success = false,
                //        data = new AutorizeDataJWT(),
                //        message = "Only AP Customer Can Regist to the System !!"
                //    };
                //}

                List<GetBillingTrackingMobile> getBilling = _UserRepository.GetUserBillingTrackingByProjectandUnit(data.Project, data.Unit);
                if (getBilling.Count == null)
                {
                    return new
                    {
                        success = false,
                        data = new AutorizeDataJWT(),
                        message = "There is no Assosiate Phone Number with this IDCard Number!!"
                    };
                }
                BillingFinalTrackingGroup FinalList = new BillingFinalTrackingGroup();
                FinalList.BookingList = new List<BillingTrackingGroup>();
                FinalList.ContractList = new List<BillingTrackingGroup>();
                FinalList.TransferList = new List<BillingTrackingGroup>();
                FinalList.DownpayList = new List<BillingTrackingGroup>();
                List<GetBillingTrackingMobile> TempForDelete = new List<GetBillingTrackingMobile>();
                BillingTrackingGroup ContactGroup = new BillingTrackingGroup();
                ContactGroup.GetBillingTrackingMobile = new List<GetBillingTrackingMobile>();
                BillingTrackingGroup BookingGroup = new BillingTrackingGroup();
                BookingGroup.GetBillingTrackingMobile = new List<GetBillingTrackingMobile>();
                BillingTrackingGroup TransferGroup = new BillingTrackingGroup();
                TransferGroup.GetBillingTrackingMobile = new List<GetBillingTrackingMobile>();
                for (int i = 0; i < getBilling.Count(); i++)
                {
                    bool HaveFET = _UserRepository.GetUserFETByPaymentMethodID(getBilling[i].PaymentID);
                    getBilling[i].HaveFET = HaveFET;
                    if (getBilling[i].UnitPriceStage == 1 && getBilling[i].FlagBooking != null) // เงินจอง
                    {
                        BookingGroup.GetBillingTrackingMobile.Add(getBilling[i]);
                        BookingGroup.DetailDownPayment = Convert.ToInt32(getBilling[i].DetailDownPayment);
                        BookingGroup.IsOverDue = getBilling[i].FlagOverDue == "Y" ? true : false;
                        BookingGroup.PaymentAmount = Convert.ToDouble(getBilling[i].BookingAmount);
                        BookingGroup.PaymentDueDate = getBilling[i].PaymentDueDate;
                        //--------------------------
                        BookingGroup.DownPerInstallment = getBilling[i].DownPerInstallment;
                        BookingGroup.NormalDownPerInstallment = getBilling[i].NormalDownPerInstallment;
                        BookingGroup.SpecialDownPaymentFlag = getBilling[i].SpecialDownPaymentFlag;
                        BookingGroup.SpecialDownPerInstallment = getBilling[i].SpecialDownPerInstallment;
                        BookingGroup.AgreementAmount = getBilling[i].AgreementAmount;
                        BookingGroup.BookingAmount = getBilling[i].BookingAmount;
                        BookingGroup.BookingPaymentDate = getBilling[i].BookingPaymentDate;
                        BookingGroup.FlagAgreement = getBilling[i].FlagAgreement;
                        BookingGroup.FlagAgreementReceipt = getBilling[i].FlagAgreementReceipt;
                        BookingGroup.FlagBooking = getBilling[i].FlagBooking;
                        BookingGroup.FlagBookingReceipt = getBilling[i].FlagBookingReceipt;
                        BookingGroup.FlagOverDue = getBilling[i].FlagOverDue;
                        BookingGroup.FlagReceipt = getBilling[i].FlagReceipt;
                        BookingGroup.PayAgreementAmount = getBilling[i].PayAgreementAmount;
                        BookingGroup.SpecialDownPaymentFlag = getBilling[i].SpecialDownPaymentFlag;
                        BookingGroup.SpecialDownPerInstallment = getBilling[i].SpecialDownPerInstallment;
                        BookingGroup.PayRemain = Convert.ToDouble(getBilling[i].AmountBalance);
                        BookingGroup.PaymentItemNameTH = getBilling[i].PaymentItemNameTH;
                        BookingGroup.PaymentItemNameEN = getBilling[i].PaymentItemNameEN;
                        //if (BookingGroup.PayRemain == 0)
                        //{
                        //    BookingGroup.PayRemain = Convert.ToDouble(getBilling[i].BookingAmount) - Convert.ToDouble(getBilling[i].PayBookingAmount);
                        //}
                        //else
                        //{
                        //    BookingGroup.PayRemain = BookingGroup.PayRemain - Convert.ToDouble(getBilling[i].PayBookingAmount);
                        //}
                        FinalList.BookingList.Add(BookingGroup);
                        TempForDelete.Add(getBilling[i]);
                    }
                    else if (getBilling[i].UnitPriceStage == 2 && getBilling[i].FlagAgreement != null) //สัญญา
                    {
                        ContactGroup.GetBillingTrackingMobile.Add(getBilling[i]);
                        ContactGroup.DetailDownPayment = Convert.ToInt32(getBilling[i].DetailDownPayment);
                        ContactGroup.IsOverDue = getBilling[i].FlagOverDue == "Y" ? true : false;
                        ContactGroup.PaymentAmount = Convert.ToDouble(getBilling[i].BookingAmount);
                        ContactGroup.PaymentDueDate = getBilling[i].PaymentDueDate;
                        //--------------------------
                        ContactGroup.DownPerInstallment = getBilling[i].DownPerInstallment;
                        ContactGroup.NormalDownPerInstallment = getBilling[i].NormalDownPerInstallment;
                        ContactGroup.SpecialDownPaymentFlag = getBilling[i].SpecialDownPaymentFlag;
                        ContactGroup.SpecialDownPerInstallment = getBilling[i].SpecialDownPerInstallment;
                        ContactGroup.AgreementAmount = getBilling[i].AgreementAmount;
                        ContactGroup.BookingAmount = getBilling[i].BookingAmount;
                        ContactGroup.BookingPaymentDate = getBilling[i].BookingPaymentDate;
                        ContactGroup.FlagAgreement = getBilling[i].FlagAgreement;
                        ContactGroup.FlagAgreementReceipt = getBilling[i].FlagAgreementReceipt;
                        ContactGroup.FlagBooking = getBilling[i].FlagBooking;
                        ContactGroup.FlagBookingReceipt = getBilling[i].FlagBookingReceipt;
                        ContactGroup.FlagOverDue = getBilling[i].FlagOverDue;
                        ContactGroup.FlagReceipt = getBilling[i].FlagReceipt;
                        ContactGroup.PayAgreementAmount = getBilling[i].PayAgreementAmount;
                        ContactGroup.SpecialDownPaymentFlag = getBilling[i].SpecialDownPaymentFlag;
                        ContactGroup.SpecialDownPerInstallment = getBilling[i].SpecialDownPerInstallment;
                        ContactGroup.PayRemain = Convert.ToDouble(getBilling[i].AmountBalance);
                        ContactGroup.PaymentItemNameTH = getBilling[i].PaymentItemNameTH;
                        ContactGroup.PaymentItemNameEN = getBilling[i].PaymentItemNameEN;
                        //if (ContactGroup.PayRemain == 0)
                        //{
                        //    ContactGroup.PayRemain = Convert.ToDouble(getBilling[i].AgreementAmount) - Convert.ToDouble(getBilling[i].PayAgreementAmount);
                        //}
                        //else
                        //{
                        //    ContactGroup.PayRemain = BookingGroup.PayRemain - Convert.ToDouble(getBilling[i].PayAgreementAmount);
                        //}
                        FinalList.ContractList.Add(ContactGroup);
                        TempForDelete.Add(getBilling[i]);
                    }
                    else if (getBilling[i].UnitPriceStage == 5 && getBilling[i].FlagTransfer != null)//โอน
                    {
                        TransferGroup.GetBillingTrackingMobile.Add(getBilling[i]);
                        TransferGroup.DetailDownPayment = Convert.ToInt32(getBilling[i].DetailDownPayment);
                        TransferGroup.IsOverDue = getBilling[i].FlagOverDue == "Y" ? true : false;
                        TransferGroup.PaymentAmount = Convert.ToDouble(getBilling[i].BookingAmount);
                        TransferGroup.PaymentDueDate = getBilling[i].PaymentDueDate;
                        //--------------------------
                        TransferGroup.DownPerInstallment = getBilling[i].DownPerInstallment;
                        TransferGroup.NormalDownPerInstallment = getBilling[i].NormalDownPerInstallment;
                        TransferGroup.SpecialDownPaymentFlag = getBilling[i].SpecialDownPaymentFlag;
                        TransferGroup.SpecialDownPerInstallment = getBilling[i].SpecialDownPerInstallment;
                        TransferGroup.AgreementAmount = getBilling[i].AgreementAmount;
                        TransferGroup.BookingAmount = getBilling[i].BookingAmount;
                        TransferGroup.BookingPaymentDate = getBilling[i].BookingPaymentDate;
                        TransferGroup.FlagTransfer = getBilling[i].FlagTransfer;
                        TransferGroup.TransferAmount = getBilling[i].TransferAmount;
                        TransferGroup.TransferPaymentDate = getBilling[i].TransferPaymentDate;
                        TransferGroup.FlagTransferReceipt = getBilling[i].FlagTransferReceipt;
                        TransferGroup.FlagAgreement = getBilling[i].FlagAgreement;
                        TransferGroup.FlagAgreementReceipt = getBilling[i].FlagAgreementReceipt;
                        TransferGroup.FlagBooking = getBilling[i].FlagBooking;
                        TransferGroup.FlagBookingReceipt = getBilling[i].FlagBookingReceipt;
                        TransferGroup.FlagOverDue = getBilling[i].FlagOverDue;
                        TransferGroup.FlagReceipt = getBilling[i].FlagReceipt;
                        TransferGroup.PayAgreementAmount = getBilling[i].PayAgreementAmount;
                        TransferGroup.SpecialDownPaymentFlag = getBilling[i].SpecialDownPaymentFlag;
                        TransferGroup.SpecialDownPerInstallment = getBilling[i].SpecialDownPerInstallment;
                        TransferGroup.PayRemain = Convert.ToDouble(getBilling[i].AmountBalance);
                        //if (ContactGroup.PayRemain == 0)
                        //{
                        //    ContactGroup.PayRemain = Convert.ToDouble(getBilling[i].TransferAmount) - Convert.ToDouble(getBilling[i].PayTransferAmount);
                        //}
                        //else
                        //{
                        //    ContactGroup.PayRemain = BookingGroup.PayRemain - Convert.ToDouble(getBilling[i].PayTransferAmount);
                        //}
                        FinalList.TransferList.Add(TransferGroup);
                        TempForDelete.Add(getBilling[i]);
                    }

                }
                for (int i = 0; i < TempForDelete.Count(); i++)
                {
                    getBilling.Remove(TempForDelete[i]);
                }
                List<GetBillingTrackingMobile> DistinctList = getBilling.DistinctBy(p => p.DetailDownPayment).ToList();
                List<BillingTrackingGroup> GroupList = new List<BillingTrackingGroup>();
                int DownPay = 1;
                for (int i = 0; i < DistinctList.Count(); i++)
                {
                    double Balance = 0;
                    List<GetBillingTrackingMobile> BillingGroup = getBilling.Where(S => S.DetailDownPayment == DownPay.ToString()).ToList();
                    BillingTrackingGroup Group = new BillingTrackingGroup();
                    Group.GetBillingTrackingMobile = new List<GetBillingTrackingMobile>();
                    for (int ii = 0; ii < BillingGroup.Count(); ii++)
                    {
                        if (BillingGroup[ii].UnitPriceStageName.Trim() == "จอง")
                        {
                            //FinalList.bookingList.Add(BillingGroup[ii]);
                        }
                        else if (BillingGroup[ii].UnitPriceStageName.Trim() == "สัญญา")
                        {
                            //FinalList.ContractList.Add(BillingGroup[ii]);
                        }
                        else
                        {
                            Group.GetBillingTrackingMobile.Add(BillingGroup[ii]);
                            Group.DetailDownPayment = DownPay;
                            Group.IsOverDue = BillingGroup[ii].FlagOverDue == "Y" ? true : false;
                            Group.PaymentAmount = Convert.ToDouble(BillingGroup[ii].BookingAmount);
                            Group.PaymentDueDate = BillingGroup[ii].PaymentDueDate;
                            //--------------------------
                            Group.DownPerInstallment = BillingGroup[ii].DownPerInstallment;
                            Group.NormalDownPerInstallment = BillingGroup[ii].NormalDownPerInstallment;
                            Group.SpecialDownPaymentFlag = BillingGroup[ii].SpecialDownPaymentFlag;
                            Group.SpecialDownPerInstallment = BillingGroup[ii].SpecialDownPerInstallment;
                            Group.AgreementAmount = BillingGroup[ii].AgreementAmount;
                            Group.BookingAmount = BillingGroup[ii].BookingAmount;
                            Group.BookingPaymentDate = BillingGroup[ii].BookingPaymentDate;
                            Group.FlagAgreement = BillingGroup[ii].FlagAgreement;
                            Group.FlagAgreementReceipt = BillingGroup[ii].FlagAgreementReceipt;
                            Group.FlagBooking = BillingGroup[ii].FlagBooking;
                            Group.FlagBookingReceipt = BillingGroup[ii].FlagBookingReceipt;
                            Group.FlagOverDue = BillingGroup[ii].FlagOverDue;
                            Group.FlagReceipt = BillingGroup[ii].FlagReceipt;
                            Group.PayAgreementAmount = BillingGroup[ii].PayAgreementAmount;
                            Group.SpecialDownPaymentFlag = BillingGroup[ii].SpecialDownPaymentFlag;
                            Group.SpecialDownPerInstallment = BillingGroup[ii].SpecialDownPerInstallment;
                            Group.PayRemain = Convert.ToDouble(BillingGroup[ii].AmountBalance);
                            Group.PaymentItemNameTH = BillingGroup[ii].PaymentItemNameTH;
                            Group.PaymentItemNameEN = BillingGroup[ii].PaymentItemNameEN;
                            //if (Group.PayRemain == 0)
                            //{
                            //    Group.PayRemain = Convert.ToDouble(BillingGroup[ii].DownPerInstallment) - Convert.ToDouble(BillingGroup[ii].AmountPaid);
                            //}
                            //else
                            //{
                            //    Group.PayRemain = Group.PayRemain - Convert.ToDouble(BillingGroup[ii].AmountPaid);
                            //}
                        }
                    }
                    GroupList.Add(Group);
                    DownPay++;
                }
                FinalList.BillingTrackingGroup = GroupList;
                if (FinalList.BillingTrackingGroup == null || FinalList.BillingTrackingGroup.Count() == 0)
                {
                    FinalList.BillingTrackingGroup = null;
                }
                if (FinalList.BookingList == null || FinalList.BookingList.Count() == 0)
                {
                    FinalList.BookingList = null;
                }
                if (FinalList.ContractList == null || FinalList.ContractList.Count() == 0)
                {
                    FinalList.ContractList = null;
                }
                if (FinalList.TransferList == null || FinalList.TransferList.Count() == 0)
                {
                    FinalList.TransferList = null;
                }
                if (FinalList.DownpayList == null || FinalList.DownpayList.Count() == 0)
                {
                    FinalList.DownpayList = null;
                }
                return new
                {
                    success = true,
                    data = FinalList,
                    message = "Get User iBooking Success !"
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }
        [HttpPost]
        [Route("PersonalContact")]
        [SwaggerOperation(Summary = "เรียกดูเบอร์โทรศัพท์ของลูกค้าจากระบบ CRM ทั้งหมด",
Description = "Access Key ใช้ในการเรียหใช้ Function ถึงจะเรียกใช้ Function ได้")]
        public async Task<object> GetUserBillingTracking([FromBody]PersonalContactParam data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;
                //if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
                //{
                //    string AccessKey = api_key.First();
                //    string EmpCodeKey = EmpCode.First();

                //    if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                //    {
                //        return new
                //        {
                //            success = false,
                //            data = new AutorizeDataJWT(),
                //            message = "Require Key to Access the Function"
                //        };
                //    }
                //    else
                //    {
                //        string APApiKey = Environment.GetEnvironmentVariable("API_Key");
                //        if (APApiKey == null)
                //        {
                //            APApiKey = UtilsProvider.AppSetting.ApiKey;
                //        }
                //        if (api_key != APApiKey)
                //        {
                //            return new
                //            {
                //                success = false,
                //                data = new AutorizeDataJWT(),
                //                message = "Incorrect API KEY !!"
                //            };
                //        }
                //    }
                //}

                //Model.CRMWeb.Contact contact = _UserRepository.GetCRMContactByID(data.UserID);
                //if (contact == null)
                //{
                //    return new
                //    {
                //        success = false,
                //        data = new AutorizeDataJWT(),
                //        message = "Only AP Customer Can Regist to the System !!"
                //    };
                //}
                List<PersonalContact> PS = new List<PersonalContact>();
                PersonalContact personal = new PersonalContact();
                personal.Name = "Puriwat Sudror";
                personal.PhoneNumber = "0969879494";
                personal.Remark = "คุณภูริวัช หัวหน้าสมาคม อาบอบนวด ";
                PS.Add(personal);
                PersonalContact personal2 = new PersonalContact();
                personal2.Name = "Wiwat Sudror";
                personal2.PhoneNumber = "0641956694";
                personal2.Remark = "หัวหน้าสมาคมกวนตรีนสัส!";
                PS.Add(personal2);
                PersonalContact personal3 = new PersonalContact();
                personal3.Name = "Kusuma Sudswai";
                personal3.PhoneNumber = "0994142269";
                personal3.Remark = "หัวหน้าไม้กระดานแห้ง ประเทศไทย!";
                PS.Add(personal3);
                return new
                {
                    success = true,
                    data = PS,
                    message = "Get User PersonalContact Success !"
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("Ads")]
        [SwaggerOperation(Summary = "เรียกดูเบอร์โทรศัพท์ของลูกค้าจากระบบ CRM ทั้งหมด",
Description = "Access Key ใช้ในการเรียหใช้ Function ถึงจะเรียกใช้ Function ได้")]
        public async Task<object> Ads([FromBody]AdsParam data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;
                //if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
                //{
                //    string AccessKey = api_key.First();
                //    string EmpCodeKey = EmpCode.First();

                //    if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                //    {
                //        return new
                //        {
                //            success = false,
                //            data = new AutorizeDataJWT(),
                //            message = "Require Key to Access the Function"
                //        };
                //    }
                //    else
                //    {
                //        string APApiKey = Environment.GetEnvironmentVariable("API_Key");
                //        if (APApiKey == null)
                //        {
                //            APApiKey = UtilsProvider.AppSetting.ApiKey;
                //        }
                //        if (api_key != APApiKey)
                //        {
                //            return new
                //            {
                //                success = false,
                //                data = new AutorizeDataJWT(),
                //                message = "Incorrect API KEY !!"
                //            };
                //        }
                //    }
                //}

                var AdsURL = Environment.GetEnvironmentVariable("AdsUrl");
                if (AdsURL == null)
                {
                    AdsURL = UtilsProvider.AppSetting.AdsUrl;
                }
                var FilePath = Path.Combine(_hostingEnvironment.WebRootPath, "Ads");
                string[] FileExt = { ".PNG", ".jpg", ".png", "png" };
                var picture = GetFilesFrom(FilePath + "//", FileExt, true);
                List<AdsObject> adsObjects = new List<AdsObject>();
                List<string> MobileAdsURL = new List<string>();
                foreach (var path in picture)
                {
                    AdsObject ads = new AdsObject();
                    var newpath = path.Split("//");
                    string fileName = newpath[newpath.Count() - 1];
                    string AdsFileUrl = AdsURL + fileName;
                    ads.AdsUrl = AdsFileUrl;
                    ads.Link = "http://www.apintranet.com/";
                    adsObjects.Add(ads);
                }

                return new
                {
                    success = true,
                    data = adsObjects,
                    message = "Get User PersonalContact Success !"
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("GetUseriCRMContact")]
        [SwaggerOperation(Summary = "เรียกดูเบอร์โทรศัพท์ของลูกค้าจากระบบ CRM ทั้งหมด",
Description = "Access Key ใช้ในการเรียหใช้ Function ถึงจะเรียกใช้ Function ได้")]
        public async Task<object> GetUseriCRMContact([FromBody]GetUserICRMContactParam data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;
                //if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
                //{
                //    string AccessKey = api_key.First();
                //    string EmpCodeKey = EmpCode.First();

                //    if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                //    {
                //        return new
                //        {
                //            success = false,
                //            data = new AutorizeDataJWT(),
                //            message = "Require Key to Access the Function"
                //        };
                //    }
                //    else
                //    {
                //        string APApiKey = Environment.GetEnvironmentVariable("API_Key");
                //        if (APApiKey == null)
                //        {
                //            APApiKey = UtilsProvider.AppSetting.ApiKey;
                //        }
                //        if (api_key != APApiKey)
                //        {
                //            return new
                //            {
                //                success = false,
                //                data = new AutorizeDataJWT(),
                //                message = "Incorrect API KEY !!"
                //            };
                //        }
                //    }
                //}

                //Model.CRMWeb.Contact contact = _UserRepository.GetCRMContactByID(data.UserID);
                //if (contact == null)
                //{
                //    return new
                //    {
                //        success = false,
                //        data = new AutorizeDataJWT(),
                //        message = "Only AP Customer Can Regist to the System !!"
                //    };
                //}
                List<iCRMContact> iCRMContacts = _UserRepository.GetUseriCRMContact_Web(data.ContactNo);
                if (iCRMContacts.Count == null)
                {
                    return new
                    {
                        success = false,
                        data = new iCRMContact(),
                        message = "There is no Assosiate Phone Number with this IDCard Number!!"
                    };
                }
                return new
                {
                    success = true,
                    data = iCRMContacts,
                    message = "Get User iBooking Success !"
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("GetUserCard")]
        [SwaggerOperation(Summary = "เรียกดูเบอร์โทรศัพท์ของลูกค้าจากระบบ CRM ทั้งหมด",
Description = "Access Key ใช้ในการเรียหใช้ Function ถึงจะเรียกใช้ Function ได้")]
        public async Task<object> GetUserCard([FromBody]GetUserCardParam data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;
                //if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
                //{
                //    string AccessKey = api_key.First();
                //    string EmpCodeKey = EmpCode.First();

                //    if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                //    {
                //        return new
                //        {
                //            success = false,
                //            data = new AutorizeDataJWT(),
                //            message = "Require Key to Access the Function"
                //        };
                //    }
                //    else
                //    {
                //        string APApiKey = Environment.GetEnvironmentVariable("API_Key");
                //        if (APApiKey == null)
                //        {
                //            APApiKey = UtilsProvider.AppSetting.ApiKey;
                //        }
                //        if (api_key != APApiKey)
                //        {
                //            return new
                //            {
                //                success = false,
                //                data = new AutorizeDataJWT(),
                //                message = "Incorrect API KEY !!"
                //            };
                //        }
                //    }
                //}

                //Model.CRMWeb.Contact contact = _UserRepository.GetCRMContactByID(data.UserID);
                //if (contact == null)
                //{
                //    return new
                //    {
                //        success = false,
                //        data = new AutorizeDataJWT(),
                //        message = "Only AP Customer Can Regist to the System !!"
                //    };
                //}
                List<GetUserCardReturnObj> getUserCard = _UserRepository.GetUserCardByProjectandUnit(data.ContactNo);

                //Model.CRMMobile.UserProfile Contact = _UserRepository.GetUserProfileByCRMContactID_Mobile(data.ContactNo);

                if (getUserCard == null || getUserCard.Count() == 0)
                {
                    return new
                    {
                        success = false,
                        data = new GetUserCardReturnObj(),
                        message = "Cannot Find User Data!"
                    };
                }

                return new
                {
                    success = true,
                    data = getUserCard,
                    message = "Get User's Card Success !"
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("GetICRMOwnerData")]
        [SwaggerOperation(Summary = "เรียกดูเบอร์โทรศัพท์ของลูกค้าจากระบบ CRM ทั้งหมด",
Description = "Access Key ใช้ในการเรียหใช้ Function ถึงจะเรียกใช้ Function ได้")]
        public object GetICRMOwnerData([FromBody]GetUserICRMOwner data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;
                //if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
                //{
                //    string AccessKey = api_key.First();
                //    string EmpCodeKey = EmpCode.First();

                //    if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                //    {
                //        return new
                //        {
                //            success = false,
                //            data = new AutorizeDataJWT(),
                //            message = "Require Key to Access the Function"
                //        };
                //    }
                //    else
                //    {
                //        string APApiKey = Environment.GetEnvironmentVariable("API_Key");
                //        if (APApiKey == null)
                //        {
                //            APApiKey = UtilsProvider.AppSetting.ApiKey;
                //        }
                //        if (api_key != APApiKey)
                //        {
                //            return new
                //            {
                //                success = false,
                //                data = new AutorizeDataJWT(),
                //                message = "Incorrect API KEY !!"
                //            };
                //        }
                //    }
                //}

                //Model.CRMWeb.Contact contact = _UserRepository.GetCRMContactByID(data.UserID);
                //if (contact == null)
                //{
                //    return new
                //    {
                //        success = false,
                //        data = new AutorizeDataJWT(),
                //        message = "Only AP Customer Can Regist to the System !!"
                //    };
                //}
                List<GetiCRMOwnerReturnObj> getICRMOwner = _UserRepository.GetUserICRMOwnerByProjectUnitAndCRMContactID(data.ContactID, data.UnitNo, data.ProjectNo);

                //Model.CRMMobile.UserProfile Contact = _UserRepository.GetUserProfileByCRMContactID_Mobile(data.ContactNo);

                if (getICRMOwner == null || getICRMOwner.Count() == 0)
                {
                    return new
                    {
                        success = false,
                        data = new GetiCRMOwnerReturnObj(),
                        message = "Cannot Find ICRM Owner Data!"
                    };
                }

                return new
                {
                    success = true,
                    data = getICRMOwner,
                    message = "Get User's ICRM Owner Success !"
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }


        [HttpPost]
        [Route("GetUserCreditCard")]
        [SwaggerOperation(Summary = "เรียกดูเบอร์โทรศัพท์ของลูกค้าจากระบบ CRM ทั้งหมด",
Description = "Access Key ใช้ในการเรียหใช้ Function ถึงจะเรียกใช้ Function ได้")]
        public async Task<object> GetUserCreditCard([FromBody]GetUserCreditCardParam data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;

                //if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
                //{
                //    string AccessKey = api_key.First();
                //    string EmpCodeKey = EmpCode.First();

                //    if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                //    {
                //        return new
                //        {
                //            success = false,
                //            data = new AutorizeDataJWT(),
                //            message = "Require Key to Access the Function"
                //        };
                //    }
                //    else
                //    {
                //        string APApiKey = Environment.GetEnvironmentVariable("API_Key");
                //        if (APApiKey == null)
                //        {
                //            APApiKey = UtilsProvider.AppSetting.ApiKey;
                //        }
                //        if (api_key != APApiKey)
                //        {
                //            return new
                //            {
                //                success = false,
                //                data = new AutorizeDataJWT(),
                //                message = "Incorrect API KEY !!"
                //            };
                //        }
                //    }
                //}

                //Model.CRMWeb.Contact contact = _UserRepository.GetCRMContactByID(data.UserID);
                //if (contact == null)
                //{
                //    return new
                //    {
                //        success = false,
                //        data = new AutorizeDataJWT(),
                //        message = "Only AP Customer Can Regist to the System !!"
                //    };
                //}
                GetUserCreditCardReturnObj getUserCard = _UserRepository.GetUserCreditCardByProjectandUnit(data.ProjectNo, data.UnitNo);
                if (getUserCard == null)
                {
                    return new
                    {
                        success = false,
                        data = new GetUserCreditCardReturnObj(),
                        message = "Cannot Find Data!"
                    };
                }
                var cardNumber = getUserCard.AccountNO;

                var firstDigits = cardNumber.Substring(0, 6);
                var lastDigits = cardNumber.Substring(cardNumber.Length - 4, 4);

                var requiredMask = new String('X', cardNumber.Length - firstDigits.Length - lastDigits.Length);

                var maskedString = string.Concat(firstDigits, requiredMask, lastDigits);
                var maskedCardNumberWithSpaces = Regex.Replace(maskedString, ".{4}", "$0 ");
                getUserCard.AccountNO = maskedCardNumberWithSpaces;
                return new
                {
                    success = true,
                    data = getUserCard,
                    message = "Get User's Card Success !"
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("GenerateQACode")]
        [SwaggerOperation(Summary = "GenerateQR Code สำหรับลูกค้าระบบ CRM ",
Description = "Access Key ใช้ในการเรียหใช้ Function ถึงจะเรียกใช้ Function ได้")]
        public async Task<object> GenerateQACode([FromBody]GetUserCreditCardParam data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;

                //if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
                //{
                //    string AccessKey = api_key.First();
                //    string EmpCodeKey = EmpCode.First();

                //    if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                //    {
                //        return new
                //        {
                //            success = false,
                //            data = new AutorizeDataJWT(),
                //            message = "Require Key to Access the Function"
                //        };
                //    }
                //    else
                //    {
                //        string APApiKey = Environment.GetEnvironmentVariable("API_Key");
                //        if (APApiKey == null)
                //        {
                //            APApiKey = UtilsProvider.AppSetting.ApiKey;
                //        }
                //        if (api_key != APApiKey)
                //        {
                //            return new
                //            {
                //                success = false,
                //                data = new AutorizeDataJWT(),
                //                message = "Incorrect API KEY !!"
                //            };
                //        }
                //    }
                //}

                //Model.CRMWeb.Contact contact = _UserRepository.GetCRMContactByID(data.UserID);
                //if (contact == null)
                //{
                //    return new
                //    {
                //        success = false,
                //        data = new AutorizeDataJWT(),
                //        message = "Only AP Customer Can Regist to the System !!"
                //    };
                //}
                //var QRCodeBarcodeText = "|" + TaxNo + IIf(ProjectNo == "70013", "01", "00") + Strings.Chr(13) + Ref1 + Strings.Chr(13) + Ref2 + Strings.Chr(13) + "000";
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode("The text which should be encoded.", QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);
                var bitmapBytes = BitmapToBytes(qrCodeImage); //Convert bitmap into a byte array


                return new
                {
                    success = true,
                    data = File(bitmapBytes, "image/jpeg"), //Return as file result
                    message = "Get User's Card Success !"
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("SendMobileNotification")]
        [SwaggerOperation(Summary = "เรียกดูเบอร์โทรศัพท์ของลูกค้าจากระบบ CRM ทั้งหมด User = MIN | OAT",
Description = "Access Key ใช้ในการเรียหใช้ Function ถึงจะเรียกใช้ Function ได้")]
        public async Task<object> SendMobileNotification([FromBody]CreateMobileNotificationParam data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;

                //if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
                //{
                //    string AccessKey = api_key.First();
                //    string EmpCodeKey = EmpCode.First();

                //    if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                //    {
                //        return new
                //        {
                //            success = false,
                //            data = new AutorizeDataJWT(),
                //            message = "Require Key to Access the Function"
                //        };
                //    }
                //    else
                //    {
                //        string APApiKey = Environment.GetEnvironmentVariable("API_Key");
                //        if (APApiKey == null)
                //        {
                //            APApiKey = UtilsProvider.AppSetting.ApiKey;
                //        }
                //        if (api_key != APApiKey)
                //        {
                //            return new
                //            {
                //                success = false,
                //                data = new AutorizeDataJWT(),
                //                message = "Incorrect API KEY !!"
                //            };
                //        }
                //    }
                //}

                string Token = "";
                string CRMContactID = "";
                if (data.User == "MIN")
                {
                    Token = "fd2JuQd5HKs:APA91bFdBvhFD0zFP1nP_Q5nP_WR6lkSQmIkxcSPR7xOSsmuLBc0eeYS2jaIuKIm9M3RKeQXeNYaYCzqT0KzsBVP60BAFZRfBBXTIrT8oMgDwIURd4yrwD1gHU5FTFOLQyy0sdMxkoCK";
                    CRMContactID = "c1ee1cc1-cf81-4f80-9411-b5184f6c3efc";
                }
                else if (data.User == "BOM")
                {
                    Token = "c0i-n2uiTTiIYMTNaxWNY3:APA91bFnIzYDJ4RSWTT7FZJA6AmIJaBdj-svi3CR4wglzhcWtKNiQrnk7na1y3opdpjr8WQlGsoSuAdBhOsZKpOYF-GxysFFVAnxpHIin-fmjUlmq6NfN-EHQcclJssSdtr9dEIxUUkB";
                    CRMContactID = "c1ee1cc1-cf81-4f80-9411-b5184f6c3efc";
                }
                else
                {
                    Token = "c03ntc-jHUv2vdoFonSb5v:APA91bFAPKMJJMWwFr8qDHppXy4CRJ_G3871hVkeus0zlIcqc7BpSKijiJyzH-bbo_L2WaCgFgQPvv4Ww5JX-MxPezYgrgG4UXaPFv9bM5CWIVPUJXTww2I2KBXDEVLi-3aXzoMXhy_t";
                    CRMContactID = "5b3c2e99-d792-45c0-a726-859b853d0333";
                }

                var a = _mobileMessagingClient.CreateNotification("tests", "My First Notifications", Token);
                string MsgTitleTH = "พี่ปอม";
                string MsgTitleEN = "P'Pom";
                string BodyMsgTH = "คิดถึงน้องพลอยจัง วันนี้ไม่เจอเลย /r/n " + "จะออกไปแตะขอบฟ้า แต่เหมือนว่าโชคชะตาไม่เข้าใจ มองไปไม่มัหนทาง แต่รู้ว่าจะต้องไปต่อไป สิ้นแสงขอบฟ้าสีคราม ร้องต่อไม่ได้แล้ว อยู่กับพี่จะมัแต่เสียงร้องของดนตรี ไม่มีเสียงร้องพร้อมน้ำตาแน่นอน <3";
                string BodyMsgEN = "I miss N'Ploy /r/n " + "I want to touch the end of the sky. but distiny never understand me. there is no way ahead but i have to make a way through. If N'Ploy is with me there will be only a sound of joy , Never tears.";
                if (data.Language.ToLower() == "th")
                {
                    var b = _mobileMessagingClient.SendNotification(Token, MsgTitleTH, BodyMsgTH);
                }
                else
                {
                    var b = _mobileMessagingClient.SendNotification(Token, MsgTitleEN, BodyMsgEN);
                }


                Model.CRMMobile.NotificationHistory Nh = new Model.CRMMobile.NotificationHistory();
                Nh.Created = DateTime.Now.ToString();
                Nh.ProjectNo = "Test Project";
                Nh.CRMContactID = CRMContactID;
                Nh.ProjectNameTH = "Project Test NH";
                Nh.MsgType = "Manuel Type";
                Nh.SendMsgStatus = true;
                Nh.MessageTitleTH = MsgTitleTH;
                Nh.MsgTH = BodyMsgTH;
                Nh.MessageTitleENG = BodyMsgEN;
                Nh.MsgEN = BodyMsgEN;
                Nh.IsRead = false;
                bool Insert = _UserRepository.InsertNotificationHistory(Nh);
                if (Insert)
                {
                    return new
                    {
                        success = true,
                        data = "",
                        message = "Get User's Card Success !"
                    };
                }
                else
                {
                    return new
                    {
                        success = false,
                        data = "",
                        message = "Get User's Card Fail !"
                    };
                }


            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("SendMobileNotificationFromWeb")]
        [SwaggerOperation(Summary = "เรียกดูเบอร์โทรศัพท์ของลูกค้าจากระบบ CRM ทั้งหมด User = MIN | OAT",
Description = "Access Key ใช้ในการเรียหใช้ Function ถึงจะเรียกใช้ Function ได้")]
        public async Task<object> SendMobileNotificationFromWeb([FromBody]CreateMobileNotificationFromWebParam data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;

                //if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
                //{
                //    string AccessKey = api_key.First();
                //    string EmpCodeKey = EmpCode.First();

                //    if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                //    {
                //        return new
                //        {
                //            success = false,
                //            data = new AutorizeDataJWT(),
                //            message = "Require Key to Access the Function"
                //        };
                //    }
                //    else
                //    {
                //        string APApiKey = Environment.GetEnvironmentVariable("API_Key");
                //        if (APApiKey == null)
                //        {
                //            APApiKey = UtilsProvider.AppSetting.ApiKey;
                //        }
                //        if (api_key != APApiKey)
                //        {
                //            return new
                //            {
                //                success = false,
                //                data = new AutorizeDataJWT(),
                //                message = "Incorrect API KEY !!"
                //            };
                //        }
                //    }
                //}

                string Token = "";
                string CRMContactID = "";
                Model.CRMWeb.Contact contact = _UserRepository.GetCRMContactByID(data.CRMContactID);
                Model.CRMMobile.UserProfile userProfile = _UserRepository.GetUserProfileByCRMContactID_Mobile(data.CRMContactID);
                Model.CRMMobile.UserLogin userLogin = _UserRepository.GetUserLoginByID_Mobile(userProfile.UserProfileID);
                if (contact == null)
                {
                    return new
                    {
                        success = false,
                        data = new AutorizeDataJWT(),
                        message = "Only AP Customer Can Regist to the System !!"
                    };
                }
                

                //var a = _mobileMessagingClient.CreateNotification("tests", "My First Notifications", Token);
                //string MsgTitleTH = "พี่ปอม";
                //string MsgTitleEN = "P'Pom";
                //string BodyMsgTH = "คิดถึงน้องพลอยจัง วันนี้ไม่เจอเลย /r/n " + "จะออกไปแตะขอบฟ้า แต่เหมือนว่าโชคชะตาไม่เข้าใจ มองไปไม่มัหนทาง แต่รู้ว่าจะต้องไปต่อไป สิ้นแสงขอบฟ้าสีคราม ร้องต่อไม่ได้แล้ว อยู่กับพี่จะมัแต่เสียงร้องของดนตรี ไม่มีเสียงร้องพร้อมน้ำตาแน่นอน <3";
                //string BodyMsgEN = "I miss N'Ploy /r/n " + "I want to touch the end of the sky. but distiny never understand me. there is no way ahead but i have to make a way through. If N'Ploy is with me there will be only a sound of joy , Never tears.";
                if (userProfile.Language.ToLower() == "th")
                {
                    var b = _mobileMessagingClient.SendNotification(userLogin.FireBaseToken, data.MsgTitleTH, data.BodyMsgTH);
                }
                else
                {
                    var b = _mobileMessagingClient.SendNotification(userLogin.FireBaseToken, data.MsgTitleEN, data.BodyMsgEN);
                }


                Model.CRMMobile.NotificationHistory Nh = new Model.CRMMobile.NotificationHistory();
                Nh.Created = DateTime.Now.ToString();
                Nh.ProjectNo = data.ProjectCode;
                Nh.CRMContactID = CRMContactID;
                Nh.UnitNo = data.UnitNo;
                Nh.ProjectNameTH = "";
                Nh.MsgType = data.MsgReferenceKey;
                Nh.MsgFrom = "Web";
                Nh.SendMsgStatus = true;
                Nh.MessageTitleTH = data.MsgTitleTH;
                Nh.MsgTH = data.BodyMsgTH;
                Nh.MessageTitleENG = data.BodyMsgEN;
                Nh.MsgEN = data.BodyMsgEN;
                Nh.IsRead = false;
                if (userProfile.Language.ToLower() == "th")
                {
                    Nh.MsgLanguage = "TH";
                }
                else
                {
                    Nh.MsgLanguage = "EN";
                }
                bool Insert = _UserRepository.InsertNotificationHistory(Nh);
                if (Insert)
                {
                    return new
                    {
                        success = true,
                        data = "",
                        message = "Get User's Card Success !"
                    };
                }
                else
                {
                    return new
                    {
                        success = false,
                        data = "",
                        message = "Get User's Card Fail !"
                    };
                }


            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

    //    [HttpPost]
    //    [Route("SendMobileNotification")]
    //    [SwaggerOperation(Summary = "Log In เข้าสู้ระบบเพื่อรับ Access Key ",
    //Description = "Access Key ใช้ในการเรียหใช้ Function ต่างๆ เพื่อไม่ให้ User Login หลายเครื่องในเวลาเดียวกัน")]
    //    public async Task<object> SendMobileNotification([FromBody] CreateMobileNotificationFromWebParam data)
    //    {
    //        try
    //        {
              

    //            string APApiKey = Environment.GetEnvironmentVariable("API_Key");
    //            if (APApiKey == null)
    //            {
    //                APApiKey = UtilsProvider.AppSetting.ApiKey;
    //            }
    //            string APApiToken = Environment.GetEnvironmentVariable("Api_Token");
    //            if (APApiToken == null)
    //            {
    //                APApiToken = UtilsProvider.AppSetting.ApiToken;
    //            }

    //            CreateMobileNotificationFromWebParam Param = new CreateMobileNotificationFromWebParam();

    //            var client = new HttpClient();
    //            var Content = new StringContent(JsonConvert.SerializeObject(Param));
    //            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
    //            Content.Headers.Add("api_key", APApiKey);
    //            Content.Headers.Add("api_token", APApiToken);
    //            string PostURL = Environment.GetEnvironmentVariable("CRMMobilePath");
    //            PostURL = PostURL + "SendMobileNotificationFromWeb";
    //            if (PostURL == null)
    //            {
    //                PostURL = UtilsProvider.AppSetting.AuthorizeURL + "SendMobileNotificationFromWeb"; // จาก Appsetting.json
    //            }
    //            var Respond = await client.PostAsync(PostURL, Content);
    //            if (Respond.StatusCode != System.Net.HttpStatusCode.OK)
    //            {
    //                return new
    //                {
    //                    success = false,
    //                    data = new AutorizeDataJWT(),
    //                    valid = false
    //                };
    //            }
    //            var RespondData = await Respond.Content.ReadAsStringAsync();
    //            AutorizeDataJWT Result = JsonConvert.DeserializeObject<AutorizeDataJWT>(RespondData);
    //            AutorizeDataJWTReturnObject Return = new AutorizeDataJWTReturnObject();
    //            Return.AccountExpirationDate = Result.AccountExpirationDate;
    //            Return.AppUserRole = Result.AppUserRole;
    //            Return.AuthenticationProvider = Result.AuthenticationProvider;
    //            Return.CostCenterCode = Result.CostCenterCode;
    //            Return.CostCenterName = Result.CostCenterName;
    //            Return.DisplayName = Result.DisplayName;
    //            Return.Division = Result.Division;
    //            Return.DomainUserName = Result.DomainUserName;
    //            Return.Email = Result.Email;
    //            Return.EmployeeID = Result.EmployeeID;
    //            Return.FirstName = Result.FirstName;
    //            Return.LastLogon = Result.LastLogon;
    //            Return.LastName = Result.LastName;
    //            Return.LoginResult = Result.LoginResult;
    //            Return.LoginResultMessage = Result.LoginResultMessage;
    //            Return.SysAppCode = Result.SysAppCode;
    //            Return.SysUserData = JsonConvert.DeserializeObject<UserModel>(Result.SysUserData);
    //            Return.SysUserId = Result.SysUserId;
    //            Return.SysUserRoles = JsonConvert.DeserializeObject<vwUserRole>(Result.SysUserRoles);
    //            Return.Token = Result.Token;
    //            Return.UserApp = JsonConvert.DeserializeObject<List<vwUserApp>>(Result.UserApp);
    //            Return.UserPrincipalName = Result.UserPrincipalName;
    //            List<UserProject> userProjects = JsonConvert.DeserializeObject<List<UserProject>>(Result.UserProject);

    //            List<UserProjectType> userProjectTypes = new List<UserProjectType>();
    //            for (int i = 0; i < userProjects.Count(); i++)
    //            {
    //                ICONEntFormsProduct Prd = _masterRepo.GetProductDataFromCRM_Sync(userProjects[i].ProjectCode);
    //                string obj = JsonConvert.SerializeObject(userProjects[i]);
    //                UserProjectType ProductObj = JsonConvert.DeserializeObject<UserProjectType>(obj);
    //                if (Prd != null)
    //                {
    //                    if (Prd.Producttype == "โครงการแนวราบ")
    //                    {
    //                        ProductObj.producttypecate = "H";
    //                    }
    //                    if (Prd.Producttype == "โครงการแนวสูง")
    //                    {
    //                        ProductObj.producttypecate = "V";
    //                    }
    //                }

    //                userProjectTypes.Add(ProductObj);
    //            }

    //            Return.UserProject = userProjectTypes;
    //            if (Result.LoginResult == false)
    //            {
    //                return new
    //                {
    //                    success = false,
    //                    data = Result.LoginResultMessage,
    //                    valid = false
    //                };
    //            }
    //            AccessKeyControl AC = _UserRepository.GetUserAccessKey(Result.EmployeeID);
    //            if (AC == null)
    //            {
    //                AccessKeyControl accessKeyControl = new AccessKeyControl();
    //                accessKeyControl.EmpCode = Result.EmployeeID;
    //                accessKeyControl.AccessKey = generateAccessKey(Result.EmployeeID);
    //                accessKeyControl.LoginDate = DateTime.Now;

    //                bool Insert = _UserRepository.InsertUserAccessKey(accessKeyControl);

    //                return new
    //                {
    //                    success = true,
    //                    data = Return,
    //                    AccessKey = accessKeyControl.AccessKey,
    //                    valid = false
    //                };
    //            }
    //            else
    //            {
    //                AC.AccessKey = generateAccessKey(Result.EmployeeID);
    //                AC.LoginDate = DateTime.Now;

    //                bool Update = _UserRepository.UpdateUserAccessKey(AC);

    //                return new
    //                {
    //                    success = true,
    //                    data = Return,
    //                    AccessKey = AC.AccessKey,
    //                    valid = false
    //                };
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            return StatusCode(500, "Internal server error :: " + ex.Message);
    //        }
    //    }

        [HttpPost]
        [Route("UserNotiHistroies")]
        [SwaggerOperation(Summary = "เปลี่ยนภาษาของบุคคลนั้นๆ",
       Description = "เปลี่ยนภาษาของบุคคลนั้นๆ")]
        public async Task<object> UserNotiHistroies([FromBody]UserNotiHistoriesParam data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;

                //if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
                //{
                //    string AccessKey = api_key.First();
                //    string EmpCodeKey = EmpCode.First();

                //    if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                //    {
                //        return new
                //        {
                //            success = false,
                //            data = new AutorizeDataJWT(),
                //            message = "Require Key to Access the Function"
                //        };
                //    }
                //    else
                //    {
                //        string APApiKey = Environment.GetEnvironmentVariable("API_Key");
                //        if (APApiKey == null)
                //        {
                //            APApiKey = UtilsProvider.AppSetting.ApiKey;
                //        }
                //        if (api_key != APApiKey)
                //        {
                //            return new
                //            {
                //                success = false,
                //                data = new AutorizeDataJWT(),
                //                message = "Incorrect API KEY !!"
                //            };
                //        }
                //    }
                //}
                List<Model.CRMMobile.NotificationHistory> notificationHistories = _UserRepository.GetUserNotificationHistoryByCRMContactID_Mobile(data.CRMContactID);

                return new
                {
                    success = true,
                    data = notificationHistories,
                    message = "Get UserNotiHistroies Success !"
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("Notification")]
        [SwaggerOperation(Summary = "Register User เพื่อใช่ระบบ ซึ่งจะไป หาข้อมูลจากระบบ CRM",
       Description = "Access Key ใช้ในการเรียหใช้ Function ต่างๆ เพื่อไม่ให้ User Login หลายเครื่องในเวลาเดียวกัน")]
        public async Task<object> OnOffNotification([FromBody]OnOffNotification data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;

                //Model.CRMWeb.Contact cRMContact = _UserRepository.GetCRMContactByIDCardNO(data.CitizenIdentityNo);
                //if (cRMContact == null)
                //{
                //    return new
                //    {
                //        success = false,
                //        data = new AutorizeDataJWT(),
                //        message = "Only AP Customer Can Regist to the System !!"
                //    };
                //}
                VerifyPINReturnObj cSUserProfile = _UserRepository.GetUserLogin_Mobile(data.AccessKey);
                if (cSUserProfile == null)
                {
                    return new
                    {
                        success = false,
                        data = new VerifyPINReturnObj(),
                        message = "Cannot Find the Matach Data"
                    };
                }
                else
                {
                    if (data.IsOn == true)
                    {
                        Model.CRMMobile.UserLogin userLogin = _UserRepository.GetUserLoginByID_Mobile(cSUserProfile.UserLoginID);
                        //string GenerateAccessToken = SHAHelper.ComputeHash(data.DeviceID, "SHA512", null);
                        //userLogin.FireBaseToken = data.FireBaseToken;
                        userLogin.Notification = true;
                        bool UpdateUserToken = _UserRepository.UpdateCSUserLogin(userLogin);

                        return new
                        {
                            success = true,
                            data = cSUserProfile,
                            message = "PIN Correct!"
                        };
                    }
                    else
                    {
                        Model.CRMMobile.UserLogin userLogin = _UserRepository.GetUserLoginByID_Mobile(cSUserProfile.UserLoginID);
                        //string GenerateAccessToken = SHAHelper.ComputeHash(data.DeviceID, "SHA512", null);
                        userLogin.Notification = false;
                        bool UpdateUserToken = _UserRepository.UpdateCSUserLogin(userLogin);

                        return new
                        {
                            success = true,
                            data = cSUserProfile,
                            message = "PIN Correct!"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("SetNotiIsRead")]
        [SwaggerOperation(Summary = "Register User เพื่อใช่ระบบ ซึ่งจะไป หาข้อมูลจากระบบ CRM",
       Description = "Access Key ใช้ในการเรียหใช้ Function ต่างๆ เพื่อไม่ให้ User Login หลายเครื่องในเวลาเดียวกัน")]
        public async Task<object> SetNotiIsRead([FromBody]SetNotiIsReadParam data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;

                //Model.CRMWeb.Contact cRMContact = _UserRepository.GetCRMContactByIDCardNO(data.CitizenIdentityNo);
                //if (cRMContact == null)
                //{
                //    return new
                //    {
                //        success = false,
                //        data = new AutorizeDataJWT(),
                //        message = "Only AP Customer Can Regist to the System !!"
                //    };
                //}
                VerifyPINReturnObj cSUserProfile = _UserRepository.GetUserLogin_Mobile(data.AccessKey);
                if (cSUserProfile == null)
                {
                    return new
                    {
                        success = false,
                        data = new VerifyPINReturnObj(),
                        message = "Cannot Find the Matach Data"
                    };

                }
                else
                {
                    Model.CRMMobile.NotificationHistory notification = _UserRepository.GetUserNotificationHistoryByNotiHistoryID_Mobile(data.NotiHistoryID);
                    notification.IsRead = true;
                    bool updateIsRead = _UserRepository.UpdateIsReadForNotification(notification);
                    return new
                    {
                        success = true,
                        data = new VerifyPINReturnObj(),
                        message = "Set Flag IsRead For Notification Success!"
                    };
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("GetUserRecieptByRecieptNo")]
        [SwaggerOperation(Summary = "Register User เพื่อใช่ระบบ ซึ่งจะไป หาข้อมูลจากระบบ CRM",
       Description = "Access Key ใช้ในการเรียหใช้ Function ต่างๆ เพื่อไม่ให้ User Login หลายเครื่องในเวลาเดียวกัน")]
        public async Task<object> GetUserRecieptByRecieptNo([FromBody]GetReceiptByReceiptID data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;

               
                VerifyPINReturnObj cSUserProfile = _UserRepository.GetUserLogin_Mobile(data.AccessKey);
                if (cSUserProfile == null)
                {
                    return new
                    {
                        success = false,
                        data = new VerifyPINReturnObj(),
                        message = "Cannot Find the User Matach Data"
                    };
                }
                else
                {
                    string Url = "";
                    bool Exist = false;
                    GetGetReceiptInfoReturnObj result = new GetGetReceiptInfoReturnObj();
                    if (data.IsTemp == false)
                    {
                        //Url = await GetFileUrlAsync("erecipt", data.ProjectCode, data.ReceiptNo);
                        List<string> bucketList = await _UserRepository.GetListFile("erecipt", data.ProjectCode + "/");
                        bool FileExist = bucketList.Contains(data.ProjectCode + "/" + data.ReceiptNo + ".pdf");
                        if (FileExist == true)
                        {
                            Url = await _UserRepository.GetFileUrlAsync("erecipt", data.ProjectCode, data.ProjectCode + "/" + data.ReceiptNo + ".pdf");
                        }
                        else
                        {
                            Url = "";
                        }
                        result = _UserRepository.GetReceiptInfoByReceiptNo(data.ReceiptNo);
                    }
                    else
                    {
                        List<string> bucketList = await _UserRepository.GetListFile("ereceipt-temp", data.ProjectCode + "/");
                        bool FileExist = bucketList.Contains(data.ProjectCode + "/" + data.ReceiptNo + ".pdf");
                        if (FileExist == true)
                        {
                            Url = await _UserRepository.GetFileUrlAsync("ereceipt-temp", data.ProjectCode, data.ProjectCode + "/" + data.ReceiptNo + ".pdf");
                        }
                        else
                        {
                            Url = "";
                        }
                        result = _UserRepository.GetReceiptTempInfoByReceiptNo(data.ReceiptNo);
                    }
                    result.URL = Url;
                    //Model.CRMMobile.NotificationHistory notification = _UserRepository.GetUserNotificationHistoryByNotiHistoryID_Mobile(data.NotiHistoryID);
                    //notification.IsRead = true;
                    //bool updateIsRead = _UserRepository.UpdateIsReadForNotification(notification);
                    return new
                    {
                        success = true,
                        data = result,
                        message = "Set Flag IsRead For Notification Success!"
                    };
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("GetUserRecieptListByRecieptNo")]
        [SwaggerOperation(Summary = "Register User เพื่อใช่ระบบ ซึ่งจะไป หาข้อมูลจากระบบ CRM",
       Description = "Access Key ใช้ในการเรียหใช้ Function ต่างๆ เพื่อไม่ให้ User Login หลายเครื่องในเวลาเดียวกัน")]
        public async Task<object> GetUserRecieptListByRecieptNo([FromBody]GetReceiptListByReceiptID data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;


                VerifyPINReturnObj cSUserProfile = _UserRepository.GetUserLogin_Mobile(data.getReceiptListByReceiptIDs[0].AccessKey);
                if (cSUserProfile == null)
                {
                    return new
                    {
                        success = false,
                        data = new VerifyPINReturnObj(),
                        message = "Cannot Find the User Matach Data"
                    };
                }
                else
                {
                    List<GetGetReceiptInfoReturnObj> FinalResult = new List<GetGetReceiptInfoReturnObj>();
                    for (int i = 0; i < data.getReceiptListByReceiptIDs.Count(); i++)
                    {
                        string Url = "";
                        bool Exist = false;
                        GetGetReceiptInfoReturnObj result = new GetGetReceiptInfoReturnObj();
                        if (data.getReceiptListByReceiptIDs[i].IsTemp == false)
                        {
                            //Url = await GetFileUrlAsync("erecipt", data.ProjectCode, data.ReceiptNo);
                            List<string> bucketList = await _UserRepository.GetListFile("erecipt", data.getReceiptListByReceiptIDs[i].ProjectCode + "/");
                            bool FileExist = bucketList.Contains(data.getReceiptListByReceiptIDs[i].ProjectCode + "/" + data.getReceiptListByReceiptIDs[i].ReceiptNo + ".pdf");
                            if (FileExist == true)
                            {
                                Url = await _UserRepository.GetFileUrlAsync("erecipt", data.getReceiptListByReceiptIDs[i].ProjectCode, data.getReceiptListByReceiptIDs[i].ProjectCode + "/" + data.getReceiptListByReceiptIDs[i].ReceiptNo + ".pdf");
                            }
                            else
                            {
                                Url = "";
                            }
                            result = _UserRepository.GetReceiptInfoByReceiptNo(data.getReceiptListByReceiptIDs[i].ReceiptNo);
                        }
                        else
                        {
                            List<string> bucketList = await _UserRepository.GetListFile("ereceipt-temp", data.getReceiptListByReceiptIDs[i].ProjectCode + "/");
                            bool FileExist = bucketList.Contains(data.getReceiptListByReceiptIDs[i].ProjectCode + "/" + data.getReceiptListByReceiptIDs[i].ReceiptNo + ".pdf");
                            if (FileExist == true)
                            {
                                Url = await _UserRepository.GetFileUrlAsync("ereceipt-temp", data.getReceiptListByReceiptIDs[i].ProjectCode, data.getReceiptListByReceiptIDs[i].ProjectCode + "/" + data.getReceiptListByReceiptIDs[i].ReceiptNo + ".pdf");
                            }
                            else
                            {
                                Url = "";
                            }
                            result = _UserRepository.GetReceiptTempInfoByReceiptNo(data.getReceiptListByReceiptIDs[i].ReceiptNo);
                        }
                        if (result.ReceiptTempNo != null)
                        {
                            result.ReceiptNo = result.ReceiptTempNo;
                            result.IsTemp = true;
                        }
                        else
                        {
                            result.IsTemp = false;
                        }
                        result.URL = Url;
                        //Model.CRMMobile.NotificationHistory notification = _UserRepository.GetUserNotificationHistoryByNotiHistoryID_Mobile(data.NotiHistoryID);
                        //notification.IsRead = true;
                        //bool updateIsRead = _UserRepository.UpdateIsReadForNotification(notification);
                        FinalResult.Add(result);
                    }
                    return new
                    {
                        success = true,
                        data = FinalResult,
                        message = "Set Flag IsRead For Notification Success!"
                    };
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("GetFET")]
        [SwaggerOperation(Summary = "Register User เพื่อใช่ระบบ ซึ่งจะไป หาข้อมูลจากระบบ CRM",
       Description = "Access Key ใช้ในการเรียหใช้ Function ต่างๆ เพื่อไม่ให้ User Login หลายเครื่องในเวลาเดียวกัน")]
        public async Task<object> GetFET([FromBody]GetFETByFETID data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;


                VerifyPINReturnObj cSUserProfile = _UserRepository.GetUserLogin_Mobile(data.AccessKey);
                if (cSUserProfile == null)
                {
                    return new
                    {
                        success = false,
                        data = new VerifyPINReturnObj(),
                        message = "Cannot Find the User Matach Data"
                    };
                }
                else
                {
                    var FET = _UserRepository.GetUserFETDataByPaymentMethodID(data.PaymentMethodID);
                    if (FET == null)
                    {
                        return new
                        {
                            success = true,
                            //data = Url,
                            message = "Cannot Find FET Data.!"
                        };
                    }
                    string Url = "";
                    
                    Url = await _UserRepository.GetFETFileUrlAsync("finances", FET.AttachFileUrl + "/" + FET.AttachFileName);
                   
                    return new
                    {
                        success = true,
                        data = Url,
                        message = "Set Flag IsRead For Notification Success!"
                    };
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("GetFETList")]
        [SwaggerOperation(Summary = "Register User เพื่อใช่ระบบ ซึ่งจะไป หาข้อมูลจากระบบ CRM",
Description = "Access Key ใช้ในการเรียหใช้ Function ต่างๆ เพื่อไม่ให้ User Login หลายเครื่องในเวลาเดียวกัน")]
        public async Task<object> GetFETList([FromBody]GetFETByFETIDList data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;


                VerifyPINReturnObj cSUserProfile = _UserRepository.GetUserLogin_Mobile(data.AccessKey);
                if (cSUserProfile == null)
                {
                    return new
                    {
                        success = false,
                        data = new VerifyPINReturnObj(),
                        message = "Cannot Find the User Matach Data"
                    };
                }
                else
                {
                    List<FetListResult> Result = new List<FetListResult>();
                    for (int i = 0; i < data.PaymentMethodID.Count(); i++)
                    {
                        FetListResult datas = new FetListResult();
                        var FET = _UserRepository.GetUserFETDataByPaymentMethodID(data.PaymentMethodID[i]);
                        if (FET == null)
                        {
                            datas.PaymentMethodID = data.PaymentMethodID[i].ToString();
                            datas.Url = null;
                            Result.Add(datas);
                        }
                        else
                        {
                            string Url = "";
                            Url = await _UserRepository.GetFETFileUrlAsync("finances", FET.AttachFileUrl + "/" + FET.AttachFileName);
                            datas.PaymentMethodID = data.PaymentMethodID[i].ToString();
                            datas.Url = Url;
                            Result.Add(datas);
                        }
                    }

                    return new
                    {
                        success = true,
                        data = Result,
                        message = "Set Flag IsRead For Notification Success!"
                    };
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("GetUserBillingTracking")]
        [SwaggerOperation(Summary = "เรียกดูเบอร์โทรศัพท์ของลูกค้าจากระบบ CRM ทั้งหมด",
Description = "Access Key ใช้ในการเรียหใช้ Function ถึงจะเรียกใช้ Function ได้")]
        public async Task<object> GetUserBillingTracking([FromBody]GetUserBillingTrackingParam data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;
                //if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
                //{
                //    string AccessKey = api_key.First();
                //    string EmpCodeKey = EmpCode.First();

                //    if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                //    {
                //        return new
                //        {
                //            success = false,
                //            data = new AutorizeDataJWT(),
                //            message = "Require Key to Access the Function"
                //        };
                //    }
                //    else
                //    {
                //        string APApiKey = Environment.GetEnvironmentVariable("API_Key");
                //        if (APApiKey == null)
                //        {
                //            APApiKey = UtilsProvider.AppSetting.ApiKey;
                //        }
                //        if (api_key != APApiKey)
                //        {
                //            return new
                //            {
                //                success = false,
                //                data = new AutorizeDataJWT(),
                //                message = "Incorrect API KEY !!"
                //            };
                //        }
                //    }
                //}

                //Model.CRMWeb.Contact contact = _UserRepository.GetCRMContactByID(data.UserID);
                //if (contact == null)
                //{
                //    return new
                //    {
                //        success = false,
                //        data = new AutorizeDataJWT(),
                //        message = "Only AP Customer Can Regist to the System !!"
                //    };
                //}

                List<GetBillingTrackingMobile> getBilling = _UserRepository.GetUserBillingTrackingByProjectandUnit(data.Project, data.Unit);
                if (getBilling.Count == null)
                {
                    return new
                    {
                        success = false,
                        data = new AutorizeDataJWT(),
                        message = "There is no Assosiate Phone Number with this IDCard Number!!"
                    };
                }
                BillingFinalTrackingGroup FinalList = new BillingFinalTrackingGroup();
                FinalList.BookingList = new List<BillingTrackingGroup>();
                FinalList.ContractList = new List<BillingTrackingGroup>();
                FinalList.TransferList = new List<BillingTrackingGroup>();
                FinalList.DownpayList = new List<BillingTrackingGroup>();
                List<GetBillingTrackingMobile> TempForDelete = new List<GetBillingTrackingMobile>();
                BillingTrackingGroup ContactGroup = new BillingTrackingGroup();
                ContactGroup.GetBillingTrackingMobile = new List<GetBillingTrackingMobile>();
                BillingTrackingGroup BookingGroup = new BillingTrackingGroup();
                BookingGroup.GetBillingTrackingMobile = new List<GetBillingTrackingMobile>();
                List<GetBillingTrackingMobile> BookingList = new List<GetBillingTrackingMobile>();
                List<GetBillingTrackingMobile> ContactList = new List<GetBillingTrackingMobile>();
                List<GetBillingTrackingMobile> TransferList = new List<GetBillingTrackingMobile>();
                BillingTrackingGroup TransferGroup = new BillingTrackingGroup();
                TransferGroup.GetBillingTrackingMobile = new List<GetBillingTrackingMobile>();

                for (int i = 0; i < getBilling.Count(); i++)
                {
                    if (getBilling[i].UnitPriceStage == 1 && getBilling[i].FlagBooking != null) // เงินจอง
                    {
                        bool HaveFET = _UserRepository.GetUserFETByPaymentMethodID(getBilling[i].BookingPaymentID);
                        getBilling[i].HaveFET = HaveFET;
                        BookingGroup.GetBillingTrackingMobile.Add(getBilling[i]);
                        BookingList.Add(getBilling[i]);
                        BookingGroup.DetailDownPayment = Convert.ToInt32(getBilling[i].DetailDownPayment);
                        BookingGroup.IsOverDue = getBilling[i].FlagOverDue == "Y" ? true : false;
                        BookingGroup.PaymentAmount = Convert.ToDouble(getBilling[i].BookingAmount);
                        BookingGroup.PaymentDueDate = getBilling[i].PaymentDueDate;
                        //--------------------------
                        BookingGroup.DownPerInstallment = getBilling[i].DownPerInstallment;
                        BookingGroup.NormalDownPerInstallment = getBilling[i].NormalDownPerInstallment;
                        BookingGroup.SpecialDownPaymentFlag = getBilling[i].SpecialDownPaymentFlag;
                        BookingGroup.SpecialDownPerInstallment = getBilling[i].SpecialDownPerInstallment;
                        BookingGroup.AgreementAmount = getBilling[i].AgreementAmount;
                        BookingGroup.BookingAmount = getBilling[i].BookingAmount;
                        BookingGroup.BookingPaymentDate = getBilling[i].BookingPaymentDate;
                        BookingGroup.FlagAgreement = getBilling[i].FlagAgreement;
                        BookingGroup.FlagAgreementReceipt = getBilling[i].FlagAgreementReceipt;
                        BookingGroup.FlagBooking = getBilling[i].FlagBooking;
                        BookingGroup.FlagBookingReceipt = getBilling[i].FlagBookingReceipt;
                        BookingGroup.FlagOverDue = getBilling[i].FlagOverDue;
                        BookingGroup.FlagReceipt = getBilling[i].FlagReceipt;
                        BookingGroup.PayAgreementAmount = getBilling[i].PayAgreementAmount;
                        BookingGroup.SpecialDownPaymentFlag = getBilling[i].SpecialDownPaymentFlag;
                        BookingGroup.SpecialDownPerInstallment = getBilling[i].SpecialDownPerInstallment;
                        BookingGroup.PayRemain = Convert.ToDouble(getBilling[i].AmountBalance);
                        BookingGroup.PaymentItemNameTH = getBilling[i].PaymentItemNameTH;
                        BookingGroup.PaymentItemNameEN = getBilling[i].PaymentItemNameEN;
                        BookingGroup.PayRemain = Convert.ToDouble(getBilling[i].AmountBalance);
                        //if (BookingGroup.PayRemain == 0)
                        //{
                        //    BookingGroup.PayRemain = Convert.ToDouble(getBilling[i].BookingAmount) - Convert.ToDouble(getBilling[i].PayBookingAmount);
                        //}
                        //else
                        //{
                        //    BookingGroup.PayRemain = BookingGroup.PayRemain - Convert.ToDouble(getBilling[i].PayBookingAmount);
                        //}
                        //FinalList.BookingList.Add(BookingGroup);
                        TempForDelete.Add(getBilling[i]);
                    }
                    else if (getBilling[i].UnitPriceStage == 2 && getBilling[i].FlagAgreement != null) //สัญญา
                    {
                        ContactGroup.GetBillingTrackingMobile.Add(getBilling[i]);
                        bool HaveFET = _UserRepository.GetUserFETByPaymentMethodID(getBilling[i].AgreementPaymentID);
                        getBilling[i].HaveFET = HaveFET;
                        ContactList.Add(getBilling[i]);
                        ContactGroup.DetailDownPayment = Convert.ToInt32(getBilling[i].DetailDownPayment);
                        ContactGroup.IsOverDue = getBilling[i].FlagOverDue == "Y" ? true : false;
                        ContactGroup.PaymentAmount = Convert.ToDouble(getBilling[i].BookingAmount);
                        ContactGroup.PaymentDueDate = getBilling[i].PaymentDueDate;
                        //--------------------------
                        ContactGroup.DownPerInstallment = getBilling[i].DownPerInstallment;
                        ContactGroup.NormalDownPerInstallment = getBilling[i].NormalDownPerInstallment;
                        ContactGroup.SpecialDownPaymentFlag = getBilling[i].SpecialDownPaymentFlag;
                        ContactGroup.SpecialDownPerInstallment = getBilling[i].SpecialDownPerInstallment;
                        ContactGroup.AgreementAmount = getBilling[i].AgreementAmount;
                        ContactGroup.BookingAmount = getBilling[i].BookingAmount;
                        ContactGroup.BookingPaymentDate = getBilling[i].BookingPaymentDate;
                        ContactGroup.FlagAgreement = getBilling[i].FlagAgreement;
                        ContactGroup.FlagAgreementReceipt = getBilling[i].FlagAgreementReceipt;
                        ContactGroup.FlagBooking = getBilling[i].FlagBooking;
                        ContactGroup.FlagBookingReceipt = getBilling[i].FlagBookingReceipt;
                        ContactGroup.FlagOverDue = getBilling[i].FlagOverDue;
                        ContactGroup.FlagReceipt = getBilling[i].FlagReceipt;
                        ContactGroup.PayAgreementAmount = getBilling[i].PayAgreementAmount;
                        ContactGroup.SpecialDownPaymentFlag = getBilling[i].SpecialDownPaymentFlag;
                        ContactGroup.SpecialDownPerInstallment = getBilling[i].SpecialDownPerInstallment;
                        ContactGroup.PayRemain = Convert.ToDouble(getBilling[i].AmountBalance);
                        ContactGroup.PaymentItemNameTH = getBilling[i].PaymentItemNameTH;
                        ContactGroup.PaymentItemNameEN = getBilling[i].PaymentItemNameEN;
                        //if (ContactGroup.PayRemain == 0)
                        //{
                        //    ContactGroup.PayRemain = Convert.ToDouble(getBilling[i].AgreementAmount) - Convert.ToDouble(getBilling[i].PayAgreementAmount);
                        //}
                        //else
                        //{
                        //    ContactGroup.PayRemain = BookingGroup.PayRemain - Convert.ToDouble(getBilling[i].PayAgreementAmount);
                        //}
                        //FinalList.ContractList.Add(ContactGroup);
                        TempForDelete.Add(getBilling[i]);
                    }
                    else if (getBilling[i].UnitPriceStage == 5 && getBilling[i].FlagTransfer != null)//โอน
                    {
                        TransferGroup.GetBillingTrackingMobile.Add(getBilling[i]);
                        ContactGroup.GetBillingTrackingMobile.Add(getBilling[i]);
                        bool HaveFET = _UserRepository.GetUserFETByPaymentMethodID(getBilling[i].TransferPaymentID);
                        TransferList.Add(getBilling[i]);
                        TransferGroup.DetailDownPayment = Convert.ToInt32(getBilling[i].DetailDownPayment);
                        TransferGroup.IsOverDue = getBilling[i].FlagOverDue == "Y" ? true : false;
                        TransferGroup.PaymentAmount = Convert.ToDouble(getBilling[i].BookingAmount);
                        TransferGroup.PaymentDueDate = getBilling[i].PaymentDueDate;
                        //--------------------------
                        TransferGroup.DownPerInstallment = getBilling[i].DownPerInstallment;
                        TransferGroup.NormalDownPerInstallment = getBilling[i].NormalDownPerInstallment;
                        TransferGroup.SpecialDownPaymentFlag = getBilling[i].SpecialDownPaymentFlag;
                        TransferGroup.SpecialDownPerInstallment = getBilling[i].SpecialDownPerInstallment;
                        TransferGroup.AgreementAmount = getBilling[i].AgreementAmount;
                        TransferGroup.BookingAmount = getBilling[i].BookingAmount;
                        TransferGroup.BookingPaymentDate = getBilling[i].BookingPaymentDate;
                        TransferGroup.FlagTransfer = getBilling[i].FlagTransfer;
                        TransferGroup.TransferAmount = getBilling[i].TransferAmount;
                        TransferGroup.TransferPaymentDate = getBilling[i].TransferPaymentDate;
                        TransferGroup.FlagTransferReceipt = getBilling[i].FlagTransferReceipt;
                        TransferGroup.FlagAgreement = getBilling[i].FlagAgreement;
                        TransferGroup.FlagAgreementReceipt = getBilling[i].FlagAgreementReceipt;
                        TransferGroup.FlagBooking = getBilling[i].FlagBooking;
                        TransferGroup.FlagBookingReceipt = getBilling[i].FlagBookingReceipt;
                        TransferGroup.FlagOverDue = getBilling[i].FlagOverDue;
                        TransferGroup.FlagReceipt = getBilling[i].FlagReceipt;
                        TransferGroup.PayAgreementAmount = getBilling[i].PayAgreementAmount;
                        TransferGroup.SpecialDownPaymentFlag = getBilling[i].SpecialDownPaymentFlag;
                        TransferGroup.SpecialDownPerInstallment = getBilling[i].SpecialDownPerInstallment;
                        TransferGroup.PayRemain = Convert.ToDouble(getBilling[i].AmountBalance);
                        TransferGroup.PaymentItemNameTH = getBilling[i].PaymentItemNameTH;
                        TransferGroup.PaymentItemNameEN = getBilling[i].PaymentItemNameEN;
                        //if (ContactGroup.PayRemain == 0)
                        //{
                        //    ContactGroup.PayRemain = Convert.ToDouble(getBilling[i].TransferAmount) - Convert.ToDouble(getBilling[i].PayTransferAmount);
                        //}
                        //else
                        //{
                        //    ContactGroup.PayRemain = BookingGroup.PayRemain - Convert.ToDouble(getBilling[i].PayTransferAmount);
                        //}
                        //FinalList.TransferList.Add(TransferGroup);
                        TempForDelete.Add(getBilling[i]);
                    }
                }
                FinalList.BookingList.Add(BookingGroup);
                FinalList.ContractList.Add(ContactGroup);
                FinalList.TransferList.Add(TransferGroup);
                for (int i = 0; i < TempForDelete.Count(); i++)
                {
                    getBilling.Remove(TempForDelete[i]);
                }
                // Billing Tracking Group Object Creation
                //BillingTrackingGroup BillingGroup = new BillingTrackingGroup();
               
                //FinalList.BookingList.Add(BillingGroup);
                //-------------
                List<GetBillingTrackingMobile> DistinctList = getBilling.DistinctBy(p => p.DetailDownPayment).ToList();
                List<BillingTrackingGroup> GroupList = new List<BillingTrackingGroup>();
                int DownPay = 1;
                for (int i = 0; i < DistinctList.Count(); i++)
                {
                    double Balance = 0;
                    List<GetBillingTrackingMobile> BillingGroup = getBilling.Where(S => S.DetailDownPayment == DownPay.ToString()).ToList();
                    BillingTrackingGroup Group = new BillingTrackingGroup();
                    Group.GetBillingTrackingMobile = new List<GetBillingTrackingMobile>();
                    for (int ii = 0; ii < BillingGroup.Count(); ii++)
                    {
                        if (BillingGroup[ii].UnitPriceStageName.Trim() == "จอง")
                        {
                            //FinalList.bookingList.Add(BillingGroup[ii]);
                        }
                        else if (BillingGroup[ii].UnitPriceStageName.Trim() == "สัญญา")
                        {
                            //FinalList.ContractList.Add(BillingGroup[ii]);
                        }
                        else
                        {
                            Group.GetBillingTrackingMobile.Add(BillingGroup[ii]);
                            ContactGroup.GetBillingTrackingMobile.Add(getBilling[i]);
                            bool HaveFET = _UserRepository.GetUserFETByPaymentMethodID(getBilling[i].BookingPaymentID);
                            Group.DetailDownPayment = DownPay;
                            Group.IsOverDue = BillingGroup[ii].FlagOverDue == "Y" ? true : false;
                            Group.PaymentAmount = Convert.ToDouble(BillingGroup[ii].BookingAmount);
                            Group.PaymentDueDate = BillingGroup[ii].PaymentDueDate;
                            //--------------------------
                            Group.DownPerInstallment = BillingGroup[ii].DownPerInstallment;
                            Group.NormalDownPerInstallment = BillingGroup[ii].NormalDownPerInstallment;
                            Group.SpecialDownPaymentFlag = BillingGroup[ii].SpecialDownPaymentFlag;
                            Group.SpecialDownPerInstallment = BillingGroup[ii].SpecialDownPerInstallment;
                            Group.AgreementAmount = BillingGroup[ii].AgreementAmount;
                            Group.BookingAmount = BillingGroup[ii].BookingAmount;
                            Group.BookingPaymentDate = BillingGroup[ii].BookingPaymentDate;
                            Group.FlagAgreement = BillingGroup[ii].FlagAgreement;
                            Group.FlagAgreementReceipt = BillingGroup[ii].FlagAgreementReceipt;
                            Group.FlagBooking = BillingGroup[ii].FlagBooking;
                            Group.FlagBookingReceipt = BillingGroup[ii].FlagBookingReceipt;
                            Group.FlagOverDue = BillingGroup[ii].FlagOverDue;
                            Group.FlagReceipt = BillingGroup[ii].FlagReceipt;
                            Group.PayAgreementAmount = BillingGroup[ii].PayAgreementAmount;
                            Group.SpecialDownPaymentFlag = BillingGroup[ii].SpecialDownPaymentFlag;
                            Group.SpecialDownPerInstallment = BillingGroup[ii].SpecialDownPerInstallment;
                            Group.PayRemain = Convert.ToDouble(BillingGroup[ii].AmountBalance);
                            Group.PaymentItemNameTH = BillingGroup[ii].PaymentItemNameTH;
                            Group.PaymentItemNameEN = BillingGroup[ii].PaymentItemNameEN;
                            //if (Group.PayRemain == 0)
                            //{
                            //    Group.PayRemain = Convert.ToDouble(BillingGroup[ii].DownPerInstallment) - Convert.ToDouble(BillingGroup[ii].AmountPaid);
                            //}
                            //else
                            //{
                            //    Group.PayRemain = Group.PayRemain - Convert.ToDouble(BillingGroup[ii].AmountPaid);
                            //}
                        }
                    }
                    GroupList.Add(Group);
                    DownPay++;
                }
                FinalList.BillingTrackingGroup = GroupList;
                if (FinalList.BillingTrackingGroup == null || FinalList.BillingTrackingGroup.Count() == 0)
                {
                    FinalList.BillingTrackingGroup = null;
                }
                if (FinalList.BookingList == null || FinalList.BookingList.Count() == 0)
                {
                    FinalList.BookingList = null;
                }
                if (FinalList.ContractList == null || FinalList.ContractList.Count() == 0)
                {
                    FinalList.ContractList = null;
                }
                if (FinalList.TransferList == null || FinalList.TransferList.Count() == 0)
                {
                    FinalList.TransferList = null;
                }
                if (FinalList.DownpayList == null || FinalList.DownpayList.Count() == 0)
                {
                    FinalList.DownpayList = null;
                }
                return new
                {
                    success = true,
                    data = FinalList,
                    message = "Get User iBooking Success !"
                };

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        // [HttpPost]
        // [Route("GetUserFET")]
        // [SwaggerOperation(Summary = "Register User เพื่อใช่ระบบ ซึ่งจะไป หาข้อมูลจากระบบ CRM",
        //Description = "Access Key ใช้ในการเรียหใช้ Function ต่างๆ เพื่อไม่ให้ User Login หลายเครื่องในเวลาเดียวกัน")]
        // public async Task<FileDTO> GetAttachFile(Guid? fetID, Guid? paymentID)
        // {
        //     var FETModel = new FET();
        //     var id = Guid.Empty;

        //     if (fetID.HasValue)
        //     {
        //         id = fetID ?? Guid.Empty;

        //         FETModel = await DB.FETs.IgnoreQueryFilters().Where(o => o.ID == id).FirstOrDefaultAsync() ?? new FET();
        //     }
        //     else if (paymentID.HasValue)
        //     {
        //         id = paymentID ?? Guid.Empty;

        //         FETModel = await DB.FETs
        //                 .Include(o => o.PaymentMethod)
        //                     .ThenInclude(o => o.Payment)
        //                .Where(o => o.PaymentMethod.PaymentID == paymentID)
        //             .FirstOrDefaultAsync() ?? new FET();
        //     }

        //     if (!string.IsNullOrEmpty(FETModel.AttachFileUrl) && !string.IsNullOrEmpty(FETModel.AttachFileName))
        //     {
        //         var minioBucket = FETModel.AttachFileUrl;
        //         var minioFileName = FETModel.AttachFileName;

        //         //Base.DTOs.Extensions.ConvertToMinIOFileParam(ref minioBucket, ref minioFileName);

        //         //string GeUrl = await FileHelper.GetFileUrlAsync(minioBucket, minioFileName) ?? "";
        //         string tmpFileBucket = minioBucket + "/" + minioFileName;
        //         string GeUrl = await FileHelper.GetFileUrlAsync(tmpFileBucket);
        //         return new FileDTO()
        //         {
        //             Name = minioFileName,
        //             Url = GeUrl,
        //             IsTemp = !string.IsNullOrEmpty(GeUrl) ? true : false
        //         };
        //     }

        //     return new FileDTO();
        // }
        // [HttpPost]
        // [Route("UserNotiHistroies")]
        // [SwaggerOperation(Summary = "เปลี่ยนภาษาของบุคคลนั้นๆ",
        //Description = "เปลี่ยนภาษาของบุคคลนั้นๆ")]
        // public async Task<object> UserNotiHistroies([FromBody]UserNotiHistoriesParam data)
        // {
        //     try
        //     {
        //         StringValues api_key;
        //         StringValues EmpCode;

        //         //if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
        //         //{
        //         //    string AccessKey = api_key.First();
        //         //    string EmpCodeKey = EmpCode.First();

        //         //    if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
        //         //    {
        //         //        return new
        //         //        {
        //         //            success = false,
        //         //            data = new AutorizeDataJWT(),
        //         //            message = "Require Key to Access the Function"
        //         //        };
        //         //    }
        //         //    else
        //         //    {
        //         //        string APApiKey = Environment.GetEnvironmentVariable("API_Key");
        //         //        if (APApiKey == null)
        //         //        {
        //         //            APApiKey = UtilsProvider.AppSetting.ApiKey;
        //         //        }
        //         //        if (api_key != APApiKey)
        //         //        {
        //         //            return new
        //         //            {
        //         //                success = false,
        //         //                data = new AutorizeDataJWT(),
        //         //                message = "Incorrect API KEY !!"
        //         //            };
        //         //        }
        //         //    }
        //         //}
        //         List<Model.CRMMobile.NotificationHistory> notificationHistories = _UserRepository.GetUserNotificationHistoryByCRMContactID_Mobile(data.CRMContactID);

        //         return new
        //         {
        //             success = true,
        //             data = notificationHistories,
        //             message = "Get UserNotiHistroies Success !"
        //         };

        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, "Internal server error :: " + ex.Message);
        //     }
        // }lll

        [ApiExplorerSettings(IgnoreApi = true)]
        public string generateToken(string PhoneNumber)
        {
            return string.Format("{0}_{1:N}", PhoneNumber, Guid.NewGuid());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public bool VerifyHeader(out string ErrorMsg)
        {

            string ipaddress = "5555555";
            StringValues api_key;
            StringValues EmpCode;

            var isValidHeader = false;
            //APIITVendor //VendorData = new APIITVendor();
            if (Request.Headers.TryGetValue("api_Accesskey", out api_key))
            {
                string AccessKey = api_key.First();
                string EmpCodeKey = EmpCode.First();

                if (!string.IsNullOrEmpty(AccessKey))
                {
                    bool CorrectACKey = SHAHelper.VerifyHash("APiCRMMobile", "SHA512", AccessKey);
                    if (CorrectACKey)
                    {
                        ErrorMsg = "";
                        return true;
                    }
                }
            }
            else
            {
                if (!isValidHeader)
                {
                    //_log.LogDebug(ipaddress + " :: Missing Authorization Header.");
                    ErrorMsg = ipaddress + " :: Missing Authorization Header.";
                    //VendorData = new APIITVendor();
                    return false;
                    //  return BadRequest("Missing Authorization Header.");
                }
            }
            //VendorData = new APIITVendor();
            ErrorMsg = "SomeThing Wrong with Header Contact Developer ASAP";
            return false;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private static byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static String[] GetFilesFrom(String searchFolder, String[] filters, bool isRecursive)
        {
            List<String> filesFound = new List<String>();
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var filter in filters)
            {
                filesFound.AddRange(Directory.GetFiles(searchFolder, String.Format("*.{0}", filter), searchOption));
                var a = Directory.GetFiles(searchFolder, String.Format("*.{0}", filter), searchOption);
            }
            return filesFound.ToArray();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string ReplaceWithPublicURL(string url)
        {
            string _minioEndpoint = "http://192.168.2.29:9001";
            //string _tempBucket = "timeattendence";
            if (!string.IsNullOrEmpty(_publicURL))
            {
                url = url.Replace("https://", "");
                url = url.Replace("http://", "");

                url = url.Replace(_minioEndpoint, _publicURL);
            }
            return url;
        }

        private int _expireHours = 24;
        public string _publicURL;
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<string> GetFileUrlAsync(string bucket, string ReceiptNo, string name)
        {
            string _minioEndpoint = "192.168.2.29:9001"; //192.168.2.29:9001"; // CRM 
            string _minioAccessKey = "XNTYE7HIMF6KK4BVEIXA";
            string _minioSecretKey = "naD+esQ+uV7+xwfF3bPfAn5iC7C1XUyXeM8HkBlO";
            string _defaultBucket = "erecipt";
            string _tempBucket = "erecipt";
            bool _withSSL = false;
            MinioClient minio;
            if (_withSSL)
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey).WithSSL();
            else
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey);

            var url = await minio.PresignedGetObjectAsync(bucket, name, (int)TimeSpan.FromHours(_expireHours).TotalSeconds);
            //url = (!string.IsNullOrEmpty(_publicURL)) ? url.Replace(_minioEndpoint, _publicURL) : url;
            url = ReplaceWithPublicURL(url);

            return url;
        }
        //public static async Task<FileDTO> CreateFromFileNameAsync(string name, FileHelper fileHelper, IConfiguration Configuration = null)
        //{
        //    if (!string.IsNullOrEmpty(name))
        //    {
        //        var url = await fileHelper.GetFileUrlAsync(name);
        //        var publicURL = "";
        //        if (Configuration != null)
        //        {
        //            var endpoint = Configuration["Minio:Endpoint"];
        //            var publicEndpoint = Configuration["Minio:PublicURL"];

        //            publicURL = (!string.IsNullOrEmpty(publicEndpoint)) ? url.Replace(endpoint, publicEndpoint) : url;
        //        }

        //        var result = new FileDTO()
        //        {
        //            //Set Data
        //            Name = System.IO.Path.GetFileName(name),
        //            Url = url,
        //        };

        //        return result;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //public static async Task<FileDTO> CreateFromBucketandFileNameAsync(string bucket, string name, FileHelper fileHelper)
        //{
        //    if (!string.IsNullOrEmpty(name))
        //    {
        //        var url = await fileHelper.GetFileUrlAsync(bucket, name);
        //        var result = new FileDTO()
        //        {
        //            //Set Data
        //            Name = System.IO.Path.GetFileName(name),
        //            Url = url
        //        };
        //        return result;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
    }
}