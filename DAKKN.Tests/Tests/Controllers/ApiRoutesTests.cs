using DAKKN.Appearence.Routes;

namespace DAKKN.Tests.Tests.Controllers
{
    public class ApiRoutesTests
    {
        [Fact]
        public void Base_ShouldHaveCorrectFormat()
        {
            ApiRoutes.Base.Should().Be("api/v{version:apiVersion}");
        }

        [Fact]
        public void Translation_Get_ShouldCombineWithBase()
        {
            ApiRoutes.Translation.Get.Should().Be("api/v{version:apiVersion}/translations");
        }

        [Fact]
        public void Auth_Routes_ShouldBeCorrect()
        {
            ApiRoutes.Auth.Signup.Should().Be("api/v{version:apiVersion}/auth/signup");
            ApiRoutes.Auth.Login.Should().Be("api/v{version:apiVersion}/auth/login");
            ApiRoutes.Auth.Logout.Should().Be("api/v{version:apiVersion}/auth/logout");
            ApiRoutes.Auth.ForgetPassword.Should().Be("api/v{version:apiVersion}/auth/forget-password");
            ApiRoutes.Auth.VerifyForgetPasswordOtp.Should().Be("api/v{version:apiVersion}/auth/verify-forget-password-otp");
            ApiRoutes.Auth.ResetPassword.Should().Be("api/v{version:apiVersion}/auth/reset-password");
            ApiRoutes.Auth.LoginWithGoogle.Should().Be("api/v{version:apiVersion}/auth/login-google");
        }

        [Fact]
        public void Users_Routes_ShouldBeCorrect()
        {
            ApiRoutes.Users.GetAll.Should().Be("api/v{version:apiVersion}/users");
            ApiRoutes.Users.GetById.Should().Be("api/v{version:apiVersion}/users/{id}");
            ApiRoutes.Users.Create.Should().Be("api/v{version:apiVersion}/users");
            ApiRoutes.Users.Update.Should().Be("api/v{version:apiVersion}/users/{id}");
            ApiRoutes.Users.Delete.Should().Be("api/v{version:apiVersion}/users/{id}");
            ApiRoutes.Users.ChangePassword.Should().Be("api/v{version:apiVersion}/users/{id}/change-password");
        }

        [Fact]
        public void Catalog_Routes_ShouldBeCorrect()
        {
            ApiRoutes.Catalog.Products.Should().Be("api/v{version:apiVersion}/catalog/products");
            ApiRoutes.Catalog.Categories.Should().Be("api/v{version:apiVersion}/catalog/categories");
            ApiRoutes.Catalog.FeaturedProducts.Should().Be("api/v{version:apiVersion}/catalog/products/featured");
            ApiRoutes.Catalog.PriceRange.Should().Be("api/v{version:apiVersion}/catalog/products/price-range");
        }

        [Fact]
        public void Cart_Routes_ShouldBeCorrect()
        {
            ApiRoutes.Cart.Get.Should().Be("api/v{version:apiVersion}/cart");
            ApiRoutes.Cart.Count.Should().Be("api/v{version:apiVersion}/cart/count");
            ApiRoutes.Cart.Add.Should().Be("api/v{version:apiVersion}/cart/add");
            ApiRoutes.Cart.Remove.Should().Be("api/v{version:apiVersion}/cart/remove/{productId}");
            ApiRoutes.Cart.UpdateQuantity.Should().Be("api/v{version:apiVersion}/cart/update-quantity");
            ApiRoutes.Cart.UpdateShipping.Should().Be("api/v{version:apiVersion}/cart/shipping");
        }

        [Fact]
        public void Settings_Routes_ShouldBeCorrect()
        {
            ApiRoutes.Settings.Get.Should().Be("api/v{version:apiVersion}/settings");
            ApiRoutes.Settings.Update.Should().Be("api/v{version:apiVersion}/settings");
        }

        [Fact]
        public void Shipping_Routes_ShouldBeCorrect()
        {
            ApiRoutes.Shipping.Governorates.Should().Be("api/v{version:apiVersion}/shipping/governorates");
        }

        [Fact]
        public void ProductRatings_Routes_ShouldBeCorrect()
        {
            ApiRoutes.ProductRatings.GetSummary.Should().Be("api/v{version:apiVersion}/products/{productId}/rating");
            ApiRoutes.ProductRatings.Rate.Should().Be("api/v{version:apiVersion}/products/{productId}/rating");
        }

        [Fact]
        public void StickerSuggestions_Routes_ShouldBeCorrect()
        {
            ApiRoutes.StickerSuggestions.Base.Should().Be("api/v{version:apiVersion}/sticker-suggestions");
            ApiRoutes.StickerSuggestions.My.Should().Be("api/v{version:apiVersion}/sticker-suggestions/my");
            ApiRoutes.StickerSuggestions.ById.Should().Be("api/v{version:apiVersion}/sticker-suggestions/{id:guid}");
            ApiRoutes.StickerSuggestions.Status.Should().Be("api/v{version:apiVersion}/sticker-suggestions/{id:guid}/status");
        }

        [Fact]
        public void BrandReviews_Routes_ShouldBeCorrect()
        {
            ApiRoutes.BrandReviews.My.Should().Be("api/v{version:apiVersion}/brand-reviews/my");
            ApiRoutes.BrandReviews.AdminAll.Should().Be("api/v{version:apiVersion}/brand-reviews/admin/all");
            ApiRoutes.BrandReviews.Create.Should().Be("api/v{version:apiVersion}/brand-reviews");
            ApiRoutes.BrandReviews.Approve.Should().Be("api/v{version:apiVersion}/brand-reviews/{id}/approve");
        }

        [Fact]
        public void Support_Routes_ShouldBeCorrect()
        {
            ApiRoutes.Support.CreateTicket.Should().Be("api/v{version:apiVersion}/support/tickets");
            ApiRoutes.Support.MyTickets.Should().Be("api/v{version:apiVersion}/support/tickets/my");
            ApiRoutes.Support.Close.Should().Be("api/v{version:apiVersion}/support/tickets/{ticketId:guid}/close");
            ApiRoutes.Support.Dashboard.Should().Be("api/v{version:apiVersion}/support/admin/dashboard");
            ApiRoutes.Support.Settings.Should().Be("api/v{version:apiVersion}/support/admin/settings");
            ApiRoutes.Support.PublicFAQs.Should().Be("api/v{version:apiVersion}/support/faqs");
            ApiRoutes.Support.PublicCategories.Should().Be("api/v{version:apiVersion}/support/categories");
        }

        [Fact]
        public void Attachments_Routes_ShouldBeCorrect()
        {
            ApiRoutes.Attachments.UploadImage.Should().Be("api/v{version:apiVersion}/attachments/upload-image");
            ApiRoutes.Attachments.UpdateImage.Should().Be("api/v{version:apiVersion}/attachments/update-image");
        }
    }
}
