using System.Security.Claims;
using System.Text.Json;

namespace Fantasy_Blazor
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // temp token expires june 12
            string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtZXRhbGtvbnNjQGhvdG1haWwuY29tIiwidXNlcm5hbWUiOiJNZXRhbGtvbiIsInJvbGUiOiJBZG1pbiIsImV4cCI6MTY4NTk2NzczMSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzAwMCIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcwMDAifQ.R6J5mEqi6I28ShQ6JNVr4VHiYZwWvTNEIYVYacOOC98";


            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            //var identity = new ClaimsIdentity();

            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);

            NotifyAuthenticationStateChanged(Task.FromResult(state));

;            return state;
        }

        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
