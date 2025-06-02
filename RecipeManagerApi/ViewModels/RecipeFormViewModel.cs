using RecipeManager.Models;

namespace RecipeManager.ViewModels
{
    public class RecipeFormViewModel
    {
        public Recipe Recipe { get; set; }
        public List<DietaryTag> DietaryTags { get; set; }
    }
}
