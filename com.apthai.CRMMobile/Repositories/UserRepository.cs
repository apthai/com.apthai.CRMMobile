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
using Minio;
using Minio.DataModel;
using Minio.Exceptions;
using System.Reactive.Linq;

namespace com.apthai.CRMMobile.Repositories
{
    public class UserRepository : BaseRepository , IUserRepository
    {

        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _hostingEnvironment;
        private int _expireHours = 24;
        public string _publicURL;
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

        public Model.CRMWeb.Project GetProjectByProjectNo(string ProjectNO)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMWeb.Project>("SELECT * FROM PRJ.Project WITH(NOLOCK) " +
                    "where ProjectNo=@ProjectNO", new { ProjectNO = ProjectNO }).FirstOrDefault();

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
        public List<Model.CRMMobile.UserLogin> GetallUserLoginByID_Mobile(int UserLoginID)
        {
            using (IDbConnection conn = MobileConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMMobile.UserLogin>("select * from CS.UserLogin WITH(NOLOCK) " +
                    " where CS.UserLogin.UserLoginID=@UserLoginID", new { UserLoginID = UserLoginID }).ToList();

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
        public Model.CRMMobile.UserLogin GetUserLoginByPhoneNumbandDeviceandUserProfileID_Mobile(string DeviceID, string UserPhoneNumber,int UserProfileID)
        {
            using (IDbConnection conn = MobileConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMMobile.UserLogin>("select * from CS.UserLogin WITH(NOLOCK) " +
                    " where CS.UserLogin.UserPhoneNumber=@UserPhoneNumber And DeviceID=@DeviceID And UserProfileID=@UserProfileID", new { UserPhoneNumber = UserPhoneNumber, DeviceID = DeviceID, UserProfileID= UserProfileID }).FirstOrDefault();

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
        public List<Model.CRMMobile.NotificationHistory> GetUserNotificationHistoryByCRMContactID_Mobile(string CRMContactID)
        {
            using (IDbConnection conn = MobileConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMMobile.NotificationHistory>("select * from NT.NotificationHistory WITH(NOLOCK) " +
                    " where NT.NotificationHistory.CRMContactID=@CRMContactID Order by Created Desc", new { CRMContactID = CRMContactID }).ToList();

                return result;
            }
        }
        public Model.CRMMobile.PaymentTransaction GetUserPaymentTransactionByUserID(string TransactionID)
        {
            using (IDbConnection conn = MobileConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMMobile.PaymentTransaction>("select * from PaymentTransaction WITH(NOLOCK) " +
                    " WHERE transactionID=@TransactionID ", new { TransactionID = TransactionID }).FirstOrDefault();

                return result;
            }
        }
        public Model.CRMMobile.NotificationHistory GetUserNotificationHistoryByNotiHistoryID_Mobile(string NotiHistory)
        {
            using (IDbConnection conn = MobileConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMMobile.NotificationHistory>("select * from NT.NotificationHistory WITH(NOLOCK) " +
                    " where NT.NotificationHistory.NotiHistoryID=@NotiHistory", new { NotiHistory = NotiHistory }).FirstOrDefault();

                return result;
            }
        }
        public Model.CRMWeb.FET GetUserFETDataByPaymentMethodID(Guid PaymentMethodID)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMWeb.FET>("select * from FIN.PaymentMethod WITH(NOLOCK) " +
                    " Left join FIN.FET ft ON FIN.PaymentMethod.ID = ft.PaymentMethodID " +
                    " where Fin.PaymentMethod.PaymentID=@PaymentMethodID", new { PaymentMethodID = PaymentMethodID }).FirstOrDefault();

                return result;
            }
        }
        public bool GetUserFETByPaymentMethodID(Guid PaymentMethodID)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<Model.CRMWeb.FET>("select * from FIN.PaymentMethod WITH(NOLOCK) " +
                    " Left join FIN.FET ft ON FIN.PaymentMethod.ID = ft.PaymentMethodID " +
                    " where Fin.PaymentMethod.PaymentID=@PaymentMethodID", new { PaymentMethodID = PaymentMethodID }).FirstOrDefault();
                
                if (result == null)
                {
                    return false;
                }
                return true;
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
        public bool DeleteNotificationTemp(Model.CRMMobile.NotificationTemp data)
        {
            using (IDbConnection conn = MobileConnection)
            {
                try
                {
                    conn.Open();
                    var tran = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                    var result = conn.Delete(data, tran);
                    tran.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.InsertCSUserProfile() :: Error ", ex);
                }
            }
        }
        public bool InsertNotificationHistory(Model.CRMMobile.NotificationHistory data)
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
                    throw new Exception("MasterRepository.InsertNotificationHistory() :: Error ", ex);
                }
            }
        }
        public bool InsertSCBTransaction(Model.CRMMobile.PaymentTransaction data)
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
                    throw new Exception("MasterRepository.InsertNotificationHistory() :: Error ", ex);
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
        public bool UpdatePaymentTransaction(Model.CRMMobile.PaymentTransaction data)
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
        public bool UpdateIsReadForNotification(Model.CRMMobile.NotificationHistory data)
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
                var result = conn.Query<iCRMBooking>("sp_iCRM_GetUserBookingMobile", new { ContactID = UserID }, commandType: CommandType.StoredProcedure).ToList();

