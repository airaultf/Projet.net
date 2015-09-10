using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace TestWPF
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //InitializeComponent();
            Random random = new Random();
            List<Point> points = new List<Point>();
            for (int i = 0; i < 30; i++)
            {
                Point point = new Point(i, random.Next(10));
                points.Add(point);
            }
            RawDataSource rawDataSource = new RawDataSource(points);
            //chartPlotter.AddLineGraph(rawDataSource, Colors.Black, 2, "Portfolio Hist Positions");
            //chartPlotter.DataContext = rawDataSource;
            //lineGraph.DataSource = rawDataSource;
        }
    }
}

