

namespace ClientLibrary.Services.Contracts
{
    public interface IToastrService
    {
        public Task Success(string message, string title = "");
        public Task Error(string message, string title = "");
        public Task Info(string message, string title = "");
        public Task Warning(string message, string title = "");
    }
}
