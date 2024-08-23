namespace ExpenseTracking.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? NickName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PassRepeat { get; set; }
        public IFormFile? Img { get; set; }
        public string? ImgPath { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
