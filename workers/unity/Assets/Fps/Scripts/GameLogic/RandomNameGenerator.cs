using UnityEngine;

namespace Fps
{
    public static class RandomNameGenerator
    {
        public static string GetName()
        {
            var prefix = Prefixes[Random.Range(0, Prefixes.Length)];
            var postfix = Postfixes[Random.Range(0, Postfixes.Length)];

            var timeOut = 200;
            while (postfix.Length + prefix.Length > 15 && timeOut > 0)
            {
                postfix = Postfixes[Random.Range(0, Postfixes.Length)];
                timeOut--;
            }

            var name = prefix + postfix;

            if (timeOut == 0)
            {
                return name.Substring(0, 15);
            }

            if (name.Length < 13)
            {
                name += Random.Range(10, 99).ToString();
            }

            return name;
        }


        private static readonly string[] Prefixes =
        {
            "Angry",
            "Splendid",
            "Mega",
            "Ugly",
            "Noob",
            "Dead",
            "Dr",
            "Cold",
            "Fast",
            "Good",
            "Spark",
            "Low",
            "High",
            "Twisted"
        };

        private static readonly string[] Postfixes =
        {
            "Fox",
            "Bear",
            "Noob",
            "Ghost",
            "Kid",
            "Fragger",
            "Man",
            "Woman",
            "Zombie",
            "Lion",
            "Jack",
            "Karen",
            "Undead"
        };
    }
}
