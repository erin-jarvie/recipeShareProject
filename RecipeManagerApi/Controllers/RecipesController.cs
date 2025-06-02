using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Commands;
using RecipeManager.Data;
using RecipeManager.Models;
using RecipeManager.ViewModels;

namespace RecipeManager.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public RecipesController(AppDbContext dbContext) => _dbContext = dbContext;

        [HttpGet]
        public async Task<ActionResult<RecipesViewModel>> GetRecipes([FromQuery] GetRecipesCommand command)
        {
            var recipes = await _dbContext.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.DietaryTag)
            .Where(r =>
                (command.TagId == null || r.DietaryTagId == command.TagId) &&
                (string.IsNullOrEmpty(command.SearchString) ||
                 r.Name.Contains(command.SearchString) ||
                 r.Ingredients.Any(i => i.Name.Contains(command.SearchString))))
            .ToListAsync();

            var dietaryTags = await _dbContext.DietaryTags.ToListAsync();

            return new RecipesViewModel()
            {
                Recipes = recipes,
                DietaryTags = dietaryTags
            };
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Recipe>> GetRecipe(int id)
        {
            var recipe = await _dbContext.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.DietaryTag)
                .FirstOrDefaultAsync(r => r.Id == id);

            return recipe == null ? NotFound() : Ok(recipe);
        }

        [HttpGet("form")]
        public async Task<ActionResult<RecipeFormViewModel>> GetForm([FromQuery] int? id)
        {
            var result = new RecipeFormViewModel();

            if (id != null)
            {
                var recipe = await _dbContext.Recipes
                    .Include(r => r.Ingredients)
                    .Include(r => r.DietaryTag)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (recipe == null) return NotFound();

                result.Recipe = recipe;
            }

            result.DietaryTags = await _dbContext.DietaryTags.ToListAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Recipe>> CreateRecipe([FromBody] Recipe recipe)
        {
            var duplicate = await _dbContext.Recipes.FirstOrDefaultAsync(r => r.Name == recipe.Name);
            if (duplicate != null) return BadRequest("A recipe already exists with the name " + recipe.Name);

            if (string.IsNullOrEmpty(recipe.Name)) return BadRequest("Recipe must have name");
            if (recipe.CookingTimeMins == 0) return BadRequest("Recipe must have a cooking time");
            if (recipe.Ingredients == null || recipe.Ingredients.Count() == 0) return BadRequest("Recipe must have at least 1 ingredient");

            _dbContext.Add(recipe);

            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Recipe>> UpdateRecipe([FromRoute] int id, [FromBody] Recipe recipe)
        {
            var existingRecipe = await _dbContext.Recipes
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existingRecipe == null) return NotFound();

            var duplicate = await _dbContext.Recipes
                .FirstOrDefaultAsync(r => r.Name == recipe.Name && r.Id != id);
            if (duplicate != null) return BadRequest("A recipe already exists with the name " + recipe.Name);

            if (string.IsNullOrEmpty(recipe.Name)) return BadRequest("Recipe must have name");
            if (recipe.CookingTimeMins == 0) return BadRequest("Recipe must have a cooking time");
            if (recipe.Ingredients == null || recipe.Ingredients.Count() == 0) return BadRequest("Recipe must have at least 1 ingredient");


            var incomingIds = recipe.Ingredients.Select(i => i.Id).ToList();

            var removedIngredients = existingRecipe.Ingredients.Where(i => !incomingIds.Contains(i.Id)).ToList();
            _dbContext.RecipeIngredients.RemoveRange(removedIngredients);

            foreach (var ingredient in recipe.Ingredients)
            {
                var existing = existingRecipe.Ingredients.FirstOrDefault(i => i.Id == ingredient.Id);
                if (existing != null)
                {
                    existing.Name = ingredient.Name;
                    existing.Amount = ingredient.Amount;
                    existing.Unit = ingredient.Unit;
                }
                else
                {
                    existingRecipe.Ingredients.Add(ingredient);
                }
            }

            existingRecipe.Name = recipe.Name;
            existingRecipe.Description = recipe.Description;
            existingRecipe.Method = recipe.Method;
            existingRecipe.CookingTimeMins = recipe.CookingTimeMins;
            existingRecipe.NoOfServings = recipe.NoOfServings;
            existingRecipe.DietaryTagId = recipe.DietaryTagId;

            _dbContext.Update(existingRecipe);

            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRecipe), new { id = id }, recipe);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe = await _dbContext.Recipes.FindAsync(id);
            if (recipe == null) return NotFound();

            _dbContext.Recipes.Remove(recipe);
            await _dbContext.SaveChangesAsync();

            //check if you need to remove ingredients

            return NoContent();
        }


        
    }
}
