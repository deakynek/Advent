using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace Advent
{
    public class DeckSorter
    {
        List<Tuple<CommandType, int>> commands = new List<Tuple<CommandType, int>>();

        Dictionary<string, List<long>> cycles = new Dictionary<string, List<long>>();
        enum CommandType
        {
            stack,
            cut,
            increment,
        }

        long deckSize = 0;
        long iterations = 101741582076661;

        public DeckSorter(List<string> inputs)
        {
            foreach(var input in inputs)
            {
                if(input.Contains("new stack"))
                    commands.Add(new Tuple<CommandType, int>(CommandType.stack, 0));
                else if(input.Contains("increment"))
                {
                    var split = input.Split(' ');
                    commands.Add(new Tuple<CommandType, int>(CommandType.increment, Int32.Parse(split.Last().ToString())));
                }
                else if(input.Contains("cut"))
                {
                    var split = input.Split(' ');
                    commands.Add(new Tuple<CommandType, int>(CommandType.cut, Int32.Parse(split.Last().ToString())));
                }
            }

            SortWithCommands();
            largeDeck();

            // deckSize = 10;
            // var starting = ReverseIndex(9);
        }

        private void SortWithCommands()
        {
            deckSize = 87;
            var newDeck = new List<long>(){0};
            while(newDeck.Count() < deckSize)
            {
                newDeck.Add(newDeck.Last()+1);
            }
            var freshDeck = newDeck.ToList();
            var realDeck = newDeck.ToList();
            

            
            realDeck = ApplyAllCommands(realDeck);

            

            var finalOffset = realDeck.IndexOf(0)-freshDeck.IndexOf(0);
            freshDeck = Cut(freshDeck, finalOffset);
            finalOffset = freshDeck.IndexOf(realDeck.First());
            Console.WriteLine("Part 1 - Position of card 2019: " + realDeck.IndexOf(2019));
        }

        private List<long> ApplyAllCommands(List<long> beginingState)
        {
            foreach(var command in commands)
            {
                switch(command.Item1)
                {
                    case CommandType.cut:
                        beginingState = Cut(beginingState,command.Item2);
                        break;
                    case CommandType.increment:
                        beginingState = DealWithIncrement(beginingState, command.Item2);
                        break;
                    case CommandType.stack:
                        beginingState = Deal(beginingState);
                        break;
                }
            }
            return beginingState;
        }

        private List<long> Deal(List<long> deck)
        {
            var copy = deck.ToList();
            copy.Reverse();
            return copy;
        }

        private List<long> Cut(List<long> deck, int ammount)
        {
            if(ammount == 0)
                return deck;
            else if(ammount > 0)
            {
                var ret = deck.Skip(ammount).ToList();
                ret.AddRange(deck.Take(ammount).ToList());
                return  ret;
            }
            
            deck.Reverse();
            var part2 = deck.Skip(Math.Abs(ammount)).ToList();
            part2.Reverse();
            var part1 = deck.Take(Math.Abs(ammount)).ToList();
            part1.Reverse();
            part1.AddRange(part2);        

            return part1;  

        }

        private List<long> DealWithIncrement(List<long> deck, int inc)
        {
            return deck.OrderBy(val => (deck.IndexOf(val)*inc%deckSize)).ToList();
        }

        private long ProgressIndex(long index)
        {
            foreach(var command in commands)
            {
                switch(command.Item1)
                {
                    case CommandType.cut:
                        index = (index+deckSize-command.Item2)%deckSize;
                        break;
                    case CommandType.increment:
                        index = (index*command.Item2)%deckSize;
                        break;
                    case CommandType.stack:
                        index = (deckSize-1)-index;
                        break;
                }
            }

            return index;
        }

        private long ReverseIndex(long index)
        {
            var copy = commands.ToList();

            copy.Reverse();
            foreach(var command in copy)
            {
                switch(command.Item1)
                {
                    case CommandType.cut:
                        index = (index+deckSize+command.Item2)%deckSize;
                        break;
                    case CommandType.increment:
                        while(index % command.Item2 !=0)
                        {
                            index+=deckSize;
                        }
                        index = index/command.Item2;

                        break;
                    case CommandType.stack:
                        index = (deckSize-1)-index;
                        break;
                }
            }

            return index;
        }

        private void largeDeck()
        {
            //double deckSizeDub = 119315717514047;
            long deckSizeDub = 119315717514047;
 

            long offset = 0;
            long deal = 1;
            foreach(var command in commands)
            {
                switch(command.Item1)
                {
                    case CommandType.cut:
                        offset = (offset+command.Item2)%deckSizeDub;
                        break;
                    case CommandType.increment:
                        offset = (offset*command.Item2)%deckSizeDub;
                        deal = (deal*command.Item2)%deckSizeDub;
                        break;
                    case CommandType.stack:
                        offset = (offset*(deckSizeDub-1)+1)%deckSizeDub;
                        deal = (deal*(deckSizeDub-1))%deckSizeDub;
                        break;
                     
                }
                while(offset<0)
                    offset+= deckSizeDub;

                while(deal<0)
                    deal += deckSizeDub;

                Console.WriteLine(String.Format("Deal: {0}  Offset:{1}",deal, offset));  
            }

            var goal = 2020;
            int iters = (int)(Math.Sqrt(deckSizeDub));
            List<long> firstSq = new List<long>();
            firstSq.AddRange(Enumerable.Range(1, iters).Select(x=>(long)x));

            //List<List<long>> loops = new List<List<long>>();
            //loops.Add(firstSq);

            long iterationCount = 0;
            long num = 0;
            long? iterNumber = null;
            var goalIters  =0;
            long goalHeader = 0;
            var goalFound = false;
            Dictionary<long,long> hops = new Dictionary<long, long>();

            for(int i = 0; i<iters+1; i++)
            {
                var startNum = num;
                for(int j = 0; j<iters+1; j++)
                {
                    num = (num*deal - offset)%deckSizeDub;
                    while (num< 0)
                        num+= deckSizeDub;
                    
                    if(!goalFound && num== goal)
                    {
                        goalFound = true;
                        goalIters = i;
                        goalHeader = startNum;
                    }
                }
                hops[startNum] = num;

                if(i%1000 == 0)
                    Console.WriteLine(String.Format("PercentComplete - {0} goal found {1}", ((double)hops.Count*100)/(iters+1), goalFound));

            }

            Console.WriteLine(String.Format("{0} iterations before 2020 = {1}", iterations, iterNumber));

        
        }

    }
}