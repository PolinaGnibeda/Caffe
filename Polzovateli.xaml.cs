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
    /// Логика взаимодействия для Polzovateli.xaml
    /// </summary>
    public partial class Polzovateli : Window
    {
        CoffeEntities db = new CoffeEntities();
        private int currentID;

        public Polzovateli(int userID)
        {
            InitializeComponent();
            currentID = userID;
            LoadPol();
        }

        private void LoadPol()
        {
            try
            {
                // Загружаем всех пользователей
                var polzovatel = db.Client.ToList();

                if (polzovatel.Count == 0)
                {
                    MessageBox.Show("Список пользователей пуст!");
                    Pol.ItemsSource = null;
                    return;
                }

                // Создаем список для отображения с полным именем
                var displayList = polzovatel.Select(c => new
                {
                    c.id_client,
                    FullName = $"{c.LastName} {c.Name} {c.MiddleName}".Trim(),
                    c.Name,
                    c.LastName,
                    c.MiddleName,
                    Status = c.id_status == 1 ? "Активен" : "Заблокирован",
                    Role = c.id_rol == 1 ? "Менеджер" : "Пользователь"
                }).ToList();

                Pol.ItemsSource = displayList;
                Pol.DisplayMemberPath = "FullName";

                MessageBox.Show($"Загружено {polzovatel.Count} пользователей");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        // Заблокировать
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var selectedItem = Pol.SelectedItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Выберите пользователя из списка!");
                return;
            }

            // Получаем id_client из анонимного объекта
            int clientId = (int)selectedItem.GetType().GetProperty("id_client").GetValue(selectedItem, null);
            var client = db.Client.FirstOrDefault(c => c.id_client == clientId);

            if (client == null)
            {
                MessageBox.Show("Пользователь не найден!");
                return;
            }

            if (client.id_status == 2)
            {
                MessageBox.Show("Пользователь уже заблокирован!");
                return;
            }

            if (MessageBox.Show($"Заблокировать пользователя {client.LastName}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                client.id_status = 2;
                db.SaveChanges();
                LoadPol();
                MessageBox.Show("Пользователь заблокирован!");
            }
        }

        // Разблокировать
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var selectedItem = Pol.SelectedItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Выберите пользователя из списка!");
                return;
            }

            int clientId = (int)selectedItem.GetType().GetProperty("id_client").GetValue(selectedItem, null);
            var client = db.Client.FirstOrDefault(c => c.id_client == clientId);

            if (client == null)
            {
                MessageBox.Show("Пользователь не найден!");
                return;
            }

            if (client.id_status == 1)
            {
                MessageBox.Show("Пользователь уже активен!");
                return;
            }

            if (MessageBox.Show($"Разблокировать пользователя {client.LastName}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                client.id_status = 1;
                db.SaveChanges();
                LoadPol();
                MessageBox.Show("Пользователь разблокирован!");
            }
        }

        // Назначить менеджером
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var selectedItem = Pol.SelectedItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Выберите пользователя из списка!");
                return;
            }

            int clientId = (int)selectedItem.GetType().GetProperty("id_client").GetValue(selectedItem, null);
            var client = db.Client.FirstOrDefault(c => c.id_client == clientId);

            if (client == null)
            {
                MessageBox.Show("Пользователь не найден!");
                return;
            }

            if (client.id_rol == 1)
            {
                MessageBox.Show("Пользователь уже является менеджером!");
                return;
            }

            if (MessageBox.Show($"Назначить {client.LastName} менеджером?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                client.id_rol = 1;
                db.SaveChanges();
                LoadPol();
                MessageBox.Show("Пользователь назначен менеджером!");
            }
        }

        // Вернуться в админку
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Admin admin = new Admin(currentID);
            admin.Show();
            this.Close();
        }
    }
}
