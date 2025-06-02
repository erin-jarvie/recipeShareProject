namespace RecipeManager.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Method { get; set; }
        public int CookingTimeMins { get; set; }
        public int NoOfServings { get; set; }
        public int? DietaryTagId { get; set; }
        public virtual List<RecipeIngredient> Ingredients { get; set; }
        public virtual DietaryTag DietaryTag { get; set; }
    }
}
