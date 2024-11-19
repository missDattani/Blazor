

using BaseLibrary.Responses;

namespace ServerLibrary.Repositories.Contracts
{
    public interface IGenericRepositoryInterface<T>
    {
        Task<List<T>> GetAll();
        Task<T> GetById(string id);
        Task<GeneralResponse> Insert(T entity);
        Task<GeneralResponse> Update(T entity);
        Task<GeneralResponse> DeleteById(string id);
    }
}
