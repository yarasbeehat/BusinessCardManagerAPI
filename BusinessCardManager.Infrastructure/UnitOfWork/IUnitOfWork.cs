using BusinessCardManager.Domain.Entities;
using BusinessCardManager.Domain.Interfaces.Repository;

namespace BusinessCardManager.Infrastructure.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBusinessCardRepository BusinessCardRepository { get; }
        IBaseRepository<Role> RoleRepository { get; }
        IBaseRepository<RoleUser> RoleUserRepository { get; }
        IUserRepository UserRepository { get; }
        Task Commit();
    }
}
