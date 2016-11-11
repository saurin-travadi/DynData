using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynData.LKQ
{
    public class LKQImageClient
    {
        public string GetImage()
        {
            var image = new DeepZoom.Image();
            return image.Generate("123", 1);
        }
    }
}
