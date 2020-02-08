using Architecture.CrossCutting;

namespace Architecture.Model
{
    public sealed class AuthModel
    {
        public AuthModel
        (
            string login,
            string password,
            string salt,
            Roles roles
        )
        {
            Login = login;
            Password = password;
            Salt = salt;
            Roles = roles;
        }

        public string Login { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        public Roles Roles { get; set; }
    }
}
