using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;


namespace Advent
{
    public class WebCrawler
    {

        public WebCrawler()
        {
            var myClient = new WebClient();
            Stream response = myClient.OpenRead("https://randomwordgenerator.com");

            // The stream data is used here.
            response.Close();
        }
    }
}