                return result;
            }
        }
        public List<iCRMMyProperty> GetUseriCRMMyPropoty(string ContactNo)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<iCRMMyProperty>("sp_iCRM_GetiCRMMyProperty", new { ContactNo = ContactNo }, commandType: CommandType.StoredProcedure).ToList();

                return result;
            }
        }
        public List<GetBillingTrackingMobile> GetUserBillingTrackingByProjectandUnit(string ProjectNo, string UnitNo)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<GetBillingTrackingMobile>("sp_iCRM_GetBillingTrackingMobile", new { ProjectNo = ProjectNo, UnitNo = UnitNo }, commandType: CommandType.StoredProcedure).ToList();

                return result;
            }
        }
        public List<iCRMContact> GetUseriCRMContact_Web(string Contact )
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<iCRMContact>("sp_iCRM_iConTactForMobile", new { Contact = Contact }, commandType: CommandType.StoredProcedure).ToList();

                return result;
            }
        }
        public List<GetUserCardReturnObj> GetUserCardByProjectandUnit(string ContactNo)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<GetUserCardReturnObj>("sp_iCRM_GetUserCardMobile", new { ContactNo = ContactNo}, commandType: CommandType.StoredProcedure).ToList();

                return result;
            }
        }
        public List<GetUserCardReturnObj> GetUserCardByUserCardByUnitMobile(string ContactNo , string UnitNo)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<GetUserCardReturnObj>("sp_iCRM_GetUserCardByUnitMobile", new { ContactNo = ContactNo , UnitNo = UnitNo }, commandType: CommandType.StoredProcedure).ToList();

                return result;
            }
        }
        public List<GetiCRMOwnerReturnObj> GetUserICRMOwnerByProjectUnitAndCRMContactID(string ContactID,string UnitNo,string ProjectNO)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<GetiCRMOwnerReturnObj>("sp_iCRM_GetiCRMOwner", new { ContactID = ContactID ,UnitNo = UnitNo , ProjectNo = ProjectNO}, commandType: CommandType.StoredProcedure).ToList();

                return result;
            }
        }
        public GetUserCreditCardReturnObj GetUserCreditCardByProjectandUnit(string ProjectNo, string UnitNo)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<GetUserCreditCardReturnObj>("sp_iCRM_GetUserCreditCardMobile", new { ProjectNo = ProjectNo, UnitNo = UnitNo }, commandType: CommandType.StoredProcedure).FirstOrDefault();

                return result;
            }
        }
        public GetGetReceiptInfoNewReturnObj GetReceiptInfoByReceiptNo(string ReceiptTempNo)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open(); 
                //var result = conn.Query<GetGetReceiptInfoReturnObj>("sp_iCRM_GetReceiptInfo", new { ReceiptTempNo = ReceiptTempNo }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                var result = conn.Query<GetGetReceiptInfoNewReturnObj>("sp_iCRM_GetReceiptInfo_Detail", new { ReceiptTempNo = ReceiptTempNo }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return result;
            }
        }
        public GetGetReceiptInfoNewReturnObj GetReceiptTempInfoByReceiptNo(string ReceiptTempNo)
        {
            using (IDbConnection conn = WebConnection)
            {
                conn.Open();
                var result = conn.Query<GetGetReceiptInfoNewReturnObj>("sp_iCRM_GetTempReceiptInfo", new { ReceiptTempNo = ReceiptTempNo }, commandType: CommandType.StoredProcedure).FirstOrDefault();

                return result;
            }
        }
        public async Task<FileUploadResult> UploadFileFromStreamWithOutGuid(Stream fileStream, string bucketName, string filePath, string fileName, string contentType)
        {
            MinioClient minio;

            string _minioEndpoint = "192.168.2.29:9001"; // CRM 
            string _minioAccessKey = "XNTYE7HIMF6KK4BVEIXA";
            string _minioSecretKey = "naD+esQ+uV7+xwfF3bPfAn5iC7C1XUyXeM8HkBlO";
            string _defaultBucket = "erecipt";
            string _tempBucket = "erecipt";
            bool _withSSL = false;

            if (_withSSL)
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey).WithSSL();
            else
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey);

            bool bucketExisted = await minio.BucketExistsAsync(bucketName);
            if (!bucketExisted)
                await minio.MakeBucketAsync(bucketName);

            string objectName = fileName;
            objectName = Path.Combine(filePath, objectName);
            objectName = objectName.Replace('\\', '/');
            await minio.PutObjectAsync(bucketName, objectName, fileStream, fileStream.Length, contentType);
            // expire in 1 day
            var url = await minio.PresignedGetObjectAsync(bucketName, objectName, (int)TimeSpan.FromHours(_expireHours).TotalSeconds);
            url = ReplaceWithPublicURL(url);
            return new FileUploadResult()
            {
                Name = objectName,
                BucketName = bucketName,
                Url = url
            };
        }
        public async Task<string> GetFileUrlAsync(string name)
        {
            string _minioEndpoint = "192.168.2.29:9001"; // CRM 
            string _minioAccessKey = "XNTYE7HIMF6KK4BVEIXA";
            string _minioSecretKey = "naD+esQ+uV7+xwfF3bPfAn5iC7C1XUyXeM8HkBlO";
            string _defaultBucket = "erecipt";
            string _tempBucket = "erecipt";
            bool _withSSL = false;
            MinioClient minio;
            if (_withSSL)
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey).WithSSL();
            else
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey);

            var url = await minio.PresignedGetObjectAsync(_defaultBucket, name, (int)TimeSpan.FromHours(_expireHours).TotalSeconds);

            url = ReplaceWithPublicURL(url);

            return url;
        }
        public async Task<string> GetFileUrlAsync(string bucket,string ReceiptNo,string name)
        {
            string _minioEndpoint = "192.168.2.29:9001"; //192.168.2.29:9001"; // CRM 
            string _minioAccessKey = "XNTYE7HIMF6KK4BVEIXA";
            string _minioSecretKey = "naD+esQ+uV7+xwfF3bPfAn5iC7C1XUyXeM8HkBlO";
            string _defaultBucket = "erecipt";
            string _tempBucket = "erecipt";
            bool _withSSL = false;
            MinioClient minio;
            if (_withSSL)
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey).WithSSL();
            else
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey);

            //var observable = minio.StatObjectAsync(bucket,name );
            
            var url = await minio.PresignedGetObjectAsync(bucket ,name, (int)TimeSpan.FromHours(_expireHours).TotalSeconds);
            //url = (!string.IsNullOrEmpty(_publicURL)) ? url.Replace(_minioEndpoint, _publicURL) : url;
            url = ReplaceWithPublicURL(url);

            return url;
        }
        public async Task<string> GetFETFileUrlAsync(string bucket, string FETFilePath)
        {
            string _minioEndpoint = "192.168.2.29:9001"; //192.168.2.29:9001"; // CRM 
            string _minioAccessKey = "XNTYE7HIMF6KK4BVEIXA";
            string _minioSecretKey = "naD+esQ+uV7+xwfF3bPfAn5iC7C1XUyXeM8HkBlO";
            string _defaultBucket = "finances";
            string _tempBucket = "erecipt";
            bool _withSSL = false;
            MinioClient minio;
            if (_withSSL)
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey).WithSSL();
            else
                minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey);

            var url = await minio.PresignedGetObjectAsync(bucket, FETFilePath, (int)TimeSpan.FromHours(_expireHours).TotalSeconds);
            //url = (!string.IsNullOrEmpty(_publicURL)) ? url.Replace(_minioEndpoint, _publicURL) : url;
            url = ReplaceWithPublicURL(url);

            return url;
        }
        public async Task<List<string>> GetListFile(string bucketName, string prefix)
        {
            try
            {
                string _minioEndpoint = "192.168.2.29:9001"; //192.168.2.29:9001"; // CRM 
                string _minioAccessKey = "XNTYE7HIMF6KK4BVEIXA";
                string _minioSecretKey = "naD+esQ+uV7+xwfF3bPfAn5iC7C1XUyXeM8HkBlO";
                string _defaultBucket = "erecipt";
                string _tempBucket = "erecipt";
                bool _withSSL = false;

                MinioClient minio;
                if (_withSSL)
                    minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey).WithSSL();
                else
                    minio = new MinioClient(_minioEndpoint, _minioAccessKey, _minioSecretKey);

                bool bucketExisted = await minio.BucketExistsAsync(bucketName);
                if (bucketExisted)
                {
                    List<string> bucketKeys = new List<string>();
                    var observable = minio.ListObjectsAsync(bucketName, prefix);
                    IDisposable subscription = observable.Subscribe(item => bucketKeys.Add(item.Key), ex => throw new Exception("Error", ex), () => Console.WriteLine("OnComplete: {0}"));
                    observable.Wait();

                    return bucketKeys;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }
        public string ReplaceWithPublicURL(string url)
        {
            string _minioEndpoint = "http://192.168.2.29:9001";
            //string _tempBucket = "timeattendence";
            if (!string.IsNullOrEmpty(_publicURL))
            {
                url = url.Replace("https://", "");
                url = url.Replace("http://", "");

                url = url.Replace(_minioEndpoint, _publicURL);
            }
            return url;
        }

    }

}
