using System.Data;
using Dapper;
using domain.interfaces.repository;
using domain.models;

namespace infrastructure.repository
{
    public class UserWriteRepository : IUserWriteRepository
    {
        private IDbConnection Connection { get; }

        public UserWriteRepository(IDbConnection connection)
        {
            Connection = connection;
        }

        public async Task<string> GetPasswordHash(Authentication auth, CancellationToken cancellationToken)
        {
            var query = $@"SELECT PASSWORD_HASH 
                            FROM TB_USERS
                            WHERE USERNAME = @username";

            var passwordHash = await Connection.QueryFirstOrDefaultAsync<string>(query, new { username =  auth.User });

            return passwordHash ?? string.Empty;
        }

        public Task<int> GetRole(string user)
        {
            var query = $@"SELECT ACCESS_GROUP
                            FROM TB_USERS
                            WHERE USERNAME = @username";

            var role = Connection.QueryFirstOrDefaultAsync<int>(query, new { username = user });
            return role;
        }
    }
}