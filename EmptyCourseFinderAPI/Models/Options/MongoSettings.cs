namespace EmptyCourseFinderAPI.Models.Options
{
    public class MongoSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public string UserCollection { get; set; }
        public string LeagueCollection { get; set; }
    }
}
