using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerTransportOptimizer
{
    public class ContainerLocation
    {
        public int containerId { get; set; }
        public int xPosition { get; set; }
        public int yPosition { get; set; }
        public bool orientation { get; set; }
        public int level { get; set; }
    }
}
