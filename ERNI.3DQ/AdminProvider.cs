using System.Collections.Generic;
using System.Security.Principal;

namespace ERNI.Q3D
{
    public class AdminProvider : IAdminProvider
    {
        private readonly HashSet<string> _admins = new HashSet<string>();

        public AdminProvider(string[] adminNames)
        {
            foreach (var name in adminNames)
            {
                _admins.Add(name);
            }
        }

        public bool IsAdmin(IIdentity identity)
        {
            return identity != null && _admins.Contains(identity.Name);
        }
    }
}
