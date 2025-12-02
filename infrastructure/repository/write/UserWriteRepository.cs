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

            try
            { 
                var passwordHash = await Connection.QueryFirstOrDefaultAsync<string>(query, new { username =  auth.User });
                return passwordHash ?? string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter hash da senha do usuário.", ex);
            }
        }

        public Task<int> GetRole(string user)
        {
            var query = $@"SELECT ACCESS_GROUP
                            FROM TB_USERS
                            WHERE USERNAME = @username";

            try
            {
                var role = Connection.QueryFirstOrDefaultAsync<int>(query, new { username = user });
                return role;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter o papel do usuário.", ex);
            }
        }
    }
}