using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerTransportOptimizer
{
    public class Shipment
    {
        private List<ContainerLocation> containerLocationsList;
        private bool[,,] freeSpace;
        private int noLevels = 0;
        private Ship ship;

        public Shipment(int shipId, int containerHeight)
        {
            containerLocationsList = new List<ContainerLocation>();
            var shipService = new ShipService();
            ship = shipService.GetShipById(shipId);
            noLevels = ship.height / containerHeight;
            ClearSpace();
        }

        private void ClearSpace()
        {
            int x = ship.width;
            int y = ship.length;
            freeSpace = new bool[noLevels, x, y];
            for (int k = 0; k < noLevels; k++)
            {
                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        freeSpace[k, i, j] = true;
                    }
                }
            }
        }
        
        private bool PutContainerOnShip(Container container, int xPosition, int yPosition, bool orientation, int level)
        {
            int containerXSize, containerYSize;

            if(orientation == true)
            {
                containerXSize = container.length;
                containerYSize = container.width;
            }else
            {
                containerXSize = container.width;
                containerYSize = container.length;
            }

            if (xPosition + containerXSize > ship.width || yPosition + containerYSize > ship.length) return false;

            for(int i=xPosition; i<xPosition+containerXSize;i++)
            {
                for(int j=yPosition; j<yPosition+containerYSize;j++)
                {
                    if (freeSpace[level, i, j] == false) return false;
                }
            }

            for (int i = xPosition; i < xPosition + containerXSize; i++)
            {
                for (int j = yPosition; j < yPosition + containerYSize; j++)
                {
                    freeSpace[level, i, j] = false;
                }
            }
            var containerLocation = new ContainerLocation();
            containerLocation.containerId = container.id;
            containerLocation.level = level;
            containerLocation.orientation = orientation;
            containerLocation.xPosition = xPosition;
            containerLocation.yPosition = yPosition;
            containerLocationsList.Add(containerLocation);
            return true;
        }

        private List<Container> FillShipLevel(List<Container> containersList, int level)
        {
            containersList = containersList.OrderByDescending(o => o.timestamp).ThenBy(o => (o.width * o.length)).ToList();
            for(int i=containersList.Count-1; i>=0;i--)
            {
                if(TryToPutContainerOnShip(containersList[i], level)) containersList.RemoveAt(i);
            }
            return containersList;
        }

        private List<Container> FillShipLevel2(List<Container> containersList, int level)
        {
            containersList = containersList.OrderByDescending(o => o.timestamp).ThenByDescending(o => o.id).ToList();
            for (int i = containersList.Count - 1; i >= 0; i--)
            {
                if (TryToPutContainerOnShip(containersList[i], level)) containersList.RemoveAt(i);
            }
            return containersList;
        }

        private bool TryToPutContainerOnShip(Container container, int level)
        {
            for (int j = 0; j < ship.width; j++)
            {
                for (int k = 0; k < ship.length; k++)
                {
                    if (PutContainerOnShip(container, j, k, false, level)) return true;
                    if (PutContainerOnShip(container, j, k, true, level)) return true;
                }
            }
            return false;
        }

        public List<Container> FillTheShip(List<Container> containersList, int algorithm)
        {
            switch (algorithm)
            {
                case 0:
                    for (int i = 0; i < noLevels; i++)
                    {
                        containersList = FillShipLevel(containersList, i);
                    }
                    break;
                case 1:
                    for (int i = 0; i < noLevels; i++)
                    {
                        containersList = FillShipLevel2(containersList, i);
                    }
                    break;
            }
            return containersList;
        }

        public bool[,] GetSpaceArrayForLevel(int level)
        {
            bool[,] tempArray = new bool[ship.width, ship.length];
            for(int i=0;i<ship.width;i++)
            {
                for(int j=0;j<ship.length;j++)
                {
                    tempArray[i, j] = freeSpace[level, i, j];
                }
            }
            return tempArray;
        }

        public List<ContainerLocation> GetContainerLocations()
        {
            return containerLocationsList;
        }

        public int GetNoLevels()
        {
            return noLevels;
        }

        public Ship GetShip()
        {
            return ship;
        }
    }
}
