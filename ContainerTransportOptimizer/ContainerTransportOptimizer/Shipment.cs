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
        /// <summary>
        /// Initializes shipment - sets number of levels, clears space on the ship.
        /// </summary>
        /// <param name="shipId">For which ship the shipment will be considered.</param>
        /// <param name="containerHeight">Constant value of height of containers in this shipment.</param>
        public Shipment(int shipId, int containerHeight)
        {
            containerLocationsList = new List<ContainerLocation>();
            var shipService = new ShipService();
            ship = shipService.GetShipById(shipId);
            noLevels = ship.height / containerHeight;
            ClearSpace();
        }
        /// <summary>
        /// Sets space on all ship levels to "free".
        /// </summary>
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
        /// <summary>
        /// Searches for empty space on ship to put a container there.
        /// </summary>
        /// <param name="container">Container to be places on ship.</param>
        /// <param name="xPosition">X position where the container should be placed.</param>
        /// <param name="yPosition">Y position where the container should be placed.</param>
        /// <param name="orientation">Determines if the container should be placed rotated or not.</param>
        /// <param name="level">Ship level on which the container should be placed.</param>
        /// <returns>Return true if the container has been placed - false otherwise.</returns>
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
        /// <summary>
        /// Chooses the biggest containers and tries to put them on the ship first.
        /// </summary>
        /// <param name="containersList">List of all containers to be placed on ship.</param>
        /// <param name="level">Level to be filled.</param>
        /// <returns>List of containers that haven't been placed.</returns>
        private List<Container> FillShipLevel(List<Container> containersList, int level)
        {
            containersList = containersList.OrderByDescending(o => o.timestamp).ThenBy(o => (o.width * o.length)).ToList();
            for(int i=containersList.Count-1; i>=0;i--)
            {
                if(TryToPutContainerOnShip(containersList[i], level)) containersList.RemoveAt(i);
            }
            return containersList;
        }
        /// <summary>
        /// Tries to put containers on ship in a random manner.
        /// </summary>
        /// <param name="containersList">List of all containers to be placed on ship.</param>
        /// <param name="level">Level to be filled.</param>
        /// <returns>List of containers that haven't been placed.</returns>
        private List<Container> FillShipLevel2(List<Container> containersList, int level)
        {
            containersList = containersList.OrderByDescending(o => o.timestamp).ThenByDescending(o => o.id).ToList();
            for (int i = containersList.Count - 1; i >= 0; i--)
            {
                if (TryToPutContainerOnShip(containersList[i], level)) containersList.RemoveAt(i);
            }
            return containersList;
        }
        /// <summary>
        /// Tries to put container on ship level differing the location and orientation.
        /// </summary>
        /// <param name="container">Container to be placed.</param>
        /// <param name="level">Level of the ship.</param>
        /// <returns>True if the container has been placed somewhere on the ship - false otherwise.</returns>
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
        /// <summary>
        /// Fills all the ship levels with containers.
        /// </summary>
        /// <param name="containersList">List of containers to be placed on ship.</param>
        /// <param name="algorithm">Which algorithm will be used for optimization.</param>
        /// <returns>List of containers that haven't been placed in this shipment.</returns>
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
        /// <summary>
        /// Gets the map of free space for the level of ship.
        /// </summary>
        /// <param name="level">Ship level.</param>
        /// <returns>Array representing free space on ship level.</returns>
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
        /// <summary>
        /// Gets the exact locations of all containers consisted in this shipment.
        /// </summary>
        /// <returns>List of containers locations in this shipment.</returns>
        public List<ContainerLocation> GetContainerLocations()
        {
            return containerLocationsList;
        }
        /// <summary>
        /// Gets number of levels for constant container height for the ship used in this shipment.
        /// </summary>
        /// <returns>Number of levels.</returns>
        public int GetNoLevels()
        {
            return noLevels;
        }
        /// <summary>
        /// Gets the ship used in this particular shipment.
        /// </summary>
        /// <returns>Ship.</returns>
        public Ship GetShip()
        {
            return ship;
        }
    }
}
