using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    public class Arcade
    {
        IntComputer gameComputer;
        List<Tuple<int,int,int>> screen = new List<Tuple<int, int, int>>();
        long score = 0;

        public Arcade(List<string> inputs)
        {
            gameComputer = new IntComputer(inputs);

        }

        public void StartGame()
        {
            int xPos = 0;
            int yPos = 0;
            int outputNumber = 0;
            gameComputer.SetProgramIndex(0,2);
            gameComputer.debugMessages = false;
            bool startOutput  =false;

            int ballxPos = 0;
            int paddlexPos = 0;

            while(!gameComputer.programComplete)
            {
                gameComputer.RunProgram(gameComputer.nextStepIndex,false, true);

                if(outputNumber == 0)
                    xPos = (int)gameComputer.lastOutput;
                else if(outputNumber == 1)
                    yPos = (int)gameComputer.lastOutput;
                else if(outputNumber ==2)
                {
                    if(xPos == -1)
                    {
                        score = (int)gameComputer.lastOutput;
                        startOutput = true;
                        PrintScreen(false);
        
                    }
                    else
                    {
                        var existing = screen.FirstOrDefault(pixel => pixel.Item1==xPos && pixel.Item2 == yPos);
                        if(existing != null)
                            screen.Remove(existing);

                        screen.Add(new Tuple<int, int, int>(xPos, yPos, (int)gameComputer.lastOutput));
                        
                        if(gameComputer.lastOutput == 3)
                            paddlexPos = xPos;                         
                        if(gameComputer.lastOutput == 4)
                        {
                            ballxPos = xPos;

                            if(ballxPos > paddlexPos)
                                gameComputer.AddInput(1);
                            else if(ballxPos < paddlexPos)
                                gameComputer.AddInput(-1);
                            else
                                gameComputer.AddInput(0);

                            PrintScreen(false);
                        }
                    }
                }
                outputNumber = (outputNumber+1)%3;
            }

            //Part 1
            //PrintScreen(true);
        }

        private void PrintScreen(bool countBlocks)
        {
            //Console.Clear();
            int blocks = 0;

            string allLines ="";
            for(int y = 0; y <= screen.Select(tile => tile.Item2).Max(); y++)
            {
                string thisLine = "";
                for(int x = 0; x <= screen.Select(tile => tile.Item1).Max(); x++)
                {
                    if(!screen.Any(tile => tile.Item1==x && tile.Item2 ==y))
                    {
                        thisLine += " ";
                        continue;
                    }

                    var thisTile = screen.First(tile => tile.Item1==x && tile.Item2 ==y);
                    
                    switch(thisTile.Item3)
                    {
                        case 0:
                            thisLine += " ";
                            break;
                        case 1:
                            thisLine += "|";
                            break;
                        case 2:
                            blocks++;
                            thisLine += "#";
                            break;
                        case 3:
                            thisLine += "=";
                            break;
                        case 4:
                            thisLine += "O";
                            break;
                        default:
                            thisLine += " ";
                            break;
                    }
                }
                allLines+="\n"+ thisLine;
            }
            Console.WriteLine(allLines);
            if(countBlocks)
                Console.WriteLine(String.Format("{0} blocks remain", blocks));



            Console.WriteLine("Score: "+ score.ToString());
        }


    }
}