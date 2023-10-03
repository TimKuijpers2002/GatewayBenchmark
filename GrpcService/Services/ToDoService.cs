using GrpcService.Data;
using GrpcService.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace GrpcService.Services
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
                Id = toDoItem.Id
            });
        }

        public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
        {
            if(request.Id <= 0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Resource index must be greater than 0"));
            }
            var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);

            if(toDoItem != null) 
            {
                return await Task.FromResult(new ReadToDoResponse 
                { 
                    Id = toDoItem.Id,
                    Title = toDoItem.Title,
                    Description = toDoItem.Description,
                    Status = toDoItem.Status
                });
            }

            throw new RpcException(new Status(StatusCode.NotFound, $"No task with id {request.Id}"));
        }

        public override async Task<GetAllToDoResponse> GetAllToDo(GetAllToDoRequest request, ServerCallContext context)
        {
            var response = new GetAllToDoResponse();
            var toDoItems = await _dbContext.ToDoItems.ToListAsync();

            foreach( var toDoItem in toDoItems)
            {
                response.ToDo.Add(new ReadToDoResponse
                {
                    Id = toDoItem.Id,
                    Title = toDoItem.Title,
                    Description = toDoItem.Description,
                    Status = toDoItem.Status
                });
            }

            return await Task.FromResult(response);
        }

        public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0 || request.Title == string.Empty || request.Description == string.Empty)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must provide a valid input"));
            }
            var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);

            if (toDoItem == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"No task with id {request.Id}"));
            }

            toDoItem.Title = request.Title;
            toDoItem.Description = request.Description;
            toDoItem.Status = request.Status;

            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new UpdateToDoResponse
            {
                Id = toDoItem.Id
            });
        }

        public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must provide a valid input"));
            }
            var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);

            if (toDoItem == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"No task with id {request.Id}"));
            }

            _dbContext.ToDoItems.Remove(toDoItem);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new DeleteToDoResponse
            {
                Id = toDoItem.Id
            });
        }
    }
}
