using System;

namespace GameServer
{
    public class TerrainGenerator
    {
        public static int[,] GenerateTerrain(int xSize, int ySize)
        {
            return GenerateNoise(xSize, ySize, 255, 0.1f);
        }

        public static int[,] GenerateOre(int xSize, int ySize, float orePercentage, int oreItemId, float oreRandomness)
        {
            int[,] oreMap = new int[xSize, ySize];
            int[,] perlinMap = GenerateNoise(xSize, ySize, 100, oreRandomness);

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    if (perlinMap[x, y] < orePercentage * 100)
                    {
                        oreMap[x, y] = oreItemId;
                    }
                }
            }

            return oreMap;
        }

        private static int[,] GenerateNoise(int xSize, int ySize, int maxValue, float maxPercentageDifferenceBetweenValues)
        {
            int[,] noiseMap = new int[xSize, ySize];
            int maxRgb = 255;

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    Random rng = new Random();
                    if (x == 0)
                    {
                        if (y == 0)
                        {
                            noiseMap[x, y] = rng.Next(0, maxValue);
                        }
                        else
                        {
                            int multiplier = rng.Next(1, 2) - 1;
                            noiseMap[x, y] = Convert.ToInt32((rng.Next(0, maxValue) + noiseMap[x, y - 1]) / 2 * (1 + (rng.NextDouble() * maxPercentageDifferenceBetweenValues * multiplier)));
                        }
                    }
                    else if (y == 0)
                    {
                        noiseMap[x, y] = rng.Next(0, maxValue);
                    }
                    else
                    {
                        int multiplier = rng.Next(1, 2) - 1;
                        noiseMap[x, y] = Convert.ToInt32((noiseMap[x - 1, y] + noiseMap[x, y - 1] + rng.Next()) / 3 * (1 + ((rng.NextDouble() * maxPercentageDifferenceBetweenValues * multiplier) + (rng.NextDouble() * maxPercentageDifferenceBetweenValues * multiplier)/ 2)));
                    }
                    //Console.WriteLine("gen: x=" + x + ", y=" + y + ", with value: " + noiseMap[x, y]);
                }
            }

            return noiseMap;
        }
    }
}