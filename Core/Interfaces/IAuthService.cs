using MSLauncher.Core.Entities;
using System.Threading.Tasks;

namespace MSLauncher.Core.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(string username, string password);
        Task<User> LoginAsync(string username, string password);
    }
}