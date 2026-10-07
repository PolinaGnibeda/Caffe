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
    /// Логика взаимодействия для MenuManager.xaml
    /// </summary>
    public partial class MenuManager : Window
    {
        CoffeEntities db = new CoffeEntities();
        private int currentID;
        public MenuManager(int userID)
        {
            InitializeComponent();
            currentID = userID;
            LoadMenu();
        }

        private void LoadMenu()
        {
            var Menu = db.Dish.ToList();

            listMenu.ItemsSource = Menu;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager manager = new Manager(currentID);
            manager.Show();
            this.Close();
        }
    }
}
