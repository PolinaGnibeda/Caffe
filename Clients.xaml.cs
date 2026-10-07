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
    /// Логика взаимодействия для Clients.xaml
    /// </summary>
    public partial class Clients : Window
    {
        CoffeEntities db = new CoffeEntities();
        private int currentID;

        public Clients(int userId)
        {
            InitializeComponent();
            currentID = userId;
            LoadUserData();
        }

        private void LoadUserData()
        {
            try
            {
                // Находим клиента по ID
                var client = db.Client.FirstOrDefault(c => c.id_client == currentID);

                if (client != null)
                {
                    // Вставляем имя и фамилию в TextBox
                    txtUserData.Text = $"{client.Name} {client.LastName}";
                }
                else
                {
                    txtUserData.Text = "Данные не найдены";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
                txtUserData.Text = "Ошибка";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Menu menu = new Menu(currentID);
            menu.Show();
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Korzina korzina = new Korzina(currentID);
            korzina.Show();
            this.Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            History history = new History(currentID);
            history.Show();
            this.Close();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
