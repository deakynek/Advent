using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace Advent
{
    public class Day21
    {

        Dictionary<string,List<string>> ingredients = new Dictionary<string, List<string>>();
        Dictionary<string,List<string>> allergensCouldBe = new Dictionary<string, List<string>>(); 
        Dictionary<string,int> ingredientsCount = new Dictionary<string, int>();
        public Day21(List<string> inputs)
        {
            foreach(var line in inputs)
            {
                var food = line.ToString();
                var allergics = new List<string>();
                if(line.Contains("(contains"))
                {
                    var starting = line.IndexOf("(");
                    var ending = line.IndexOf(")");
                    allergics = line.Substring(starting+9,ending-starting-9).Split(',').Select(x => x.Trim()).ToList();

                    food=line.Substring(0,starting);
                }

                var ings = food.Split(' ').Select(x => x.Trim()).Where(x=>x!="").ToList();

                var ingredientsCopy = ingredients.ToDictionary(x=>x.Key, y=>y.Value.ToList());
                foreach(var ing in ings)
                {
                    if(String.IsNullOrEmpty(ing))
                        continue;

                    if(!ingredientsCount.ContainsKey(ing))
                        ingredientsCount.Add(ing,1);
                    else   
                        ingredientsCount[ing] = ingredientsCount[ing]+1;

                    if(!ingredients.ContainsKey(ing))
                        ingredients.Add(ing,new List<string>());
                    
                    foreach(var allerg in allergics)
                    {
                        if(ingredientsCopy.Where(x =>x.Value.Contains(allerg)).Count() == 0)
                            ingredients[ing].Add(allerg);
                    }
                }

                foreach(var allerg in allergics)
                {
                    foreach(var ing in ingredients.Where(x => x.Value.Contains(allerg) && !ings.Contains(x.Key)))
                    {
                        ing.Value.Remove(allerg);
                    }

                    if(!allergensCouldBe.ContainsKey(allerg))
                    {
                        allergensCouldBe[allerg] = ings.ToList();
                    }
                    else
                    {
                        var allergensCopy = allergensCouldBe[allerg].ToList();
                        foreach(var ing in allergensCopy)
                        {
                            if(!ings.Contains(ing))
                                allergensCouldBe[allerg].Remove(ing);
                        }
                    }

                }
            }

            var changeMade = false;
            do
            {
                changeMade = false;
                var ingredientsCopy = ingredients.ToDictionary(x=>x.Key, y=>y.Value.ToList());
                foreach(var ing in ingredientsCopy)
                {
                    if(ing.Value.Count() == 1)
                    {
                        foreach(var otherIng in ingredients.Where(x => x.Key!=ing.Key &&x.Value.Contains(ing.Value[0])))
                        {
                            otherIng.Value.Remove(ing.Value[0]);
                            changeMade =true;
                        }
                    }
                    else
                    {
                        foreach(var allerg in ing.Value)
                        {
                            var otherIng = ingredients.Where(x => x.Key != ing.Key && ing.Value.Contains(allerg)).Count();
                            if(otherIng == 0)
                            {
                                ingredients[ing.Key] = new List<string>(){allerg};
                                changeMade = true;
                                break;
                            }

                        }
                    }
                }
            }
            while (changeMade);

            var allAllergens = new List<string>();
            foreach(var ing in ingredients)
            {
                allAllergens.AddRange(ing.Value);
            }
            allAllergens = allAllergens.Distinct().ToList();


            var noAllergens = ingredients.Keys.Where(x => allergensCouldBe.All(y => !y.Value.Contains(x)));
            Console.WriteLine("Ingredients that could not contain anything = " + noAllergens.Count());

            var incCount = 0;

            foreach(var ing in noAllergens)
            {
                incCount+= ingredientsCount[ing];
            }
            Console.WriteLine("Times they appear " + incCount);


            var dangerousIngs = ingredients.Where(x => x.Value.Count() != 0);
            var dangerousList = String.Join(',',dangerousIngs.OrderBy(x => x.Value[0]).Select(x => x.Key));

            Console.WriteLine("Dangerous Ingredients List");
            Console.WriteLine(dangerousList);
        }
    }
}