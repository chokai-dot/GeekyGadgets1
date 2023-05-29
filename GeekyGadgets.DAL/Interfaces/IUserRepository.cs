using GeekyGadgets.Domain.Entity;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekyGadgets.DAL.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<List<T>> ExecuteQueryAsync<T>(string query, params SqlParameter[] parameters);

        Task<T> ExecuteQueryFirstOrDefaultAsync<T>(string query, params SqlParameter[] parameters);
    }
}
