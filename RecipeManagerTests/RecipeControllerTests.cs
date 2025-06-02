using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Commands;
using RecipeManager.Controllers;
using RecipeManager.Data;
using RecipeManager.Models;
using RecipeManager.ViewModels;

namespace RecipeManager.Tests
{
    public class RecipesControllerTests : IDisposable
    {
        private readonly AppDbContext _dbContext;
        private readonly RecipesController _controller;
        private readonly string _dbName;

        public RecipesControllerTests()
        {
            // Create unique database name for each test
            _dbName = Guid.NewGuid().ToString();
            _dbContext = TestDbContextFactory.CreateDbContext(_dbName);
            _controller = new RecipesController(_dbContext);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        [Fact]
        public async Task GetRecipe()
        {

            var result = await _controller.GetRecipe(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var recipe = Assert.IsAssignableFrom<Recipe>(okResult.Value);
            Assert.Equal(1, recipe.Id);
        }

        [Fact]
        public async Task GetRecipe_Nonexistant_id()
        {
            var result = await _controller.GetRecipe(1000);


            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetAllRecipes()
        {

            var command = new GetRecipesCommand() { TagId = null, SearchString = null };

            var result = await _controller.GetRecipes(command);

            var recipe = Assert.IsAssignableFrom<RecipesViewModel>(result.Value);

            Assert.Equal(2, result.Value.Recipes.Count());
            Assert.Equal(2, result.Value.Recipes.First().Ingredients.Count());
            Assert.Equal("Veg", result.Value.Recipes.First().DietaryTag.Name);
        }

        [Fact]
        public async Task GetRecipes_filterBySearch_RecipeName()
        {
            var command = new GetRecipesCommand() { TagId = null, SearchString = "Tes" };

            var result = await _controller.GetRecipes(command);

            Assert.Single(result.Value.Recipes);
        }

        [Fact]
        public async Task GetRecipes_filterBySearch_IngredientName()
        {
            
            var command = new GetRecipesCommand() { TagId = null, SearchString = "Flo" };

            var result = await _controller.GetRecipes(command);

            Assert.Single(result.Value.Recipes);
        }

        [Fact]
        public async Task GetRecipes_filterByTagId()
        {
            var command = new GetRecipesCommand() { TagId = 1, SearchString = null };

            var result = await _controller.GetRecipes(command);

            Assert.Single(result.Value.Recipes);
        }

        [Fact]
        public async Task CreateRecipe()
        {
            var ingredient = new RecipeIngredient()
            {
                Id = 0,
                Name = "Bread",
                Amount = 1,
                Unit = "Slice"
            };

            var recipe = new Recipe
            {
                Id = 0,
                Name = "New recipe",
                Description = "Testing add",
                Ingredients = new List<RecipeIngredient> { ingredient },
                DietaryTagId = 2,
                CookingTimeMins = 10,
                Method = "Cook",
                NoOfServings = 2
            };

            var result = await _controller.CreateRecipe(recipe);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdRecipe = Assert.IsType<Recipe>(createdResult.Value);
            Assert.Equal("New recipe", createdRecipe.Name);
            Assert.Equal(1, createdRecipe.Ingredients.Count());

            var insertedRecipe = _dbContext.Recipes.FirstOrDefault(r => r.Name == "New recipe");
            Assert.NotNull(insertedRecipe);
            Assert.Equal("Testing add", insertedRecipe.Description);
            Assert.Equal(2, insertedRecipe.DietaryTagId);

        }

        [Fact]
        public async Task CreateRecipe_duplicateName()
        {
            var recipe = new Recipe
            {
                Id = 0,
                Name = "Test Recipe",
                Description = "Testing add",
                DietaryTagId = 2,
                CookingTimeMins = 10,
                Method = "Cook",
                NoOfServings = 2
            };

            var result = await _controller.CreateRecipe(recipe);

            Assert.IsType<BadRequestObjectResult>(result.Result);

        }


        [Fact]
        public async Task CreateRecipe_noName()
        {
            var recipe = new Recipe
            {
                Id = 0,
                Name = "",
                Description = "Testing add",
                DietaryTagId = 2,
                CookingTimeMins = 10,
                Method = "Cook",
                NoOfServings = 2
            };

            var result = await _controller.CreateRecipe(recipe);

            Assert.IsType<BadRequestObjectResult>(result.Result);

        }

        [Fact]
        public async Task CreateRecipe_noTime()
        {
            var dbContext = TestDbContextFactory.CreateDbContext();
            var controller = new RecipesController(dbContext);

            var recipe = new Recipe
            {
                Id = 0,
                Name = "Testing Add",
                Description = "Testing add",
                DietaryTagId = 2,
                CookingTimeMins = 0,
                Method = "Cook",
                NoOfServings = 2
            };

            var result = await controller.CreateRecipe(recipe);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateRecipe_noIngredients()
        {
            var recipe = new Recipe
            {
                Id = 0,
                Name = "Testing Add",
                Description = "Testing add",
                DietaryTagId = 2,
                CookingTimeMins = 10,
                Method = "Cook",
                NoOfServings = 2
            };

            var result = await _controller.CreateRecipe(recipe);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateRecipe()
        {
            var newingredient = new RecipeIngredient()
            {
                Id = 0,
                Name = "Bread",
                Amount = 1,
                Unit = "Slice"
            };

            var existingingredient = new RecipeIngredient()
            {
                Id = 1,
                Name = "Flour",
                RecipeId = 1,
                Amount = 300,
                Unit = "g"
            };

            var recipe = new Recipe
            {
                Id = 1,
                Name = "Test Recipe - update",
                Description = "Test -update",
                Method = "Bake",
                DietaryTagId = 1,
                CookingTimeMins = 30,
                NoOfServings = 4,
                Ingredients = new List<RecipeIngredient> { newingredient, existingingredient }
            };

            var result = await _controller.UpdateRecipe(1, recipe);

            var updateresult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var recipeOut = Assert.IsType<Recipe>(updateresult.Value);
            Assert.Equal(recipe.Name, recipeOut.Name);
            Assert.Equal(2, recipe.Ingredients.Count());

            var updatedRecipe = _dbContext.Recipes.FirstOrDefault(r => r.Name == recipe.Name);
            Assert.NotNull(updatedRecipe);
            Assert.Equal(recipe.Description, updatedRecipe.Description);

            var addedIncredient = _dbContext.RecipeIngredients.FirstOrDefault(r => r.Name == "Bread");
            Assert.NotNull(addedIncredient);

            var updatedIncredient = _dbContext.RecipeIngredients.FirstOrDefault(r => r.Name == "Flour");
            Assert.NotNull(updatedIncredient);
            Assert.Equal(300, updatedIncredient.Amount);

            var removedIngredient = _dbContext.RecipeIngredients.FirstOrDefault(r => r.Name == "Eggs");
            Assert.Null(removedIngredient);

        }

        [Fact]
        public async Task UpdateRecipe_invalidId()
        {
            var recipe = new Recipe
            {
                Id = 1000,
                Name = "Second Recipe",
                Description = "Test -update",
                Method = "Bake",
                DietaryTagId = 1,
                CookingTimeMins = 30,
                NoOfServings = 4
            };

            var result = await _controller.UpdateRecipe(1000, recipe);

            Assert.IsType<NotFoundResult>(result.Result);

        }

        [Fact]
        public async Task UpdateRecipe_duplicateName()
        {
            
            var recipe = new Recipe
            {
                Id = 1,
                Name = "Second Recipe",
                Description = "Test -update",
                Method = "Bake",
                DietaryTagId = 1,
                CookingTimeMins = 30,
                NoOfServings = 4
            };

            var result = await _controller.UpdateRecipe(1, recipe);

            Assert.IsType<BadRequestObjectResult>(result.Result);

        }

        [Fact]
        public async Task UpdateRecipe_noCookingTime()
        {
            var recipe = new Recipe
            {
                Id = 1,
                Name = "test -update",
                Description = "Test -update",
                Method = "Bake",
                DietaryTagId = 1,
                CookingTimeMins = 0,
                NoOfServings = 4
            };

            var result = await _controller.UpdateRecipe(1, recipe);

            Assert.IsType<BadRequestObjectResult>(result.Result);

        }

        [Fact]
        public async Task UpdateRecipe_noIngredients()
        {
            var recipe = new Recipe
            {
                Id = 1,
                Name = "test -update",
                Description = "Test -update",
                Method = "Bake",
                DietaryTagId = 1,
                CookingTimeMins = 20,
                NoOfServings = 4
            };

            var result = await _controller.UpdateRecipe(1, recipe);

            Assert.IsType<BadRequestObjectResult>(result.Result);

        }

        [Fact]
        public async Task DeleteRecipe()
        {
            var result = await _controller.DeleteRecipe(1);

            Assert.IsType<NoContentResult>(result);

            var deletedRecipe = await _dbContext.Recipes.FindAsync(1);
            Assert.Null(deletedRecipe);

            var linkedIngredients = _dbContext.RecipeIngredients
                .Where(i => i.RecipeId == 1)
                .ToList();
            Assert.Empty(linkedIngredients);
        }

        [Fact]
        public async Task DeleteRecipe_invalidId()
        {

            var result = await _controller.DeleteRecipe(1000);

            Assert.IsType<NotFoundResult>(result);
        }

    }
}
