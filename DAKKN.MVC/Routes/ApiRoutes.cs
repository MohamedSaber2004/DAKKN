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

        public static class Settings
        {
            public const string Get = Base + "/settings";
            public const string Update = Base + "/settings";
        }

        public static class Catalog
        {
            public const string Products = Base + "/catalog/products";
            public const string Categories = Base + "/catalog/categories";
            public const string FeaturedProducts = Base + "/catalog/products/featured";
            public const string PriceRange = Base + "/catalog/products/price-range";
        }

        public static class Cart
        {
            public const string Get = Base + "/cart";
            public const string Count = Base + "/cart/count";
            public const string Add = Base + "/cart/add";
            public const string Remove = Base + "/cart/remove/{productId}";
            public const string UpdateQuantity = Base + "/cart/update-quantity";
            public const string UpdateShipping = Base + "/cart/shipping";
        }

        public static class Shipping
        {
            public const string Governorates = Base + "/shipping/governorates";
        }

        public static class ProductRatings
        {
            public const string GetSummary = Base + "/products/{productId}/rating";
            public const string Rate = Base + "/products/{productId}/rating";
        }

        public static class StickerSuggestions
        {
            public const string Base    = "api/v{version:apiVersion}/sticker-suggestions";
            public const string My      = Base + "/my";
            public const string ById    = Base + "/{id:guid}";
            public const string Status  = Base + "/{id:guid}/status";
        }

        public static class BrandReviews
        {
            public const string My = Base + "/brand-reviews/my";
            public const string AdminAll = Base + "/brand-reviews/admin/all";
            public const string Displayed = Base + "/brand-reviews/displayed";
            public const string Create = Base + "/brand-reviews";
            public const string Update = Base + "/brand-reviews/{id}";
            public const string Delete = Base + "/brand-reviews/{id}";
            public const string Approve = Base + "/brand-reviews/{id}/approve";
            public const string Reject = Base + "/brand-reviews/{id}/reject";
            public const string ToggleDisplay = Base + "/brand-reviews/{id}/toggle-display";
            public const string UpdateDisplayOrder = Base + "/brand-reviews/{id}/display-order";
        }

        public static class Support
        {
            public const string Base = ApiRoutes.Base + "/support";
            public const string CreateTicket = Base + "/tickets";
            public const string MyTickets = Base + "/tickets/my";
            public const string TicketDetails = Base + "/tickets/{ticketId:guid}";
            public const string Reply = Base + "/tickets/{ticketId:guid}/reply";
            public const string Close = Base + "/tickets/{ticketId:guid}/close";
            public const string Reopen = Base + "/tickets/{ticketId:guid}/reopen";
            public const string UploadAttachment = Base + "/tickets/{ticketId:guid}/attachments";
            public const string DeleteAttachment = Base + "/attachments/{attachmentId:guid}";
            public const string AdminTickets = Base + "/admin/tickets";
            public const string AdminTicketDetails = Base + "/admin/tickets/{ticketId:guid}";
            public const string Assign = Base + "/admin/tickets/{ticketId:guid}/assign";
            public const string UpdateStatus = Base + "/admin/tickets/{ticketId:guid}/status";
            public const string UpdatePriority = Base + "/admin/tickets/{ticketId:guid}/priority";
            public const string AddNote = Base + "/admin/tickets/{ticketId:guid}/notes";
            public const string Escalate = Base + "/admin/tickets/{ticketId:guid}/escalate";
            public const string Archive = Base + "/admin/tickets/{ticketId:guid}/archive";
            public const string Dashboard = Base + "/admin/dashboard";
            public const string Categories = Base + "/admin/categories";
            public const string CategoryById = Base + "/admin/categories/{id:guid}";
            public const string FAQs = Base + "/admin/faqs";
            public const string FAQById = Base + "/admin/faqs/{id:guid}";
            public const string FAQCategories = Base + "/admin/faq-categories";
            public const string FAQCategoryById = Base + "/admin/faq-categories/{id:guid}";
            public const string Settings = Base + "/admin/settings";
            public const string PublicFAQs = Base + "/faqs";
            public const string PublicCategories = Base + "/categories";
            public const string PublicContact = Base + "/contact";
        }
    }
}
