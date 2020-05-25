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
        [Route("CheckPIN")]
        [SwaggerOperation(Summary = "Register User เพื่อใช่ระบบ ซึ่งจะไป หาข้อมูลจากระบบ CRM",
       Description = "Access Key ใช้ในการเรียหใช้ Function ต่างๆ เพื่อไม่ให้ User Login หลายเครื่องในเวลาเดียวกัน")]
        public async Task<object> PostLogin([FromBody]CheckPinParam data)
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
                    if (!SHAHelper.VerifyHash(data.PINCode,"SHA512", cSUserProfile.PINCode))
                    {
                        return new
                        {
                            success = false,
                            data = new AutorizeDataJWT(),
                            message = "PinCode is InCorrect!"
                        };
                    }
                    Model.CRMMobile.UserLogin userLogin = _UserRepository.GetUserLoginByID_Mobile(cSUserProfile.UserLoginID);
                    string GenerateAccessToken = SHAHelper.ComputeHash(data.DeviceID, "SHA512", null);
                    userLogin.UserToken = GenerateAccessToken;
                    cSUserProfile.UserToken = GenerateAccessToken;
                    userLogin.FireBaseToken = data.FireBaseToken;
                    userLogin.AppVersion = data.AppVersion;
                    userLogin.DeviceType = data.OS;
                    bool UpdateUserToken = _UserRepository.UpdateCSUserLogin(userLogin);

                    return new
                    {
                        success = true,
                        data = cSUserProfile,
                        message = "PIN Correct!"
                    };
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("ChangePIN")]
        [SwaggerOperation(Summary = "Register User เพื่อใช่ระบบ ซึ่งจะไป หาข้อมูลจากระบบ CRM",
       Description = "Access Key ใช้ในการเรียหใช้ Function ต่างๆ เพื่อไม่ให้ User Login หลายเครื่องในเวลาเดียวกัน")]
        public async Task<object> ChangePIN([FromBody]ChangePINParam data)
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
                Model.CRMMobile.UserProfile user = _UserRepository.GetUserProfileByCRMContactID_Mobile(cSUserProfile.CRMContactID);
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
                    if (!SHAHelper.VerifyHash(data.OldPIN, "SHA512", cSUserProfile.PINCode))
                    {
                        return new
                        {
                            success = false,
                            data = new AutorizeDataJWT(),
                            message = "PinCode is InCorrect!"
                        };
                    }
                    //Model.CRMMobile.UserLogin userLogin = _UserRepository.GetUserLoginByID_Mobile(cSUserProfile.UserLoginID);

                    string NewPIN = SHAHelper.ComputeHash(data.NewPIN, "SHA512", null);
                    user.PINCode = NewPIN;
                    user.Updated = DateTime.Now.ToString();
                    bool updateUserPIN = _UserRepository.UpdateChangePINCSUserProfile(user);


                    return new
                    {
                        success = true,
                        data = new Model.CRMMobile.UserProfile() ,
                        message = "ChangePIN Success !!"
                    };
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }
        [HttpPost]
        [Route("LogOut")]
        [SwaggerOperation(Summary = "Register User เพื่อใช่ระบบ ซึ่งจะไป หาข้อมูลจากระบบ CRM",
       Description = "Access Key ใช้ในการเรียหใช้ Function ต่างๆ เพื่อไม่ให้ User Login หลายเครื่องในเวลาเดียวกัน")]
        public async Task<object> LogOut([FromBody]LogOutParam data)
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
                    /// --------- Check PIN Code IS Correct
                    //if (!SHAHelper.VerifyHash(data.PINCode, "SHA512", cSUserProfile.PINCode))
                    //{
                    //    return new
                    //    {
                    //        success = false,
                    //        data = new AutorizeDataJWT(),
                    //        message = "PinCode is InCorrect!"
                    //    };
                    //}

                    Model.CRMMobile.UserLogin userLogin = _UserRepository.GetUserLoginByID_Mobile(cSUserProfile.UserLoginID);
                    //string GenerateAccessToken = SHAHelper.ComputeHash(data.DeviceID, "SHA512", null);
                    userLogin.FireBaseToken = null;
                    bool UpdateUserToken = _UserRepository.UpdateCSUserLogin(userLogin);

                    return new
                    {
                        success = true,
                        data = cSUserProfile,
                        message = "PIN Correct!"
                    };
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error :: " + ex.Message);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string generateToken(string PhoneNumber)
        {
            return string.Format("{0}_{1:N}", PhoneNumber, Guid.NewGuid());
        }

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