using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Interfaces.Repositories;

namespace Talabat.APIs.Controllers
{
    public class BasketController : ApiBaseController
    {
        private readonly IBasketRepository BasketRepository;

        public BasketController(IBasketRepository BasketRepository)
        {
            this.BasketRepository = BasketRepository;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerBasket>> GetCustomerBasket(string BasketId)
        {
            var basket = await BasketRepository.GetBasketAsync(BasketId);
            return Ok(basket);

            return basket is null ? new CustomerBasket() { Id = BasketId } : basket;
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdateCustomerBasket(CustomerBasket basket)
        {
            var CustomerBasket = await BasketRepository.CreateOrUpdateBasketAsync(basket);
            return CustomerBasket != null
                   ? Ok(CustomerBasket)
                   : BadRequest(new ApiResponse(400));
        }

        [HttpDelete]
        public async Task<ActionResult<bool?>> DeleteCustomerBasket(string BasketId)
            => await BasketRepository.DeleteBasketAsync(BasketId);

    }
}
