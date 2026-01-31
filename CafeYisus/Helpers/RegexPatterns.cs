namespace CafeYisus.Helpers
{
    public class RegexPatterns
    {
        public const string Email = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        public const string Username = @"^[a-zA-Z0-9]{5,20}$";
        public const string Password = @"^.{8,}$";
        //public const string Password = @"^(?=.*[A-Z])(?=.*\d).{8,}$";
    }
}
