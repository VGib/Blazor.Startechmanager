using Blazor.Startechmanager.Client.Component;
using Blazored.Modal;
using Blazored.Modal.Services;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Client.Services
{
    public class MessageDisplayer : IMessageDisplayer
    {
        private readonly IModalService modalService;

        public MessageDisplayer(IModalService modalService)
        {
            this.modalService = modalService;
        }

        public async ValueTask Display(string title, string message)
        {
            var parameters = new ModalParameters();
            parameters.Add("Message", message);
            var modal = modalService.Show<DisplayMessage>(title, parameters);
            await modal.Result;
        }
    }
}
