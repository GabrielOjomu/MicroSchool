using AutoMapper;
using EventBus.Message.Events;
using Ordering.Application.Features.Commands.CheckoutOrder;

namespace Ordering.API.Mappings
{
    public class OrderingProfile : Profile
    {

        public OrderingProfile()
        {
            CreateMap<CheckoutOrderCommand, BasketCheckoutEvent>().ReverseMap();
        }
    }
}
