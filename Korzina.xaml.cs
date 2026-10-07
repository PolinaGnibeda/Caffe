using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace OnlineCaffe
{
    public partial class Korzina : Window
    {
        CoffeEntities db = new CoffeEntities();
        private int currentID;

        public Korzina(int userId)
        {
            InitializeComponent();
            currentID = userId;
            LoadBasket();
        }

        private void LoadBasket()
        {
            try
            {
                using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    conn.Open();

                    string sql = @"
                        SELECT 
                            d.Name AS DishName,
                            os.Quantity,
                            os.Price,
                            (os.Quantity * os.Price) AS TotalPrice
                        FROM Order_structure os
                        INNER JOIN Dish d ON os.id_dish = d.id_dish
                        INNER JOIN Basket b ON os.id_basket = b.id_basket
                        WHERE b.id_order_status = 1";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        var reader = cmd.ExecuteReader();
                        var items = new List<object>();
                        decimal total = 0;

                        while (reader.Read())
                        {
                            string dishName = reader["DishName"].ToString();
                            int quantity = Convert.ToInt32(reader["Quantity"]);
                            decimal price = Convert.ToDecimal(reader["Price"]);
                            decimal totalPrice = Convert.ToDecimal(reader["TotalPrice"]);
                            total += totalPrice;

                            items.Add(new
                            {
                                DishName = dishName,
                                Quantity = quantity,
                                Price = price,
                                TotalPrice = totalPrice
                            });
                        }

                        listBasket.ItemsSource = items;
                        txtTotal.Text = total.ToString("C");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки корзины: {ex.Message}");
            }
        }

        // Оформление заказа
        // Оформление заказа
        private void BtnCheckout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    conn.Open();

                    // Получить корзину и её позиции
                    string getBasketSql = @"
                SELECT b.id_basket, SUM(os.Quantity * os.Price) AS TotalCost
                FROM Basket b
                INNER JOIN Order_structure os ON b.id_basket = os.id_basket
                WHERE b.id_order_status = 1
                GROUP BY b.id_basket";

                    using (var cmd = new SqlCommand(getBasketSql, conn))
                    {
                        var reader = cmd.ExecuteReader();
                        if (!reader.Read())
                        {
                            MessageBox.Show("Корзина пуста!");
                            return;
                        }

                        int basketId = reader.GetInt32(0);
                        decimal totalCost = reader.GetDecimal(1);
                        reader.Close();

                        if (MessageBox.Show($"Оформить заказ на сумму {totalCost:C}?", "Подтверждение",
                            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            // Создать заказ
                            string createOrderSql = @"
                        INSERT INTO Orders (Registration_Date, Cost, id_status_delivery, id_status_readiness, id_client) 
                        VALUES (@date, @cost, 1, 1, @clientId);
                        SELECT SCOPE_IDENTITY();";

                            using (var orderCmd = new SqlCommand(createOrderSql, conn))
                            {
                                orderCmd.Parameters.AddWithValue("@date", DateTime.Now);
                                orderCmd.Parameters.AddWithValue("@cost", totalCost);
                                orderCmd.Parameters.AddWithValue("@clientId", currentID);
                                int newOrderId = Convert.ToInt32(orderCmd.ExecuteScalar());

                                // ✅ СОЗДАТЬ ПЛАТЕЖ
                                string createPaySql = @"
                            INSERT INTO Pay (id_order, Cost, Status_pay, Payment_Method) 
                            VALUES (@orderId, @cost, 'Ожидает оплаты', 'Наличными при получении')";

                                using (var payCmd = new SqlCommand(createPaySql, conn))
                                {
                                    payCmd.Parameters.AddWithValue("@orderId", newOrderId);
                                    payCmd.Parameters.AddWithValue("@cost", totalCost);
                                    payCmd.ExecuteNonQuery();
                                }

                                // Обновить статус корзины
                                string updateBasketSql = "UPDATE Basket SET id_order_status = 2 WHERE id_basket = @basketId";
                                using (var updateCmd = new SqlCommand(updateBasketSql, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@basketId", basketId);
                                    updateCmd.ExecuteNonQuery();
                                }

                                MessageBox.Show($"Заказ №{newOrderId} оформлен на сумму {totalCost:C}!", "Успех");
                                LoadBasket();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка оформления заказа: {ex.Message}");
            }
        }

        // Очистка корзины
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Очистить корзину?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
                    {
                        conn.Open();

                        string sql = @"
                            DELETE os
                            FROM Order_structure os
                            INNER JOIN Basket b ON os.id_basket = b.id_basket
                            WHERE b.id_order_status = 1";

                        using (var cmd = new SqlCommand(sql, conn))
                        {
                            int rows = cmd.ExecuteNonQuery();
                            MessageBox.Show($"Очищено {rows} позиций");
                            LoadBasket();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка очистки: {ex.Message}");
                }
            }
        }

        // Вернуться в меню
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Menu menu = new Menu(currentID);
            menu.Show();
            this.Close();
        }
    }
}