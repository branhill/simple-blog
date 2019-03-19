namespace SimpleBlog.Models
{
    public class Config
    {
        public DatabaseModel Database { get; set; } = new DatabaseModel();

        public string SiteName { get; set; }

        public class DatabaseModel
        {
            public ProviderEnum Provider { get; set; }

            public string ConnectionStrings { get; set; }

            public enum ProviderEnum
            {
                SqlServer,
                Sqlite
            }
        }
    }
}
