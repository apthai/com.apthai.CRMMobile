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
        Model.CRMWeb.ContactPhone GetSingleContactPhoneNumberByContactID_Web(string ContactID, string PhoneNumber);
        bool InsertCSUserProfile(Model.CRMMobile.UserProfile data);
        bool InsertCSUserLogin(Model.CRMMobile.UserLogin data);
        // Task<vwUser> GetUser(string userId);
        //AccessKeyControl GetUserAccessKey(string EmpCode);
        //bool InsertUserAccessKey(AccessKeyControl AC);
        //bool UpdateUserAccessKey(AccessKeyControl AC);
        //AccessKeyControl CheckUserAccessKey(string EmpCode, string AccessKey);
        //Task<List<vwUser>> GetAllUser();
        // Model.QISAuth.vwUser GetUserData(int UserID);

    }
}