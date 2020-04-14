using com.apthai.CRMMobile.CustomModel;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace com.apthai.CRMMobile.Repositories
{
    public interface IUserRepository
    {
        Model.CRMWeb.Contact GetCRMContactByIDCardNO(string CitizenIdentityNo);
        Model.CRMWeb.Contact GetCRMContactByID(string ID);
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
        Model.CRMMobile.UserLogin GetUserLoginByPhoneNumbandDevice_Mobile(string DeviceID, string UserPhoneNumber);
        Model.CRMMobile.UserProfile GetUserProfileByCRMContactID_Mobile(string CRMContactID);
        bool UpdateChangePINCSUserProfile(Model.CRMMobile.UserProfile data);
        bool InsertCSUserProfile(Model.CRMMobile.UserProfile data, out long ProfileID);
        bool InsertCSUserLogin(Model.CRMMobile.UserLogin data);
        bool UpdateCSUserLogin(Model.CRMMobile.UserLogin data);
        bool UpdateCSUserProfile(Model.CRMMobile.UserProfile data);
        List<iCRMBooking> GetUseriBookingByUserID(string UserID);
        List<GetBillingTrackingMobile> GetUserBillingTrackingByProjectandUnit(string ProjectID, string UnitID);
        List<iCRMContact> GetUseriCRMContact_Web(string Contact);
        GetUserCardReturnObj GetUserCardByProjectandUnit(string ProjectID, string UnitID);
        GetUserCreditCardReturnObj GetUserCreditCardByProjectandUnit(string Key, string ProjectID, string UnitID);

    }
}