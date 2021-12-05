using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace Advent
{
    public class Day22
    {

        Dictionary<int,List<int>> Decks = new Dictionary<int, List<int>>();
        Dictionary<int,Dictionary<string,int>> prevRounds = new Dictionary<int,Dictionary<string,int>>();
        public Day22(List<string> inputs)
        {
            var Deck1 = new List<int>();
            var thisDeck = new List<int>();
            foreach(var line in inputs)
            {
                if(line.Contains("Player"))
                    continue;

                if(line=="")
                {
                    Decks.Add(1,thisDeck.ToList());
                    Deck1 = thisDeck.ToList();
                    thisDeck.Clear();
                    continue;
                }

                thisDeck.Add(Int32.Parse(line));

                if(inputs.IndexOf(line)== inputs.Count()-1)
                {
                    Decks.Add(2, thisDeck.ToList());
                }
            }
            var Deck2 = thisDeck.ToList();

            // while(Decks.All(x => x.Value.Count()!=0))
            // {
            //     var newDeckState = PlayRound(Decks[1].ToList(),Decks[2].ToList());
            //     Decks[1] = newDeckState.Item1;
            //     Decks[2] = newDeckState.Item2;
            // }


            if(Decks[1].Count() > 0)
            {
                Console.WriteLine("Player 1 wins.  Score - " + ScoreDeck(Decks[1]));
            }
            else
            {
                Console.WriteLine("Player 2 wins.  Score - " + ScoreDeck(Decks[2]));
            }

            PlayRecurseGame(0,Deck1,Deck2);
        }

        private Tuple<List<int>,List<int>> PlayRound(List<int> Deck1, List<int>Deck2)
        {
            if(!Deck1.Any() || !Deck2.Any())
                return new Tuple<List<int>, List<int>>(Deck1,Deck2);


            var Card1 = Deck1[0];
            var Card2 = Deck2[0];
            Deck1.RemoveAt(0);
            Deck2.RemoveAt(0);

            if(Card1 > Card2)
            {
                Deck1.Add(Card1);
                Deck1.Add(Card2);
            }
            else if(Card2 > Card1)
            {
                Deck2.Add(Card2);
                Deck2.Add(Card1);
            }

            return new Tuple<List<int>, List<int>>(Deck1,Deck2);
        }

        private int ScoreDeck(List<int> Deck)
        {
            var score = 0;
            for(int i = 0; i<Deck.Count(); i++)
            {
                score+=Deck[i]*(Deck.Count()-i);
            }

            return score;
        }

        private int PlayRecurseGame(int level, List<int> Deck1, List<int> Deck2)
        {
            if(level<=3)
                Console.WriteLine("-->"+level);
            List<string> prev = new List<string>();
            //Console.WriteLine("Recursing into Level " +level + " to determine winner");
            bool repeat = false;
            while(Deck1.Count() > 0 && Deck2.Count() >0)
            {
                var card1 = Deck1[0];
                var card2 = Deck2[0];

                var thisDeckString = String.Join(",",Deck1)+ " " +String.Join(",",Deck2);
                if(prev.Contains(thisDeckString))
                {
                    repeat = true;
                    break;
                }
                var roundWinner = RecurseRound(level, Deck1.ToList(),Deck2.ToList());
                prev.Add(thisDeckString);

                Deck1.RemoveAt(0);
                Deck2.RemoveAt(0);   
                if(roundWinner == 1)
                {
                    Deck1.Add(card1);
                    Deck1.Add(card2);
                }
                else
                {
                    Deck2.Add(card2);
                    Deck2.Add(card1);
                }
            }


            var winner = 0;
            if(repeat || Deck1.Count() > 0 && Deck2.Count() == 0) 
                winner = 1;
            else if(Deck2.Count() > 0 && Deck1.Count() == 0) 
                winner = 2;



            if(level<=3)
            {
                Console.WriteLine("<=="+level);

            }
            if(level == 0)
                Console.WriteLine("Player " +winner +" wins - score: "+ ScoreDeck(winner==1?Deck1:Deck2));
            return winner;

        }

        private bool IsRepeatRound(List<int> Deck1, List<int> Deck2, Dictionary<int,Tuple<List<int>,List<int>>> prev)
        {
            var repeat = false;
            foreach(var round in prev)
            {
                if(round.Value.Item1.Count() != Deck1.Count() || round.Value.Item2.Count() != Deck2.Count())
                    continue;
                
                var decksMatch = true;
                for(int i = 0; i<round.Value.Item1.Count(); i++)
                {
                    if(round.Value.Item1[i] != Deck1[i])
                    {
                        decksMatch = false;
                        break;
                    }
                }

                if(!decksMatch)
                    continue;

                decksMatch = true;
                for(int i = 0; i<round.Value.Item2.Count(); i++)
                {
                    if(round.Value.Item2[i] != Deck2[i])
                    {
                        decksMatch = false;
                        break;
                    }
                }
                
                if(decksMatch)
                {
                    repeat = true;
                    break;
                }
            }
            return repeat;
        }

        private int RecurseRound(int level, List<int> Deck1, List<int> Deck2)
        {
            var Card1 = Deck1[0];
            var Card2 = Deck2[0];
            Deck1.RemoveAt(0);
            Deck2.RemoveAt(0);
            //Console.WriteLine(String.Format("Card 1 {0} Deck Size 1 {2} \tCard 2 {1} Deck Size 2 {3}",Card1,Card2,Deck1.Count(), Deck2.Count()));


            if(Card1 <= Deck1.Count() && Card2 <= Deck2.Count())
            {
                var lowerDeckString = String.Join(',',Deck1)+ " "+ String.Join(',',Deck2);
                //if(prevRounds.ContainsKey(level+1)&& prevRounds[level+1].ContainsKey(lowerDeckString))
                     ///return prevRounds[level+1][lowerDeckString];


                var lowerWinner = PlayRecurseGame(level+1,Deck1.Take(Card1).ToList(),Deck2.Take(Card2).ToList());
                    if(!prevRounds.ContainsKey(level+1))
                       prevRounds.Add(level+1,new Dictionary<string, int>());

                // prevRounds[level+1].Add(lowerDeckString, lowerWinner);
                 return lowerWinner;
            }
            else if(Card1 > Card2)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

    }


}