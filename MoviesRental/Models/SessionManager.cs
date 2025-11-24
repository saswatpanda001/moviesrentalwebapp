using Microsoft.AspNetCore.Http;
using MoviesRental.Models;

namespace MoviesRental.Controllers
{
    public static class SessionManager
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static bool IsLoggedIn => _httpContextAccessor?.HttpContext?.Session.GetInt32("UserId") != null;

        public static string UserRole => _httpContextAccessor?.HttpContext?.Session.GetString("UserRole") ?? "";

        public static string UserName => _httpContextAccessor?.HttpContext?.Session.GetString("UserName") ?? "";

        public static int? UserId => _httpContextAccessor?.HttpContext?.Session.GetInt32("UserId");

        // Helper method to set session data
        public static void SetUserSession(HttpContext context, User user)
        {
            context.Session.SetInt32("UserId", user.UserId);
            context.Session.SetString("UserName", user.Name);
            context.Session.SetString("UserRole", user.Role);
            context.Session.SetString("UserEmail", user.Email);
        }

        // Helper method to clear session
        public static void ClearSession(HttpContext context)
        {
            context.Session.Clear();
        }
    }
}