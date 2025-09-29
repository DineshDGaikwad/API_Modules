namespace APIManytoMany.Application
{
    public interface IUserPost<TEntity, TKey>
        where TEntity : class
        where TKey : notnull, IEquatable<TKey>
    {
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> GetById(TKey id);
        Task<TEntity> AddUser(TEntity entity);
        Task<TEntity> Update(int id,TEntity entity);
        Task<bool> Delete(TKey id);

    }
}
