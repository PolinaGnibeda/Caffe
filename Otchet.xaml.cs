using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
    /// Логика взаимодействия для Otchet.xaml
    /// </summary>
    public partial class Otchet : Window
    {
        CoffeEntities db = new CoffeEntities();
        private int currentID;
        public Otchet(int userID)
        {
            InitializeComponent();
            currentID = userID;
            LoadSatatus();
            LoadPay();
            LoadPayments();
        }
        private void LoadSatatus()
        {
            var status = db.Status_delivery.ToList();
            cmbFilterStatus.ItemsSource = status;
            cmbFilterStatus.DisplayMemberPath = "Name";
        }
        private void LoadPayments()
        {
            var orders = db.Orders
                .Include("Status_delivery")
                .OrderByDescending(o => o.Registration_Date)
                .ToList();

            dgPayments.ItemsSource = orders;
            UpdateTotal(orders);
        }

        // Обновление итоговой суммы
        private void UpdateTotal(List<Orders> orders)
        {
            decimal total = orders.Sum(o => o.Cost ?? 0);
            txtTotal.Text = $"{total:F2} руб";
        }

        // Обновление итоговой суммы для DataGrid
        private void UpdateTotalFromGrid()
        {
            var orders = dgPayments.ItemsSource as List<Orders>;
            if (orders != null)
            {
                decimal total = orders.Sum(o => o.Cost ?? 0);
                txtTotal.Text = $"{total:F2} руб";
            }
        }
        private void LoadPay()
        {
            string[] Pay = { "Наличными при получении", "Наличными при самовывозе" };
            cmbFilterMethod.ItemsSource = Pay;
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Admin admin = new Admin(currentID);
            admin.Show();
            this.Close();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            var selectedStatus = cmbFilterStatus.SelectedItem as Status_delivery;
            var selectedPayMethod = cmbFilterMethod.SelectedItem as string;

            // Проверяем, что хоть один фильтр выбран
            if (selectedStatus == null && string.IsNullOrEmpty(selectedPayMethod))
            {
                MessageBox.Show("Выберите статус или способ оплаты заказа!");
                return;
            }

            var query = db.Orders.AsQueryable();

            if (selectedStatus != null)
            {
                query = query.Where(o => o.id_status_delivery == selectedStatus.id_status_delivery);
            }

            if (!string.IsNullOrEmpty(selectedPayMethod))
            {
                query = query.Where(o => o.PayMethod == selectedPayMethod);
            }

            var orders = query.ToList();

            dgPayments.ItemsSource = orders;

            if (orders.Count == 0)
            {
                MessageBox.Show("Заказы не найдены!");
            }
            else
            {
                MessageBox.Show($"Найдено заказов: {orders.Count}");
            }
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
           DateTime dpStarts = dpStart.SelectedDate ?? DateTime.Now;
           DateTime dpEnds = dpEnd.SelectedDate ?? DateTime.Now;

            dpStarts = dpStarts.Date;
            dpEnds = dpEnds.Date;

            if(dpStarts.Date > dpEnds.Date)
            {
                MessageBox.Show("Дата начала не может быть позже окончания!");
                return;
            }

            var allOrders = db.Orders.Where(c => c.Registration_Date >= dpStarts && c.Registration_Date <= dpEnds).ToList();

            decimal turnover = allOrders.Sum(y => y.Cost ?? 0);

            var paidOrders = allOrders.Where(z => z.id_status_delivery ==3 || z.id_status_delivery == 4).ToList();

            decimal renever = paidOrders.Sum(f => f.Cost ?? 0);

            txtTurnover.Text = $"{turnover:F2} руб";
            txtRevenue.Text = $"{renever:F2} руб";

            MessageBox.Show($"Период: {dpStart:dd.MM.yyyy} - {dpEnd:dd.MM.yyyy}\n" +
                           $"Всего заказов: {allOrders.Count}\n" +
                           $"Оборот: {turnover:F2} руб\n" +
                           $"Оплаченных заказов: {paidOrders.Count}\n" +
                           $"Выручка: {renever:F2} руб", "Результат");
        }
    }
}
