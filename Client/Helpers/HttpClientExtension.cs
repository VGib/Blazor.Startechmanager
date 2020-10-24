using Blazor.Startechmanager.Client.Services;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Client.Helpers
{
    public static class HttpClientExtension
    {
        public static async Task DoActionByGetMethod(this HttpClient httpClient, string actionUrl, IMessageDisplayer messageDisplayer  )
        {
            var response = await httpClient.GetAsync(actionUrl);
            await TreatResponse(response, messageDisplayer);
        }

        public static async Task DoActionByPost<T>(this HttpClient httpClient, string actionUrl, T value, IMessageDisplayer messageDisplayer )
        {
            var response = await httpClient.PostAsJsonAsync(actionUrl, value );
            await TreatResponse(response, messageDisplayer);
        }

        private static async Task TreatResponse(HttpResponseMessage response, IMessageDisplayer messageDisplayer)
        {
            if (!response?.IsSuccessStatusCode ?? false)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                await messageDisplayer.DisplayErrorMessage(await response.Content.ReadAsStringAsync());
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
        }
    }
}
