using APIManytoMany.Application;
using APIManytoMany.Domain;

namespace Infra
{
    public class PostService
    {
        private readonly IUserPost<Post, int> _pstrepo;

        public PostService(IUserPost<Post, int> pstrepo)
        {
            _pstrepo = pstrepo;
        }

        public async Task<IEnumerable<Post>> GetAllUser()
        {
            return await _pstrepo.GetAll();
        }

        public async Task<Post> GetUserById(int id)
        {
            return await _pstrepo.GetById(id);
        }

        public async Task<Post> AddPst(Post post)
        {
            return await _pstrepo.AddPost(post);
        }

        public async Task<Post?> Update(int id,Post post)
        {
            return await _pstrepo.Update(id,post);
        }

        public async Task<bool> DeletePst(int id)
        {
            return await _pstrepo.Delete(id);
        }

        
    }
}
