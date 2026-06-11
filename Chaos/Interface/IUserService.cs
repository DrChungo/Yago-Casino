using Chaos.Api.Models;
using Chaos.Api.RequestEntity;
using Chaos.Api.ResponseEntity;
using Chaos.Infraestructure.Models;


namespace Chaos.Api.Interface
{
    public interface IUserService
    {
        // Asynchronous Task to ADD a user into the Databse
        Task<UserResponse> CreateUser(UserRequest request);

        // Asynchronous Task to create "N" random users WITHOUT adding into Database
        Task<List<UserResponse>> CreateNUsers(int NumUsers);

        // List of REAL users in the Database
        List<UserResponse> GetAllUsers();

        // Single UserResponse given its ID
        UserResponse? GetUserById(Guid Id);

        // A true-or-false operation for transactions into Users Wallet attributes
        Task<bool> AddIntoWallet(Guid WalletId, long AddAmount);

        // Checks if an email (unique field in the Database) exists
        public Infraestructure.Models.User? GetUserByEmail(string email);

        // Marks the user as IsAlive = false
        public bool SendUserToGraveyard(Guid IdUser);

        
        List<User> SelectedUsers(List<Guid> idPlayers);
    }
}