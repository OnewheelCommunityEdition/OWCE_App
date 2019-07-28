using System;
using System.Threading.Tasks;

namespace OWCE.DependencyInterfaces
{
    public interface IPermissionPrompt
    {
        Task<bool> PromptBLEPermission();
    }
}
