using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Repository.Data.Contexts;

namespace Talabat.APIs.Controllers
{
    public class BugController : ApiBaseController
    {
        private readonly TalabatDbContext DbContext;
        private readonly IMapper mapper;

        public BugController(TalabatDbContext DbContext, IMapper mapper)
        {
            this.DbContext = DbContext;
            this.mapper = mapper;
        }

        [HttpGet("NotFound")]
        public ActionResult GetNotFoundRequest()
        {
            var product = DbContext.Products.Find(1);
            if (product is null)
                return NotFound(new ApiResponse(404));
            return Ok(product);
        }

        [HttpGet("ServerError")]
        public ActionResult GetServerErrorRequest()
        {
            var product = DbContext.Products.Find(1);
            var MappedProduct = product.ToString();     //NullReference Exception
            return Ok(MappedProduct);
        }

        [HttpGet("BadRequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("BadRequest/{id}")]
        public ActionResult GetBadRequest(int id)
        {
            return BadRequest(new ApiResponse(400));
        }
    }
}
