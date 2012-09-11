using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Profile;
using System.Web.Security;

namespace TwoFactorWeb
{
    public class TwoFactorProfile : ProfileBase
    {
        public static TwoFactorProfile CurrentUser
        {
            get
            {
                return GetByUserName(Membership.GetUser().UserName);
            }
        }

        public static TwoFactorProfile GetByUserName(string username)
        {
            return (TwoFactorProfile)Create(username);
        }

        public DateTime? LastLoginAttemptUtc
        {
            get
            {
                try
                {
                    return (DateTime?)base["LastLoginAttemptUtc"];
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                base["LastLoginAttemptUtc"] = value;
                Save();
            }
        }

        public string TwoFactorSecret
        {
            get
            {
                return (string)base["TwoFactorSecret"];
            }
            set
            {
                base["TwoFactorSecret"] = value;
                Save();
            }
        }
    }
}