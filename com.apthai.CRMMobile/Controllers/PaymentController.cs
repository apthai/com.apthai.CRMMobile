using com.apthai.CoreApp.Data.Services;
using com.apthai.CRMMobile.CustomModel;
using com.apthai.CRMMobile.HttpRestModel;
using com.apthai.CRMMobile.Repositories;
using com.apthai.CRMMobile.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : BaseController
    {
        private readonly IMasterRepository _masterRepo;
        private readonly IAuthorizeService _authorizeService;
        private readonly IUserRepository _UserRepository;
        private readonly IConfiguration _config;
        public PaymentController(IMasterRepository masterRepo, IAuthorizeService authorizeService, IUserRepository userRepository,IConfiguration configuration)
        {
            _config = configuration;
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
                Model.CRMMobile.PaymentTransaction transaction = new Model.CRMMobile.PaymentTransaction();
                transaction.AccountFrom = data.accountFrom;
                transaction.CRMContractID = data.CRMContactID;
                transaction.PaymentAmount = Convert.ToDecimal(data.paymentAmount);
                transaction.TransactionDate = DateTime.Now;
                transaction.TransactionInfo = "SCB-PayMent";

                string ResourceOwnerID = data.CRMContactID;
                string RequestID = data.CRMContactID;
                string language = data.acceptlanguage;
                SCBAuthObj  sCB = new SCBAuthObj();
                sCB.applicationSecret = Environment.GetEnvironmentVariable("SCBAPISecret"); 
                sCB.applicationKey = Environment.GetEnvironmentVariable("SCBAPIKey");
                if (sCB.applicationSecret == null)
                {
                    sCB.applicationSecret = UtilsProvider.AppSetting.SCBAPISecret;
                }
                if (sCB.applicationKey == null)
                {
                    sCB.applicationKey  = UtilsProvider.AppSetting.SCBAPIKey; 
                }
                SCBAuthenRetrunObj Return = new SCBAuthenRetrunObj();
                var client = new HttpClient();
                var content = new StringContent(JsonConvert.SerializeObject(sCB));  //รอปั้น Obj 
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.Add("resourceOwnerId", sCB.applicationKey);
                content.Headers.Add("requestUId", RequestID);
                //content.Headers.Add("accept-language", data.acceptlanguage);
                string PostURL = Environment.GetEnvironmentVariable("SCBURL");
                if (PostURL == null)
                {
                    PostURL = UtilsProvider.AppSetting.SCBURL;
                }
                //string PostURL = "https://api-sandbox.partners.scb/partners/sandbox/v1/oauth/token"; // SCB Link Auth
                var respond = await client.PostAsync(PostURL, content);
                SCBAuthenRetrunObj SCBAuthResult = new SCBAuthenRetrunObj();
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
                    var ResponData = await respond.Content.ReadAsStringAsync();
                    SCBAuthResult = JsonConvert.DeserializeObject<SCBAuthenRetrunObj>(ResponData);

                    //return new
                    //{
                    //    success = true,
                    //    data = SCBAuthResult,
                    //    valid = "success : " + respond.StatusCode
                    //};
                }
                //----------------------------------Get Token Done -------------------------------------------
                //----------------------------------Start Request For DeepLink -------------------------------
                var Deeplinkclient = new HttpClient();
                List<string> subTransaction = new List<string>();
                subTransaction.Add("BP");
                SCBDeeplinkBodyObj sCBDeeplinkBody = new SCBDeeplinkBodyObj();
                sCBDeeplinkBody.transactionType = "PURCHASE";
                sCBDeeplinkBody.transactionSubType = subTransaction;
                sCBDeeplinkBody.sessionValidityPeriod = 60;
                sCBDeeplinkBody.sessionValidUntil = "";
                SCBDeeplinkBillPaymentRetrunObj billPayment = new SCBDeeplinkBillPaymentRetrunObj();
                billPayment.paymentAmount = data.paymentAmount;
                billPayment.accountTo = Environment.GetEnvironmentVariable("APSCBAccount");
                if (billPayment.accountTo == null || billPayment.accountTo == "")
                {
                    billPayment.accountTo = UtilsProvider.AppSetting.APSCBAccount;
                }
                //billPayment.accountFrom = data.accountFrom;
                billPayment.accountFrom = "";
                billPayment.ref1 = data.ContactNo;
                billPayment.ref2 = data.AgreementNo;
                billPayment.ref3 = "ABCDEFGHIJ1234567890";
                sCBDeeplinkBody.billPayment = billPayment;
                SCBDeeplinkmerchantMetaData merchantData = new SCBDeeplinkmerchantMetaData();
                merchantData.callbackurl = "";
                merchantData.extraData = "{}";
                //---------------------- sCBsPaymentInfo --------------------------------
                List<SCBDeeplinkpaymentInfo> sCBsPaymentInfo = new List<SCBDeeplinkpaymentInfo>();
                SCBDeeplinkpaymentInfo paymentInfo = new SCBDeeplinkpaymentInfo();
                paymentInfo.type = "TEXT_WITH_IMAGE>";
                paymentInfo.title = "";
                paymentInfo.header = "";
                paymentInfo.description = "";
                paymentInfo.imageUrl = "";
                sCBsPaymentInfo.Add(paymentInfo);
                SCBDeeplinkpaymentInfo paymentInfo2 = new SCBDeeplinkpaymentInfo();
                paymentInfo2.type = "TEXT>";
                paymentInfo2.title = "";
                paymentInfo2.header = "";
                paymentInfo2.description = "";
                paymentInfo.imageUrl = null;
                sCBsPaymentInfo.Add(paymentInfo2);
                merchantData.paymentInfo = sCBsPaymentInfo;
                // --------------------------------------------------------------------------------
                sCBDeeplinkBody.merchantMetaData = merchantData;
                string Json = JsonConvert.SerializeObject(sCBDeeplinkBody);
                var DeeplinkContent = new StringContent(JsonConvert.SerializeObject(sCBDeeplinkBody));  //รอปั้น Obj 
                DeeplinkContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                //Deeplinkclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SCBAuthResult.data.accessToken);
                Deeplinkclient.DefaultRequestHeaders.Add("authorization", "Bearer " + SCBAuthResult.data.accessToken);
                //DeeplinkContent.Headers.Add("authorization", "Bearer " + SCBAuthResult.data.accessToken);
                DeeplinkContent.Headers.Add("resourceOwnerId", sCB.applicationKey);
                DeeplinkContent.Headers.Add("requestUId", RequestID);
                DeeplinkContent.Headers.Add("channel", "scbeasy");
                //content.Headers.Add("accept-language", data.acceptlanguage);
                string DeeplinkPostURL = Environment.GetEnvironmentVariable("DeeplinkPostURL");
                if (DeeplinkPostURL == null)
                {
                    DeeplinkPostURL = UtilsProvider.AppSetting.DeeplinkPostURL;
                }
                //string DeeplinkPostURL = "https://api-sandbox.partners.scb/partners/sandbox/v3/deeplink/transactions"; // SCB Link Auth
                var Deeplinkrespond = await Deeplinkclient.PostAsync(DeeplinkPostURL, DeeplinkContent);
                SCBDeepLinkRetrunObj sCBDeepLinkRespond = new SCBDeepLinkRetrunObj();
                if (Deeplinkrespond.StatusCode != System.Net.HttpStatusCode.Created)
                {
                    return new
                    {
                        success = false,
                        data = new AutorizeDataJWT(),
                        valid = "Error On Request Deeplink From SCB : " + Deeplinkrespond.StatusCode
                    };
                }
                else
                {
                    var DeepLinkResponData = await Deeplinkrespond.Content.ReadAsStringAsync();
                    sCBDeepLinkRespond = JsonConvert.DeserializeObject<SCBDeepLinkRetrunObj>(DeepLinkResponData);
                    sCBDeepLinkRespond.SCBToken = SCBAuthResult.data.accessToken;

                    transaction.Status = "Pending";
                    transaction.AgreeMentNo = data.AgreementNo;
                    transaction.TransactionID = sCBDeepLinkRespond.data.transactionId;
                    transaction.TransactionAmount = Convert.ToDecimal(data.paymentAmount);
                    bool SaveResult = _UserRepository.InsertSCBTransaction(transaction);
                    return new
                    {
                        success = true,
                        data = sCBDeepLinkRespond,
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

        [HttpPost]
        [Route("GetSCBTransaction")]
        [SwaggerOperation(Summary = "เรียกดูเบอร์โทรศัพท์ของลูกค้าจากระบบ CRM ทั้งหมด",
       Description = "Access Key ใช้ในการเรียหใช้ Function ถึงจะเรียกใช้ Function ได้")]
        public async Task<object> GetSCBTransaction([FromBody]GETSCBTransactionParam data)
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
                string ResourceOwnerID = data.CRMContactID;
                string RequestID = data.CRMContactID;
                //string language = data.acceptLanguage;
                string TransactionID = data.TransactionID.Trim();
                SCBAuthObj sCB = new SCBAuthObj();
                sCB.applicationSecret = Environment.GetEnvironmentVariable("SCBAPISecret");
                sCB.applicationKey = Environment.GetEnvironmentVariable("SCBAPIKey");
                if (sCB.applicationSecret == null)
                {
                    sCB.applicationSecret = UtilsProvider.AppSetting.SCBAPISecret;
                }
                if (sCB.applicationKey == null)
                {
                    sCB.applicationKey = UtilsProvider.AppSetting.SCBAPIKey;
                }
                SCBAuthenRetrunObj Return = new SCBAuthenRetrunObj();
                var client = new HttpClient();
                var content = new StringContent(JsonConvert.SerializeObject(sCB));  //รอปั้น Obj 
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Add("authorization", "Bearer " + data.SCBToken);
                client.DefaultRequestHeaders.Add("resourceOwnerId", sCB.applicationKey);
                client.DefaultRequestHeaders.Add("requestUId", RequestID);
                //content.Headers.Add("resourceOwnerId", sCB.applicationKey);
                //content.Headers.Add("requestUId", RequestID);
                //content.Headers.Add("accept-language", data.acceptlanguage);
                string PostURL = "https://api-sandbox.partners.scb/partners/sandbox/v2/transactions/"; // SCB Link Auth
                PostURL = PostURL + TransactionID;
                var respond = await client.GetAsync(PostURL) ;
                SCBGetTransactionObj SCBAuthResult = new SCBGetTransactionObj();
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
                    var ResponData = await respond.Content.ReadAsStringAsync();
                    SCBAuthResult = JsonConvert.DeserializeObject<SCBGetTransactionObj>(ResponData);

                    return new
                    {
                        success = true,
                        data = SCBAuthResult,
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
