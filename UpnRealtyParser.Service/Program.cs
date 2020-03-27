using System;
using UpnRealtyParser.Business.Contexts;

namespace UpnRealtyParser.Service
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (RealtyParserContext dbContext = new RealtyParserContext()) { 
                Console.WriteLine("Hello World!");
            }
        }
    }
}
