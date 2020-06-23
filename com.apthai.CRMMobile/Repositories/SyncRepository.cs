using Dapper;
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
//using com.apthai.APTimeStamp.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Extensions;
//using com.apthai.APTimeStamp.CustomModel;
using Minio;
using com.apthai.CRMMobile.CustomModel;
using com.apthai.CRMMobile.Repositories.Interfaces;

namespace com.apthai.CRMMobile.Repositories
{
    public class SyncRepository : BaseRepository, ISyncRepository
    {

        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMobileMessagingClient _mobileMessagingClient;
        private readonly IUserRepository _userRepository;
        private int _expireHours = 24;
        public string _publicURL;
        // int ConnctionLongMaxQuerryTimeOut = 100;
        public SyncRepository(IHostingEnvironment environment, IConfiguration config,IMobileMessagingClient mobileMessagingClient,IUserRepository userRepository) : base(environment, config)
        {
            _config = config;
            _hostingEnvironment = environment;
            _mobileMessagingClient = mobileMessagingClient;
            _userRepository = userRepository;
        }
        
        public async Task<int> SendNotificationFromCRMWebToUser()
        {

            //string _minioEndpoint = "192.168.2.29:9600";
            //_minioAccessKey = "9QVUD2YBQMQ7GADI5YKQ";
            //_minioSecretKey = "a6YMuCIJ67XHyMnEcSJ5LQUwDOVa8hL5lWSVu8";
            //_defaultBucket = "timeattendence";
            //_tempBucket = "timeattendence";
            //_withSSL = false;
            try
            {
                List<Model.CRMMobile.NotificationTemp> notificationTemps = new List<Model.CRMMobile.NotificationTemp>();
                notificationTemps = GerAllApproveDirectCreditNotificationTemp();
                for (int i = 0; i < notificationTemps.Count(); i++)
                {
                    Model.CRMMobile.UserProfile userProfile = _userRepository.GetUserProfileByCRMContactID_Mobile(notificationTemps[i].CRMContactID);
                    List<Model.CRMMobile.UserLogin> userLogins = _userRepository.GetallUserLoginByID_Mobile(userProfile.UserProfileID);
                    if (userProfile != null)
                    {
                        for (int ii = 0; ii < userLogins.Count(); i++)
                        {
                            if (userProfile.Language.ToLower() == "en")
                            {
                                var sendresult = _mobileMessagingClient.SendNotification(userLogins[ii].FireBaseToken, notificationTemps[i].MsgHeaderTH, notificationTemps[i].MsgTH);
                            }
                            else
                            {
                                var sendresult = _mobileMessagingClient.SendNotification(userLogins[ii].FireBaseToken, notificationTemps[i].MsgHeaderTH, notificationTemps[i].MsgTH);
                            }
                        }
                    }
                    Model.CRMMobile.NotificationHistory notificationHistory = new Model.CRMMobile.NotificationHistory();
                    notificationHistory.BillingCreditNo = notificationTemps[i].BillingCreditNo;
                    notificationHistory.BillingDebitNo = notificationTemps[i].BillingDebitNo;
                    notificationHistory.Created = notificationTemps[i].Created;
                    notificationHistory.CreatedBy = notificationTemps[i].CreatedBy;
                    notificationHistory.CRMContactID = notificationTemps[i].CRMContactID;
                    notificationHistory.IsRead = false;
                    notificationHistory.MessageTitleENG = notificationTemps[i].MsgheaderEN;
                    notificationHistory.MessageTitleTH = notificationTemps[i].MsgHeaderTH;
                    notificationHistory.MsgEN = notificationTemps[i].MsgEN;
                    notificationHistory.MsgTH = notificationTemps[i].MsgTH;
                    notificationHistory.MsgType = notificationTemps[i].MsgType;
                    notificationHistory.ProjectNameTH = notificationTemps[i].ProjectNameTH;
                    notificationHistory.ProjectNo = notificationTemps[i].ProjectNo;
                    notificationHistory.ReceiptNo = notificationTemps[i].ReceiptNo;
                    notificationHistory.RefID = notificationTemps[i].RefID;
                    notificationHistory.SendMsgStatus = true;
                    notificationHistory.UnitNo = notificationTemps[i].UnitNo;
                    notificationHistory.Updated = notificationTemps[i].Updated;
                    notificationHistory.UpdatedBy = notificationTemps[i].UpdatedBy;

                    var InsertNoti = _userRepository.InsertNotificationHistory(notificationHistory);
                    var Deleteresult = _userRepository.DeleteNotificationTemp(notificationTemps[i]);
                }
                return 1;
            }
            catch (Exception ex)
            {

                throw;
                return 0;
            }
        }

