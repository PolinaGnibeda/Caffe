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
    /// Логика взаимодействия для HistoryOrder.xaml
    /// </summary>
    public partial class HistoryOrder : Window
    {
        CoffeEntities db = new CoffeEntities();
        private int currentID;
        public HistoryOrder(int userID)
        {
            InitializeComponent();
            LoadStatus();
            currentID = userID;
            LoadOrder();
        }
        private void LoadStatus()
        {
            var StatusList = db.Status_delivery.ToList();

            status.ItemsSource = StatusList.ToList();
            status.DisplayMemberPath = "Name";
            status.SelectedValuePath = "id_status_delivery";
            // Добавляем "Все статусы" для фильтрации
            var allStatuses = StatusList.ToList();
            allStatuses.Insert(0, new Status_delivery { id_status_delivery = 0, Name = "Все статусы" });
            status.ItemsSource = allStatuses;

            statuss.ItemsSource = StatusList.ToList();
            statuss.DisplayMemberPath = "Name";
            statuss.SelectedValuePath = "id_status_delivery";
        }
        
        private void LoadOrder()
        {

            var orders = db.Orders.ToList();
            var selectedStatus = status.SelectedItem as Status_delivery;

            List<Orders> filteredOrders;

            if (selectedStatus != null)
            {
                filteredOrders = orders.Where(c => c.id_status_delivery == selectedStatus.id_status_delivery).ToList();
            }
            else
            {
                filteredOrders = orders;
            }

            Orders.ItemsSource = filteredOrders;
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadOrder();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager manager = new Manager(currentID);
            manager.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var selectedOrder = Orders.SelectedItem as Orders;

            if (selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ!");
                return;
            }

            if (statuss.SelectedItem == null)
            {
                MessageBox.Show("Выберите новый статус!");
                return;
            }

            var selectedStatus = statuss.SelectedItem as Status_delivery;

            if (selectedStatus == null)
            {
                MessageBox.Show("Выберите корректный статус!");
                return;
            }

            var order = db.Orders.FirstOrDefault(c => c.id_order == selectedOrder.id_order);

            if (order != null)
            {
                order.id_status_delivery = selectedStatus.id_status_delivery;
                db.SaveChanges();

                MessageBox.Show($"Статус заказа №{order.id_order} изменен на '{selectedStatus.Name}'!");

                // Обновляем список заказов
                LoadOrder();

                // Если нужно сохранить фильтрацию, вызываем LoadOrderWithFilter()
                // LoadOrderWithFilter();

                // Сбрасываем выбор во втором комбо боксе (опционально)
                statuss.SelectedItem = null;
            }
        }

        private void statuss_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadStatus();
        }
    }
}
