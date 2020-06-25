﻿using com.apthai.CRMMobile.CustomModel;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace com.apthai.CRMMobile.Repositories
{
    public interface IUserRepository
    {
        Model.CRMWeb.Contact GetCRMContactByIDCardNO(string CitizenIdentityNo);
        Model.CRMWeb.Contact GetCRMContactByID(string ID);
        List<Model.CRMMobile.NotificationHistory> GetUserNotificationHistoryByCRMContactID_Mobile(string CRMContactID);
        Model.CRMMobile.NotificationHistory GetUserNotificationHistoryByNotiHistoryID_Mobile(string NotiHistory);
        List<Model.CRMWeb.TransferOwner> GetTransferOwnerByIDCardNO(string CitizenIdentityNo);
        Model.CRMWeb.Transfer GetTransferByID(string TransferID);
        Model.CRMWeb.Unit GetUnitByID(string ID);
        Model.CRMWeb.Project GetProjectByID(string ID);
        Model.CRMWeb.Floor GetFloorByID(string ID);
        List<Model.CRMWeb.ContactPhone> GetContactPhoneNumberByContactID_Web(string ContactID);
        Model.CRMWeb.ContactPhone GetContactPhoneNumberByContactIDandPhonNumber_Web(string ContactID, string PhoneNumber);
        Model.CRMWeb.ContactPhone GetSingleContactPhoneNumberByContactID_Web(string ContactID, string PhoneNumber);
        VerifyPINReturnObj GetUserLogin_Mobile(string UserToken);
        Model.CRMMobile.UserLogin GetUserLoginByID_Mobile(int UserLoginID);
        List<Model.CRMMobile.UserLogin> GetallUserLoginByID_Mobile(int UserLoginID);
        Model.CRMMobile.UserLogin GetUserLoginByPhoneNumbandDevice_Mobile(string DeviceID, string UserPhoneNumber);
        Model.CRMMobile.UserProfile GetUserProfileByCRMContactID_Mobile(string CRMContactID);
        bool UpdateChangePINCSUserProfile(Model.CRMMobile.UserProfile data);
        bool UpdateIsReadForNotification(Model.CRMMobile.NotificationHistory data);
        bool InsertCSUserProfile(Model.CRMMobile.UserProfile data, out long ProfileID);
        bool InsertCSUserLogin(Model.CRMMobile.UserLogin data);
        bool DeleteNotificationTemp(Model.CRMMobile.NotificationTemp data);
        bool InsertNotificationHistory(Model.CRMMobile.NotificationHistory data);
        bool InsertSCBTransaction(Model.CRMMobile.PaymentTransaction data);
        bool UpdateCSUserLogin(Model.CRMMobile.UserLogin data);
        bool UpdateCSUserProfile(Model.CRMMobile.UserProfile data);
        List<iCRMBooking> GetUseriBookingByUserID(string UserID);
        List<iCRMMyProperty> GetUseriCRMMyPropoty(string ContactNo);
        List<GetBillingTrackingMobile> GetUserBillingTrackingByProjectandUnit(string ProjectNo, string UnitNo);
        List<iCRMContact> GetUseriCRMContact_Web(string Contact);
        List<GetUserCardReturnObj> GetUserCardByProjectandUnit(string ContactNo);
        List<GetiCRMOwnerReturnObj> GetUserICRMOwnerByProjectUnitAndCRMContactID(string ContactID, string UnitNo, string ProjectNO);
        GetUserCreditCardReturnObj GetUserCreditCardByProjectandUnit(string ProjectID, string UnitID);
        Task<string> GetFileUrlAsync(string name);
        Task<string> GetFileUrlAsync(string bucket, string ReceiptNo, string name);
    }
}