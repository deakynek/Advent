using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    class SensorTracking
    {
        List<List<List<int>>> scannerReadings = new List<List<List<int>>>();

        public SensorTracking(List<string> lines)
        {
            List<List<int>> thisScanner = new List<List<int>>();
            foreach(var line in lines)
            {
                if(line.StartsWith("---") || String.IsNullOrEmpty(line))
                {
                    if(thisScanner.Any())
                        scannerReadings.Add(thisScanner.ToList());
                    thisScanner.Clear();
                    continue;
                }

                thisScanner.Add(line.Trim().Split(',').Select(v => int.Parse(v)).ToList());
            }
            if(thisScanner.Any())
                scannerReadings.Add(thisScanner.ToList());


            List<List<int>> probeLocations = scannerReadings[0].ToList();


            var rotations = new List<List<int>>();
            rotations.Add(new List<int>(){1,2,3});
            rotations.Add(new List<int>(){1,3,-2});
            rotations.Add(new List<int>(){1,-2,-3});
            rotations.Add(new List<int>(){1,-3,2});
            
            rotations.Add(new List<int>(){-1,2,-3});
            rotations.Add(new List<int>(){-1,3,2});
            rotations.Add(new List<int>(){-1,-2,3});
            rotations.Add(new List<int>(){-1,-3,-2});
            
            rotations.Add(new List<int>(){2,-1,3});
            rotations.Add(new List<int>(){2,3,1});
            rotations.Add(new List<int>(){2,1,-3});
            rotations.Add(new List<int>(){2,-3,-1});
            
            rotations.Add(new List<int>(){-2,1,3});
            rotations.Add(new List<int>(){-2,-3,1});
            rotations.Add(new List<int>(){-2,-1,-3});
            rotations.Add(new List<int>(){-2,3,-1});
            
            rotations.Add(new List<int>(){3,2,-1});
            rotations.Add(new List<int>(){3,1,2});
            rotations.Add(new List<int>(){3,-2,1});
            rotations.Add(new List<int>(){3,-1,-2});

            rotations.Add(new List<int>(){-3,2,1});
            rotations.Add(new List<int>(){-3,1,-2});
            rotations.Add(new List<int>(){-3,-2,-1});
            rotations.Add(new List<int>(){-3,-1,2});


            var origins = new Dictionary<int, List<int>>();
            origins.Add(0, new List<int>(){0,0,0});

            var scannerIds = new List<int>(){0,1,25,38,19,20,3,15,16,22,23,26,39,8,9,14,21,29,31,36,5,7,12,18,28,30,33,4,6,10,11,17,24,27,35,37,2,13,32,34};

            while(scannerIds.Any(c => !origins.ContainsKey(c)))
            {
                var copy = origins.Keys.ToList();
                foreach(var scanner in scannerIds.Where(x=> !copy.Contains(x)))
                {
                    
                    Console.WriteLine(String.Format("Trying scanner {0}", scanner));

                    var matchFound = false;
                    foreach(var rot in rotations)
                    {
                        var newLocation = scannerReadings[scanner].Select(a => new List<int>(){
                            
                            a[Math.Abs(rot[0])-1]*rot[0]/Math.Abs(rot[0]),
                            a[Math.Abs(rot[1])-1]*rot[1]/Math.Abs(rot[1]),
                            a[Math.Abs(rot[2])-1]*rot[2]/Math.Abs(rot[2]),}).ToList();

                        foreach(var newPos in newLocation)
                        {
                            foreach(var existPos in probeLocations)
                            {
                                //Move new Pos to existing Pos
                                var offsetLocations = newLocation.Select( a=> new List<int>()
                                    {
                                        existPos[0]-newPos[0]+a[0],
                                        existPos[1]-newPos[1]+a[1],
                                        existPos[2]-newPos[2]+a[2],
                                    }).ToList();

                                var origin = new List<int>(){
                                    existPos[0]-newPos[0],
                                    existPos[1]-newPos[1],
                                    existPos[2]-newPos[2],
                                };


                                var nonMatches = offsetLocations.Where(a => !probeLocations.Any(b => a[0]==b[0] && a[1]==b[1] && a[2]==b[2])).ToList();
                                if(nonMatches.Count()<= offsetLocations.Count()-12  )
                                {
                                    origins.Add(scanner, origin);
                                    probeLocations.AddRange(nonMatches);
                                    Console.WriteLine(String.Format("Adding scanner {0}", scanner));
                                    matchFound = true;
                                }

                                if(matchFound)
                                    break;
                            }
                            if(matchFound)
                                break;
                        }
                        if(matchFound)
                            break;


                    }
                }
            }
            Console.WriteLine(probeLocations.Count());

            var maxDist = 0;
            for(int i= 0; i< origins.Values.Count; i++)
            {
                for(int j=i+1; j<origins.Values.Count; j++)
                {
                    var first = origins.Values.ElementAt(i);
                    var second = origins.Values.ElementAt(j);

                    var dist = Math.Abs(first[0]-second[0]) +
                                Math.Abs(first[1]-second[1]) +
                                Math.Abs(first[2]-second[2]);

                    if(dist > maxDist)
                        maxDist = dist;
                }
            }

            Console.WriteLine(maxDist);
        }
    }    
}