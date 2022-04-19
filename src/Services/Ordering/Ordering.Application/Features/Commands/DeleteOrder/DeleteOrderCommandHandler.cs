using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;

        public DeleteOrderCommandHandler(IOrderRepository orderRepo, IMapper mapper, ILogger<DeleteOrderCommandHandler> logger)
        {
            _orderRepo = orderRepo ?? throw new ArgumentNullException(nameof(orderRepo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            // Updating Order
            var orderToUpdate = await _orderRepo.GetByIdAsync(request.Id);
            if (orderToUpdate == null)
            {
                _logger.LogError("Order not found");
                throw new NotFoundException(nameof(Order), request.Id);
            }
            //_mapper.Map(request, orderToUpdate, typeof(UpdateOrderCommand), typeof(Order));

            //_mapper.Map(request, orderToUpdate, typeof(DeleteOrderCommand), typeof(Order));
            await _orderRepo.DeleteAsync(orderToUpdate);
            _logger.LogInformation($"Order {orderToUpdate.Id} Deleted successfully");

            return Unit.Value; 
        }
    }

}
