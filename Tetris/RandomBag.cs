using System;
using System.Collections.Generic;

namespace Tetris
{
    public class RandomBag
    {
        List<int> bag;
        int bagSize;
        Random random = new Random();

        public RandomBag(int bagSize)
        {
            bag = new List<int>();
            this.bagSize = bagSize;
        }

        public int get()
        {
            if (bag.Count <= 0)
            {
                GenerateContents();
            }

            int output = bag[0];
            bag.RemoveAt(0);
            return output;
        }

        void GenerateContents()
        {
            for (int i = 0; i < bagSize; i++)
            {
                bool found = false;
                while (!found)
                {
                    int rand = random.Next(0, bagSize);
                    if (!bag.Contains(rand))
                    {
                        bag.Add(rand);
                        found = true;
                    }
                }
            }
        }

    }
}
