using RichardSzalay.MockHttp;
using System.Text.Json;

namespace Blazor.startechmanager.Client.UnitTests
{
    public static class MockHttpExtensions
    {
        public static MockedRequest RespondValues( this MockedRequest request, object obj )
        {
            return request.Respond("application/json", JsonSerializer.Serialize(obj));
        }
    }
}
