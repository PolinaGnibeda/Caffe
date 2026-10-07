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
    /// Логика взаимодействия для Pay.xaml
    /// </summary>
    public partial class Pay : Window
    {
        CoffeEntities db  = new CoffeEntities();
        private int currentID;
        public Pay(int userID)
        {
            InitializeComponent();
            currentID = userID;
            LoadAllPayments();
        }
        private void LoadAllPayments()
        {
            var allPayments = db.Pay.ToList();

            var ordersWithPay = db.Orders.Where(o => o.Pay.Any()).ToList();

            var displayList = db.Orders
                .Include("Client")
                .Include("Pay")
                .Where(o => o.Pay.Any())
                .SelectMany(o => o.Pay.Select(p => new
                {
                    p.id_pay,
                    p.Cost,
                    p.Status_pay,
                    p.Payment_Method,
                    OrderId = o.id_order,
                    ClientName = o.Client != null ? o.Client.Login : "НЕТ КЛИЕНТА",
                    OrderDate = o.Registration_Date
                }))
                .ToList();

            listPayments.ItemsSource = displayList;
        }

        private void LoadPaymentsByLogin(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                LoadAllPayments();
                return;
            }

            var client = db.Client.FirstOrDefault(c => c.Login.Contains(login));
            if (client == null)
            {
                MessageBox.Show($"Пользователь '{login}' не найден!");
                LoadAllPayments();
                return;
            }
           

            var displayList = db.Orders
                .Include("Client")
                .Include("Pay")
                .Where(o => o.id_client == client.id_client && o.Pay.Any())
                .SelectMany(o => o.Pay.Select(p => new
                {
                    p.id_pay,
                    p.Cost,
                    p.Status_pay,
                    p.Payment_Method,
                    OrderId = o.id_order,
                    ClientName = o.Client != null ? o.Client.Login : "Неизвестно",
                    OrderDate = o.Registration_Date
                }))
                .ToList();


            if (displayList.Count == 0)
            {
                MessageBox.Show($"Платежи для пользователя '{login}' не найдены!");
            }
        }
        private void TxtLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            string login = txtLogin.Text;
            LoadPaymentsByLogin(login);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager  manager = new Manager(currentID);
            manager.Show();
            this.Close();
        }
    }
}
