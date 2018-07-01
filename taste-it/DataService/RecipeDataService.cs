﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using taste_it.Models;

namespace taste_it.DataService
{
    public class RecipeDataService : IRecipeDataService
    {
        public async Task AddRecipeAsync(Recipe recipe,Category category, Tag tag)
        {
            var dbContext = new TasteItDbEntities();

            dbContext.Recipes.Add(recipe);
            await dbContext.SaveChangesAsync();
            Recipe currentRecipe = await dbContext.Recipes.FirstOrDefaultAsync(r => r.name == recipe.name);

            if (currentRecipe !=null)
            {

                dbContext.Have_category.Add(new Have_category{ id_c = category.id_c, id_r = currentRecipe.id_r });
                dbContext.Have_tags.Add(new Have_tags {id_t = tag.id_t,id_r=currentRecipe.id_r });
            }


            await dbContext.SaveChangesAsync();
        }

        public async Task AddRecipeAsync(Recipe recipe, Category category, List<Tag> tags)
        {
            var dbContext = new TasteItDbEntities();

            dbContext.Recipes.Add(recipe);
            await dbContext.SaveChangesAsync();
            Recipe currentRecipe = await dbContext.Recipes.FirstOrDefaultAsync(r => r.name == recipe.name);
           

            if (currentRecipe != null)
            {
                //foreach (var category in categories)
                //{
                //    dbContext.Have_category.Add(new Have_category { id_c = category.id_c, id_r = currentRecipe.id_r });
                //}
                dbContext.Have_category.Add(new Have_category { id_c = category.id_c, id_r = currentRecipe.id_r });

                foreach (var tag in tags)
                {
                    dbContext.Have_tags.Add(new Have_tags { id_t = tag.id_t, id_r = currentRecipe.id_r });
                }
                
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task AddToFavourites(User user, Recipe recipe)
        {
            var dbContext = new TasteItDbEntities();

            dbContext.Have_favourites.Add(new Have_favourites { id_u = user.id_u, id_r = recipe.id_r });

            await dbContext.SaveChangesAsync();

        }

        public async Task<IEnumerable<Recipe>> FindByCategory(Category category)  //one cateogory?
        {
            var dbContext = new TasteItDbEntities();
            Category current = await dbContext.Categories.FirstOrDefaultAsync(c => c.name == category.name);
            var list = current.Have_category.Select(f => f.id_r).ToList();
            return await dbContext.Recipes.AsNoTracking().Where(r => list.Contains(r.id_r)).ToListAsync();
        }

        public async Task<IEnumerable<Recipe>> FindByTagAsync(Tag tag)
        {
            var dbContext = new TasteItDbEntities();
            Tag current = await dbContext.Tags.FirstOrDefaultAsync(t => t.name == tag.name);
            var list = current.Have_tags.Select(t => t.id_r).ToList();
            return await dbContext.Recipes.AsNoTracking().Where(r => list.Contains(r.id_r)).ToListAsync();
        }

        public async Task<IEnumerable<Recipe>> FindFavouritesAsync(User user) //need to test
        {
            var dbContext = new TasteItDbEntities();
            User current = await dbContext.Users.FirstOrDefaultAsync(u => u.id_u == user.id_u);
            var list = current.Have_favourites.Select(f => f.id_r).ToList();
            return await dbContext.Recipes.AsNoTracking().Where(r => list.Contains(r.id_r)).ToListAsync();
        }
        public async Task<IEnumerable<Recipe>> FindByNameAsync(string name) //need to test
        {
           var dbContext = new TasteItDbEntities();
           return await  dbContext.Recipes.Where(r => r.name.Contains(name)).ToListAsync();
        }

        public async Task<IEnumerable<Recipe>> GetRecipesAsync()
        {
            var dbContext = new TasteItDbEntities();

            return await dbContext.Recipes.AsNoTracking().ToListAsync();
        }

        public async Task RemoveFavouriteRecipe(Recipe recipe, User user)
        {

            var dbContext = new TasteItDbEntities();
            var current = await dbContext.Have_favourites.AsNoTracking().ToListAsync();

            foreach (var fawRecipes in dbContext.Have_favourites.Where((f => f.id_r == recipe.id_r && f.id_u == user.id_u)).ToList())
            {
                dbContext.Have_favourites.Remove(fawRecipes);
            }
            await dbContext.SaveChangesAsync();
        }

    }
}
