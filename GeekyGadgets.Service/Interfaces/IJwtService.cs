using GeekyGadgets.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekyGadgets.Service.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }

}
