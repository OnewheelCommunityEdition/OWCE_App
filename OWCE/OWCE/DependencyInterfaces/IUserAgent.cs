using System;
using System.Threading.Tasks;

namespace OWCE.DependencyInterfaces
{
    public interface IUserAgent
    {
        Task<string> GetSystemUserAgent();
    }
}
