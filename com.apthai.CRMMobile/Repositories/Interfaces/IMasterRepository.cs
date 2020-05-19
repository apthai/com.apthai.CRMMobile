using com.apthai.CRMMobile.CustomModel;
using com.apthai.CRMMobile.Model;
using com.apthai.CRMMobile.Model.CRMMobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.apthai.CRMMobile.Repositories.Interfaces
{
    public interface IMasterRepository
    {
        List<GetProjectInformation> getProjectInformation_CRMWeb(string ProjectID);
        List<DocumentHeaderLevel1> GetAllDocumentHeaderLevel1();
        //List<calltype> GetCallCallType_Sync();
        //List<callarea> GetCallAreaByProductCat_Sync(string ProductTypeCate);
        //callTDefect GetCallTDefect_Sync(int TDefectID);
    }
}
