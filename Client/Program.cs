using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.Startechmanager.Client.Services;
using Blazor.Startechmanager.Shared.Policies;
using Blazored.Modal;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.Startechmanager.Client
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddHttpClient("Blazor.Startechmanager.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Blazor.Startechmanager.ServerAPI"));

            builder.Services.AddApiAuthorization();
            builder.Services.AddAuthorizationCore(configure => configure.AddAppicationPolicies());
            builder.Services.AddBlazoredModal();

            builder.Services.AddScoped<IMessageDisplayer, MessageDisplayer>();
            builder.Services.AddScoped<IConfirmDisplayer, ConfirmDisplayer>();
            builder.Services.AddScoped<IStartechAuthorizationService, StartechAuthorizationService>();
            builder.Services.AddStartechPoliciesHandler();

            await builder.Build().RunAsync();
        }
    }
}
