using System;
using System.Collections.Generic;

namespace GameServer
{
    public class GameInfo
    {
        private Item[] items;
        private Item[] ores;
        private Recipe[] recipes;

        public int[,] groundTexture;
        public int[,] oreMap;

        public void LoadGame()
        {
            LoadItems();
            
            if (items.Length <= 0)
            {
                Console.WriteLine("Items Empty");
            }
            else
            {
                List<Item> tempOres = new List<Item>();

                foreach (var item in items)
                {
                    if (item.mineable)
                    {
                        tempOres.Add(item);
                    }

                    Console.WriteLine("Loaded item: id= " + item.id + ", Name=" + item.name + ", Burnable=" + item.burnable + ", BurnTime=" + item.burnTime + ", Smeltable=" + item.smeltable + ", SmeltResult=" + item.smeltingResult + ", Mineable=" + item.mineable + ", MiningHardness=" + item.miningHardness);
                }

                ores = tempOres.ToArray();
            }

            LoadRecipes();

            if (recipes.Length <= 0)
            {
                Console.WriteLine("Recipes Empty");
            }
            else
            {
                foreach (var recipe in recipes)
                {
                    string debugString = "";

                    foreach (var ingredient in recipe.ingredients)
                    {
                        debugString += ingredient + " ";
                    }

                    debugString += "amountsOfIngredients= ";

                    foreach (var amount in recipe.amountsOfIngredients)
                    {
                        debugString += amount + " ";
                    }

                    Console.WriteLine("Loaded recipe: result=" + recipe.result + " ingredients= " + debugString + "buildingType=" + recipe.buildingType + " handCraftable=" + recipe.handCraftable);
                }
            }

            //groundTexture = TerrainGenerator.GenerateTerrain(1000, 1000);

            foreach (var ore in ores)
            {
                Console.WriteLine("Generating ore map for: " + ore.name);
                oreMap = TerrainGenerator.GenerateOre(100, 100, 0.4f, ore.id, 0.001f);
            }

            Console.WriteLine();
        }
        
        private void LoadItems()
        {
            Console.WriteLine("\nLoading items...");
            List<Item> tempItems = new List<Item>();

            int i = 0;

            while (true)
            {
                try
                {
                    Item tempItem = new Item(i + 1);
                    tempItems.Add(tempItem);
                    if (tempItem.name == null)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                    break;
                }

                i++;
            }

            tempItems.RemoveAt(tempItems.Count - 1);
            items = tempItems.ToArray();
        }

        public Item GetItemById(int id)
        {
            return items[id - 1];
        }

        public Item GetItemByName(string name)
        {
            foreach (var item in items)
            {
                if (item.name == name)
                {
                    return item;
                }
            }

            return null;
        }

        private void LoadRecipes()
        {
            Console.WriteLine("\nLoading recipes...");
            List<Recipe> tempRecipes = new List<Recipe>();

            int i = 0;

            while (true)
            {
                try
                {
                    Recipe tempRecipe = new Recipe(i + 1);
                    tempRecipes.Add(tempRecipe);

                    if (tempRecipe.result == 0)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }

                i++;
            }

            tempRecipes.RemoveAt(tempRecipes.Count - 1);
            recipes = tempRecipes.ToArray();
        }

        public Recipe GetRecipeById(int id)
        {
            return recipes[id - 1];
        }

        public Recipe GetRecipeByResult(int resultId)
        {
            foreach (var recipe in recipes)
            {
                if (recipe.result == resultId)
                {
                    return recipe;
                }
            }

            return null;
        }
    }
}
