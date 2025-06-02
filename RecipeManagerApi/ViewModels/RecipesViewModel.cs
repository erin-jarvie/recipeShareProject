using RecipeManager.Models;

namespace RecipeManager.ViewModels
{
    public class RecipesViewModel
    {
        public List<Recipe> Recipes { get; set; }
        public List<DietaryTag> DietaryTags { get; set; }
    }
}
