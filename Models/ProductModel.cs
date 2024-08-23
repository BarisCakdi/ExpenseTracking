using System.ComponentModel.DataAnnotations;

namespace ExpenseTracking.Models
{
    public class Gain
    {
        public int Id { get; set; }
        public string GainName { get; set; }
        public int GainAmount { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
    }
    public class Cost
    {
        public int Id { get; set; }
        public string CostName { get; set; }
        public int CostAmount { get; set; }
        public int? CategoryId { get; set; }
        public int? UserId { get; set; }
    }


        public class Category
    {
        public int Id { get; set; }
        [Required]
        public string CateName { get; set; }
        public int UserId { get; set; }
    }

    public class GainCostAndCate
    {
        public List<Gain> Gain { get; set; }
        public List<Cost> Cost { get; set; }
        public List<Category> categories { get; set; }
    }   
}