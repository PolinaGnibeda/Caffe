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

namespace OnlineCaffe
{
    /// <summary>
    /// Логика взаимодействия для Manager.xaml
    /// </summary>
    public partial class Manager : Window
    {
        private int currentID;
        public Manager(int userID)
        {
            InitializeComponent();
            currentID = userID;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MenuManager menu = new MenuManager(currentID);
            menu.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            report report = new report(currentID);
            report.Show();
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Pay pay = new Pay(currentID);
            pay.Show();
            this.Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            HistoryOrder historyOrder = new HistoryOrder(currentID);
            historyOrder.Show();
            this.Close();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            DannClient dannClient = new DannClient(currentID);
            dannClient.Show();
            this.Close();
        }
    }
}
