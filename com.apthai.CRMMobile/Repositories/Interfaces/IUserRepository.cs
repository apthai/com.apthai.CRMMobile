using com.apthai.CRMMobile.CustomModel;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace com.apthai.CRMMobile.Repositories
{
    public interface IUserRepository
    {
        Model.CRMWeb.Contact GetCRMContactByIDCardNO(string CitizenIdentityNo);
        List<Model.CRMWeb.ContactPhone> GetContactPhoneNumberByContactID_Web(string ContactID);
        Model.CRMWeb.ContactPhone GetContactPhoneNumberByContactIDandPhonNumber_Web(string ContactID, string PhoneNumber);
        Model.CRMWeb.ContactPhone GetSingleContactPhoneNumberByContactID_Web(string ContactID, string PhoneNumber);
        VerifyPINReturnObj GetUserLogin_Mobile(string UserToken);
        Model.CRMMobile.UserLogin GetUserLoginByID_Mobile(int UserLoginID);
        bool InsertCSUserProfile(Model.CRMMobile.UserProfile data, out long ProfileID);
        bool InsertCSUserLogin(Model.CRMMobile.UserLogin data);
        bool UpdateCSUserLogin(Model.CRMMobile.UserLogin data);
        

    }
}