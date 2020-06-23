using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using com.apthai.CRMMobile.Model;

namespace com.apthai.CRMMobile.Repositories.Interfaces
{
    public interface ISyncRepository
    {
        Task<int> SendNotificationFromCRMWebToUser();
        List<Model.CRMMobile.NotificationTemp> GerAllApproveDirectCreditNotificationTemp();
        List<Model.CRMMobile.NotificationTemp> GerAllOverdueInstallmentsNotificationTemp();
        List<Model.CRMMobile.NotificationTemp> GerAllThanksForPayDownInstallmentsNotificationTemp();
    }
}