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
    /// Логика взаимодействия для History.xaml
    /// </summary>
    public partial class History : Window
    {
        CoffeEntities db = new CoffeEntities();
        private int currentID;
        public History(int userId)
        {
            InitializeComponent();
            currentID = userId;
            LoadUserOrders();
        }
        private void LoadUserOrders()
        {
            try
            {
                // Получаем заказы текущего пользователя
                var userOrders = db.Orders
                    .Where(z => z.id_client == currentID)
                    .OrderByDescending(o => o.Registration_Date)
                    .ToList();

                // Сохраняем полные объекты заказов в Tag
                order.Tag = userOrders;

                // Превращаем в понятный текст с сохранением id_order
                var displayOrders = userOrders.Select(o => new
                {
                    Текст = $"Заказ от {o.Registration_Date:dd.MM.yyyy HH:mm} на сумму {o.Cost:F2} ₽",
                    id_order = o.id_order, // <-- ДОБАВЛЯЕМ id_order
                    Status = o.id_status_delivery // <-- ДОБАВЛЯЕМ статус
                }).ToList();

                order.ItemsSource = displayOrders;
                order.DisplayMemberPath = "Текст";

                if (userOrders.Count == 0)
                {
                    MessageBox.Show("У вас пока нет заказов");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Clients clients = new Clients(currentID);
            clients.Show();
            this.Close();
        }

        private void order_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = order.SelectedItem;
            if (selected == null) return;

            try
            {

                var orderId = selected.GetType().GetProperty("id_order")?.GetValue(selected);

                if (orderId != null)
                {

                    var ordersList = order.Tag as List<Orders>;
                    var fullOrder = ordersList?.FirstOrDefault(o => o.id_order == (int)orderId);

                    if (fullOrder != null)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (order.SelectedItem == null)
            {
                MessageBox.Show("Выберите заказ для отмены!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Получаем выбранный заказ
                var selectedOrder = order.SelectedItem as dynamic;

                // Получаем id заказа (используем рефлексию, так как у нас анонимный тип)
                int orderId = 0;
                var propertyInfo = selectedOrder.GetType().GetProperty("id_order");
                if (propertyInfo != null)
                {
                    orderId = (int)propertyInfo.GetValue(selectedOrder);
                }
                else
                {
                    // Альтернативный способ - если в SelectedItem хранится объект Orders
                    var orderObj = order.SelectedItem as Orders;
                    if (orderObj != null)
                    {
                        orderId = orderObj.id_order;
                    }
                }

                if (orderId == 0)
                {
                    MessageBox.Show("Не удалось определить заказ!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Находим заказ в базе данных
                var orderToCancel = db.Orders.FirstOrDefault(o => o.id_order == orderId);

                if (orderToCancel == null)
                {
                    MessageBox.Show("Заказ не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int cookingStatusId = 2; 

                if (orderToCancel.id_status_delivery == cookingStatusId)
                {
                    MessageBox.Show("Невозможно отменить заказ, так как он уже готовится!",
                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отмене заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
