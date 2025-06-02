namespace RecipeManager.Models
{
    public class RecipeIngredient
    {   
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public decimal? Amount { get; set; }
        public string? Unit { get; set; }
    }
}
