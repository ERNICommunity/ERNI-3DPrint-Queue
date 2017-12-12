using System.Security.Principal;

namespace ERNI.Q3D
{
    public interface IAdminProvider
    {
        bool IsAdmin(IIdentity identity);
    }
}