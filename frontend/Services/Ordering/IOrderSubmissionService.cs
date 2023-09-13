using Eburgan.Frontend.Models.View;

namespace Eburgan.Frontend.Services.Ordering
{
    public interface IOrderSubmissionService
    {
        Task<Guid> SubmitOrder(CheckoutViewModel checkoutViewModel);
    }
}
