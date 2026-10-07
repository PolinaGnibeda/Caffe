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
    public partial class Order : Window
    {
        CoffeEntities db = new CoffeEntities();
        private int currentID;
        private Orders selectedOrder;

        public Order(int userID)
        {
            InitializeComponent();
            currentID = userID;
            LoadOrders();
            LoadPaymentMethods();
        }

        private void LoadOrders()
        {
            var orderss = db.Orders.ToList();
            var displayList = orderss.Select(o => $"Заказ №{o.id_order} от {o.Registration_Date:dd.MM.yyyy} на сумму {o.Cost:F2} ₽").ToList();

            listOrders.ItemsSource = displayList;
            listOrders.Tag = orderss;
        }

        private void LoadPaymentMethods()
        {
            // Если есть таблица со способами оплаты
            // var methods = db.PaymentMethods.ToList();
            // cmbPaymentMethod.ItemsSource = methods;
            // cmbPaymentMethod.DisplayMemberPath = "MethodName";

            // Или просто добавляем варианты
            cmbPaymentMethod.ItemsSource = new List<string>
        {
            "Наличными при самовывозе",
            "Наличными курьеру",
        };
            cmbPaymentMethod.SelectedIndex = 0;
        }

        private void listOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listOrders.SelectedIndex >= 0)
            {
                var orderss = listOrders.Tag as List<Orders>;
                if (orderss != null && listOrders.SelectedIndex < orderss.Count)
                {
                    selectedOrder = orderss[listOrders.SelectedIndex];

                    // Показываем информацию о выбранном заказе
                    if (selectedOrder != null)
                    {
                        txtOrderInfo.Text = $"Заказ №{selectedOrder.id_order}\n" +
                                           $"Сумма: {selectedOrder.Cost:F2} ₽\n" +
                                           $"Дата: {selectedOrder.Registration_Date:dd.MM.yyyy}\n" +
                                           $"Статус: {(selectedOrder.PayMethod != null && selectedOrder.PayMethod != "" ? "Оплачен" : "Не оплачен")}";
                    }
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для оплаты!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedOrder.PayMethod != null && selectedOrder.PayMethod != "")
            {
                MessageBox.Show($"Заказ №{selectedOrder.id_order} уже оплачен!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Проверяем выбран ли способ оплаты
            if (cmbPaymentMethod.SelectedItem == null)
            {
                MessageBox.Show("Выберите способ оплаты!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var order = db.Orders.FirstOrDefault(o => o.id_order == selectedOrder.id_order);

                if (order != null)
                {
                    // Записываем способ оплаты
                    order.PayMethod = cmbPaymentMethod.SelectedItem.ToString();

                    // Если есть поле суммы оплаты
                    // order.PaidAmount = order.Cost;

                    // Если есть дата оплаты
                    // order.PaymentDate = DateTime.Now;

                    db.SaveChanges();

                    MessageBox.Show($"Заказ №{order.id_order} оплачен!\n" +
                                   $"Сумма: {order.Cost:F2} ₽\n" +
                                   $"Способ оплаты: {order.PayMethod}",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadOrders();
                    selectedOrder = null;
                    listOrders.SelectedIndex = -1;
                    txtOrderInfo.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оплате: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Admin admin = new Admin(currentID);
            admin.Show();
            this.Close();
        }
    }
}
