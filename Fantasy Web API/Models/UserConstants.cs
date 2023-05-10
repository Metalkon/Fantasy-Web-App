namespace Fantasy_Web_API.Models
{
    public class UserConstants
    {
        // Temporary model for user accounts.

        public static List<UserModel> Users = new List<UserModel>()
        {
            new UserModel()
            {
                Username = "Metalkon",
                PasswordHash = "testpassword",
                Email = "metalkon@example.com",
                Role = "admin"
            },
            new UserModel()
            {
                Username = "TestUser",
                PasswordHash = "passwordtest",
                Email = "test@example.com",
                Role = "user"
            }
        };
    }
}
