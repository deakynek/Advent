using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace Advent
{
    class ShipHeading
    {
        List<string> dirs = new List<string>();
        double facing = 0;
        Tuple<double, double> pos = new Tuple<double, double>(0,0);
        Tuple<double,double> waypoint = new Tuple<double, double>(10,1);

        public ShipHeading(List<string> lines)
        {
            dirs = lines;
            FollowTheDirs();
            FollowTheWaypoint();
        }

        private void FollowTheDirs()
        {
            foreach(var dir in dirs)
            {
                var instrucion = dir.Substring(0,1);
                var num = Int32.Parse(dir.Substring(1));

                switch(instrucion)
                {
                    case "N":
                        pos = new Tuple<double, double>(pos.Item1, pos.Item2+num);
                        break;
                    case "S":
                        pos = new Tuple<double, double>(pos.Item1, pos.Item2-num);
                        break;
                    case "E":
                        pos = new Tuple<double, double>(pos.Item1+num, pos.Item2);
                        break;
                    case "W":
                        pos = new Tuple<double, double>(pos.Item1-num, pos.Item2);
                        break;
                    case "R":
                        facing -= num;
                        break;
                    case "L":
                        facing += num;
                        break;
                    case "F":
                        pos = new Tuple<double, double>(pos.Item1+Math.Cos(facing/180.0*Math.PI)*num, pos.Item2+Math.Sin(facing/180.0*Math.PI)*num);
                        break;
                }
            }

            Console.WriteLine("Manhattan Distance  x:" + Math.Round(Math.Abs(pos.Item1)) + " y:" + Math.Round(Math.Abs(pos.Item2)) + " dist: " + Math.Round(Math.Abs(pos.Item1)+Math.Abs(pos.Item2)));

        }

        private void FollowTheWaypoint()
        {
            pos = new Tuple<double, double>(0,0);
            waypoint = new Tuple<double, double>(10,1);

            foreach(var dir in dirs)
            {
                var instrucion = dir.Substring(0,1);
                var num = Int32.Parse(dir.Substring(1));


                var distY = waypoint.Item2;
                var distX = waypoint.Item1;
                var totalDist = Math.Sqrt(Math.Pow(distY,2) + Math.Pow(distX,2));
                var angle = Math.Atan2(distY,distX);

                switch(instrucion)
                {
                    case "N":
                        waypoint = new Tuple<double, double>(waypoint.Item1, waypoint.Item2+num);
                        break;
                    case "S":
                        waypoint = new Tuple<double, double>(waypoint.Item1, waypoint.Item2-num);
                        break;
                    case "E":
                        waypoint = new Tuple<double, double>(waypoint.Item1+num, waypoint.Item2);
                        break;
                    case "W":
                        waypoint = new Tuple<double, double>(waypoint.Item1-num, waypoint.Item2);
                        break;
                    case "R":
                        angle -= num/180.0*Math.PI;
                        waypoint = new Tuple<double,double>(Math.Cos(angle)*totalDist,Math.Sin(angle)*totalDist);
                        break;
                    case "L":
                        angle += num/180.0*Math.PI;
                        waypoint = new Tuple<double,double>(Math.Cos(angle)*totalDist, Math.Sin(angle)*totalDist);
                        break;
                    case "F":
                        pos = new Tuple<double, double>(pos.Item1+waypoint.Item1*num, pos.Item2+waypoint.Item2*num);
                        break;
                }
            }

            Console.WriteLine("Manhattan Distance  x:" + Math.Round(Math.Abs(pos.Item1)) + " y:" + Math.Round(Math.Abs(pos.Item2)) + " dist: " + Math.Round(Math.Abs(pos.Item1)+Math.Abs(pos.Item2)));

        }

    }

}