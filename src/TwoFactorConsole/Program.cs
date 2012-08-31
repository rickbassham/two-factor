using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwoFactor;
using System.Security.Cryptography;

namespace TwoFactorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(HashedOneTimePassword.GeneratePassword("12345678901234567890", i));
            }

            long[] seconds = new long[] { 59, 1111111109, 1111111111, 1234567890, 2000000000, 20000000000 };

            foreach (var second in seconds)
            {
                Console.WriteLine(TimeBasedOneTimePassword.GetPassword("12345678901234567890", TimeBasedOneTimePassword.UNIX_EPOCH + TimeSpan.FromSeconds(second), TimeBasedOneTimePassword.UNIX_EPOCH, 30, 8));
            }

            Base32Encoder enc = new Base32Encoder();

            string secret = enc.Encode(Encoding.ASCII.GetBytes("1234567890"));

            Console.WriteLine(secret);

            Console.WriteLine("Enter your password: ");
            string password = Console.ReadLine();

            if (TimeBasedOneTimePassword.IsValid("1234567890", password))
            {
                Console.WriteLine("Success!");
            }
            else
            {
                Console.WriteLine("ERROR!");
            }

            return;

            while (true)
            {
                Console.WriteLine(TimeBasedOneTimePassword.GetPassword("1234567890"));
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(10));
            }
        }
        
    }
}
