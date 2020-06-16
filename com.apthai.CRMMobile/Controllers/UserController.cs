﻿using System;
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
        public UserController(IMasterRepository masterRepo , IAuthorizeService authorizeService,IUserRepository userRepository, IMobileMessagingClient mobileMessagingClient ,IHostingEnvironment hostingEnvironment)
        {

            _masterRepo = masterRepo;
            _authorizeService = authorizeService;
            _UserRepository = userRepository;
            _mobileMessagingClient = mobileMessagingClient;
            _hostingEnvironment = hostingEnvironment;
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
                Model.CRMWeb.ContactPhone contactPhone = _UserRepository.GetSingleContactPhoneNumberByContactID_Web(contact.ID.ToString(),data.PhoneNumber);
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
                    Model.CRMMobile.UserLogin userLogin = _UserRepository.GetUserLoginByPhoneNumbandDevice_Mobile(data.DeviceID, data.PhoneNumber);
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
                        userLogin.LoginDate = DateTime.Now.ToShortDateString() ;
                        CRMUserLoginWithContactID cSUserLogin = new CRMUserLoginWithContactID();
                        cSUserLogin.UserPhoneNumber = data.PhoneNumber;
                        cSUserLogin.LoginDate = userLogin.LoginDate;
                        cSUserLogin.DeviceID = userLogin.DeviceID ;
                        cSUserLogin.DeviceType = userLogin.DeviceType;
                        cSUserLogin.UserToken = GenerateAccessToken;
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
                string RequestParam = "?key=" + thaiBulkOTPRequest.key +"&secret=" + thaiBulkOTPRequest.secret + "&msisdn=" + thaiBulkOTPRequest.msisdn;
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
                   var Project = getBilling[i].Project.Split(" ");
                   string ProjectShowName = "";
                    for (int ii = 1; ii < Project.Count(); ii++)
                    {
                        if (ProjectShowName == "")
                        {
                            ProjectShowName =  Project[ii];
                        }
                        else
                        {
                            ProjectShowName = ProjectShowName + " " + Project[ii];
                        }
                    }
                    getBilling[i].ProjectShowName = ProjectShowName;
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

                List<GetBillingTrackingMobile> getBilling = _UserRepository.GetUserBillingTrackingByProjectandUnit(data.Project,data.Unit);
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
                List<GetBillingTrackingMobile> TempForDelete = new List<GetBillingTrackingMobile>();
                BillingTrackingGroup ContactGroup = new BillingTrackingGroup();
                ContactGroup.GetBillingTrackingMobile = new List<GetBillingTrackingMobile>();
                BillingTrackingGroup BookingGroup = new BillingTrackingGroup();
                BookingGroup.GetBillingTrackingMobile = new List<GetBillingTrackingMobile>();
                for (int i = 0; i < getBilling.Count(); i++)
                {
                    if (getBilling[i].UnitPriceStageName.Trim() == "จอง")
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
                        if (BookingGroup.PayRemain == 0)
                        {
                            BookingGroup.PayRemain = Convert.ToDouble(getBilling[i].BookingAmount) - Convert.ToDouble(getBilling[i].PayBookingAmount);
                        }
                        else
                        {
                            BookingGroup.PayRemain = BookingGroup.PayRemain - Convert.ToDouble(getBilling[i].PayBookingAmount);
                        }
                        FinalList.BookingList.Add(BookingGroup);
                        TempForDelete.Add(getBilling[i]);
                    }
                    else if (getBilling[i].UnitPriceStageName.Trim() == "สัญญา")
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
                        if (ContactGroup.PayRemain == 0 )
                        {
                            ContactGroup.PayRemain = Convert.ToDouble(getBilling[i].AmountBalance);
                        }
                        else
                        {
                            ContactGroup.PayRemain = BookingGroup.PayRemain - Convert.ToDouble(getBilling[i].AmountPaid);
                        }
                        FinalList.ContractList.Add(ContactGroup);
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
                            if (Group.PayRemain == 0)
                            {
                                Group.PayRemain = Convert.ToDouble(BillingGroup[ii].DownPerInstallment) - Convert.ToDouble(BillingGroup[ii].AmountPaid);
                            }
                            else
                            {
                                Group.PayRemain = Group.PayRemain - Convert.ToDouble(BillingGroup[ii].AmountPaid);
                            }
                        }
                    }
                    GroupList.Add(Group);
                    DownPay++;
                }
                FinalList.BillingTrackingGroup = GroupList;
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
                var FilePath = Path.Combine(_hostingEnvironment.WebRootPath,"Ads");
                string[] FileExt = { ".PNG", ".jpg", ".png","png" };
                var picture = GetFilesFrom(FilePath + "//", FileExt, true);
                List<AdsObject> adsObjects = new List<AdsObject>();
                List<string> MobileAdsURL = new List<string>();
                foreach (var path in picture)
                {
                    AdsObject ads = new AdsObject();
                    var newpath = path.Split("//");
                    string fileName = newpath[newpath.Count()-1];
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
                List<iCRMContact> iCRMContacts = _UserRepository.GetUseriCRMContact_Web(data.ContactNo );
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
                    data = iCRMContacts ,
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
        // }

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
    }
}