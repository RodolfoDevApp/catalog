using Catalog.Api.Contracts;
using Catalog.Application.Products.Commands.AdjustProductStock;
using Catalog.Application.Products.Commands.ChangeProductPrice;
using Catalog.Application.Products.Commands.CreateProduct;
using Catalog.Application.Products.Commands.DeactivateProduct;
using Catalog.Application.Products.Commands.DiscontinueProduct;
using Catalog.Application.Products.Commands.UpdateProductDetails;
using Catalog.Application.Products.Queries.GetProductById;
using Catalog.Application.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


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

        // POST /products (ya lo tenias; solo envolver 2xx)
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct)
        {
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

            var body = new
            {
                success = true,
                status = 201,
                message = "Product created successfully.",
                data = new { id = newId, resourceUrl = Url.Action(nameof(GetById), new { productId = newId }) },
                meta = (object?)null,
                traceId = HttpContext.TraceIdentifier,
                service = "catalog-api",
                timestampUtc = DateTime.UtcNow
            };

            return CreatedAtAction(nameof(GetById), new { productId = newId }, body);
        }

        // GET /products/{id} (real)
        [HttpGet("{productId:guid}")]
        [ProducesResponseType(typeof(GetProductByIdResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid productId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(productId), ct);

            var body = new GetProductByIdResponse
            {
                Data = new GetProductByIdResponse.DataEnvelope
                {
                    Product = new GetProductByIdResponse.Product
                    {
                        Id = result.Id,
                        VendorId = result.VendorId,
                        Sku = result.Sku,
                        Name = result.Name,
                        Description = result.Description,
                        Slug = result.Slug,
                        CategoryId = result.CategoryId,
                        Price = result.Price,
                        StockQuantity = result.StockQuantity,
                        IsActive = result.IsActive,
                        IsDiscontinued = result.IsDiscontinued,
                        CreatedAtUtc = result.CreatedAtUtc,
                        UpdatedAtUtc = result.UpdatedAtUtc,
                        MainImageUrl = result.MainImageUrl,
                        ImageUrls = result.ImageUrls
                    }
                },
                TraceId = HttpContext.TraceIdentifier
            };

            return Ok(body);
        }

        // GET /products (paginado + filtros)
        [HttpGet]
        [ProducesResponseType(typeof(GetProductsResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] GetProductsRequest q, CancellationToken ct)
        {
            var page = await _mediator.Send(new GetProductsQuery(
                q.Page, q.PageSize, q.Search, q.VendorId, q.CategoryId,
                q.MinPrice, q.MaxPrice, q.Active, q.Discontinued, q.SortBy, q.SortDir
            ), ct);

            var body = new GetProductsResponse
            {
                Data = new GetProductsResponse.GetProductsData
                {
                    Items = page.Items
    .Select(p => new GetProductsResponse.Item
    {
        Id = p.Id,
        VendorId = p.VendorId,
        Sku = p.Sku,
        Name = p.Name,
        Description = p.Description,
        Slug = p.Slug,
        CategoryId = p.CategoryId,
        Price = p.Price,
        StockQuantity = p.StockQuantity,
        IsActive = p.IsActive,
        IsDiscontinued = p.IsDiscontinued,
        CreatedAtUtc = p.CreatedAtUtc,
        UpdatedAtUtc = p.UpdatedAtUtc,
        MainImageUrl = p.MainImageUrl,
        ImageUrls = p.ImageUrls
    })
    .ToList()

                },
                Meta = new
                {
                    page = page.Page,
                    pageSize = page.PageSize,
                    totalItems = page.TotalItems,
                    totalPages = page.TotalPages,
                    hasNextPage = page.Page < page.TotalPages,
                    hasPrevPage = page.Page > 1,
                    sortBy = q.SortBy ?? "createdAt",
                    sortDir = q.SortDir ?? "desc"
                },
                TraceId = HttpContext.TraceIdentifier
            };

            return Ok(body);
        }

        [HttpPut("{productId:guid}/details")]
        [ProducesResponseType(typeof(SimpleOkResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDetails(Guid productId, [FromBody] UpdateProductDetailsRequest body, CancellationToken ct)
        {
            await _mediator.Send(new UpdateProductDetailsCommand(
                ProductId: productId,
                Name: body.Name,
                Description: body.Description,
                Slug: body.Slug,
                CategoryId: body.CategoryId
            ), ct);

            var resp = new SimpleOkResponse
            {
                Message = "Product details updated.",
                Data = new { id = productId, updatedAtUtc = DateTime.UtcNow },
                TraceId = HttpContext.TraceIdentifier
            };
            return Ok(resp);
        }

        [HttpPut("{productId:guid}/price")]
        [ProducesResponseType(typeof(SimpleOkResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangePrice(Guid productId, [FromBody] ChangeProductPriceRequest body, CancellationToken ct)
        {
            await _mediator.Send(new ChangeProductPriceCommand(productId, body.Price), ct);

            var resp = new SimpleOkResponse
            {
                Message = "Product price changed.",
                Data = new { id = productId, updatedAtUtc = DateTime.UtcNow },
                TraceId = HttpContext.TraceIdentifier
            };
            return Ok(resp);
        }

        [HttpPut("{productId:guid}/stock")]
        [ProducesResponseType(typeof(SimpleOkResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AdjustStock(Guid productId, [FromBody] AdjustProductStockRequest body, CancellationToken ct)
        {
            await _mediator.Send(new AdjustProductStockCommand(productId, body.Delta), ct);

            var resp = new SimpleOkResponse
            {
                Message = "Product stock adjusted.",
                Data = new { id = productId, updatedAtUtc = DateTime.UtcNow },
                TraceId = HttpContext.TraceIdentifier
            };
            return Ok(resp);
        }

        [HttpPut("{productId:guid}/deactivate")]
        [ProducesResponseType(typeof(SimpleOkResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(Guid productId, CancellationToken ct)
        {
            await _mediator.Send(new DeactivateProductCommand(productId), ct);

            var resp = new SimpleOkResponse
            {
                Message = "Product deactivated.",
                Data = new { id = productId, updatedAtUtc = DateTime.UtcNow },
                TraceId = HttpContext.TraceIdentifier
            };
            return Ok(resp);
        }

        // DELETE semantico de dominio: Discontinue
        [HttpDelete("{productId:guid}")]
        [ProducesResponseType(typeof(SimpleOkResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Discontinue(Guid productId, CancellationToken ct)
        {
            await _mediator.Send(new DiscontinueProductCommand(productId), ct);

            var resp = new SimpleOkResponse
            {
                Message = "Product discontinued.",
                Data = new { id = productId, updatedAtUtc = DateTime.UtcNow },
                TraceId = HttpContext.TraceIdentifier
            };
            return Ok(resp);
        }
    }
}
