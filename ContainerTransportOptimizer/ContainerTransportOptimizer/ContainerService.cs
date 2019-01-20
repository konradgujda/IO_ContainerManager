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
        /// <summary>
        /// Generating sets of containers.
        /// </summary>
        /// <param name="numberOfContainersInEachSet">List defining how many containers will be in each set. Length of this list defines the number of sets. For each set, different timestamp is being set.</param>
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
        /// <summary>
        /// Generating single container set and saving it to file.
        /// </summary>
        /// <param name="numberOfContainers">How many containers will be in this set.</param>
        /// <param name="timestamp">Timestamp of all the containers in this set.</param>
        /// <param name="isFirstSet">Determines if the file will be written or appended.</param>
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
        /// <summary>
        /// Reads container data from file. Read data is raw here.
        /// </summary>
        /// <param name="fileName">Path to the file.</param>
        /// <returns>List of raw data strings for each container.</returns>
        private List<string> ReadContainersData(string fileName)
        {
            string[] rawReadings = System.IO.File.ReadAllLines(fileName);
            List<string> data = new List<string>();
            for(int i=0;i<rawReadings.Length;i++) data.Add(rawReadings[i]);

            return data;
        }
        /// <summary>
        /// Converting raw data from file to List of Container objects.
        /// </summary>
        /// <returns>List of containers from file.</returns>
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
        /// <summary>
        /// Gets the particular container by it's ID.
        /// </summary>
        /// <param name="id">Container's ID.</param>
        /// <returns>Container</returns>
        public Container GetContainerById(int id)
        {
            List<Container> containersList = GetContainersList();
            return containersList.Find(x => x.id==id);
        }
    }
}
