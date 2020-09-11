using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Client.Services
{
    public class MessageDisplayer : IMessageDisplayer
    {
        private readonly IJSRuntime JSRuntime;

        public MessageDisplayer(IJSRuntime jSRuntime)
        {
            JSRuntime = jSRuntime;
        }

        public ValueTask Display(string title, string message)
        {
            return JSRuntime.InvokeVoidAsync("openModalWithMessage", title, message);
        }
    }
}
