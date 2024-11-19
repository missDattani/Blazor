using ClientLibrary.Services.Contracts;
using Microsoft.JSInterop;

namespace ClientLibrary.Services.Implementations
{
    public class ToastrService : IToastrService
    {
        private readonly IJSRuntime _jsRuntime;

        public ToastrService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }
        public async Task Error(string message, string title = "")
        {
            await _jsRuntime.InvokeVoidAsync("toastrFunctions.error", message, title);
        }

        public async Task Info(string message, string title = "")
        {
            await _jsRuntime.InvokeVoidAsync("toastrFunctions.info", message, title);
        }

        public async Task Success(string message, string title = "")
        {

            await _jsRuntime.InvokeVoidAsync("toastrFunctions.success", message, title);
        }

        public async Task Warning(string message, string title = "")
        {
            await _jsRuntime.InvokeVoidAsync("toastrFunctions.warning", message, title);
        }
    }
}
