using System;
using System.Collections.Generic;
using System.Linq;


namespace Advent
{
    public class ImageParser
    {
        List<string> ImageLayers = new List<string>();
        int width = 0;
        int height = 0;

        public ImageParser(List<string> inputs, int width, int height)
        {
            var imageStream = inputs[0];
            var startingIndex = 0;
            this.width = width;
            this.height = height;

            while(startingIndex< imageStream.Count())
            {
                ImageLayers.Add(imageStream.Substring(startingIndex, width*height));
                startingIndex += width*height;
            }
        }

        public void Part1_PrintFewestZerosLayerCalc()
        {
            var leastZeros = Int32.MaxValue;
            var mult = 0;

            foreach(var layer in ImageLayers)
            {
                var zeroCount =layer.Count(x => x=='0');

                if(zeroCount< leastZeros)
                {
                    leastZeros = zeroCount;
                    mult = layer.Count(x=>x == '1') * layer.Count(x => x== '2');
                }
            }

            Console.WriteLine(String.Format("Part 1 : Layer 1 count multiplied by 2 count is " + mult.ToString()));
        }

        public void Part2_PrintImage()
        {
            string image = ImageLayers[0];
            image = image.Replace('2', '*');

            for(int i = 1; i < ImageLayers.Count; i++)
            {
                for(int j = 0; j < ImageLayers[i].Count(); j++)
                {
                    if(image[j] != '*' || ImageLayers[i][j] == '2')
                    {
                        continue;
                    }
                    else
                    {
                        image = image.Remove(j,1);
                        image = image.Insert(j, ImageLayers[i][j].ToString());
                    }
                }
            }

            Console.WriteLine("Part 2");
            int startingIndex = 0;
            while(startingIndex < image.Length)
            {
                var line = image.Substring(startingIndex,width);
                line = line.Replace('0',' ');
                Console.WriteLine(line);

                startingIndex += width;
            }
        }
    }
}