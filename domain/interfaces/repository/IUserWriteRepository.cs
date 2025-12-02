using domain.models;

namespace domain.interfaces.repository
{
    public interface IUserWriteRepository
    {
        Task<string> GetPasswordHash(Authentication auth, CancellationToken cancellationToken);
        Task<int> GetRole(string user);
    }
}