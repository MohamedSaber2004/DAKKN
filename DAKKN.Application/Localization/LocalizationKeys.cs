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
            public static readonly KeyString LogoutSuccess = new("auth.logout_success");
            public static readonly KeyString RefreshTokenRequired = new("auth.refresh_token_required");
            public static readonly KeyString UserNotFound = new("auth.user_not_found");
            public static readonly KeyString ResetTokenInvalid = new("auth.reset_token_invalid");
            public static readonly KeyString PasswordSameAsOld = new("auth.password_same_as_old");
            public static readonly KeyString InvalidGoogleToken = new("auth.invalid_google_token");
            public static readonly KeyString GoogleEmailRequired = new("auth.google_email_required");
            public static readonly KeyString GoogleUserCreationFailed = new("auth.google_user_creation_failed");
            public static readonly KeyString GoogleTokenRequired = new("auth.google_token_required");
            public static readonly KeyString PhoneNumberRequired = new("auth.phone_number_required");
            public static readonly KeyString PhoneNumberFoundBefore = new("auth.phone_number_found_before");
        }

        public static class ValidationMessages
        {
            public static readonly KeyString Required = new("validation.required");
            public static readonly KeyString MinLength = new("validation.min_length");
            public static readonly KeyString MaxLength = new("validation.max_length");
            public static readonly KeyString Range = new("validation.range");
            public static readonly KeyString GreaterThanOrEqual = new("validation.greater_than_or_equal");
        }

        public static class Admin
        {
            public static readonly KeyString UsersTotal = new("admin_users_total");
            public static readonly KeyString UsersActive = new("admin_users_active");
            public static readonly KeyString UsersDeleted = new("admin_users_deleted");
            public static readonly KeyString EditUser = new("admin_edit_user");
            public static readonly KeyString DeleteUser = new("admin_delete_user");
            public static readonly KeyString DeleteUserConfirm = new("admin_delete_user_confirm");
            public static readonly KeyString UserFullName = new("admin_user_full_name");
            public static readonly KeyString UserEmail = new("admin_user_email");
            public static readonly KeyString UserPassword = new("admin_user_password");
            public static readonly KeyString UserPasswordHint = new("admin_user_password_hint");
            public static readonly KeyString UserRole = new("admin_user_role");
            public static readonly KeyString UserPhone = new("admin_user_phone");
            public static readonly KeyString UserGender = new("admin_user_gender");
            public static readonly KeyString UserBirthDate = new("admin_user_birthdate");
            public static readonly KeyString UserStatus = new("admin_user_status");
            public static readonly KeyString UserSave = new("admin_user_save");
            public static readonly KeyString UserCancel = new("admin_user_cancel");
            public static readonly KeyString UserDelete = new("admin_user_delete");
            public static readonly KeyString UserCreatedSuccess = new("admin_user_created_success");
            public static readonly KeyString UserUpdatedSuccess = new("admin_user_updated_success");
            public static readonly KeyString UserDeletedSuccess = new("admin_user_deleted_success");
            public static readonly KeyString ChangePassword = new("admin_change_password");
            public static readonly KeyString NewPassword = new("admin_new_password");
            public static readonly KeyString ConfirmPassword = new("admin_confirm_password");
            public static readonly KeyString UpdatePassword = new("admin_update_password");
            public static readonly KeyString PasswordMinLength = new("admin_password_min_length");
            public static readonly KeyString PasswordMismatch = new("admin_password_mismatch");
            public static readonly KeyString PasswordUpdatedSuccess = new("admin_password_updated_success");
        }

        public static class Users
        {
            public static readonly KeyString InvalidRole = new("users.invalid_role");
            public static readonly KeyString InvalidStatus = new("users.invalid_status");
        }

        public static class UserSettings
        {
            public static readonly KeyString InvalidLanguage = new("usersettings.invalid_language");
            public static readonly KeyString InvalidTheme = new("usersettings.invalid_theme");
            public static readonly KeyString InvalidColor = new("usersettings.invalid_color");
        }

        public static class ProfileImageMessages
        {
            public static readonly KeyString FileTooLarge    = new("profile_image.file_too_large");
            public static readonly KeyString UploadSuccess   = new("profile_image.upload_success");
            public static readonly KeyString RemoveSuccess   = new("profile_image.remove_success");
            public static readonly KeyString UploadFailed    = new("profile_image.upload_failed");
        }

        public static class Products
        {
            public static readonly KeyString NotFound = new("products.not_found");
            public static readonly KeyString Created = new("products.created");
            public static readonly KeyString Updated = new("products.updated");
            public static readonly KeyString Deleted = new("products.deleted");
            public static readonly KeyString CategoryNotFound = new("products.category_not_found");
        }

        public static class Categories
        {
            public static readonly KeyString NotFound = new("categories.not_found");
            public static readonly KeyString Created = new("categories.created");
            public static readonly KeyString Updated = new("categories.updated");
            public static readonly KeyString Deleted = new("categories.deleted");
            public static readonly KeyString NameExists = new("categories.name_exists");
        }
    }
}
