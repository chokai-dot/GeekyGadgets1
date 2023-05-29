using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GeekyGadgets.DAL;
using GeekyGadgets.DAL.Interfaces;
using GeekyGadgets.Domain.Entity;
using GeekyGadgets.Domain.Enum;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace GeekyGadgets.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly string _connectionString;

        public UserRepository(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IQueryable<User> GetAll()
        {
            return _db.Users;
        }

        public async Task Delete(User entity)
        {
            _db.Users.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task Create(User entity)
        {
            await _db.Users.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<User> Update(User entity)
        {
            _db.Users.Update(entity);
            await _db.SaveChangesAsync();

            return entity;
        }

        public async Task<User> GetById(int id)
        {
            return await _db.Users.FindAsync(id);
        }

        public async Task<List<T>> ExecuteQueryAsync<T>(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        List<T> results = new List<T>();
                        while (await reader.ReadAsync())
                        {
                            T result = default(T);
                            if (typeof(T) == typeof(string))
                            {
                                result = (T)(object)reader.GetString(0);
                            }
                            else
                            {
                                result = reader.MapToObject<T>();
                            }
                            results.Add(result);
                        }

                        return results;
                    }
                }
            }
        }

        public async Task<T> ExecuteQueryFirstOrDefaultAsync<T>(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            T result = default(T);
                            if (typeof(T) == typeof(string))
                            {
                                result = (T)(object)reader.GetString(0);
                            }
                            else
                            {
                                result = reader.MapToObject<T>();
                            }
                            return result;
                        }
                        else
                        {
                            return default(T);
                        }
                    }
                }
            }
        }

    }

    public static class SqlDataReaderExtensions
        {
        public static T MapToObject<T>(this SqlDataReader reader)
        {
            T obj = Activator.CreateInstance<T>();

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                if (reader.HasColumn(property.Name))
                {
                    var value = reader[property.Name];
                    if (value != DBNull.Value)
                    {
                        if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(obj, value.ToString());
                        }
                        else if (property.PropertyType == typeof(Role))
                        {
                            // Convert integer value to Role enumeration
                            var roleValue = (int)value;
                            var role = (Role)Enum.Parse(typeof(Role), roleValue.ToString());
                            property.SetValue(obj, role);
                        }
                        else
                        {
                            property.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
                        }
                    }
                    else if (property.PropertyType == typeof(string))
                    {
                        property.SetValue(obj, null);
                    }
                }
            }

            return obj;
        }






    }

    public static class Hascolumn
    {
        public static bool HasColumn(this SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
