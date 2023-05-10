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
                Password = "testpassword",
                EmailAddress = "metalkon@example.com",
                Role = "admin"
            },
            new UserModel()
            {
                Username = "TestUser",
                Password = "passwordtest",
                EmailAddress = "test@example.com",
                Role = "user"
            }
        };
    }
}
