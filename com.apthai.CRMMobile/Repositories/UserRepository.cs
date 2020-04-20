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
        public Model.CRMWeb.ContactPhone GetContactPhoneNumberByContactIDandPhonNumber_Web(string ContactID,string PhoneNumber)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMWeb.ContactPhone>("select * from CTM.ContactPhone WITH(NOLOCK) " +
                    "where ContactID=@ContactID AND PhoneNumber=@PhoneNumber", new { ContactID = ContactID,PhoneNumber = PhoneNumber }).FirstOrDefault();

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
                var result = conn.Query<Model.CRMWeb.Contact>("select * from CTM.Contact WITH(NOLOCK) " +
                    "where CitizenIdentityNo=@CitizenIdentityNo", new { CitizenIdentityNo = CitizenIdentityNo }).FirstOrDefault();

                return result;
            }
        }

        public Model.CRMWeb.Contact GetCRMContactByID(string ID)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMWeb.Contact>("select * from CTM.Contact WITH(NOLOCK) " +
                    "where ID=@ID", new { ID = ID }).FirstOrDefault();

                return result;
            }
        }

        public List<Model.CRMWeb.TransferOwner> GetTransferOwnerByIDCardNO(string CitizenIdentityNo)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMWeb.TransferOwner>("SELECT * FROM SAL.TransferOwner WITH(NOLOCK) " +
                    "where CitizenIdentityNo=@CitizenIdentityNo", new { CitizenIdentityNo = CitizenIdentityNo }).ToList();

                return result;
            }
        }

        public Model.CRMWeb.Transfer GetTransferByID(string TransferID)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMWeb.Transfer>("SELECT * FROM SAL.Transfer WITH(NOLOCK) " +
                    "where ID=@TransferID", new { TransferID = TransferID }).FirstOrDefault();

                return result;
            }
        }

        public Model.CRMWeb.Unit GetUnitByID(string ID)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMWeb.Unit>("SELECT * FROM PRJ.Unit WITH(NOLOCK) " +
                    "where ID=@ID", new { ID = ID }).FirstOrDefault();

                return result;
            }
        }

        public Model.CRMWeb.Project GetProjectByID(string ID)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMWeb.Project>("SELECT * FROM PRJ.Project WITH(NOLOCK) " +
                    "where ID=@ID", new { ID = ID }).FirstOrDefault();

                return result;
            }
        }

        public Model.CRMWeb.Floor GetFloorByID(string ID)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMWeb.Floor>("SELECT * FROM PRJ.Floor WITH(NOLOCK) " +
                    "where ID=@ID", new { ID = ID }).FirstOrDefault();

                return result;
            }
        }

        public VerifyPINReturnObj GetUserLogin_Mobile(string UserToken)
        {
            using (IDbConnection conn = MobileConnection)
            {
                conn.Open();
                var result = conn.Query<VerifyPINReturnObj>("select * from CS.UserLogin WITH(NOLOCK) " +
                    " LEFT JOIN CS.UserProfile ON CS.UserLogin.UserprofileID = CS.UserProfile.UserProfileID " +
                    " where  CS.UserLogin.UserToken=@UserToken", new { UserToken = UserToken }).FirstOrDefault();

                return result;
            }
        }
        public Model.CRMMobile.UserLogin GetUserLoginByID_Mobile(int UserLoginID)
        {
            using (IDbConnection conn = MobileConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMMobile.UserLogin>("select * from CS.UserLogin WITH(NOLOCK) " +
                    " where CS.UserLogin.UserLoginID=@UserLoginID", new { UserLoginID = UserLoginID }).FirstOrDefault();

                return result;
            }
        }
        public Model.CRMMobile.UserLogin GetUserLoginByPhoneNumbandDevice_Mobile(string DeviceID,string UserPhoneNumber)
        {
            using (IDbConnection conn = MobileConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMMobile.UserLogin>("select * from CS.UserLogin WITH(NOLOCK) " +
                    " where CS.UserLogin.UserPhoneNumber=@UserPhoneNumber And DeviceID=@DeviceID", new { UserPhoneNumber = UserPhoneNumber , DeviceID  = DeviceID }).FirstOrDefault();

                return result;
            }
        }
        public Model.CRMMobile.UserProfile GetUserProfileByCRMContactID_Mobile(string CRMContactID)
        {
            using (IDbConnection conn = MobileConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMMobile.UserProfile>("select * from CS.UserProfile WITH(NOLOCK) " +
                    " where CS.UserProfile.CRMContactID=@CRMContactID", new { CRMContactID = CRMContactID }).FirstOrDefault();

                return result;
            }
        }
        public bool InsertCSUserProfile(Model.CRMMobile.UserProfile data,out long ProfileID)
        {
            using (IDbConnection conn = MobileConnection)
            {
                try
                {
                    conn.Open();
                    var tran = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                    ProfileID = conn.Insert(data, tran);
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

                    var result = conn.Insert(data, tran);
                    tran.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.InsertCSUserProfile() :: Error ", ex);
                }
            }
        }
        public bool UpdateCSUserLogin(Model.CRMMobile.UserLogin data)
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
        public bool UpdateCSUserProfile(Model.CRMMobile.UserProfile data)
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
                    throw new Exception("MasterRepository.UpdateCSUserProfile() :: Error ", ex);
                }
            }
        }
        public bool UpdateChangePINCSUserProfile(Model.CRMMobile.UserProfile data)
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
                    throw new Exception("MasterRepository.UpdateCSUserProfile() :: Error ", ex);
                }
            }
        }
        //---------------------------------------------------------------------
        //-------------------------- Get billing Tracking ---------------------
        public List<iCRMBooking> GetUseriBookingByUserID(string UserID)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<iCRMBooking>("GetUserBookingMobile", new { ContactID = UserID }, commandType: CommandType.StoredProcedure).ToList();

                return result;
            }
        }
        public List<GetBillingTrackingMobile> GetUserBillingTrackingByProjectandUnit(string ProjectID,string UnitID)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<GetBillingTrackingMobile>("GetBillingTrackingMobile", new { ProjectID = ProjectID, UnitID=UnitID }, commandType: CommandType.StoredProcedure).ToList();

                return result;
            }
        }
        public List<iCRMContact> GetUseriCRMContact_Web(string Contact )
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<iCRMContact>("iCRMConTactForMobile", new { Contact = Contact }, commandType: CommandType.StoredProcedure).ToList();

                return result;
            }
        }
        public List<GetUserCardReturnObj> GetUserCardByProjectandUnit(string ContactNo)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<GetUserCardReturnObj>("GetUserCardMobile", new { ContactNo = ContactNo}, commandType: CommandType.StoredProcedure).ToList();

                return result;
            }
        }
        public GetUserCreditCardReturnObj GetUserCreditCardByProjectandUnit(string ProjectNo, string UnitID)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<GetUserCreditCardReturnObj>("GetUserCreditCardMobile", new { ProjectNo = ProjectNo, UnitID = UnitID }, commandType: CommandType.StoredProcedure).FirstOrDefault();

                return result;
            }
        }
    }

}
