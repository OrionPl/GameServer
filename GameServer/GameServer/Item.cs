using System.IO;
using System.Text;

public class Item
{
    public int id;
    public string name;
    public bool burnable;
    public float burnTime;
    public bool smeltable;
    public int smeltingResult;
    public bool mineable;
    public float miningHardness;

    public Item(int ID)
    {
        id = ID;
        AssignVariables(ID);
    }


    /////////////////////////////////////////

    private void AssignVariables(int ID)
    {
        bool assigned = false;

        foreach (var l in File.ReadAllLines(@"Item.info"))
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

                    if (tempId == id)
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
                if (tempL.Contains("name"))
                {
                    tempL = tempL.Remove(0, 10);
                    tempL = tempL.Remove(tempL.Length - 2);
                    name = tempL;
                }
                else if (tempL.Contains("burnable"))
                {
                    if (tempL.Contains("false"))
                        burnable = false;
                    else if (tempL.Contains("true"))
                        burnable = true;
                }
                else if (tempL.Contains("burnTime"))
                {
                    if (!tempL.Contains("null"))
                    {
                        tempL = tempL.Remove(0, tempL.LastIndexOf('=') + 2);
                        foreach (var c in Encoding.ASCII.GetBytes(tempL))
                        {
                            if (c >= 48 && c <= 57)
                            {
                                burnTime *= 10;
                                burnTime += c - 48;
                            }
                            else
                                break;
                        }
                    }
                    else
                    {
                        burnTime = 0;
                    }
                }
                else if (tempL.Contains("smeltable"))
                {
                    if (tempL.Contains("false"))
                        smeltable = false;
                    else if (tempL.Contains("true"))
                        smeltable = true;
                }
                else if (tempL.Contains("smeltingResult"))
                {
                    if (!tempL.Contains("null"))
                    {
                        tempL = tempL.Remove(0, tempL.LastIndexOf('=') + 2);
                        foreach (var c in Encoding.ASCII.GetBytes(tempL))
                        {
                            if (c >= 48 && c <= 57)
                            {
                                smeltingResult *= 10;
                                smeltingResult += c - 48;
                            }
                            else
                                break;
                        }
                    }
                    else
                    {
                        smeltingResult = 0;
                    }
                }
                else if (tempL.Contains("mineable"))
                {
                    if (tempL.Contains("false"))
                        mineable = false;
                    else if (tempL.Contains("true"))
                        mineable = true;
                }
                else if (tempL.Contains("miningHardness"))
                {
                    if (!tempL.Contains("null"))
                    {
                        tempL = tempL.Remove(0, tempL.LastIndexOf('=') + 2);
                        foreach (var c in Encoding.ASCII.GetBytes(tempL))
                        {
                            if (c >= 48 && c <= 57)
                            {
                                miningHardness *= 10;
                                miningHardness += c - 48;
                            }
                            else
                                break;
                        }
                    }
                    else
                    {
                        miningHardness = 0;
                    }
                }
            }
        }
    }
}