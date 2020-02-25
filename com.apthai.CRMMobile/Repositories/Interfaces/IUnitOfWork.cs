using com.apthai.CRMMobile.Repositories.Interfaces;

namespace com.apthai.CRMMobile.Repositories
{
    public interface IUnitOfWork
    {
        IMasterRepository MasterRepository { get; }
        //ISyncRepository SyncRepository { get; }
        IUserRepository UserRepository { get; }
        void Save();
    }
}