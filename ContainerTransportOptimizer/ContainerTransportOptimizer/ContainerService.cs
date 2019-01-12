using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerTransportOptimizer
{
    public class ContainerService
    {
        private const int maxSize = 40;
        private const int minSize = 1;
        private string fileName = "containerSet.txt";
        private int constHeight, idCounter;

        public void GenerateContainerSets(List<int> numberOfContainersInEachSet)
        {
            var i = 0;
            var isFirstSet = true;
            foreach (var item in numberOfContainersInEachSet)
            {
                GenerateContainerSet(item, i++, isFirstSet);
                isFirstSet = false;
            }
        }

        private void GenerateContainerSet(int numberOfContainers, int timestamp, bool isFirstSet)
        {
            Random rnd = new Random();
            Container container = new Container();
            List<string> lines = new List<string>();
            if (isFirstSet)
            {
                constHeight =rnd.Next(minSize, maxSize + 1);
                idCounter = 0;
            }

            for (int i = 0; i < numberOfContainers; i++)
            {
                container.id = idCounter++;
                container.length = rnd.Next(minSize, maxSize + 1);
                container.width = rnd.Next(minSize, maxSize + 1);
                container.height = constHeight;
                container.timestamp = timestamp;
                lines.Add(container.id + ";" + container.length + ";" + container.width + ";" + container.height + ";" + container.timestamp);
            }
            if(isFirstSet) System.IO.File.WriteAllLines(fileName, lines);
            else System.IO.File.AppendAllLines(fileName,lines);
        }

        private List<string> ReadContainersData(string fileName)
        {
            string[] rawReadings = System.IO.File.ReadAllLines(fileName);
            List<string> data = new List<string>();
            for(int i=0;i<rawReadings.Length;i++) data.Add(rawReadings[i]);

            return data;
        }

        public List<Container> GetContainersList()
        {
            List<Container> containersList = new List<Container>();
            List<string> rawDataList = ReadContainersData(fileName);
            for(int i=0; i<rawDataList.Count; i++)
            {
                Container container = new Container();
                string[] splitWords = rawDataList[i].Split(';');
                container.id = Int32.Parse(splitWords[0]);
                container.length = Int32.Parse(splitWords[1]);
                container.width = Int32.Parse(splitWords[2]);
                container.height = Int32.Parse(splitWords[3]);
                container.timestamp = Int32.Parse(splitWords[4]);
                containersList.Add(container);
            }
            return containersList;
        }

        public Container GetContainerById(int id)
        {
            List<Container> containersList = GetContainersList();
            return containersList.Find(x => x.id==id);
        }
    }
}
