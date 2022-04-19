using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcService;
using Basket.API.Repositories;
using EventBus.Message.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepo;
        private readonly ILogger<BasketController> _logger;
        private readonly DiscountGrpcService _discountSrv;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public BasketController(IBasketRepository basketRepo, ILogger<BasketController> logger, DiscountGrpcService discountSrv, 
            IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _basketRepo = basketRepo ?? throw new ArgumentNullException(nameof(basketRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _discountSrv = discountSrv ?? throw new ArgumentNullException(nameof(discountSrv));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }



        [HttpGet("{username}")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string username)
        {
            var basktet = await _basketRepo.GetBasket(username);

            return Ok(basktet ?? new ShoppingCart(username));
        }


        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            // TODO : Get dicsount from Discount.Grpc to compute the latest price of product
            foreach (var item in basket.Items)
            {
                var coupon = await _discountSrv.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }
            
            var basktet = await _basketRepo.UpdateBasket(basket);

            return Ok(basktet);
        }


        [HttpDelete("{username}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> DeleteBasket(string username)
        {
            await _basketRepo.DeleteBasket(username);

            return Ok();
        }

        [HttpPost, Route("[action]")]
        public async Task<ActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            // GEt existing basket with total price
            var basket = await _basketRepo.GetBasket(basketCheckout.UserName);
            if(basket == null)
                return BadRequest();

            // Create basketCheckoutEvent -- set totalproce on basketcheckout eventmessage
            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMessage);


            // Send checkout event to RabbitMQ


            // Remove the basket
            await _basketRepo.DeleteBasket(basket.UserName);

            return Accepted();
        }
    }
}