        public async Task<int> TanonchaiJobSample()
        {
            int a = 20;
            int b = 60;
            int c = a + b;

            return c;
        }
        //public async Task<int> DeletePictureSchdule()
        //{
        //    var yearPath = DateTime.Now.Year;
        //    var MonthPath = DateTime.Now.Month;
        //    var date = DateTime.Now;
        //    var dirPath1 = $"{yearPath}/{MonthPath}";
        //    int dataPath = 0;
        //    var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "data", "uploads", dirPath1);
        //    string[] SubDirectory = Directory.GetDirectories(uploads);
        //    for (int i = 0; i < SubDirectory.Count(); i++)
        //    {
        //        string[] File = Directory
        //    }
        //    string[] files = Directory.GetFiles(dirName);

        //    foreach (string file in files)
        //    {
        //        FileInfo fi = new FileInfo(file);
        //        if (fi.LastAccessTime < DateTime.Now.AddMonths(-3))
        //            fi.Delete();
        //    }
        //    int a = 20;
        //    int b = 60;
        //    int c = a + b;

        //    return c;
        //}
        public async Task<FileUploadResult> UploadFileFromStreamWithOutGuid(Stream fileStream, string bucketName, string filePath, string fileName, string contentType)
        {
            MinioClient minio;

            string _minioEndpoint = "192.168.2.29:9600";
            string _minioAccessKey = "9QVUD2YBQMQ7GADI5YKQ";
            string _minioSecretKey = "a6YMuCIJ67XHyMnEcSJ5LQUwDOVa8hL5lWSVu8+v";
            string _defaultBucket = "timeattendence";
            string _tempBucket = "timeattendence";
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
        public string ReplaceWithPublicURL(string url)
        {
            string _minioEndpoint = "192.168.2.29:9600";
            string _tempBucket = "timeattendence";
            if (!string.IsNullOrEmpty(_publicURL))
            {
                url = url.Replace("https://", "");
                url = url.Replace("http://", "");

                url = url.Replace(_minioEndpoint, _publicURL);
            }
            return url;
        }

        public List<Model.CRMMobile.NotificationTemp> GerAllApproveDirectCreditNotificationTemp()
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "SELECT * from NotificationTemp where MsgType = 'ApproveDirectCredit'";
                    var result = conn.Query<Model.CRMMobile.NotificationTemp>(sQuery).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.getProjectInformation_CRMWeb() :: Error ", ex);
                }
            }
        }
        public List<Model.CRMMobile.NotificationTemp> GerAllOverdueInstallmentsNotificationTemp()
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "SELECT * from NotificationTemp where MsgType = 'OverdueInstallments'";
                    var result = conn.Query<Model.CRMMobile.NotificationTemp>(sQuery).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.getProjectInformation_CRMWeb() :: Error ", ex);
                }
            }
        }
        public List<Model.CRMMobile.NotificationTemp> GerAllThanksForPayDownInstallmentsNotificationTemp()
        {
            using (IDbConnection conn = WebConnection)
            {
                try
                {
                    string sQuery = "SELECT * from NotificationTemp where MsgType = 'ThanksForPayDownInstallments'";
                    var result = conn.Query<Model.CRMMobile.NotificationTemp>(sQuery).ToList();
                    return result;

                }
                catch (Exception ex)
                {
                    throw new Exception("MasterRepository.getProjectInformation_CRMWeb() :: Error ", ex);
                }
            }
        }
    }

}
