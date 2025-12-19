using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomiodPublisher.ConfigFiles
{
    public class Configs
    {
        public static string baseURI = @"http://localhost:58066"; //TODO: needs to be updated!

        public Configs()
        {
            RestClient client = new RestClient(baseURI);
        }
    }
}
