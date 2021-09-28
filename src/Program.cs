var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDatabase, InMemoryDatabase>();

var app = builder.Build();

app.MapGet("/todos", (IDatabase database) =>
{
    var toDos = database.Get();
    return Results.Ok(new GetAllToDoCollectionResponse(toDos.Select(x => new GetToDoItemResponse(x.Id, x.Description, x.Done))));
});

app.MapGet("/todos/{id}", (string id, IDatabase database) =>
{
    return database.Get(id) is ToDo toDo
            ? Results.Ok(new GetToDoItemResponse(toDo.Id, toDo.Description, toDo.Done))
            : Results.NotFound();
});

app.MapPost("/todos", (CreateToDoRequest toDo, IDatabase database) =>
{
    if (toDo is null || string.IsNullOrWhiteSpace(toDo.Description))
        return Results.BadRequest("A todo description must be specified.");

    var newToDo = new ToDo
    {
        Id = Guid.NewGuid().ToString(),
        Description = toDo.Description,
        Done = false,
    };

    database.SaveOrUpdate(newToDo);

    return Results.Created($"todos/{newToDo.Id}", new CreateToDoResponse(newToDo.Id, newToDo.Description, newToDo.Done));
});

app.Run();

public record GetToDoItemResponse(string Id, string Description, bool Done);
public record GetAllToDoCollectionResponse(IEnumerable<GetToDoItemResponse> Items);

public record CreateToDoRequest(string Description);
public record CreateToDoResponse(string Id, string Description, bool Done);

public interface IDatabase
{
    IEnumerable<ToDo> Get();
    ToDo Get(string id);
    void SaveOrUpdate(ToDo todo);
}

public class InMemoryDatabase : IDatabase
{
    protected static Dictionary<string, ToDo> todos = new Dictionary<string, ToDo>();

    public void SaveOrUpdate(ToDo todo)
    {
        if (todos.ContainsKey(todo.Id))
            todos[todo.Id] = todo;
        else
            todos.Add(todo.Id, todo);
    }

    public IEnumerable<ToDo> Get()
    {
        return todos.Select(x => x.Value);
    }

    public ToDo Get(string id)
    {
        return todos.GetValueOrDefault(id);
    }
}

public class ToDo
{
    public string Id { get; set; }

    public string Description { get; set; }

    public bool Done { get; set; }
}


