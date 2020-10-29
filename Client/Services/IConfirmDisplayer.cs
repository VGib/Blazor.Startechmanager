using System.Threading.Tasks;

namespace Blazor.Startechmanager.Client.Services
{
    public interface IConfirmDisplayer
    {
        Task<bool> Confirm(string title, string message);
    }
}