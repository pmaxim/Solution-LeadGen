using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BusinessLogic.Lib
{
    public static class WorkLib
    {
        private static string OnlyLetter(this string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : new string(str.Where(char.IsLetter).ToArray());
        }

        public static string OnlyNumber(this string str)
        {
            return string.IsNullOrEmpty(str) ? "0" : new string(str.Where(char.IsNumber).ToArray());
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static string AddBr(this string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            str = str.Trim();
            return str.Replace("\n", "<br>");
        }

        public static string DecimalTo2DotString(decimal? n)
        {
            return n == null ? string.Empty : $"{n:0.00}".Replace(",", ".");
        }

        public static async Task<string> GetHttp(string url)
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
