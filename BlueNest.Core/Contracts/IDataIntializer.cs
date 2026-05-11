using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Core.Contracts
{
    public interface IDataIntializer
    {
        Task InitializeAdminAndRoleAsync();
    }
}
