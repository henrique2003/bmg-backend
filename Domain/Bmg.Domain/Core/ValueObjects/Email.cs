using CSharpFunctionalExtensions;
using System.Text.RegularExpressions;

namespace Bmg.Domain.Core.ValueObjects
{
    public class Email : ValueObject<Email>
    {
        private const string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        private Email(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        protected override bool EqualsCore(Email other)
        {
            return other.Value == Value;
        }

        public static Result<Email> Create(string email)
        {
            if (!Validate(email))
            {
                return Result.Failure<Email>("Email inválido");
            }

            var result = new Email(email);

            return Result.Success(result);
        }

        public static Result<Email> CreateForTest(string email)
        {
            if (!Validate(email))
            {
                return Result.Failure<Email>("Email inválido");
            }

            var result = new Email(email);

            return Result.Success(result);
        }

        public static Result<Email> CreateForTest()
        {
            var result = new Email(string.Empty);

            return Result.Success(result);
        }

        protected override int GetHashCodeCore()
        {
            return Value.GetHashCode();
        }

        private static bool Validate(string email)
        {
            return Regex.IsMatch(email, pattern);
        }

        public static implicit operator string(Email email)
        {
            return email.Value;
        }

        public Email Clone()
        {
            return (Email)MemberwiseClone();
        }
    }
}
