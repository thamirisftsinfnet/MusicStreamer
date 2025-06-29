using System.Text.Json;

namespace MusicStreamer.Web.Models
{
    public static class JwtHelper
    {
        public static string GetUserIdFromToken(string token) => GetClaim(token, "userId") ?? "0";
        public static string GetFirstNameFromToken(string token) => GetClaim(token, "firstName") ?? "Usuário";

        public static string? GetClaim(string token, string claimName)
        {
            var jwt = token.Split('.')[1];
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(PadBase64(jwt)));
            var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty(claimName, out var value))
                return value.GetString();
            return null;
        }

        private static string PadBase64(string base64) =>
            base64.Length % 4 == 0 ? base64 : base64.PadRight(base64.Length + (4 - base64.Length % 4), '=');
    }
}
