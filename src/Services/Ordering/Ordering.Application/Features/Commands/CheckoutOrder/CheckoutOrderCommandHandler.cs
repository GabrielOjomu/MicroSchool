using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Commands.CheckoutOrder
{
    public  class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailSvc;
        private readonly ILogger<CheckoutOrderCommandHandler> _logger;

        public CheckoutOrderCommandHandler(IOrderRepository orderRepo, IMapper mapper, ILogger<CheckoutOrderCommandHandler> logger) // IEmailService emailSvc, 
        {
            _orderRepo = orderRepo ?? throw new ArgumentNullException(nameof(orderRepo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            //_emailSvc = emailSvc ?? throw new ArgumentNullException(nameof(emailSvc));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = _mapper.Map<Order>(request);

            var newOrder = await _orderRepo.AddAsync(orderEntity);
            _logger.LogInformation($"Order {newOrder.Id} created sucessfully");
            await SendEmail(newOrder);

            return newOrder.Id;
        }

        private async Task SendEmail(Order newOrder)
        {
            var email = new Email() { Message = $"New Order created", To = "orders@mailinator.com", Subject = "New Order" };

            try
            {
                //await _emailSvc.SendEmail(email);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Order {newOrder.Id}  failed due to error with the mail service: {ex.Message}");
            }
        }
    }
}
