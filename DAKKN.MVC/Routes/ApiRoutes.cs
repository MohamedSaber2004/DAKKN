namespace DAKKN.Appearence.Routes
{
    public static class ApiRoutes
    {
        public const string Base = "api/v{version:apiVersion}";

        public static class Translation
        {
            public const string Get = Base + "/translations";
        }

        public static class Attachments
        {
            public const string UploadImage = Base + "/attachments/upload-image";
            public const string UploadMultiImage = Base + "/attachments/upload-multi-image";
            public const string UpdateImage = Base + "/attachments/update-image";
        }

        public static class Auth
        {
            public const string Signup = Base + "/auth/signup";
            public const string Login = Base + "/auth/login";
            public const string Logout = Base + "/auth/logout";
            public const string ForgetPassword = Base + "/auth/forget-password";
            public const string VerifyForgetPasswordOtp = Base + "/auth/verify-forget-password-otp";
            public const string ResetPassword = Base + "/auth/reset-password";
            public const string LoginWithGoogle = Base + "/auth/login-google";
        }

        public static class Users
        {
            public const string GetAll = Base + "/users";
            public const string GetById = Base + "/users/{id}";
            public const string Create = Base + "/users";
            public const string Update = Base + "/users/{id}";
            public const string Delete = Base + "/users/{id}";
            public const string ChangePassword = Base + "/users/{id}/change-password";
        }
    }
}
