﻿using Dapper;
using com.apthai.CRMMobile.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Hosting;
using com.apthai.CRMMobile.CustomModel;
using com.apthai.CRMMobile.Model.CRMMobile;

namespace com.apthai.CRMMobile.Repositories
{
    public class MasterRepository : BaseRepository, IMasterRepository
    {


        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _hostingEnvironment;
        string TAG = "MasterRepository";

        public MasterRepository(IHostingEnvironment environment, IConfiguration config) : base(environment, config)
        {
            _config = config;
            _hostingEnvironment = environment;

        }
        public async Task<int> TanonchaiJobSample()
        {
            int a = 20;
            int b = 60;
            int c = a + b;
            return c;
        }

        //public List<callarea> GetCallAreaByProductCat_Sync(string ProductTypeCate)
        //{
        //    using (IDbConnection conn = WebConnection)
        //    {
        //        try
        //        {
        //            if (ProductTypeCate == null || ProductTypeCate == "")
        //            {
        //                string sQuery = "Select * From callarea where Active = 1 ";
        //                var result = conn.Query<callarea>(sQuery).ToList();
        //                return result;
        //            }
        //            else
        //            {
        //                string sQuery = "Select * From callarea where ProductTypeCate = @ProductTypeCate And Active = 1 ";
        //                var result = conn.Query<callarea>(sQuery, new { ProductTypeCate = ProductTypeCate }).ToList();
        //                return result;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("MasterRepository.GetCallAreaByProductCat_Sync() :: Error ", ex);
        //        }
        //    }
        //}
        public List<GetProjectInformation> getProjectInformation_CRMWeb(string ProjectID)
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "SELECT DISTINCT(ba.BillerID),com.TaxID,mst.name AS 'AccountType',mstB.NameTH as 'AccountNameTH',mst.NameEN AS 'AccountNameEN'  ,com.NameTH ,com.NameEN ,ba.CompanyCode, ba.*,ba.BillerID, ba.CompanyCode , prj.projectNo " +
  "FROM MST.BankAccount ba " +
  "INNER JOIN Mst.MasterCenter mst ON mst.ID = ba.BankAccountTypeMasterCenterID " +
  "LEFT JOIN MST.Bank mstB ON mstB.ID = ba.BankID " +
  "INNER JOIN MST.Company com ON com.ID = ba.CompanyID " +
  "INNER JOIN PRJ.Project prj ON prj.CompanyID = com.ID " +
  "WHERE ba.IsBillPayment = 1 AND prj.ProjectNo = @ProjectID ";
                    var result = conn.Query<GetProjectInformation>(sQuery, new { ProjectID = ProjectID }).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.getProjectInformation_CRMWeb() :: Error ", ex);
                }
            }
        }
        public List<DocumentHeaderLevel1> GetAllDocumentHeaderLevel1()
        {
            using (IDbConnection conn = MobileConnection)
            {
                try
                {
                    string sQuery = "SELECT * From DOC.DocumentHeaderLevel1 ";
                    var result = conn.Query<DocumentHeaderLevel1>(sQuery).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.GetAllDocumentHeaderLevel1() :: Error ", ex);
                }
            }
        }
        public List<DocumentDetailLevel2> getDocumentDetailLevel2_CRMMobile(string RefDocumentHeaderID)
        {
            using (IDbConnection conn = MobileConnection)
            {
                try
                {
                    string sQuery = "SELECT * FROM DOC.DocumentDetailLevel2 where RefDocumentHeaderID = @RefDocumentHeaderID";
                    var result = conn.Query<DocumentDetailLevel2>(sQuery, new { RefDocumentHeaderID = RefDocumentHeaderID }).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.getDocumentDetailLevel2_CRMMobile() :: Error ", ex);
                }
            }
        }
        public List<SubDocumentDetailLevel3> getSubDocumentDetailLevel3_CRMMobile(int RefDocumentDetailID)
        {
            using (IDbConnection conn = MobileConnection)
            {
                try
                {
                    string sQuery = "SELECT * FROM DOC.SubDocumentDetailLevel3 where RefDocumentDetailID = @RefDocumentDetailID";
                    var result = conn.Query<SubDocumentDetailLevel3>(sQuery, new { RefDocumentDetailID = RefDocumentDetailID }).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.getSubDocumentDetailLevel3_CRMMobile() :: Error ", ex);
                }
            }
        }
        //public callTDefect GetCallTDefect_Sync(int TDefectID)
        //{
        //    using (IDbConnection conn = WebConnection)
        //    {
        //        try
        //        {
        //            string sQuery = "Select * From callTDefect " +
        //                "where TDefectID = @TDefectID And DocIsActive = 1 ";
        //            var result = conn.Query<callTDefect>(sQuery, new { TDefectID = TDefectID }).FirstOrDefault();
        //            return result;

        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("MasterRepository.GetCallAreaByProductCat_Sync() :: Error ", ex);
        //        }
        //    }
        //}
    }
}
