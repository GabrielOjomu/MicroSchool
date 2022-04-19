using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Queries.GetOrdersList
{
    public class GetOrdersListQueryHandler : IRequestHandler<GetOrdersListQuery, List<OrdersVM>>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepo;

        public GetOrdersListQueryHandler(IMapper mapper, IOrderRepository orderRepo)
        {
            _mapper = mapper;
            _orderRepo = orderRepo;
        }

        public async Task<List<OrdersVM>> Handle(GetOrdersListQuery request, CancellationToken cancellationToken)
        {
            var orderList = await _orderRepo.GetOrdersByUsername(request.UserName);

            return _mapper.Map<List<OrdersVM>>(orderList);
        }
    }
}
