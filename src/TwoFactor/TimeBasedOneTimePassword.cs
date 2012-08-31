using System;

namespace TwoFactor
{
    public static class TimeBasedOneTimePassword
    {
        public static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static string GetPassword(string secret)
        {
            return GetPassword(secret, GetCurrentCounter());
        }

        public static string GetPassword(string secret, DateTime epoch, int timeStep)
        {
            long counter = GetCurrentCounter(DateTime.UtcNow, epoch, timeStep);

            return HashedOneTimePassword.GeneratePassword(secret, counter);
        }

        public static string GetPassword(string secret, DateTime now, DateTime epoch, int timeStep, int digits)
        {
            long counter = GetCurrentCounter(now, epoch, timeStep);

            return HashedOneTimePassword.GeneratePassword(secret, counter, digits);
        }

        private static string GetPassword(string secret, long counter)
        {
            return HashedOneTimePassword.GeneratePassword(secret, counter);
        }

        private static long GetCurrentCounter()
        {
            return GetCurrentCounter(DateTime.UtcNow, UNIX_EPOCH, 30);
        }

        private static long GetCurrentCounter(DateTime now, DateTime epoch, int timeStep)
        {
            return (long)(now - epoch).TotalSeconds / timeStep;
        }

        public static bool IsValid(string secret, string password, int checkAdjacentIntervals = 1)
        {
            if (password == GetPassword(secret))
                return true;

            for (int i = 1; i <= checkAdjacentIntervals; i++)
            {
                if (password == GetPassword(secret, GetCurrentCounter() + i))
                    return true;

                if (password == GetPassword(secret, GetCurrentCounter() - i))
                    return true;
            }

            return false;
        }
    }
}