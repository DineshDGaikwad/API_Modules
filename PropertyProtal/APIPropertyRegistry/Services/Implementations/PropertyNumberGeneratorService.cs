using System;
using System.Text;

namespace APIPropertyRegistry.Services.Implementations
{
    public class PropertyNumberGeneratorService
    {
        private static readonly Random _random = new Random();

        public string GeneratePropertyNumber()
        {
            const string prefix = "PU";
            var randomDigits = GenerateRandomDigits(5);
            var randomSuffix = GenerateRandomSuffix(3);

            return $"{prefix}{randomDigits}{randomSuffix}";
        }

        private static string GenerateRandomDigits(int length)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(_random.Next(0, 10));
            }
            return sb.ToString();
        }

        private static string GenerateRandomSuffix(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[_random.Next(chars.Length)]);
            }
            return sb.ToString();
        }
    }
}
