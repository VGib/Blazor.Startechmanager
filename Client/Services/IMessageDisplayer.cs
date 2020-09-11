using System.Threading.Tasks;

namespace Blazor.Startechmanager.Client.Services
{
    public interface IMessageDisplayer
    {
        ValueTask Display(string title, string message);
    }
}