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

        public static readonly KeyString BackToTop = new("back_to_top");

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
            public static readonly KeyString InvalidPhoneNumber = new("alert.phone_invalid");
            public static readonly KeyString InvalidOtp = new("alert.invalid_otp");
            public static readonly KeyString AccessDeniedMessage = new("auth.access_denied.message");
        }

        public static class ValidationMessages
        {
            public static readonly KeyString Required = new("validation.required");
            public static readonly KeyString MinLength = new("validation.min_length");
            public static readonly KeyString MaxLength = new("validation.max_length");
            public static readonly KeyString Range = new("validation.range");
            public static readonly KeyString GreaterThanOrEqual = new("validation.greater_than_or_equal");
            public static readonly KeyString InvalidEnum = new("validation.invalid_enum");
            public static readonly KeyString InvalidValue = new("validation.invalid_value");
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
            public static readonly KeyString FeaturedEmptyTitle = new("products.featured_empty_title");
            public static readonly KeyString FeaturedEmptyMsg = new("products.featured_empty_msg");
        }

        public static class Categories
        {
            public static readonly KeyString NotFound = new("categories.not_found");
            public static readonly KeyString Created = new("categories.created");
            public static readonly KeyString Updated = new("categories.updated");
            public static readonly KeyString Deleted = new("categories.deleted");
            public static readonly KeyString Restored = new("categories.restored");
            public static readonly KeyString NameExists = new("categories.name_exists");
            public static readonly KeyString CategoriesEmptyTitle = new("categories.empty_title");
            public static readonly KeyString CategoriesEmptyMsg = new("categories.empty_msg");
        }

        public static class CartMessages
        {
            public static readonly KeyString Added = new("cart.added");
            public static readonly KeyString Removed = new("cart.removed");
            public static readonly KeyString Updated = new("cart.updated");
            public static readonly KeyString Empty = new("cart.empty");
            public static readonly KeyString QuantityMustBePositive = new("cart.quantity_must_be_positive");
            public static readonly KeyString ProductNotAvailable = new("cart.product_not_available");
            public static readonly KeyString OutOfStock = new("cart.product_out_of_stock");
            public static readonly KeyString OnlyAvailable = new("cart.only_available");
            public static readonly KeyString ProductNotFound = new("cart.product_not_found");
            public static readonly KeyString FreeShipping = new("cart.free_shipping");
            public static readonly KeyString ShippingUpdated = new("cart.shipping_updated");
            public static readonly KeyString BrowseProducts = new("cart.browse_products");
            public static readonly KeyString ContinueShopping = new("cart.continue_shopping");
        }

        public static class CheckoutMessages
        {
            public static readonly KeyString ShippingCardTitle = new("checkout_shipping_card_title");
            public static readonly KeyString GovLabel = new("checkout_gov_label");
            public static readonly KeyString ShippingCost = new("checkout_shipping_cost");
        }

        public static class ShippingMessages
        {
            public static readonly KeyString NotFound = new("shipping.not_found");
            public static readonly KeyString Created = new("shipping.created");
            public static readonly KeyString Updated = new("shipping.updated");
            public static readonly KeyString Deleted = new("shipping.deleted");
            public static readonly KeyString NameExists = new("shipping.name_exists");
            public static readonly KeyString GovPlaceholder = new("shipping.gov_placeholder");
            public static readonly KeyString SelectGov = new("shipping.select_gov");
            public static readonly KeyString ShippingInfo = new("shipping.info");
            public static readonly KeyString Governorate = new("shipping.governorate");
            public static readonly KeyString Price = new("shipping.price");
        }

        public static class Inventory
        {
            public static readonly KeyString QuantityInStock = new("inventory_quantity_in_stock");
            public static readonly KeyString DangerQuantity = new("inventory_danger_quantity");
            public static readonly KeyString LastUpdated = new("inventory_last_updated");
            public static readonly KeyString InStock = new("inventory_in_stock");
            public static readonly KeyString LowStock = new("inventory_low_stock");
            public static readonly KeyString OutOfStock = new("inventory_out_of_stock");
            public static readonly KeyString OnlyXLeft = new("inventory_only_x_left");
            public static readonly KeyString AvailableX = new("inventory_available_x");
            public static readonly KeyString MaxReached = new("inventory_max_reached");
            public static readonly KeyString OutOfStockMsg = new("inventory_out_of_stock_msg");
            public static readonly KeyString OnlyXAvailable = new("inventory_only_x_available");
            public static readonly KeyString StockStatus = new("inventory_stock_status");
            public static readonly KeyString ItemsInStock = new("inventory_items_in_stock");
            public static readonly KeyString ItemsLowStock = new("inventory_items_low_stock");
            public static readonly KeyString ItemsOutOfStock = new("inventory_items_out_of_stock");
            public static readonly KeyString LowStockCount = new("inventory_low_stock_count");
            public static readonly KeyString OutOfStockCount = new("inventory_out_of_stock_count");
            public static readonly KeyString FilterInStock = new("inventory_filter_in_stock");
            public static readonly KeyString FilterLowStock = new("inventory_filter_low_stock");
            public static readonly KeyString FilterOutOfStock = new("inventory_filter_out_of_stock");
            public static readonly KeyString SortLowest = new("inventory_sort_lowest");
            public static readonly KeyString SortHighest = new("inventory_sort_highest");
            public static readonly KeyString WarningLowStock = new("inventory_warning_low_stock");
            public static readonly KeyString Quantity = new("inventory_quantity");
            public static readonly KeyString AdjustStock = new("inventory_adjust_stock");
            public static readonly KeyString UpdateStock = new("inventory_update_stock");
            public static readonly KeyString DashboardLowStockTitle = new("inventory_dashboard_low_stock_title");
            public static readonly KeyString DashboardOutOfStockTitle = new("inventory_dashboard_out_of_stock_title");
            public static readonly KeyString SortDefault = new("admin_sort_default");
            public static readonly KeyString SortLowToHigh = new("admin_sort_low_to_high");
            public static readonly KeyString SortHighToLow = new("admin_sort_high_to_low");
            public static readonly KeyString QuantityInStockLabel = new("admin_quantity_in_stock");
            public static readonly KeyString DangerQuantityLabel = new("admin_danger_quantity");
            public static readonly KeyString DangerQuantityHint = new("admin_danger_quantity_hint");
            public static readonly KeyString GlobalDangerQuantity = new("admin_global_danger_quantity");
            public static readonly KeyString GlobalDangerQuantityDesc = new("admin_global_danger_quantity_desc");
            public static readonly KeyString InventorySettings = new("admin_inventory_settings");
            public static readonly KeyString InventorySettingsDesc = new("admin_inventory_settings_desc");
            public static readonly KeyString SaveSettings = new("admin_save_settings");
            public static readonly KeyString ResetDefault = new("admin_reset_default");
            public static readonly KeyString ApplyToAll = new("admin_apply_to_all");
            public static readonly KeyString ApplyToAllConfirm = new("admin_apply_to_all_confirm");
            public static readonly KeyString Updated = new("admin_inventory_updated");
            public static readonly KeyString GlobalDangerApplied = new("admin_global_danger_applied");
        }

        public static class Error
        {
            public static readonly KeyString Title = new("error_title");
            public static readonly KeyString Heading = new("error_heading");
            public static readonly KeyString Subheading = new("error_subheading");
            public static readonly KeyString RequestId = new("error_request_id");
            public static readonly KeyString DevMode = new("error_dev_mode");
            public static readonly KeyString DevDesc = new("error_dev_desc");
            public static readonly KeyString DevWarning = new("error_dev_warning");
            public static readonly KeyString DevSensitive = new("error_dev_sensitive");
            public static readonly KeyString BackSafety = new("error_back_safety");
            public static readonly KeyString ServerError = new("error_server_error");
            public static readonly KeyString ServerErrorMessage = new("error_server_error_message");
            public static readonly KeyString ServiceUnavailable = new("error_service_unavailable");
            public static readonly KeyString ServiceUnavailableMessage = new("error_service_unavailable_message");
            public static readonly KeyString ServerErrorIcon = new("error_server_error_icon");
        }

        public static class AuthView
        {
            public static readonly KeyString SyncingTitle = new("auth_syncing_title");
            public static readonly KeyString LoggingOutTitle = new("auth_logging_out_title");
            public static readonly KeyString SyncingSession = new("auth_syncing_session");
            public static readonly KeyString LoggingOut = new("auth_logging_out");
        }

        public static class Profile
        {
            public static readonly KeyString Title = new("profile_title");
            public static readonly KeyString ChangePwd = new("profile_change_pwd");
            public static readonly KeyString ChangePwdP = new("profile_change_pwd_p");
            public static readonly KeyString DeleteAcc = new("profile_delete_acc");
            public static readonly KeyString DeleteAccP = new("profile_delete_acc_p");
            public static readonly KeyString AvatarP = new("profile_avatar_p");
            public static readonly KeyString BasicP = new("profile_basic_p");
            public static readonly KeyString SettingsSubtitle = new("profile_settings_subtitle");
            public static readonly KeyString ProfilePicFormat = new("profile_pic_format");

            public static readonly KeyString CurrentPassword = new("profile_current_password");
            public static readonly KeyString NewPassword = new("profile_new_password");
            public static readonly KeyString ConfirmPassword = new("profile_confirm_password");
            public static readonly KeyString PasswordChanged = new("profile_password_changed");
            public static readonly KeyString PasswordChangeError = new("profile_password_change_error");
            public static readonly KeyString WrongCurrentPassword = new("profile_wrong_current_password");
            public static readonly KeyString PasswordRequirements = new("profile_password_requirements");
            public static readonly KeyString WeakPassword = new("profile_weak_password");
            public static readonly KeyString ShowPassword = new("profile_show_password");
            public static readonly KeyString HidePassword = new("profile_hide_password");

            public static readonly KeyString DeleteAccountWarning = new("profile_delete_account_warning");
            public static readonly KeyString DeleteAccountConfirm = new("profile_delete_account_confirm");
            public static readonly KeyString DeleteAccountTypeConfirm = new("profile_delete_account_type_confirm");
            public static readonly KeyString DeleteAccountPassword = new("profile_delete_account_password");
            public static readonly KeyString DeleteAccountSuccess = new("profile_delete_account_success");
            public static readonly KeyString DeleteAccountError = new("profile_delete_account_error");
            public static readonly KeyString Cancel = new("profile_cancel");
            public static readonly KeyString Confirm = new("profile_confirm");
            public static readonly KeyString TypeDeleteToConfirm = new("profile_type_delete_to_confirm");
            public static readonly KeyString AccountDeleted = new("profile_account_deleted");
        }

        public static class Support
        {
            public static readonly KeyString NewTicket = new("supp_new_ticket");
            public static readonly KeyString NewTicketDesc = new("supp_new_ticket_desc");
            public static readonly KeyString TicketCatPh = new("supp_ticket_cat_ph");
            public static readonly KeyString OrderIdLabel = new("supp_ticket_order_id_label");
            public static readonly KeyString OrderIdPh = new("supp_ticket_order_id_ph");
            public static readonly KeyString SubjectPh = new("supp_ticket_subject_ph");
            public static readonly KeyString MsgPh = new("supp_ticket_msg_ph");
            public static readonly KeyString AttachLabel = new("supp_ticket_attach_label");
            public static readonly KeyString AttachSub = new("supp_ticket_attach_sub");
            public static readonly KeyString SubmitBtn = new("supp_ticket_submit");
            public static readonly KeyString LastActivity = new("supp_last_activity");
            public static readonly KeyString ViewDetails = new("supp_view_details");
            public static readonly KeyString QuickHelp = new("supp_quick_help");
            public static readonly KeyString InstantChat = new("supp_instant_chat");
            public static readonly KeyString EmailSupport = new("supp_email_support");
            public static readonly KeyString FollowUpdates = new("supp_follow_updates");
            public static readonly KeyString NoTickets = new("supp_no_tickets");
            public static readonly KeyString NoTicketsDesc = new("supp_no_tickets_desc");
            public static readonly KeyString CreateFirstTicket = new("supp_create_first_ticket");
            public static readonly KeyString SocialInstagram = new("social_instagram");
            public static readonly KeyString SocialTiktok = new("social_tiktok");
            public static readonly KeyString SocialYoutube = new("social_youtube");
            public static readonly KeyString TicketCreated = new("supp_ticket_created");
            public static readonly KeyString TicketCreatedTitle = new("supp_ticket_created_title");
            public static readonly KeyString TicketNumber = new("supp_ticket_number");
            public static readonly KeyString EstimatedResponse = new("supp_estimated_response");
            public static readonly KeyString DefaultResponseTime = new("supp_default_response_time");
            public static readonly KeyString FullName = new("supp_full_name");
            public static readonly KeyString Email = new("supp_email");
            public static readonly KeyString Phone = new("supp_phone");
            public static readonly KeyString Subject = new("supp_subject");
            public static readonly KeyString Category = new("supp_category");
            public static readonly KeyString Priority = new("supp_priority");
            public static readonly KeyString Message = new("supp_message");
            public static readonly KeyString Attachments = new("supp_attachments");
            public static readonly KeyString DropFiles = new("supp_drop_files");
            public static readonly KeyString MaxSize = new("supp_max_size");
            public static readonly KeyString ReplyPlaceholder = new("supp_reply_placeholder");
            public static readonly KeyString SendReply = new("supp_send_reply");
            public static readonly KeyString TicketHistory = new("supp_ticket_history");
            public static readonly KeyString StatusTimeline = new("supp_status_timeline");
            public static readonly KeyString InternalNotes = new("supp_internal_notes");
            public static readonly KeyString AddNote = new("supp_add_note");
            public static readonly KeyString NotePlaceholder = new("supp_note_placeholder");
            public static readonly KeyString AssignTicket = new("supp_assign_ticket");
            public static readonly KeyString ReassignTicket = new("supp_reassign_ticket");
            public static readonly KeyString CloseTicket = new("supp_close_ticket");
            public static readonly KeyString ReopenTicket = new("supp_reopen_ticket");
            public static readonly KeyString ArchiveTicket = new("supp_archive_ticket");
            public static readonly KeyString EscalateTicket = new("supp_escalate_ticket");
            public static readonly KeyString ChangePriority = new("supp_change_priority");
            public static readonly KeyString MarkWaitingCustomer = new("supp_mark_waiting_customer");
            public static readonly KeyString MarkWaitingStaff = new("supp_mark_waiting_staff");
            public static readonly KeyString Dashboard = new("supp_dashboard");
            public static readonly KeyString TotalTickets = new("supp_total_tickets");
            public static readonly KeyString OpenTickets = new("supp_open_tickets");
            public static readonly KeyString PendingTickets = new("supp_pending_tickets");
            public static readonly KeyString WaitingCustomerTickets = new("supp_waiting_customer");
            public static readonly KeyString ClosedTickets = new("supp_closed_tickets");
            public static readonly KeyString HighPriorityTickets = new("supp_high_priority");
            public static readonly KeyString AvgResponseTime = new("supp_avg_response_time");
            public static readonly KeyString AvgResolutionTime = new("supp_avg_resolution_time");
            public static readonly KeyString TicketsByDay = new("supp_tickets_by_day");
            public static readonly KeyString TicketsByWeek = new("supp_tickets_by_week");
            public static readonly KeyString TicketsByMonth = new("supp_tickets_by_month");
            public static readonly KeyString PriorityDistribution = new("supp_priority_distribution");
            public static readonly KeyString CategoryDistribution = new("supp_category_distribution");
            public static readonly KeyString StatusDistribution = new("supp_status_distribution");
            public static readonly KeyString TopIssues = new("supp_top_issues");
            public static readonly KeyString Settings = new("supp_settings");
            public static readonly KeyString SupportEmail = new("supp_support_email");
            public static readonly KeyString DefaultPriority = new("supp_default_priority");
            public static readonly KeyString MaxAttachmentSize = new("supp_max_attachment_size");
            public static readonly KeyString AllowedExtensions = new("supp_allowed_extensions");
            public static readonly KeyString AutoCloseDays = new("supp_auto_close_days");
            public static readonly KeyString NotifyNewTicket = new("supp_notify_new_ticket");
            public static readonly KeyString NotifyReply = new("supp_notify_reply");
            public static readonly KeyString NotifyAssignment = new("supp_notify_assignment");
            public static readonly KeyString NotifyStatusChange = new("supp_notify_status_change");
            public static readonly KeyString EmailTemplates = new("supp_email_templates");
            public static readonly KeyString ConfirmClose = new("supp_confirm_close");
            public static readonly KeyString ConfirmArchive = new("supp_confirm_archive");
            public static readonly KeyString ConfirmReopen = new("supp_confirm_reopen");
            public static readonly KeyString ConfirmDeleteCategory = new("supp_confirm_delete_category");
            public static readonly KeyString ConfirmDeleteFAQ = new("supp_confirm_delete_faq");
            public static readonly KeyString TicketReplied = new("supp_ticket_replied");
            public static readonly KeyString TicketAssigned = new("supp_ticket_assigned");
            public static readonly KeyString TicketClosed = new("supp_ticket_closed");
            public static readonly KeyString TicketReopened = new("supp_ticket_reopened");
            public static readonly KeyString TicketPriorityChanged = new("supp_ticket_priority_changed");
            public static readonly KeyString TicketStatusChanged = new("supp_ticket_status_changed");
            public static readonly KeyString NoResults = new("supp_no_results");
            public static readonly KeyString SearchTickets = new("supp_search_tickets");
            public static readonly KeyString FilterStatus = new("supp_filter_status");
            public static readonly KeyString FilterPriority = new("supp_filter_priority");
            public static readonly KeyString FilterCategory = new("supp_filter_category");
            public static readonly KeyString FilterDateRange = new("supp_filter_date_range");
            public static readonly KeyString FilterAssignedTo = new("supp_filter_assigned_to");
            public static readonly KeyString Export = new("supp_export");
            public static readonly KeyString BulkActions = new("supp_bulk_actions");
            public static readonly KeyString CustomerInfo = new("supp_customer_info");
            public static readonly KeyString Conversation = new("supp_conversation");
            public static readonly KeyString ActivityTimeline = new("supp_activity_timeline");
            public static readonly KeyString RelatedTickets = new("supp_related_tickets");
            public static readonly KeyString AuditHistory = new("supp_audit_history");
            public static readonly KeyString WaitingFor = new("supp_waiting_for");
            public static readonly KeyString HasAttachments = new("supp_has_attachments");
            
            // FAQ Localization
            public static readonly KeyString FAQQuestion = new("supp_faq_question");
            public static readonly KeyString FAQArQuestion = new("supp_faq_ar_question");
            public static readonly KeyString FAQAnswer = new("supp_faq_answer");
            public static readonly KeyString FAQArAnswer = new("supp_faq_ar_answer");
            public static readonly KeyString FAQCategory = new("supp_category");
            public static readonly KeyString FAQDisplayOrder = new("supp_display_order");
            public static readonly KeyString FAQPublished = new("supp_published");
            public static readonly KeyString FAQSelectCategory = new("supp_select_category");
            public static readonly KeyString FAQQuestionPh = new("supp_faq_question_ph");
            public static readonly KeyString FAQArQuestionPh = new("supp_faq_ar_question_ph");
            public static readonly KeyString FAQAnswerPh = new("supp_faq_answer_ph");
            public static readonly KeyString FAQArAnswerPh = new("supp_faq_ar_answer_ph");
            public static readonly KeyString AddFAQ = new("supp_add_faq");
            public static readonly KeyString EditFAQ = new("supp_edit_faq");
            public static readonly KeyString Save = new("supp_save");
            public static readonly KeyString Update = new("supp_update");
            public static readonly KeyString Cancel = new("supp_cancel");
            public static readonly KeyString DeleteFAQTitle = new("supp_delete_faq_title");
            public static readonly KeyString DeleteFAQMessage = new("supp_delete_faq_message");
            public static readonly KeyString DeleteFAQ = new("supp_delete");
            public static readonly KeyString NoFAQs = new("supp_no_faqs");

            // Landing Page FAQ Section
            public static readonly KeyString SectionFAQ = new("section_faq");
            public static readonly KeyString AdminFAQDesc = new("admin_faq_desc");
            public static readonly KeyString AdminFAQTitle = new("admin_faq_title");
            public static readonly KeyString AdminFAQDescLabel = new("admin_faq_desc");
            public static readonly KeyString AdminFAQDisplayLimit = new("admin_faq_display_limit");
            public static readonly KeyString AdminFAQAutoFetch = new("admin_faq_auto_fetch");
            public static readonly KeyString AdminFAQAutoFetchDesc = new("admin_faq_auto_fetch_desc");
            public static readonly KeyString AdminFAQAutoFetchHint = new("admin_faq_auto_fetch_hint");
            public static readonly KeyString AdminContentPHFaqTitleAr = new("admin_content_ph_faq_title_ar");
            public static readonly KeyString AdminContentPHFaqTitleEn = new("admin_content_ph_faq_title_en");
            public static readonly KeyString AdminContentPHFaqDescAr = new("admin_content_ph_faq_desc_ar");
            public static readonly KeyString AdminContentPHFaqDescEn = new("admin_content_ph_faq_desc_en");
        }

        public static class OrderConfirmation
        {
            public static readonly KeyString Date = new("conf_date");
            public static readonly KeyString Payment = new("conf_payment");
            public static readonly KeyString Cod = new("conf_cod");
            public static readonly KeyString NeedHelp = new("conf_need_help");
            public static readonly KeyString ContactSupport = new("conf_contact_support");
        }

        public static class CustomOrder
        {
            public static readonly KeyString OrDivider = new("custom_or_divider");
            public static readonly KeyString ProTip = new("custom_pro_tip");
            public static readonly KeyString MaterialLabel = new("custom_material_label");
            public static readonly KeyString MaterialDesc = new("custom_material_desc");
            public static readonly KeyString Cm = new("custom_cm");
            public static readonly KeyString Pieces = new("custom_pieces");
            public static readonly KeyString FileTooLarge = new("custom_file_too_large");
            public static readonly KeyString PinterestError = new("custom_pinterest_error");
            public static readonly KeyString ImageLoadError = new("custom_image_load_error");
            public static readonly KeyString FetchBtn = new("custom_fetch_btn");
        }

        public static class Reviews
        {
            public static readonly KeyString NotFound = new("reviews.not_found");
            public static readonly KeyString AlreadyReviewed = new("reviews.already_reviewed");
            public static readonly KeyString OnlyApprovedCanBeFeatured = new("reviews.only_approved_can_be_featured");
            public static readonly KeyString MaxFeaturedReached = new("reviews.max_featured_reached");
            public static readonly KeyString Created = new("reviews.created");
            public static readonly KeyString Updated = new("reviews.updated");
            public static readonly KeyString Deleted = new("reviews.deleted");
            public static readonly KeyString Approved = new("reviews.approved");
            public static readonly KeyString Rejected = new("reviews.rejected");
            public static readonly KeyString Featured = new("reviews.featured");
            public static readonly KeyString Unfeatured = new("reviews.unfeatured");
            public static readonly KeyString Hidden = new("reviews.hidden");
            public static readonly KeyString Unhidden = new("reviews.unhidden");
        }

        public static class ProductRatings
        {
            public static readonly KeyString ProductNotFound = new("product_ratings.product_not_found");
            public static readonly KeyString RatingSubmitted = new("product_ratings.rating_submitted");
            public static readonly KeyString RatingUpdated = new("product_ratings.rating_updated");
            public static readonly KeyString RateThisProduct = new("product_ratings.rate_this_product");
            public static readonly KeyString RatingsCount = new("product_ratings.ratings_count");
            public static readonly KeyString AverageStars = new("product_ratings.average_stars");
        }

        public static class BrandReviews
        {
            public static readonly KeyString NotFound = new("brand_reviews.not_found");
            public static readonly KeyString MaxDisplayedReached = new("brand_reviews.max_displayed_reached");
            public static readonly KeyString Created = new("brand_reviews.created");
            public static readonly KeyString Updated = new("brand_reviews.updated");
            public static readonly KeyString Deleted = new("brand_reviews.deleted");
            public static readonly KeyString Approved = new("brand_reviews.approved");
            public static readonly KeyString Rejected = new("brand_reviews.rejected");
            public static readonly KeyString Displayed = new("brand_reviews.displayed");
            public static readonly KeyString Undisplayed = new("brand_reviews.undisplayed");
            public static readonly KeyString DisplayOrderUpdated = new("brand_reviews.display_order_updated");
        }

        public static class AdminContent
        {
            public static readonly KeyString TestimonialsDesc = new("admin_testimonials_desc");
            public static readonly KeyString ThFeatured = new("admin_th_featured");
            public static readonly KeyString ThCustomer = new("admin_th_customer");
            public static readonly KeyString ThQuote = new("admin_th_quote");
            public static readonly KeyString ThRating = new("admin_th_rating");
            public static readonly KeyString HeroDescPh = new("admin_hero_desc_ph");
            public static readonly KeyString MapIntegrationH = new("admin_map_integration_h");
            public static readonly KeyString MapIntegrationP = new("admin_map_integration_p");
        }

        public static class AdminSettings
        {
            public static readonly KeyString Subtitle = new("admin_settings_subtitle");
            public static readonly KeyString ProfilePicFormat = new("admin_profile_pic_format");
            public static readonly KeyString ColorTeal = new("color_teal");
            public static readonly KeyString ColorBlue = new("color_blue");
            public static readonly KeyString ColorPurple = new("color_purple");
            public static readonly KeyString ColorPink = new("color_pink");
            public static readonly KeyString ColorOrange = new("color_orange");
            public static readonly KeyString ColorSlate = new("color_slate");
        }

        public static class Shared
        {
            public static readonly KeyString ThemeToggle = new("theme_toggle");
            public static readonly KeyString PasswordToggleAria = new("password_toggle_aria");
            public static readonly KeyString AltLogo = new("alt_logo");
            public static readonly KeyString AltHeroSticker = new("alt_hero_sticker");
            public static readonly KeyString AltTestimonialUser = new("alt_testimonial_user");
            public static readonly KeyString AltTestimonialCustomer = new("alt_testimonial_customer");
            public static readonly KeyString AltAdminAvatar = new("alt_admin_avatar");
            public static readonly KeyString AltUserAvatar = new("alt_user_avatar");
            public static readonly KeyString AltFinalPreview = new("alt_final_preview");
            public static readonly KeyString AltStickerPreview = new("alt_sticker_preview");
            public static readonly KeyString AltProductImage = new("alt_product_image");
            public static readonly KeyString AltCategory = new("alt_category");
            public static readonly KeyString Loading = new("loading");
        }

        public static class Favorites
        {
            public static readonly KeyString NotFound = new("fav_not_found");
        }

        public static class OrderMessages
        {
            public static readonly KeyString InvalidStatusTransition = new("order.invalid_status_transition");
            public static readonly KeyString CannotCancel = new("order.cannot_cancel");
            public static readonly KeyString NotFound = new("order.not_found");
            public static readonly KeyString Created = new("order.created");
            public static readonly KeyString StatusUpdated = new("order.status_updated");
            public static readonly KeyString Cancelled = new("order.cancelled");
            public static readonly KeyString Deleted = new("order.deleted");
        }

        public static class SuggestionMessages
        {
            public static readonly KeyString Submitted = new("suggestion.submitted");
            public static readonly KeyString NotFound = new("suggestion.not_found");
            public static readonly KeyString Unauthorized = new("suggestion.unauthorized");
            public static readonly KeyString StatusUpdated = new("suggestion.status_updated");
            public static readonly KeyString InvalidStatusTransition = new("suggestion.invalid_status_transition");
            public static readonly KeyString TitleRequired = new("suggestion.title_required");
            public static readonly KeyString DescriptionRequired = new("suggestion.description_required");
        }

        public static class CommonKeys
        {
            public static readonly KeyString Previous = new("common_previous");
            public static readonly KeyString Next = new("common_next");
            public static readonly KeyString PageOf = new("common_page_of");
        }

        public static class SuggestionStatusKeys
        {
            public static readonly KeyString Pending = new("suggestion_status_pending");
            public static readonly KeyString UnderReview = new("suggestion_status_underreview");
            public static readonly KeyString Approved = new("suggestion_status_approved");
            public static readonly KeyString Rejected = new("suggestion_status_rejected");
        }

        public static class StickerSuggestionKeys
        {
            public static readonly KeyString AdminSuggestions = new("admin_sticker_suggestions");
            public static readonly KeyString AdminSizes = new("admin_sticker_sizes");
            public static readonly KeyString AdminSubtitle = new("admin_sticker_suggestions_subtitle");
            public static readonly KeyString FilterStatus = new("admin_sticker_suggestions_filter_status");
            public static readonly KeyString FilterAll = new("admin_sticker_suggestions_filter_all");
            public static readonly KeyString Empty = new("admin_sticker_suggestions_empty");
            public static readonly KeyString User = new("admin_sticker_suggestions_user");
            public static readonly KeyString ThTitle = new("admin_sticker_suggestions_th_title");
            public static readonly KeyString ThStatus = new("admin_sticker_suggestions_th_status");
            public static readonly KeyString ThDate = new("admin_sticker_suggestions_th_date");
            public static readonly KeyString ThActions = new("admin_sticker_suggestions_th_actions");
            public static readonly KeyString Details = new("admin_sticker_suggestions_details");
            public static readonly KeyString DetailsTitle = new("admin_sticker_suggestion_details_title");
            public static readonly KeyString Back = new("admin_sticker_suggestion_back");
            public static readonly KeyString Image = new("admin_sticker_suggestion_image");
            public static readonly KeyString AdminNote = new("admin_sticker_suggestion_admin_note");
            public static readonly KeyString SubmittedBy = new("admin_sticker_suggestion_submitted_by");
            public static readonly KeyString UpdateStatus = new("admin_sticker_suggestion_update_status");
            public static readonly KeyString NewStatus = new("admin_sticker_suggestion_new_status");
            public static readonly KeyString ConvertedProduct = new("admin_sticker_suggestion_converted_product");
            public static readonly KeyString NotePh = new("admin_sticker_suggestion_note_ph");
            public static readonly KeyString ProductPh = new("admin_sticker_suggestion_product_ph");
            public static readonly KeyString UpdateBtn = new("admin_sticker_suggestion_update_btn");

            public static readonly KeyString SubmitTitle = new("sticker_suggestion_submit_title");
            public static readonly KeyString SubmitHeading = new("sticker_suggestion_submit_heading");
            public static readonly KeyString SubmitSubtitle = new("sticker_suggestion_submit_subtitle");
            public static readonly KeyString FormTitle = new("sticker_suggestion_form_title");
            public static readonly KeyString FormTitlePh = new("sticker_suggestion_form_title_ph");
            public static readonly KeyString FormDesc = new("sticker_suggestion_form_desc");
            public static readonly KeyString FormDescPh = new("sticker_suggestion_form_desc_ph");
            public static readonly KeyString FormTags = new("sticker_suggestion_form_tags");
            public static readonly KeyString FormTagsPh = new("sticker_suggestion_form_tags_ph");
            public static readonly KeyString FormTagsHint = new("sticker_suggestion_form_tags_hint");
            public static readonly KeyString FormImage = new("sticker_suggestion_form_image");
            public static readonly KeyString FormSubmit = new("sticker_suggestion_form_submit");
            public static readonly KeyString MyTitle = new("sticker_suggestion_my_title");
            public static readonly KeyString MyHeading = new("sticker_suggestion_my_heading");
            public static readonly KeyString MySubtitle = new("sticker_suggestion_my_subtitle");
            public static readonly KeyString MyNew = new("sticker_suggestion_my_new");
            public static readonly KeyString MyEmpty = new("sticker_suggestion_my_empty");
            public static readonly KeyString MyEmptySub = new("sticker_suggestion_my_empty_sub");
            public static readonly KeyString MyEmptyBtn = new("sticker_suggestion_my_empty_btn");
            public static readonly KeyString CustomerDetailsTitle = new("sticker_suggestion_details_title");
            public static readonly KeyString CustomerDetailsBack = new("sticker_suggestion_details_back");
            public static readonly KeyString DetailsConverted = new("sticker_suggestion_details_converted");
            public static readonly KeyString DetailsViewProduct = new("sticker_suggestion_details_view_product");
            public static readonly KeyString SubmittedOn = new("sticker_suggestion_details_submitted_on");
        }

        public static class AdminShipping
        {
            public static readonly KeyString Title = new("admin_shipping.title");
            public static readonly KeyString AddNew = new("admin_shipping.add_new");
            public static readonly KeyString Edit = new("admin_shipping.edit");
            public static readonly KeyString Delete = new("admin_shipping.delete");
            public static readonly KeyString Name = new("admin_shipping.name");
            public static readonly KeyString ArName = new("admin_shipping.ar_name");
            public static readonly KeyString Price = new("admin_shipping.price");
            public static readonly KeyString DisplayOrder = new("admin_shipping.display_order");
            public static readonly KeyString Status = new("admin_shipping.status");
            public static readonly KeyString Actions = new("admin_shipping.actions");
            public static readonly KeyString Active = new("admin_shipping.active");
            public static readonly KeyString Inactive = new("admin_shipping.inactive");
            public static readonly KeyString Save = new("admin_shipping.save");
            public static readonly KeyString Cancel = new("admin_shipping.cancel");
            public static readonly KeyString DeleteConfirm = new("admin_shipping.delete_confirm");
            public static readonly KeyString DeleteMessage = new("admin_shipping.delete_message");
            public static readonly KeyString ToggleStatus = new("admin_shipping.toggle_status");
            public static readonly KeyString CreatedSuccess = new("admin_shipping.created_success");
            public static readonly KeyString UpdatedSuccess = new("admin_shipping.updated_success");
            public static readonly KeyString DeletedSuccess = new("admin_shipping.deleted_success");
            public static readonly KeyString ToggledSuccess = new("admin_shipping.toggled_success");
            public static readonly KeyString NoGovernorates = new("admin_shipping.no_governorates");
            public static readonly KeyString Subtitle = new("admin_shipping.subtitle");
        }

        public static class CustomOrderSubmission
        {
            public static readonly KeyString Submitted = new("custom_order.submitted");
            public static readonly KeyString SubmitError = new("custom_order.submit_error");
            public static readonly KeyString NotFound = new("custom_order.not_found");
            public static readonly KeyString NameLabel = new("custom_order.name_label");
            public static readonly KeyString NamePh = new("custom_order.name_ph");
            public static readonly KeyString PhoneLabel = new("custom_order.phone_label");
            public static readonly KeyString PhonePh = new("custom_order.phone_ph");
            public static readonly KeyString AddressLabel = new("custom_order.address_label");
            public static readonly KeyString AddressPh = new("custom_order.address_ph");
            public static readonly KeyString NotesLabel = new("custom_order.notes_label");
            public static readonly KeyString NotesPh = new("custom_order.notes_ph");
            public static readonly KeyString SubmitBtn = new("custom_order.submit_btn");
        }

        public static class AdminCustomOrder
        {
            public static readonly KeyString Title = new("admin_custom_order.title");
            public static readonly KeyString Details = new("admin_custom_order.details");
            public static readonly KeyString Approve = new("admin_custom_order.approve");
            public static readonly KeyString Reject = new("admin_custom_order.reject");
            public static readonly KeyString Approved = new("admin_custom_order.approved");
            public static readonly KeyString Rejected = new("admin_custom_order.rejected");
            public static readonly KeyString Empty = new("admin_custom_order.empty");
            public static readonly KeyString Status = new("admin_custom_order.status");
            public static readonly KeyString CustomerInfo = new("admin_custom_order.customer_info");
            public static readonly KeyString OrderInfo = new("admin_custom_order.order_info");
            public static readonly KeyString Attachment = new("admin_custom_order.attachment");
        }
    }
}
