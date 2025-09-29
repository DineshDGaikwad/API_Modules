using APIManytoMany.Application;
using APIManytoMany.Domain;

namespace Infra
{
    public class UserService
    {
        private readonly IUserPost<User, int> _usrrepo;

        public UserService(IUserPost<User, int> usrrepo)
        {
            _usrrepo = usrrepo;
        }

        public async Task<IEnumerable<User>> GetAllUser()
        {
            return await _usrrepo.GetAll();
        }

        public async Task<User> GetUserById(int id)
        {
            return await _usrrepo.GetById(id);
        }

        public async Task<User> AddUsr(User user)
        {
            return await _usrrepo.AddUser(user);
        }

        public async Task<User?> Update(int id,User user)
        {
            return await _usrrepo.Update(id,user);
        }

        public async Task<bool> DeleteUsr(int id)
        {
            return await _usrrepo.Delete(id);
        }

        
    }
}
