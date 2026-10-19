using MediatR;

namespace DAKKN.Application.Features.Products.Commands.DeleteProductImage
{
    /// <summary>
    /// Removes product image files from disk (upload place 1 = Products).
    /// Called by the admin controller AFTER the product row has been committed, so a
    /// failed save never leaves the database pointing at files that are already gone.
    /// Intentionally does not touch the Products row — the caller has already done that.
    /// </summary>
    public record DeleteProductImagesCommand(IReadOnlyList<string> FileNames) : IRequest<bool>;
}
