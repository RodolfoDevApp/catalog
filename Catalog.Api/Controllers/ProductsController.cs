using Catalog.Api.Contracts;
using Catalog.Application.Products.Commands.CreateProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Crea un nuevo producto en el catálogo (y deja mensaje en outbox)
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductRequest request,
            CancellationToken ct)
        {
            // Map request -> command (Application)
            var command = new CreateProductCommand(
                VendorId: request.VendorId,
                Sku: request.Sku,
                Name: request.Name,
                Description: request.Description,
                Slug: request.Slug,
                CategoryId: request.CategoryId,
                Price: request.Price,
                InitialStock: request.InitialStock,
                IsActive: request.IsActive,
                MainImageUrl: request.MainImageUrl,
                ImageUrls: request.ImageUrls
            );

            var newId = await _mediator.Send(command, ct);

            // 201 Created con Location header
            return CreatedAtAction(
                actionName: nameof(GetById),
                routeValues: new { productId = newId },
                value: new { id = newId }
            );
        }

        /// <summary>
        /// Get /products/{id}
        /// (por ahora dummy hasta que hagamos Queries reales)
        /// </summary>
        [HttpGet("{productId:guid}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult GetById(Guid productId)
        {
            return Ok(new { productId });
        }
    }
}
