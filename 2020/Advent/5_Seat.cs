using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace Advent
{
    class SeatFinder
    {
        List<string> tickets = new List<string>();
        List<int> ids = new List<int>();
        public SeatFinder(List<string> lines)
        {
            tickets = lines;
        }

        private int FindRow(string rowDir)
        {
            int remainingRows=128;
            int baseRow = 0;

            foreach(char dir in rowDir)
            {
                remainingRows = remainingRows/2;
                if(dir =='B')
                {
                    baseRow += remainingRows;
                }
            }

            return baseRow;
        }

        private int FindSeat(string rowDir)
        {
            int remainingSeats=8;
            int baseSeat= 0;

            foreach(char dir in rowDir)
            {
                remainingSeats = remainingSeats/2;
                if(dir =='R')
                {
                    baseSeat += remainingSeats;
                }
            }

            return baseSeat;
        }        


        public void GetTicketIds()
        {
            int maxId = 0;
            foreach(var ticket in tickets)
            {
                var row = FindRow(ticket.Substring(0,7));
                var seat = FindSeat(ticket.Substring(7,3));

                var id = row*8+seat;
                ids.Add(id);
                if(id>maxId)
                    maxId = id;
            }

            Console.WriteLine("Max ID = "+maxId.ToString());

            int mySeat;
            int dummy;
            for(int i = 1; i<127; i++)
            {
                string thisRow = "";
                if(
                    !idsPresentForThisRow(i,out mySeat) &&
                    idsPresentForThisRow(i-1,out dummy) &&
                    idsPresentForThisRow(i+1,out dummy))
                {
                    Console.WriteLine("My ID = " + (i*8+mySeat).ToString());
                    break;
                }
            }
        }

        private bool idsPresentForThisRow(int rowId, out int seat)
        {
            seat = 0;
            for(int i=0; i<8; i++)
            {
                if(!ids.Contains(rowId*8+i))
                {
                    seat = i;
                    return false;
                }
                    

            }
            return true;
        }


    }
}