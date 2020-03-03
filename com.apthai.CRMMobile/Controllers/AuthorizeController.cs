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

namespace com.apthai.CRMMobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : BaseController
    {


        private readonly IMasterRepository _masterRepo;
        private readonly IAuthorizeService _authorizeService;
        private readonly IUserRepository _UserRepository;
        public AuthorizeController(IMasterRepository masterRepo , IAuthorizeService authorizeService,IUserRepository userRepository)
        {

            _masterRepo = masterRepo;
            _authorizeService = authorizeService;
            _UserRepository = userRepository;
        }

        [HttpPost]
        [Route("Register")]
        [SwaggerOperation(Summary = "Register User เพื่อใช่ระบบ ซึ่งจะไป หาข้อมูลจากระบบ CRM",
       Description = "Access Key ใช้ในการเรียหใช้ Function ต่างๆ เพื่อไม่ให้ User Login หลายเครื่องในเวลาเดียวกัน")]
        public async Task<object> PostLogin([FromBody] LoginData data)
        {
            try
            {
                StringValues api_key;
                StringValues EmpCode;
                if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
                {
                    string AccessKey = api_key.First();
                    string EmpCodeKey = EmpCode.First();

                    if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                    {
                        return new
                        {
                            success = false,
                            data = new AutorizeDataJWT(),
                            message = "Incorrect API KEY !!"
                        };
                    }
                }
                
                Model.CRMWeb.Contact cRMContact = _UserRepository.GetCRMContactByIDCardNO(data.CitizenIdentityNo);
                if (cRMContact == null)
                {
                    return new
                    {
                        success = false,
                        data = new AutorizeDataJWT(),
                        message = "Only AP Customer Can Regist to the System !!"
                    };
                }
                Model.CRMWeb.ContactPhone contactPhone = new Model.CRMWeb.ContactPhone();
                Model.CRMMobile.UserProfile cSUserProfile = new Model.CRMMobile.UserProfile();
                //cSUserProfile.CRMContactID = cRMContact.ContactID.ToString();

                cSUserProfile.TitleExtEN = cRMContact.TitleExtTH;
                cSUserProfile.FirstNameTH = cRMContact.FirstNameTH;
                cSUserProfile.LastNameTH = cRMContact.LastNameTH;
                cSUserProfile.Nickname = cRMContact.Nickname;
                cSUserProfile.TitleExtEN = cRMContact.TitleExtEN;
                cSUserProfile.FirstNameEN = cRMContact.FirstNameEN;
                cSUserProfile.MiddleNameEN = cRMContact.MiddleNameEN;
                cSUserProfile.LastNameEN = cRMContact.LastNameEN;
                cSUserProfile.CitizenIdentityNo = cRMContact.CitizenIdentityNo;
                cSUserProfile.Created = DateTime.Now.ToShortDateString();
                cSUserProfile.CreatedBy = "System-Register";
                cSUserProfile.Updated = null;
                cSUserProfile.UpdatedBy = null;
                cSUserProfile.IsActive = true;
                cSUserProfile.PINCode = data.PINCode;

                string APApiKey = Environment.GetEnvironmentVariable("API_Key");
                if (APApiKey == null)
                {
                    APApiKey = UtilsProvider.AppSetting.ApiKey;
                }
                string APApiToken = Environment.GetEnvironmentVariable("Api_Token");
                if (APApiToken == null)
                {
                    APApiToken = UtilsProvider.AppSetting.ApiToken;
                }

                var client = new HttpClient();
                var Content = new StringContent(JsonConvert.SerializeObject(data));
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                Content.Headers.Add("api_key", APApiKey);
                Content.Headers.Add("api_token", APApiToken);
                string PostURL = Environment.GetEnvironmentVariable("AuthenticationURL");
                if (PostURL == null)
                {
                    PostURL = UtilsProvider.AppSetting.AuthorizeURL;
                }
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
                var Result = JsonConvert.DeserializeObject<AutorizeDataJWT>(RespondData);
                if (Result.LoginResult == false)
                {
                    return new
                    {
                        success = false,
                        data = Result.LoginResultMessage,
                        valid = false
                    };
                }
                return new
                {
                    success = false,
                    data = Result.LoginResultMessage,
                    valid = false
                };
                // เป็นส่วนของการ Control Access Key ของระบบ ตัวอย่างนี้เป็นของระบบ Defect
                //AccessKeyControl AC = _UserRepository.GetUserAccessKey(Result.EmployeeID);
                //if (AC == null)
                //{
                //    AccessKeyControl accessKeyControl = new AccessKeyControl();
                //    accessKeyControl.EmpCode = Result.EmployeeID;
                //    accessKeyControl.AccessKey = generateAccessKey(Result.EmployeeID);
                //    accessKeyControl.LoginDate = DateTime.Now;

                //    bool Insert = _UserRepository.InsertUserAccessKey(accessKeyControl);

                //    return new
                //    {
                //        success = true,
                //        data = Result,
                //        AccessKey = accessKeyControl.AccessKey,
                //        valid = false
                //    };
                //}
                //else
                //{
                //    AC.AccessKey = generateAccessKey(Result.EmployeeID);
                //    AC.LoginDate = DateTime.Now;

                //    bool Update = _UserRepository.UpdateUserAccessKey(AC);

                //    return new
                //    {
                //        success = true,
                //        data = Result,
                //        AccessKey = AC.AccessKey,
                //        valid = false
                //    };
                //}
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        //[HttpPost]
        //[Route("login")]
        //[SwaggerOperation(Summary = "Log In เข้าสู้ระบบเพื่อรับ Access Key ",
        //Description = "Access Key ใช้ในการเรียหใช้ Function ต่างๆ เพื่อไม่ให้ User Login หลายเครื่องในเวลาเดียวกัน")]
        //public async Task<object> PostLogin([FromBody] LoginData data)
        //{
        //    try
        //    {

        //        var userName = data.UserName;
        //        var password = data.Password;
        //        var appCode = data.AppCode;

        //        string APApiKey = Environment.GetEnvironmentVariable("API_Key");
        //        if (APApiKey == null)
        //        {
        //            APApiKey = UtilsProvider.AppSetting.ApiKey;
        //        }
        //        string APApiToken = Environment.GetEnvironmentVariable("Api_Token");
        //        if (APApiToken == null)
        //        {
        //            APApiToken = UtilsProvider.AppSetting.ApiToken;
        //        }
                
        //        var client = new HttpClient();
        //        var Content = new StringContent(JsonConvert.SerializeObject(data));
        //        Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //        Content.Headers.Add("api_key", APApiKey);
        //        Content.Headers.Add("api_token", APApiToken);
        //        string PostURL = Environment.GetEnvironmentVariable("AuthenticationURL");
        //        if (PostURL == null)
        //        {
        //            PostURL = UtilsProvider.AppSetting.AuthorizeURL; 
        //        }
        //        var Respond = await client.PostAsync(PostURL, Content);
        //        if (Respond.StatusCode != System.Net.HttpStatusCode.OK)
        //        {
        //            return new
        //            {
        //                success = false,
        //                data = new AutorizeDataJWT(),
        //                valid = false
        //            };
        //        }
        //        var RespondData = await Respond.Content.ReadAsStringAsync();
        //        var Result = JsonConvert.DeserializeObject<AutorizeDataJWT>(RespondData);
        //        if (Result.LoginResult == false)
        //        {
        //            return new
        //            {
        //                success = false,
        //                data = Result.LoginResultMessage,
        //                valid = false
        //            };
        //        }
        //        return new
        //        {
        //            success = false,
        //            data = Result.LoginResultMessage,
        //            valid = false
        //        };
        //        // เป็นส่วนของการ Control Access Key ของระบบ ตัวอย่างนี้เป็นของระบบ Defect
        //        //AccessKeyControl AC = _UserRepository.GetUserAccessKey(Result.EmployeeID);
        //        //if (AC == null)
        //        //{
        //        //    AccessKeyControl accessKeyControl = new AccessKeyControl();
        //        //    accessKeyControl.EmpCode = Result.EmployeeID;
        //        //    accessKeyControl.AccessKey = generateAccessKey(Result.EmployeeID);
        //        //    accessKeyControl.LoginDate = DateTime.Now;

        //        //    bool Insert = _UserRepository.InsertUserAccessKey(accessKeyControl);

        //        //    return new
        //        //    {
        //        //        success = true,
        //        //        data = Result,
        //        //        AccessKey = accessKeyControl.AccessKey,
        //        //        valid = false
        //        //    };
        //        //}
        //        //else
        //        //{
        //        //    AC.AccessKey = generateAccessKey(Result.EmployeeID);
        //        //    AC.LoginDate = DateTime.Now;

        //        //    bool Update = _UserRepository.UpdateUserAccessKey(AC);

        //        //    return new
        //        //    {
        //        //        success = true,
        //        //        data = Result,
        //        //        AccessKey = AC.AccessKey,
        //        //        valid = false
        //        //    };
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Internal server error :: " + ex.Message);
        //    }
        //}

        [ApiExplorerSettings(IgnoreApi = true)]
        public string generateAccessKey(string EmpCode)
        {
            return string.Format("{0}_{1:N}", EmpCode, Guid.NewGuid());
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
                    bool CorrectACKey = SHAHelper.VerifyHash("APiCRMMobile","SHA512", AccessKey);
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
    }
}