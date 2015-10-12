using System;
using System.Collections.Generic;

namespace TwoFactor
{
    public static class TimeBasedOneTimePassword
    {
        public static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static Dictionary<string, DateTime> _cache;

        static TimeBasedOneTimePassword()
        {
            _cache = new Dictionary<string, DateTime>();
        }

        public static string GetPassword(string secret)
        {
            return GetPassword(secret, GetCurrentCounter());
        }

        public static string GetPassword(string secret, DateTime epoch, int timeStep)
        {
            long counter = GetCurrentCounter(DateTime.UtcNow, epoch, timeStep);

            return GetPassword(secret, counter);
        }

        public static string GetPassword(string secret, DateTime now, DateTime epoch, int timeStep, int digits)
        {
            long counter = GetCurrentCounter(now, epoch, timeStep);

            return GetPassword(secret, counter, digits);
        }

        private static string GetPassword(string secret, long counter, int digits = 6)
        {
            return HashedOneTimePassword.GeneratePassword(secret, counter, digits);
        }

        private static long GetCurrentCounter()
        {
            return GetCurrentCounter(DateTime.UtcNow, UNIX_EPOCH, 30);
        }

        private static long GetCurrentCounter(DateTime now, DateTime epoch, int timeStep)
        {
            return (long)(now - epoch).TotalSeconds / timeStep;
        }

        private static void CleanCache()
        {
            List<string> keysToRemove = new List<string>(_cache.Count);

            foreach (KeyValuePair<string, DateTime> pair in _cache)
            {
                if ((DateTime.Now - pair.Value).TotalMinutes > 2)
                {
                    keysToRemove.Add(pair.Key);
                }
            }

            foreach (string key in keysToRemove)
            {
                _cache.Remove(key);
            }
        }

        public static bool IsValid(string secret, string password, int checkAdjacentIntervals = 1)
        {
            // Keeping a cache of the secret/password combinations that have been requested allows us to
            // make this a real one time use system. Once a secret/password combination has been tested,
            // it cannot be tested again until after it is no longer valid.
            // See http://tools.ietf.org/html/rfc6238#section-5.2 for more info.

            CleanCache();

            string cache_key = string.Format("{0}_{1}", secret, password);

            if (_cache.ContainsKey(cache_key))
            {
                throw new OneTimePasswordException("You cannot use the same secret/iterationNumber combination more than once.");
            }

            _cache.Add(cache_key, DateTime.Now);

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