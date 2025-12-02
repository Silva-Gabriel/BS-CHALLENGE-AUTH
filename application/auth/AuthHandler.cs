using domain.interfaces.repository;
using domain.models;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using services;

namespace application.auth
{
    public class AuthHandler : IRequestHandler<AuthRequest, AuthResponse>
    {
        private IUserWriteRepository Repository { get; }
        
        private ILogger<AuthHandler> Logger { get; }

        private IAuthService AuthService { get; }

        private IConfiguration Configuration { get; }
        

        public AuthHandler(ILogger<AuthHandler> logger, IAuthService authService, IUserWriteRepository repository, IConfiguration configuration)
        {
            Logger = logger;
            AuthService = authService;
            Repository = repository;
            Configuration = configuration;
        }

        public async Task<AuthResponse> Handle(AuthRequest request, CancellationToken cancellationToken)
        {
            var response = new AuthResponse();

            var expiration = Configuration.GetValue<int>("jwtToken:expirationMinutes");
            var expirationDateTime = DateTime.UtcNow.AddMinutes(expiration);
            var authentication = new Authentication
            {
                User = request.User,
                Password = request.Password
            };

            try
            {
                var role = await Repository.GetRole(authentication.User);
                var token = AuthService.GenerateToken(authentication, role, Configuration.GetValue<string>("jwtToken:key"), expirationDateTime);
                var passwordHash = await Repository.GetPasswordHash(authentication, cancellationToken);


                if (passwordHash == null || passwordHash == string.Empty)
                {
                    Logger.LogWarning("Usuário ou senha inválidos!", request.User);
                    return response;
                }
                
                var authValidation = BCrypt.Net.BCrypt.Verify(request.Password, passwordHash);

                if (authValidation)
                {
                    Logger.LogInformation("Usuário autenticado com sucesso!");
                    response = new AuthResponse { Token = token };

                    return response;
                }

                return response;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"[{GetType().Name}] Erro ao autenticar usuário: {ex.Message}");
                return response;
            }
        }
    }
}