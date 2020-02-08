using Architecture.CrossCutting;
using DotNetCore.Domain;
using System.Collections.Generic;

namespace Architecture.Domain
{
    public sealed class Auth : ValueObject
    {
        public Auth
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

        public string Login { get; }

        public string Password { get; }

        public string Salt { get; }

        public Roles Roles { get; }

        protected override IEnumerable<object> GetEquals()
        {
            yield return Login;
            yield return Password;
            yield return Salt;
            yield return Roles;
        }
    }
}
