using BusinessCardManager.Domain.Entities;
using BusinessCardManager.Domain.Interfaces.Repository;
using BusinessCardManager.Infrastructure.Context;
using BusinessCardManager.Infrastructure.Interfaces;
using BusinessCardManager.Infrastructure.Repositories;

namespace BusinessCardManager.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BusinessCardManagerContext _context;
        private IBaseRepository<BusinessCard> _businessCardRepository;
        private IBaseRepository<Role> _roleRepository;
        private IBaseRepository<RoleUser> _roleUserRepository;
        private IBaseRepository<User> _userRepository;

        public UnitOfWork(BusinessCardManagerContext context)
        {
            _context = context;           
        

         BusinessCardRepository = new BusinessCardRepository(_context);
         UserRepository = new UserRepository(_context);
        }

        public IBusinessCardRepository BusinessCardRepository { get; }
        public IBaseRepository<Role> RoleRepository =>
            _roleRepository ??= new BaseRepository<Role>(_context);
        
        public IBaseRepository<RoleUser> RoleUserRepository =>
            _roleUserRepository ??= new BaseRepository<RoleUser>(_context);

        public IUserRepository UserRepository { get; }


        public async Task Commit()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

    }   
}
