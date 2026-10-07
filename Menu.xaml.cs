using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace OnlineCaffe
{
    public partial class Menu : Window
    {
        CoffeEntities db = new CoffeEntities();
        private int currentID;

        public Menu(int userID)
        {
            InitializeComponent();
            LoadCategory();
            currentID = userID;
        }

        private void LoadCategory()
        {
            var allkategory = db.Kategory.ToList();
            kategory.ItemsSource = allkategory;
            kategory.DisplayMemberPath = "Name";
            kategory.SelectedValuePath = "id_kategory";

            if (allkategory.Any())
            {
                kategory.SelectedIndex = 0;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (kategory.SelectedItem == null) return;

            Kategory selectedCategory = (Kategory)kategory.SelectedItem;
            int selectedId = selectedCategory.id_kategory;

            var dishes = db.Dish
                .Where(d => d.id_kategory == selectedId)
                .ToList();

            foreach (var dish in dishes)
            {
                dish.ImagePath = "./" + dish.Name + ".jpg";
            }

            listDish.ItemsSource = dishes;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string dish = search.Text;

            if (string.IsNullOrWhiteSpace(dish))
            {
                MessageBox.Show("Введите название блюда для поиска!");
                return;
            }

            var searchResult = db.Dish
                .Where(c => c.Name.Contains(dish))
                .ToList();

            foreach (var item in searchResult)
            {
                string fileName = item.Name + ".jpg";
                if (File.Exists(fileName))
                {
                    item.ImagePath = fileName;
                }
                else
                {
                    item.ImagePath = null;
                }
            }

            listDish.ItemsSource = searchResult;

            if (searchResult.Count == 0)
            {
                MessageBox.Show("Блюда не найдены!");
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var selectedDish = listDish.SelectedItem as Dish;

            if (selectedDish == null)
            {
                MessageBox.Show("Выберите блюдо!");
                return;
            }

            try
            {
                // ПОКАЗЫВАЕМ ВЫБРАННОЕ БЛЮДО
                MessageBox.Show($"Выбрано блюдо: ID={selectedDish.id_dish}, Название={selectedDish.Name}, Цена={selectedDish.Price}");

                // Ищем корзину со статусом 1
                var activeBasket = db.Basket.FirstOrDefault(b => b.id_order_status == 1);

                if (activeBasket == null)
                {
                    MessageBox.Show("Корзина не найдена, создаю новую...");
                    activeBasket = new Basket { id_order_status = 1 };
                    db.Basket.Add(activeBasket);
                    db.SaveChanges();
                    MessageBox.Show($"Создана корзина с ID={activeBasket.id_basket}");
                }
                else
                {
                    MessageBox.Show($"Найдена корзина с ID={activeBasket.id_basket}, статус={activeBasket.id_order_status}");
                }

                // СОЗДАЕМ ЗАПИСЬ В Order_structure
                Order_structure newItem = new Order_structure
                {
                    id_dish = selectedDish.id_dish,
                    id_basket = activeBasket.id_basket,
                    Quantity = 1,
                    Price = selectedDish.Price
                };

                db.Order_structure.Add(newItem);
  
                // СОХРАНЯЕМ
                int result = db.SaveChanges();
                

                // ПРОВЕРЯЕМ
                var check = db.Order_structure.Where(o => o.id_basket == activeBasket.id_basket).ToList();
              
                MessageBox.Show($"\"{selectedDish.Name}\" добавлено в корзину!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ОШИБКА: {ex.Message}\n\n{ex.InnerException?.Message}");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Korzina korzina = new Korzina(currentID);
            korzina.Show();
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Clients clients = new Clients(currentID);
            clients.Show();
            this.Close();
        }
    }
}