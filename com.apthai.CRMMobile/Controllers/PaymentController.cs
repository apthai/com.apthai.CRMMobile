using com.apthai.CoreApp.Data.Services;
using com.apthai.CRMMobile.CustomModel;
using com.apthai.CRMMobile.HttpRestModel;
using com.apthai.CRMMobile.Repositories;
using com.apthai.CRMMobile.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace com.apthai.CRMMobile.Controllers
{
    public class PaymentController
    {
        private readonly IMasterRepository _masterRepo;
        private readonly IAuthorizeService _authorizeService;
        private readonly IUserRepository _UserRepository;
        public PaymentController(IMasterRepository masterRepo, IAuthorizeService authorizeService, IUserRepository userRepository)
        {

            _masterRepo = masterRepo;
            _authorizeService = authorizeService;
            _UserRepository = userRepository;
        }

        [HttpPost]
        [Route("SCBPayment")]
        [SwaggerOperation(Summary = "เรียกดูเบอร์โทรศัพท์ของลูกค้าจากระบบ CRM ทั้งหมด",
       Description = "Access Key ใช้ในการเรียหใช้ Function ถึงจะเรียกใช้ Function ได้")]
        public async Task<object> SCBPayment([FromBody]SCBPaymentParam data)
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
                string ResourceOwnerID = data.resourceOwnerId;
                string RequestID = data.requestUId;
                string language = data.acceptlanguage;
                var Header = @"{ 'resourceOwnerId': '" + ResourceOwnerID + "', 'requestUId': '" + RequestID + "', 'accept-language': '" + language + "' }";
                SCBAuthenRetrunObj Return = new SCBAuthenRetrunObj();
                var client = new HttpClient();
                var content = new StringContent(JsonConvert.SerializeObject(Header));  //รอปั้น Obj 
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                string PostURL = "https://api-sandbox.partners.scb/partners/sandbox/v1/oauth/token"; // SCB Link
                var respond = await client.PostAsync(PostURL, content);
                if (respond.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new
                    {
                        success = false,
                        data = new AutorizeDataJWT(),
                        valid = "Cannot connect to API : " + respond.StatusCode
                    };
                }
                else
                {
                    var ResponData = await respond.Content.ReadAsStreamAsync(); 

                    return new
                    {
                        success = true,
                        data = new AutorizeDataJWT(),
                        valid = "success : " + respond.StatusCode
                    };
                }
                
            }
            catch (Exception ex)
            {
                return new
                {
                    success = false,
                    data = new AutorizeDataJWT(),
                    valid = "Internal Server Error 500 : " + ex
                };
            }
        }
    }
}
