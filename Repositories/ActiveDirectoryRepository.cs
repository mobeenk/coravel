using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using CR_API.DB;

using activedirectory.Models;
using AttendanceNotification.Helper;
using Microsoft.Extensions.Configuration;

namespace CR_API.Repositories
{
    public interface IActiveDirectoryRepository
    {
        //Task<ADUser> GetADUserInfoByEmail(string email);
        //Task<ActiveDirectoryUser> GetADUserInfoByUserName(string userName);
        Task<List<ActiveDirectoryUser>> GetAllUsers();
        //Task<List<ADUser>> GetAllUsersSSO(string eMail, string flag);
        //Task<List<ADUser>> GetActiveDirectoryUsers();

        //Task<ResponseResult<List<ActiveDirectoryUser>>> EF_GetAllUsers();//EF way
    }
    public class ActiveDirectoryRepository : IActiveDirectoryRepository
    {

        private SQLDBHelper _entities;

        public readonly IConfiguration _configuration;
        public ActiveDirectoryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionStrings connections = configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>();
            _entities = new SQLDBHelper(connections.TBCDBConnection);
        }
  
        public async Task<List<ActiveDirectoryUser>> GetAllUsers()
        {
            try
            {
                var users =  await _entities.ExecuteToDataTable<ActiveDirectoryUser>("sp_GetADUsers").ConfigureAwait(false);
                return users;
            }
            catch (Exception ex)
            {
                return new List<ActiveDirectoryUser>(); ;
            }
            
        }

        //public async Task<object> GetUserInfoByEmail(string email)
        //{
        //    return await _entities.ExecuteSelectOne<object>("sp_GetUserInfoByEmail", new SqlParameter[] { new SqlParameter("@Email", email) }).ConfigureAwait(false);
        //}
        //// AD Logged in user
        //public async Task<ADUser> GetADUserInfoByEmail(string email)
        //{
        //    return await _entities.ExecuteSelectOne<ADUser>("sp_GetADUserInfoByEmail", new SqlParameter[] { new SqlParameter("@Email", email) }).ConfigureAwait(false);
        //}
        //public async Task<List<ADUser>> GetAllUsersSSO(string eMail, string flag)
        //{

        //    return await _entities.ExecuteToDataTable<ADUser>("sp_GetUsersDropdown",
        //        new SqlParameter[] { new SqlParameter("@Email", eMail), new SqlParameter("@Flag", flag) }
        //        )
        //        .ConfigureAwait(false);
        //}
    }
}
