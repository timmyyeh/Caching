using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public class UserService : IUserService
    {
        private static List<string> _users = new List<string>()
        {
            "abc",
            "cba",
            "aaa"
        };

        public List<string> GetUsers()
        {
            return _users;
        }
    }

    public interface IUserService
    {
        List<string> GetUsers();
    }
}