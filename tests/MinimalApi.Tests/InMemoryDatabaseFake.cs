using System.Collections.Generic;

namespace MinimalApi.Tests
{
    internal class InMemoryDatabaseFake : InMemoryDatabase
    {
        public const string TODO_ITEM1_ID = "a1342443-827e-4674-8161-4e5892fc9da4";
        public const string TODO_ITEM2_ID = "05300732-c36d-4d8c-a0f4-7d925f4bd692";
        public const string TODO_ITEM3_ID = "1e21d12c-baf2-4d53-a09f-ad73e71a585d";

        public static int ItemsCount 
        { 
            get {
                return todos.Count;  
            } 
        }

        public InMemoryDatabaseFake()
        {
            todos = new Dictionary<string, ToDo>
            {
                { TODO_ITEM1_ID, new ToDo { Id = TODO_ITEM1_ID, Description = $"Description {TODO_ITEM1_ID}", Done = false } },
                { TODO_ITEM2_ID, new ToDo { Id = TODO_ITEM2_ID, Description = $"Description {TODO_ITEM2_ID}", Done = false } },
                { TODO_ITEM3_ID, new ToDo { Id = TODO_ITEM3_ID, Description = $"Description {TODO_ITEM3_ID}", Done = false } }
            };
        }
    }
}
