using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using ResultPatternTesting;
using ResultPatternTesting.Entity;
using ResultPatternTesting.Features.Command;
using ResultPatternTesting.Features.Query;
using ResultPatternTesting.Services;

namespace ResultPatternTesting.Controllers
{

    public class PaginationActionConstraint : IActionConstraint
    {
        private readonly string _parameterName;

        public PaginationActionConstraint(string parameterName)
        {
            _parameterName = parameterName;
        }

        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            return context.RouteContext.HttpContext.Request.Query.ContainsKey(_parameterName);
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
    }

    [Route("data")]
    public class DataController : ControllerBase
    {
        private readonly DataDbContext _dbContext;
        private readonly DataService _dataService;
        private readonly IMediator _mediator;

        public DataController(DataDbContext dbContext, DataService dataService, IMediator mediator)
        {
            _dbContext = dbContext;
            _dataService = dataService;
            _mediator = mediator;
        }
        [HttpGet("as")]
        public IActionResult GetAllProducts()
        {
            // Logic to retrieve all products
            var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop" },
            new Product { Id = 2, Name = "Smartphone" }
        };
            return Ok(products);
        }

        // GET: api/products?category=electronics
        [HttpGet("as")]
        public IActionResult GetProductsByCategory([FromQuery] string category)
        {
            // Logic to filter products by category
            var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Category = "Electronics" },
            new Product { Id = 3, Name = "Tablet", Category = "Electronics" }
        }.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

            return Ok(products);
        }
        [HttpPost]
        public async Task<ActionResult/*<int>*/> Post([FromBody] CreateCommand command)
        {
            //var result = _dataService.Post(data);
            //if(result.IsSuccess)
            //{
            //    return Ok(result.Value);
            //}
            //return BadRequest();
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return HandleFailure(result);
            //alternatywa dla HandleFailure

            //IValidationResult result = (IValidationResult)result;
            //return BadRequest(new ProblemDetails()
            //{
            //    Title = "validation-error",
            //    Status = StatusCodes.Status400BadRequest,
            //    Type = result.Error.Code,
            //    Detail = result.Error.Message,
            //    Extensions = { { nameof(validationResult.Errors), validationResult.Errors } }
            //});
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Data>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllQuery());
            return Ok(result.Value);
        }
        //[HttpGet("123")]
        //public async Task<ActionResult<IEnumerable<Data>>> GetAllFromDbContext()
        //{
        //    var result = await _dbContext.Datas.ToListAsync();
        //    return Ok(result);
        //}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Data>> GetById([FromRoute]int id)
        {
            var result = await _mediator.Send(new GetByIdQuery(id));
            if (result.IsFailure)
                return HandleFailure(result.Error);
            return Ok(result.Value);
        }
        private ActionResult HandleFailure(Result result)
        => result switch
        {
            { IsSuccess: true } => throw new InvalidOperationException(),
            IValidationResult validationResult => BadRequest(CreateProblemDetails(
                "Validation Error", StatusCodes.Status400BadRequest, result.Error, validationResult.Errors
                )),
            _ => BadRequest(result.Error)
        };
        private static ProblemDetails CreateProblemDetails(string title, int status, Error error, Error[]? errors = null)
        {
            return new()
            {
                Title = title,
                Status = status,
                Type = error.Code,
                Detail = error.Description,
                Errors = errors
            };
        }

    }
    public class ProblemDetails
    {
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public string? Detail { get; set; }
        public string Type { get; set; } = string.Empty;
        public Error[]? Errors { get; set; } = default!;
    }
    //public class Extension
    //{
    //    public string Code { get; set; } = string.Empty;
    //    public string Message { get; set; } = string.Empty;
    //}
}

