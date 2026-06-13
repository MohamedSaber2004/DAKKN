namespace DAKKN.Application.Localization
{
    public static class LocalizationKeys
    {
        public static class Attachment
        {
            public static readonly KeyString AttachmentNotFound = new("attachment.not_found");
        }

        public static class ExceptionMessages
        {
            public static readonly KeyString Validation = new("exception.validation");
            public static readonly KeyString InvalidModelState = new("exception.invalid_model_state");
            public static readonly KeyString NotFound = new("exception.not_found");
            public static readonly KeyString BadRequest = new("exception.bad_request");
            public static readonly KeyString Unauthorized = new("exception.unauthorized");
            public static readonly KeyString UnknownException = new("exception.unknown");
        }

        public static class UploadFileMessages
        {
            public static readonly KeyString Requried = new("uploadfile.required");
            public static readonly KeyString FileNotValid = new("uploadfile.filenotvalid");
            public static readonly KeyString FileUploadFailed = new("uploadfile.fileuploadfailed");
            public static readonly KeyString InvalidContentType = new("uploadfile.invalidcontenttype");
            public static readonly KeyString FileNotFound = new("uploadfile.filenotfound");
            public static readonly KeyString PlaceRequried = new("uploadfile.placerequired");
            public static readonly KeyString PlaceNotValid = new("uploadfile.placenotvalid");
            public static readonly KeyString FileFailedToDeleted = new("uploadfile.filefailedtodeleted");
        }

        public static class ActionResultMessage
        {
            public static readonly KeyString Ok = new("actionresult.ok");
            public static readonly KeyString Created = new("actionresult.created");
            public static readonly KeyString Accepted = new("actionresult.accepted");
            public static readonly KeyString Deleted = new("actionresult.deleted");
        }

        public static class AuthMessages
        {
            public static readonly KeyString PasswordMismatch = new("auth.password_mismatch");
            public static readonly KeyString WeakPassword = new("auth.weak_password");
            public static readonly KeyString EmailFoundBefore = new("auth.email_found_before");
            public static readonly KeyString InvalidCredentials = new("auth.invalid_credentials");
            public static readonly KeyString InvalidEmail = new("auth.invalid_email");
            public static readonly KeyString RefreshTokenInvalid  = new("auth.refresh_token_invalid");
        }

        public static class ValidationMessages
        {
            public static readonly KeyString Required = new("validation.required");
            public static readonly KeyString MinLength = new("validation.min_length");
        }
    }
}
