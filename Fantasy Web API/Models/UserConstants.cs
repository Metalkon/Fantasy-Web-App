namespace Fantasy_Web_API.Models
{
    public class UserConstants
    {
        public static List<UserModel> Users = new List<UserModel>()
        {
            new UserModel()
            {
                Username = "Metalkon",
                Password = "password2",
                EmailAddress = "metalkonsc@hotmail.com",
                Role = "admin"
            },
            new UserModel()
            {
                Username = "TestUser",
                Password = "password1",
                EmailAddress = "clanfirewing@yahoo.com",
                Role = "user"
            }
        };
    }
}
