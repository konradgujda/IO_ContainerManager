using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContainerTransportOptimizer
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var containerService = new ContainerService();
            containerService.GenerateContainerSets(new List<int>() {100, 60, 30});
            var shipService = new ShipService();
            shipService.GenerateShipSet();

            Controls.Clear();
            InitializeComponent();
            MessageBox.Show("Data generated succesfully", "Save", MessageBoxButtons.OK);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Graphics loadingGraphic = CreateGraphics();
            ShowLoading(loadingGraphic);

            ContainerService containerService = new ContainerService();
            List<Shipment> shipmentsList = new List<Shipment>();
            Shipment shipment;

            for (int i = 0; i < 2; i++)
            {
                List<Shipment> tempShipmentsList = new List<Shipment>();
                List<Container> containersLeft =
                    ChooseTheShipAndSendIt(containerService.GetContainersList(), i, out shipment);
                tempShipmentsList.Add(shipment);
                while (containersLeft.Count > 0)
                {
                    containersLeft = ChooseTheShipAndSendIt(containersLeft, i, out shipment);
                    tempShipmentsList.Add(shipment);
                }

                if (shipmentsList.Count == 0 || shipmentsList.Count > tempShipmentsList.Count)
                    shipmentsList = tempShipmentsList;
            }

            ComboBox comboBox1 = new ComboBox();
            comboBox1.Location = new System.Drawing.Point((Screen.PrimaryScreen.Bounds.Width / 2) - 100, 20);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new System.Drawing.Size(200, 50);
            comboBox1.BackColor = System.Drawing.Color.White;
            comboBox1.ForeColor = System.Drawing.Color.Black;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox comboBox2 = new ComboBox();
            comboBox2.Location = new System.Drawing.Point((Screen.PrimaryScreen.Bounds.Width / 2) - 350, 20);
            comboBox2.Name = "comboBox1";
            comboBox2.Size = new System.Drawing.Size(200, 50);
            comboBox2.BackColor = System.Drawing.Color.White;
            comboBox2.ForeColor = System.Drawing.Color.Black;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;

            for (int i = 0; i < shipmentsList.Count; i++)
            {
                comboBox2.Items.Add("Shipment " + i + ", ShipID: " + shipmentsList[i].GetShip().id);
            }
            comboBox2.SelectedIndex = 0;
            Controls.Add(comboBox2);
            shipment = shipmentsList.First();
            
            for (int i=0; i<shipment.GetNoLevels();i++)
            {
                comboBox1.Items.Add("Level " + i);
            }
            comboBox1.SelectedIndex = 0;
            Controls.Add(comboBox1);

            DrawShipmentLevel(shipment,0);
            comboBox1.SelectedIndexChanged += (senderCombo, eCombo) => comboBox1_SelectedIndexChanged(senderCombo, eCombo, shipment);
            comboBox2.SelectedIndexChanged += (senderCombo2, eCombo2) => comboBox2_SelectedIndexChanged(senderCombo2, eCombo2, shipmentsList, comboBox1, out shipment);
            DisposeLoading(loadingGraphic);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            (sender as Button).Parent.Dispose();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var shipService = new ShipService();
            shipService.AddNewShip();
            Controls.Clear();
            InitializeComponent();
            MessageBox.Show("Ship added succesfully", "ShipAdd", MessageBoxButtons.OK);
            //button2_Click(sender, e);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e, Shipment shipment)
        {
            DrawShipmentLevel(shipment, (sender as ComboBox).SelectedIndex);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e, List<Shipment> shipmentsList, ComboBox combobox, out Shipment shipment)
        {
            shipment = shipmentsList[(sender as ComboBox).SelectedIndex];
            combobox.Items.Clear();
            for (int i = 0; i < shipment.GetNoLevels(); i++)
            {
                combobox.Items.Add("Level " + i);
            }
            if (combobox.SelectedIndex == 0) DrawShipmentLevel(shipment, combobox.SelectedIndex);
            else combobox.SelectedIndex = 0;
        }

        private void DrawShipmentLevel(Shipment shipment, int level)
        {
            Refresh();
            var table = shipment.GetSpaceArrayForLevel(level);
            var constX = (Screen.PrimaryScreen.Bounds.Width - 40) / table.GetLength(0);
            var constY = (Screen.PrimaryScreen.Bounds.Height - 140) / table.GetLength(1);
            var wholeRectanglePositionX = Screen.PrimaryScreen.Bounds.Width / 2 - constX * table.GetLength(0) / 2;
            var wholeRectanglePositionY = Screen.PrimaryScreen.Bounds.Height / 2 - constY * table.GetLength(1) / 2;
            Graphics formGraphics = CreateGraphics();
            formGraphics.DrawRectangle(new Pen(Color.Black), new Rectangle(wholeRectanglePositionX, wholeRectanglePositionY, table.GetLength(0) * constX, table.GetLength(1) * constY));

            for (int i = 0; i < table.GetLength(0); i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    if (table[i, j] == true) formGraphics.FillRectangle(new SolidBrush(Color.Green), new Rectangle(wholeRectanglePositionX + i * constX, wholeRectanglePositionY + j * constY, constX, constY));
                    else formGraphics.FillRectangle(new SolidBrush(Color.Brown), new Rectangle(wholeRectanglePositionX + i * constX, wholeRectanglePositionY + j * constY, constX, constY));
                }
            }

            foreach(var item in shipment.GetContainerLocations().FindAll(x => x.level == level))
            {
                var container = new ContainerService().GetContainerById(item.containerId);
                int containerXSize, containerYSize;
                if (item.orientation == true)
                {
                    containerXSize = container.length;
                    containerYSize = container.width;
                }
                else
                {
                    containerXSize = container.width;
                    containerYSize = container.length;
                }

                var xPos = wholeRectanglePositionX + item.xPosition * constX;
                var yPos = wholeRectanglePositionY + item.yPosition * constY;
                var xSize = containerXSize * constX;
                var ySize = containerYSize * constY;
                var fontSize = 14;
                var rect1 = new Rectangle(xPos, yPos, xSize, ySize);
                formGraphics.DrawRectangle(new Pen(Color.Black), rect1);

                if (containerYSize < 2) fontSize = 5;
                else if (containerXSize < 3 || containerYSize < 4) fontSize = 10;

                StringFormat format = new StringFormat();
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Center;
                formGraphics.DrawString("C"+container.id,new Font("Arial",fontSize,FontStyle.Bold, GraphicsUnit.Point), Brushes.Black, rect1, format);
            }

            formGraphics.Dispose();
        }

        private List<Container> ChooseTheShipAndSendIt(List<Container> containersList, int algorithm, out Shipment shipment)
        {
            var containerHeight = containersList.First().height;
            List<Container> containersLeft = new List<Container>();
            List<Ship> shipsList = new ShipService().GetShipsList();
            var bestShipId = 0;
            var prevContainersCount = Int32.MaxValue;

            foreach (var item in shipsList)
            {
                shipment = new Shipment(item.id, containerHeight);
                containersLeft = shipment.FillTheShip(containersList, algorithm);
                if (containersLeft.Count < prevContainersCount)
                {
                    bestShipId = item.id;
                    prevContainersCount = containersLeft.Count;
                }
            }
            shipment = new Shipment(bestShipId, containerHeight);
            containersLeft = shipment.FillTheShip(containersList, algorithm);

            return containersLeft;
        }

        private void ShowLoading(Graphics loadingGraphic)
        {
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;

            var xPos = Screen.PrimaryScreen.Bounds.Width / 2 - 150;
            var yPos = Screen.PrimaryScreen.Bounds.Height / 2 - 100;
            var rect1 = new Rectangle(xPos, yPos, 300, 200);
            loadingGraphic.FillRectangle(new SolidBrush(Color.Gray), rect1);
            loadingGraphic.DrawRectangle(new Pen(Color.Black), rect1);
            loadingGraphic.DrawString("LOADING...", new Font("Arial", 20, FontStyle.Bold, GraphicsUnit.Point), Brushes.Black, rect1, format);
        }

        private void DisposeLoading(Graphics loadingGraphic)
        {
            loadingGraphic.Dispose();
        }
    }
}
