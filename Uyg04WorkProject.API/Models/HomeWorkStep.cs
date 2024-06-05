namespace HomeWorkDelivery.API.Models
{
    public class HomeWorkStep
    {
        internal int homeWorkId;

        public int Id { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public int Score { get; set; }
        public int Order { get; set; }
        public int HomeWorkId { get; set; }
        public HomeWork HomeWork { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        
    }
}
