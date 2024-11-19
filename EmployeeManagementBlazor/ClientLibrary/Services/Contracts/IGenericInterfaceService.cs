

using BaseLibrary.Responses;

namespace ClientLibrary.Services.Contracts
{
    public interface IGenericInterfaceService<T>
    {
        Task<List<T>> GetAll(string baseUrl);
        Task<T> GetById(string id, string baseUrl);
        Task<GeneralResponse> Insert(T entity, string baseUrl);
        Task<GeneralResponse> Update(T entity, string baseUrl);
        Task<GeneralResponse> DeleteById(string id, string baseUrl);
    }
}
