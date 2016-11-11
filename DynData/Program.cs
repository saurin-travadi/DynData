using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynData
{
    class Program
    {
        static void Main(string[] args)
        {
            //new LKQ.LKQClient().GetData();
            //new IAA.IAAClient().GetData();
            //new LKQ.LKQClient().PushData();

            new LKQ.LKQImageClient().GetImage();
        }

        
    }
}
