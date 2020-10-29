using Blazor.Startechmanager.Client.Component;
using Blazored.Modal;
using Blazored.Modal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Client.Services
{
    public enum ConfirmDisplayerResult
    {
        Yes,
        No
    }

    public class ConfirmDisplayer : IConfirmDisplayer
    {
        private readonly IModalService modalService;

        public ConfirmDisplayer(IModalService modalService)
        {
            this.modalService = modalService;
        }

        public async Task<bool> Confirm(string title, string message)
        {
            var parameters = new ModalParameters();
            parameters.Add("Message", message);
            var options = new ModalOptions
            {
                HideCloseButton = true
            };
            var modal = modalService.Show<YesNoModal>(title, parameters, options);
            var result = await modal.Result;
            return result.DataType == typeof(int) && (int)result.Data == (int) ConfirmDisplayerResult.Yes;
        }
    }
}
