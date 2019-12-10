using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Students.Luca.Scripts.Helper
{
    [CreateAssetMenu(menuName = "Generators/Random Name Generator")]
    public class RandomNameGenerator : ScriptableObject
    {
        public List<string> firstName;
        public List<string> secondName;

        public string GetRandomName()
        {
            var name = "";
            if (firstName?.Count > 0)
            {
                name += firstName[Random.Range(0, firstName.Count)]+" ";
            }
            if (secondName?.Count > 0)
            {
                name += secondName[Random.Range(0, secondName.Count)];
            }

            return name;
        }
    }
}