using System.Threading.Tasks;

namespace Blazor.Startechmanager.Client.Services
{
    public interface IMessageDisplayer
    {
        ValueTask Display(string title, string message);

        public ValueTask DisplayErrorMessage(string errorMesage)
        {
            return Display("something occurs", $"an error occured: {errorMesage}");
        }
    }
}