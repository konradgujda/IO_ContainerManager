using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContainerTransportOptimizer
{
    public class ShipService
    {
        private const int maxSize = 100;
        private const int minSize = 50;
        private string fileName = "shipSet.txt";

        public void GenerateShipSet()
        {
            var numberOfShips = 3;
            Random rnd = new Random();
            Ship ship = new Ship();
            List<string> lines = new List<string>();

            for (int i = 0; i < numberOfShips; i++)
            {
                ship.id = i;
                ship.length = rnd.Next(minSize, maxSize + 1);
                ship.width = rnd.Next(minSize, maxSize + 1);
                ship.height = rnd.Next(minSize, maxSize + 1);
                ship.timestamp = DateTime.Now.ToFileTime();
                lines.Add(ship.id + ";" + ship.length + ";" + ship.width + ";" + ship.height + ";" + ship.timestamp);

                System.Threading.Thread.Sleep(1); //for the timestamp to differ
            }
            SaveShipsToFile(lines);
        }

        private void SaveShipsToFile(List<string> lines)
        {
            System.IO.File.WriteAllLines(fileName, lines);
        }

        private List<string> ReadShipsData()
        {
            string[] rawReadings = System.IO.File.ReadAllLines(fileName);
            List<string> data = new List<string>();
            foreach (var item in rawReadings)
            {
                data.Add(item);
            }

            return data;
        }

        public List<Ship> GetShipsList()
        {
            List<Ship> shipsList = new List<Ship>();
            List<string> rawDataList = ReadShipsData();
            for (int i = 0; i < rawDataList.Count; i++)
            {
                Ship ship = new Ship();
                string[] splitWords = rawDataList[i].Split(';');
                ship.id = Int32.Parse(splitWords[0]);
                ship.length = Int32.Parse(splitWords[1]);
                ship.width = Int32.Parse(splitWords[2]);
                ship.height = Int32.Parse(splitWords[3]);
                ship.timestamp = long.Parse(splitWords[4]);
                shipsList.Add(ship);
            }
            return shipsList;
        }

        public Ship GetShipById(int id)
        {
            List<Ship> shipsList = GetShipsList();
            return shipsList.Find(x => x.id == id);
        }

        public void AddNewShip()
        {
            var shipsList = GetShipsList();
            shipsList.OrderBy(x => x.timestamp);
            shipsList.Remove(shipsList[0]);

            List<string> lines = new List<string>();
            Random rnd = new Random();
            Ship ship = new Ship();
            shipsList.OrderBy(x => x.id);
            ship.id = shipsList[1].id + 1;
            ship.length = rnd.Next(minSize, maxSize + 1);
            ship.width = rnd.Next(minSize, maxSize + 1);
            ship.height = rnd.Next(minSize, maxSize + 1);
            ship.timestamp = DateTime.Now.ToFileTime();
            lines.Add(shipsList[0].id + ";" + shipsList[0].length + ";" + shipsList[0].width + ";" + shipsList[0].height + ";" + shipsList[0].timestamp);
            lines.Add(shipsList[1].id + ";" + shipsList[1].length + ";" + shipsList[1].width + ";" + shipsList[1].height + ";" + shipsList[1].timestamp);
            lines.Add(ship.id + ";" + ship.length + ";" + ship.width + ";" + ship.height + ";" + ship.timestamp);

            SaveShipsToFile(lines);
        }
    }
}
