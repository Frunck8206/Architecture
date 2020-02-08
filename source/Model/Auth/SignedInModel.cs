using Architecture.CrossCutting;

namespace Architecture.Model
{
    public class SignedInModel
    {
        public long UserId { get; set; }

        public Roles Roles { get; set; }
    }
}
