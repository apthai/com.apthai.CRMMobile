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

       // [HttpPost]
       // [Route("GetUserPropoty")]
       // [SwaggerOperation(Summary = "เรียกดูเบอร์โทรศัพท์ของลูกค้าจากระบบ CRM ทั้งหมด",
       //Description = "Access Key ใช้ในการเรียหใช้ Function ถึงจะเรียกใช้ Function ได้")]
       // public async Task<object> GetUserPropoty([FromBody]GetUserPropotyParam data)
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
       //         string ResourceOwnerID = "l79746708bafd440288a9da8ea96fa487d";
       //         string RequestID = "AP004134";
       //         string language = "EN";
       //         var Header = @"{ 'resourceOwnerId': '" +ResourceOwnerID + "', 'requestUId': '" + RequestID + "', 'accept-language': '" + language + "' }";
       //         SCBAuthenRetrunObj Return = new SCBAuthenRetrunObj();
       //         var client = new HttpClient();
       //         var content = new StringContent(JsonConvert.SerializeObject(Header));  //รอปั้น Obj 
       //         content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
       //         string PostURL = "https://api-sandbox.partners.scb/partners/sandbox/v1/oauth/token"; // SCB Link
       //         var respond = await client.PostAsync(PostURL,content);

       //         GetUserPropotyReturnObj ReturnObj = new GetUserPropotyReturnObj();
       //         Model.CRMWeb.Contact contact = _UserRepository.GetCRMContactByID(data.CustometID);
       //         if (contact == null)
       //         {
       //             return new
       //             {
       //                 success = false,
       //                 data = new Model.CRMWeb.Transfer(),
       //                 message = "Cannot Find Contact Data !"
       //             };
       //         }
       //         List<Model.CRMWeb.TransferOwner> transferOwners = _UserRepository.GetTransferOwnerByIDCardNO(contact.CitizenIdentityNo);
       //         for (int i = 0; i < transferOwners.Count(); i++)
       //         {
       //             GetUserPropotyObj getUserPropotyobj = new GetUserPropotyObj();
       //             Model.CRMWeb.Transfer transfer = _UserRepository.GetTransferByID(transferOwners[i].TransferID.ToString());
       //             if (transfer == null)
       //             {
       //                 return new
       //                 {
       //                     success = false,
       //                     data = new Model.CRMWeb.Transfer(),
       //                     message = "Cannot Find data on Transfer Table !"
       //                 };
       //             }
       //             Model.CRMWeb.Unit unit = _UserRepository.GetUnitByID(transfer.UnitID.ToString());
       //             if (unit == null)
       //             {
       //                 return new
       //                 {
       //                     success = false,
       //                     data = new Model.CRMWeb.Transfer(),
       //                     message = "Cannot Find data on Transfer Table !"
       //                 };
       //             }
       //             Model.CRMWeb.Project project = _UserRepository.GetProjectByID(unit.ProjectID.ToString());
       //             if (project == null)
       //             {
       //                 return new
       //                 {
       //                     success = false,
       //                     data = new Model.CRMWeb.Transfer(),
       //                     message = "Cannot Find data on Transfer Table !"
       //                 };
       //             }
       //             Model.CRMWeb.Floor floor = _UserRepository.GetFloorByID(unit.FloorID.ToString());
       //             if (floor == null)
       //             {
       //                 return new
       //                 {
       //                     success = false,
       //                     data = new Model.CRMWeb.Transfer(),
       //                     message = "Cannot Find data on Transfer Table !"
       //                 };
       //             }

       //             getUserPropotyobj.transfer = transfer;
       //             getUserPropotyobj.Unit = unit;
       //             getUserPropotyobj.Project = project;
       //             getUserPropotyobj.Floor = floor;

       //             ReturnObj.getUserPropotyObjs.Add(getUserPropotyobj);

       //         }

       //         return new
       //         {
       //             success = true,
       //             data = ReturnObj,
       //             message = "Get User Phone Success !"
       //         };

       //     }
       //     catch (Exception ex)
       //     {
       //         return StatusCode(500, "Internal server error :: " + ex.Message);
       //     }
       // }
    }
}
