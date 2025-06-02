using Microsoft.EntityFrameworkCore;
using RecipeManager.Data;
using RecipeManager.Models;

namespace RecipeManager.Tests
{
    public static class TestDbContextFactory
    {
        public static AppDbContext CreateDbContext(string dbName = "TestDb")
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName) 
                .Options;

            var context = new AppDbContext(options);

            SeedData(context);

            return context;
        }

        private static void SeedData(AppDbContext context)
        {
            var recipe1 = new Recipe
            {
                Id = 1,
                Name = "Test Recipe",
                Description = "Test",
                Method = "Bake",
                DietaryTagId = 1,
                CookingTimeMins = 30,
                NoOfServings = 4
            };

            var recipe2 = new Recipe
            {
                Id = 2,
                Name = "Second Recipe",
                Description = "Test",
                Method = "Fry",
                CookingTimeMins = 30,
                NoOfServings = 4
            };

            var ingredientA = new RecipeIngredient
            {
                Id = 1,
                Name = "Flour",
                RecipeId = 1,
                Amount = 200,
                Unit = "g"
            };

            var ingredientB = new RecipeIngredient
            {
                Id = 2,
                Name = "Eggs",
                RecipeId = 1,
                Amount = 2
            };

            var ingredientC = new RecipeIngredient
            {
                Id = 3,
                Name = "Onion",
                RecipeId = 2,
                Amount = 1
            };

            var tagA = new DietaryTag
            {
                Id = 1,
                Name = "Veg"
            };

            var tagB = new DietaryTag
            {
                Id = 2,
                Name = "Vegan"
            };


            context.Recipes.Add(recipe1);
            context.Recipes.Add(recipe2);
            context.RecipeIngredients.Add(ingredientA);
            context.RecipeIngredients.Add(ingredientB);
            context.RecipeIngredients.Add(ingredientC);
            context.DietaryTags.Add(tagA);
            context.DietaryTags.Add(tagB);
            context.SaveChanges();
        }
    }
}
