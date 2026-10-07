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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;

namespace OnlineCaffe
{
    public partial class MainWindow : Window
    {
        CoffeEntities db = new CoffeEntities();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Regestrazia regestrazia = new Regestrazia();
            regestrazia.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string Login = login.Text;
            string Password;

            // Получаем пароль из активного поля
            if (passwordBox.Visibility == Visibility.Visible)
            {
                Password = passwordBox.Password;
            }
            else
            {
                Password = passwordVisible.Text;
            }

            if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Не все поля заполнены!");
                return;
            }

            try
            {
                    var user = db.Client
                        .FirstOrDefault(c => c.Login == Login && c.Password == Password);

                    if (user == null)
                    {
                        MessageBox.Show("Неверный логин или пароль!", "Ошибка авторизации");
                        return;
                    }

                    if (user.id_status == 2)
                    {
                        MessageBox.Show("Ваш аккаунт заблокирован! Обратитесь к администратору.",
                            "Доступ запрещён");
                        return; 
                    }

                    int userID = user.id_client;

                    switch (user.id_rol)
                    {
                        case 1:
                            MessageBox.Show($"Добро пожаловать, {user.Name} {user.LastName}!",
                                "Администратор");
                            Admin adminWindow = new Admin(userID);
                            adminWindow.Show();
                            this.Close();
                            break;

                        case 2:
                            MessageBox.Show($"Добро пожаловать, {user.Name} {user.LastName}!",
                                "Менеджер");
                            Manager managerWindow = new Manager(userID);
                            managerWindow.Show();
                            this.Close();
                            break;

                        case 3:
                            MessageBox.Show($"Добро пожаловать, {user.Name} {user.LastName}!",
                                "Клиент");
                            Clients clientWindow = new Clients(userID);
                            clientWindow.Show();
                            this.Close();
                            break;

                        default:
                            MessageBox.Show("Неизвестная роль пользователя!");
                            break;
                    }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при авторизации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Visibility == Visibility.Visible)
            {
                // Показываем пароль в TextBox
                passwordVisible.Text = passwordBox.Password;
                passwordBox.Visibility = Visibility.Collapsed;
                passwordVisible.Visibility = Visibility.Visible;
                showPasswordBtn.Content = "🙈"; // Глазик зачеркнутый
            }
            else
            {
                // Скрываем пароль в PasswordBox
                passwordBox.Password = passwordVisible.Text;
                passwordVisible.Visibility = Visibility.Collapsed;
                passwordBox.Visibility = Visibility.Visible;
                showPasswordBtn.Content = "👁"; // Обычный глазик
            }
        }
    }
}

