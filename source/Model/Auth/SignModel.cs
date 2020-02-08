using Architecture.CrossCutting;

namespace Architecture.Model
{
    public class SignModel
    {
        public SignModel
        (
            long id,
            string password,
            string salt,
            Roles roles
        )
        {
            UserId = id;
            Password = password;
            Salt = salt;
            Roles = roles;
        }

        public long UserId { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        public Roles Roles { get; set; }
    }
}
