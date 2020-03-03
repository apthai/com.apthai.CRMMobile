using Dapper;
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

namespace com.apthai.CRMMobile.Repositories
{
    public class UserRepository : BaseRepository , IUserRepository
    {

        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _hostingEnvironment;

        public UserRepository(IHostingEnvironment environment, IConfiguration config) : base(environment, config)
        {
            _config = config;
            _hostingEnvironment = environment;

        }
        //---------------- Log In Module --------------------------------------
        public List<Model.CRMWeb.ContactPhone> GetContactPhoneNumberByContactID_Web(string ContactID)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMWeb.ContactPhone>("select * from CTM.ContactPhone WITH(NOLOCK) " +
                    "where ContactID=@ContactID", new { ContactID = ContactID }).ToList();

                return result;
            }
        }
        public Model.CRMWeb.ContactPhone GetSingleContactPhoneNumberByContactID_Web(string ContactID,string PhoneNumber)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMWeb.ContactPhone>("select * from CTM.ContactPhone WITH(NOLOCK) " +
                    "where ContactID=@ContactID AND PhoneNumber=@PhoneNumber", new { ContactID = ContactID, PhoneNumber = PhoneNumber }).FirstOrDefault();

                return result;
            }
        }
        public Model.CRMWeb.Contact GetCRMContactByIDCardNO(string CitizenIdentityNo)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMWeb.Contact>("select * from ctm.Contact WITH(NOLOCK) " +
                    "where CitizenIdentityNo=@CitizenIdentityNo", new { CitizenIdentityNo = CitizenIdentityNo }).FirstOrDefault();

                return result;
            }
        }
        public bool InsertCSUserProfile(Model.CRMMobile.UserProfile data)
        {
            using (IDbConnection conn = MobileConnection)
            {
                try
                {
                    conn.Open();
                    var tran = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                    var result = conn.Update(data, tran);
                    tran.Commit();
                    
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.InsertCSUserProfile() :: Error ", ex);
                }
            }
        }
        public bool InsertCSUserLogin(Model.CRMMobile.UserLogin data)
        {
            using (IDbConnection conn = MobileConnection)
            {
                try
                {
                    conn.Open();
                    var tran = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                    var result = conn.Update(data, tran);
                    tran.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.InsertCSUserProfile() :: Error ", ex);
                }
            }
        }
        //---------------------------------------------------------------------
    }

}
