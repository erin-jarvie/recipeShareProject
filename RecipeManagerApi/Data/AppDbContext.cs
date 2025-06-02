using Microsoft.EntityFrameworkCore;
using RecipeManager.Models;

namespace RecipeManager.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<DietaryTag> DietaryTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Recipe>().HasData(
                new Recipe
                {
                    Id = 1,
                    Name = "Basic Tomato Pasta",
                    Description = "Your go to easy meal",
                    CookingTimeMins = 30,
                    NoOfServings = 4,
                    DietaryTagId = null,
                    Method = "Chop your onions, mushrooms and chorizo. Heat the oil in the pan. Once hot, fry the onions and chorizo until the onion is traslucent. Add your mushrooms and garlic and cook for a fit longer. Then add your tinned tomatoes and some water. Cook until the oil starts to separate from the sauce. In the mean time, boil some water with some salt. Add your pasta and cook until el dente. Add some herbs, chilli or whatever you want. Sprinkle some cheese on top. And enjoy"
                },
                new Recipe
                {
                    Id = 2,
                    Name = "Toasted sandwich",
                    Description = "Simple. But tastey.",
                    CookingTimeMins = 5,
                    NoOfServings = 1,
                    DietaryTagId = 1,
                    Method = "Spread some mayo on each slice of bread. Add cheese and tomato, salt and pepper. Put in the sandwhich press until the cheese is melty"
                },
                new Recipe
                {
                    Id = 3,
                    Name = "Homemade Icecream",
                    Description = "The best icecream ever. I promise",
                    CookingTimeMins = 15,
                    NoOfServings = 12,
                    DietaryTagId = 1,
                    Method = "Whip your cream until it's stiff. Fold in the condesned milk and whatever other flavours you want. Put it in the freezer and leave it alone until it's frozen. Then try not to devour it in one go"
                }
            );

            modelBuilder.Entity<RecipeIngredient>().HasData(
                new RecipeIngredient
                {
                    Id = 1,
                    RecipeId = 1,
                    Name = "Tinned Tomato",
                    Amount = 2,
                    Unit = "Cans"
                },
                new RecipeIngredient
                {
                    Id = 2,
                    RecipeId = 1,
                    Name = "Onion",
                    Amount = 1
                },
                new RecipeIngredient
                {
                    Id = 3,
                    RecipeId = 1,
                    Name = "Mushrooms",
                    Amount = 1,
                    Unit = "punnet"
                },
                new RecipeIngredient
                {
                    Id = 4,
                    RecipeId = 1,
                    Name = "Chorizo",
                    Amount = 100,
                    Unit = "grams"
                },
                new RecipeIngredient
                {
                    Id = 5,
                    RecipeId = 1,
                    Name = "Garlic",
                    Amount = 3,
                    Unit = "cloves"
                },
                new RecipeIngredient
                {
                    Id = 6,
                    RecipeId = 1,
                    Name = "Pasta",
                    Amount = 300,
                    Unit = "grams"
                },
                new RecipeIngredient
                {
                    Id = 7,
                    RecipeId = 2,
                    Name = "Bread",
                    Amount = 2,
                    Unit = "slices"
                },
                new RecipeIngredient
                {
                    Id = 8,
                    RecipeId = 2,
                    Name = "Mayo"
                },
                new RecipeIngredient
                {
                    Id = 9,
                    RecipeId = 2,
                    Name = "Tomato",
                    Amount = 1
                },
                new RecipeIngredient
                {
                    Id = 10,
                    RecipeId = 2,
                    Name = "Cheese",
                    Amount = 10,
                    Unit = "Slices"
                },
                new RecipeIngredient
                {
                    Id = 11,
                    RecipeId = 3,
                    Name = "Cream",
                    Amount = 500,
                    Unit = "ml"
                },
                new RecipeIngredient
                {
                    Id = 12,
                    RecipeId = 3,
                    Name = "Condensed Milk",
                    Amount = 1,
                    Unit = "can"
                },
                new RecipeIngredient
                {
                    Id = 13,
                    RecipeId = 3,
                    Name = "Whatever flavours you want to add (eg. Milo, oreos, peanut butter etc)"
                }
            );

            modelBuilder.Entity<DietaryTag>().HasData(
               new DietaryTag
               {
                   Id = 1,
                   Name = "Vegegarian"
               },
               new DietaryTag 
               {
                   Id = 2,
                   Name = "Vegan",
               },
               new DietaryTag
               {
                   Id = 3,
                   Name = "Gluten Fee",
               }
           );
        }
    }
}
