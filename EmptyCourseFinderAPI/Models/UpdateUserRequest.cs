namespace EmptyCourseFinderAPI.Models
{
    public class UpdateUserRequest
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Lon { get; set; }
        public string Lat { get; set; }
        public int TimeStart { get; set; }
        public int TimeEnd { get; set; }
    }
}
