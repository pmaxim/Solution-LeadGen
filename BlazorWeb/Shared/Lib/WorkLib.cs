namespace BlazorWeb.Shared.Lib
{
    public static class WorkLib
    {
        private static string OnlyLetter(this string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : new string(str.Where(char.IsLetter).ToArray());
        }

        public static string OnlyGoodSymbols(this string str)
        {
            return str.Replace("'", "").Replace("&", "").Replace("-", "");
        }

        public static string OnlyNumber(this string str)
        {
            return string.IsNullOrEmpty(str) ? "0" : new string(str.Where(char.IsNumber).ToArray());
        }

        public static bool IsValidPhoneUsa(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return false;
            var phoneNumber = phone.OnlyNumber();
            if (string.IsNullOrEmpty(phoneNumber)) return false;
            return phoneNumber.Length is <= 11 and >= 10;
        }

        public static async Task<string> GetHttp(string url)
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public static string CorrectFloat(float f)
        {
            return f.ToString("0.0000");
        }
    }
}
