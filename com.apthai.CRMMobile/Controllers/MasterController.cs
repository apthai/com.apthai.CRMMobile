using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using com.apthai.CRMMobile.Model;
using com.apthai.CRMMobile.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using Ionic.Zip;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using com.apthai.CRMMobile.Repositories;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Microsoft.Extensions.Configuration;
using com.apthai.CRMMobile.Configuration;
using Microsoft.AspNetCore.StaticFiles;
using com.apthai.CRMMobile.CustomModel;
using com.apthai.CoreApp.Data.Services;
using com.apthai.CRMMobile.HttpRestModel;
using Microsoft.Extensions.Primitives;
using com.apthai.CRMMobile.Model.CRMMobile;

namespace com.apthai.CRMMobile.Controllers
{
    public class MasterController : ControllerBase
    {
        private readonly IAuthorizeService _authorizeService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IMasterRepository _masterRepository;
        //private List<MStorageServer> _QISStorageServer;
        protected AppSettings _appSetting;

        public MasterController(IAuthorizeService authorizeService,IUnitOfWork unitOfWork, IMasterRepository masterRepository)
        {
            _hostingEnvironment  = UtilsProvider.HostingEnvironment;
            _config = UtilsProvider.Config;
            _appSetting = UtilsProvider.AppSetting;
            _unitOfWork = unitOfWork;
            // ------------- แบบนี้จะอ้าง ถึง Repo ตรงๆ ไม่รอรับการทำ AutoMate-Test ได้
            //_masterRepository = new MasterRepository(_hostingEnvironment, _config);
            // ------------- ควรใช้แบบนี้
            _masterRepository = masterRepository;
            _authorizeService = authorizeService;
        }
        [HttpGet]
        [Route("Test")]
        public async Task<object> GetMasterCallType()
        {
                    return new
                    {
                        success = true,
                        data = "Hello World!"
                    };
             
        }

        [HttpGet]
        [Route("GetProjectInformation")]
        public async Task<object> GetProjectInformation(string ProjectID)
        {

            List<GetProjectInformation> projectInformation = _masterRepository.getProjectInformation_CRMWeb(ProjectID);

            return new
            {
                success = true,
                data = projectInformation,
                Message = "GetProjectInformantion Successfully"
            };

        }

        [HttpGet]
        [Route("DocumentHeaderList")]
        public async Task<object> DocumentHeaderList()
        {

            List<DocumentHeaderLevel1> projectInformation = _masterRepository.GetAllDocumentHeaderLevel1();

            return new
            {
                success = true,
                data = projectInformation,
                Message = "DocumentHeaderList Successfully"
            };

        }

        [HttpPost]
        [Route("DocumentDetailList")]
        public async Task<object> DocumentDetailList([FromBody]GetDocumentDetailParam Data)
        {

            List<DocumentDetailLevel2> detailLists = _masterRepository.getDocumentDetailLevel2_CRMMobile(Data.HeaderID);
            DocumentDetailListResult Result = new DocumentDetailListResult();
            Result.DocumentDetailList = new List<DocumentDetailList>();
            for (int i = 0; i < detailLists.Count(); i++)
            {
            DocumentDetailList Obj = new DocumentDetailList();
                Obj.Created = detailLists[i].Created;
                Obj.CreatedBy = detailLists[i].CreatedBy;
                Obj.DocumentDetailID = detailLists[i].DocumentDetailID;
                Obj.DocumentDetailName = detailLists[i].DocumentDetailName;
                Obj.OrderOfDocumentDetail = detailLists[i].OrderOfDocumentDetail;
                Obj.RefDocumentHeaderID = detailLists[i].RefDocumentHeaderID;
                Obj.StatusDocumentDetail = detailLists[i].StatusDocumentDetail;
                Obj.Updated = detailLists[i].Updated;
                Obj.UpdatedBy = detailLists[i].UpdatedBy;
                Obj.URLDocumentDetail = detailLists[i].URLDocumentDetail;
                List<SubDocumentDetailLevel3> subDocuments = _masterRepository.getSubDocumentDetailLevel3_CRMMobile(detailLists[i].DocumentDetailID);
                Obj.SubDoct = subDocuments;
                Result.DocumentDetailList.Add(Obj);
            }
            return new
            {
                success = true,
                data = Result,
                Message = "DocumentDetailList Successfully"
            };

        }
        //-------------- Verify Key and Token From HTTP Header ------------------
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool VerifyHeader(out string ErrorMsg)
        {
            //if (data == null)
            //{
            //    return BadRequest();
            //}

            //string ipaddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            string ipaddress = "5555555";
            StringValues api_key;
            StringValues EmpCode;

            var isValidHeader = false;
            //APIITVendor //VendorData = new APIITVendor();
            if (Request.Headers.TryGetValue("api_Accesskey", out api_key) && Request.Headers.TryGetValue("EmpCode", out EmpCode))
            {
                string AccessKey = api_key.First();
                string EmpCodeKey = EmpCode.First();

                if (!string.IsNullOrEmpty(AccessKey) && !string.IsNullOrEmpty(EmpCodeKey))
                {
                    //bool CanAccess = _authorizeService.AccessKeyAuthentication(AccessKey, EmpCodeKey);
                    //if (CanAccess == true)
                    //{
                    //    ErrorMsg = "";
                    //    return true;
                    //}
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
