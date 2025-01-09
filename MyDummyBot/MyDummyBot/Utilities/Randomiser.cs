using System;
using System.Collections.Generic;

namespace MyDummyBot.Utilities
{
    public class Randomiser
    {
        public static int RandomiseQuestion(List<int> questionNumber)
        {
            // TODO: Taking the total number of questions
            int lowerBound = 1, higherBound = 7;

            Random random = new Random();
            int randomNumber = random.Next(lowerBound, higherBound);

            while (questionNumber.Contains(randomNumber))
            {
                randomNumber = random.Next(lowerBound, higherBound);
            }

            return randomNumber;
        }
    }
}
