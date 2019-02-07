using System.Collections.Generic;
using System.IO;
using System.Text;

public class Recipe
{
    public int id;
    public int result = 0;
    public int amountOfResult;
    public int[] ingredients;
    public int[] amountsOfIngredients;
    public string buildingType;
    public bool handCraftable;

    public Recipe(int ID)
    {
        id = ID;
        AssignVariables(id);
    }

    private void AssignVariables(int ID)
    {
        bool assigned = false;

        foreach (var l in File.ReadAllLines(@"Recipe.info"))
        {
            string tempL = l;
            if (l.Contains("id"))
            {
                if (assigned == false)
                {
                    int tempId = 0;
                    tempL = tempL.Remove(0, 3);

                    foreach (var c in Encoding.ASCII.GetBytes(tempL))
                    {
                        if (c >= 48 && c <= 57)
                        {
                            if (tempId == 0)
                                tempId = c - 48;
                            else
                            {
                                tempId *= 10;
                                tempId += c - 48;
                            }
                        }
                        else
                            break;
                    }

                    if (tempId == ID)
                    {
                        assigned = true;
                        continue;
                    }
                }
                else
                    break;
            }

            if (assigned)
            {
                if (tempL.Contains("result") && result == 0)
                {
                    tempL = tempL.Remove(0, tempL.LastIndexOf('=') + 2);
                    foreach (var c in Encoding.ASCII.GetBytes(tempL))
                    {
                        if (c >= 48 && c <= 57)
                        {
                            result *= 10;
                            result += c - 48;
                        }
                        else
                            break;
                    }
                }
                else if (tempL.Contains("amountOfRes"))
                {
                    tempL = tempL.Remove(0, tempL.LastIndexOf('=') + 2);
                    foreach (var c in Encoding.ASCII.GetBytes(tempL))
                    {
                        if (c >= 48 && c <= 57)
                        {
                            amountOfResult *= 10;
                            amountOfResult += c - 48;
                        }
                        else
                            break;
                    }
                }
                else if (tempL.Contains("ingredients"))
                {
                    tempL = tempL.Remove(0, 17);

                    List<int> tempIngred = new List<int>();

                    int i = 0;

                    foreach (var c in Encoding.ASCII.GetBytes(tempL))
                    {
                        if (c >= 48 && c <= 57)
                        {
                            if ((tempIngred.Count - 1) <= i)
                                tempIngred.Add(0);

                            tempIngred[i] *= 10;
                            tempIngred[i] += c - 48;
                            i++;
                        }
                        else
                        {
                            i++;
                        }
                    }

                    ingredients = tempIngred.ToArray();
                }
                else if (tempL.Contains("amountsOfIngredients"))
                {
                    tempL = tempL.Remove(0, 26);

                    List<int> tempAmount = new List<int>();

                    int i = 0;

                    foreach (var c in Encoding.ASCII.GetBytes(tempL))
                    {
                        if (c >= 48 && c <= 57)
                        {
                            if ((tempAmount.Count - 1) <= i)
                                tempAmount.Add(0);

                            tempAmount[i] *= 10;
                            tempAmount[i] += c - 48;
                            i++;
                        }
                        else
                        {
                            i++;
                        }
                    }

                    amountsOfIngredients = tempAmount.ToArray();
                }
                else if (tempL.Contains("buildingType"))
                {
                    tempL = tempL.Remove(0, 18);
                    tempL = tempL.Remove(tempL.Length - 2);
                    buildingType = tempL;
                }
                else if (tempL.Contains("handCraftable"))
                {
                    if (tempL.Contains("false"))
                        handCraftable = false;
                    else if (tempL.Contains("true"))
                        handCraftable = true;
                }
            }
        }
    }
}
