using GatewayBenchmark.Data;
using GatewayBenchmark.Models;
using Grpc.Core;

namespace GatewayBenchmark.Services
{
    public class ToDoService : ToDoIt.ToDoItBase
    {
        private readonly AppDbContext _dbContext;
        public ToDoService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<CreateToDoResponse> CreateToDo(CreateToDoRequest request, ServerCallContext context) 
        { 
            if (request.Title == string.Empty || request.Description == string.Empty) 
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "You must provide a valid input"));
                }
            var toDoItem = new ToDoItem
            {
                Title = request.Title,
                Description = request.Description,
            };

            await _dbContext.AddAsync(toDoItem);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new CreateToDoResponse
            {
                Id = toDoItem.Id,
            });
        }
    }
}
