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
            public const string Base = ApiRoutes.Base + "/attachments";
            public const string Upload = "";
        }
    }
}
