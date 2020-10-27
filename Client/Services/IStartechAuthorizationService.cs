using Blazor.Startechmanager.Shared.Models;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Client.Services
{
    public interface IStartechAuthorizationService
    {
        Task<bool> IsMemberOrLeaderOf(Startechs startech, bool isLeader = true);
    }
}