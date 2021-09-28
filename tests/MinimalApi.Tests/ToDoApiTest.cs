using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace MinimalApi.Tests
{
    public class ToDoApiTest
    {
        [Fact]
        public async Task GetAllToDoItems_ShouldReturnAllToDoItems()
        {
            await using var application = new MinimalApiApplication();

            var client = application.CreateClient();
            var todos = await client.GetFromJsonAsync<GetAllToDoCollectionResponse>("/todos");
            
            Assert.NotNull(todos);
            Assert.Equal(InMemoryDatabaseFake.ItemsCount, todos.Items.Count());
        }

        [Theory]
        [InlineData(InMemoryDatabaseFake.TODO_ITEM1_ID)]
        [InlineData(InMemoryDatabaseFake.TODO_ITEM2_ID)]
        [InlineData(InMemoryDatabaseFake.TODO_ITEM3_ID)]
        public async Task GetOneToDoItem_WhenCallingWithAnExistingId_ShouldReturnOneToDoItem(string id)
        {
            await using var application = new MinimalApiApplication();

            var client = application.CreateClient();
            var toDo = await client.GetFromJsonAsync<GetToDoItemResponse>($"/todos/{id}");

            Assert.NotNull(toDo);
            Assert.Equal(id, toDo.Id);
        }

        [Fact]
        public async Task GetOneToDoItem_WhenCallingWithANotExistingId_ShouldReturnNotFound()
        {
            const string notExistingId = "notExistingId";

            await using var application = new MinimalApiApplication();

            var client = application.CreateClient();
            var response = await client.GetAsync($"/todos/{notExistingId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostToDoItem_ShouldCreateNewToDoItem()
        {
            const string toDoDescription = "new todo item";
            const bool doneDefaultValue = false;

            await using var application = new MinimalApiApplication();

            var client = application.CreateClient();
            var response = await client.PostAsJsonAsync("/todos", new CreateToDoRequest(toDoDescription));

            var todoItem = JsonSerializer.Deserialize<CreateToDoResponse>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(todoItem);
            Assert.Equal(toDoDescription, todoItem.Description);
            Assert.Equal(doneDefaultValue, todoItem.Done);
            Assert.False(string.IsNullOrWhiteSpace(todoItem.Id));
        }

        [Fact]
        public async Task PostToDoItem_WhenNotSpecifyADescription_ShouldReturnBadRequest()
        {
            await using var application = new MinimalApiApplication();

            var client = application.CreateClient();
            var response = await client.PostAsJsonAsync("/todos", new { });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostToDoItem_WhenSpecifyAnEmptyDescription_ShouldReturnBadRequest()
        {
            const string toDoDescription = "";

            await using var application = new MinimalApiApplication();

            var client = application.CreateClient();
            var response = await client.PostAsJsonAsync("/todos", new CreateToDoRequest(toDoDescription));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
