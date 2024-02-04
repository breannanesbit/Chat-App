using Redis.OM.Modeling;

namespace RedisApp.Model
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "Person" })]
    public class Person
    {
        [RedisIdField]
        public string Id { get; set; }

        [Indexed]
        public string FirstName { get; set; }

        [Indexed]
        public string? LastName { get; set; }

        [Indexed(Sortable = true)]
        public int Age { get; set; }

        [Indexed]
        public bool Verified { get; set; }

        [Indexed]
        public GeoLoc Location { get; set; }

        [Indexed]
        public string[] Skills { get; set; } = Array.Empty<string>();

        [Searchable]
        public string PersonalStatement { get; set; }
    }
}
