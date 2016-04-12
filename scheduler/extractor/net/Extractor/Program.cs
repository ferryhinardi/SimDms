using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extractor
{
    class Program
    {
        static void Main(string[] args)
        {
            var xtor = new MyExtractor();

            for (int i = 0; i < 1000; i++)
            {
                xtor.Run();
                Console.WriteLine(i);
            }
        }
    }
}
