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
    /// Логика взаимодействия для MenuAdmin.xaml
    /// </summary>
    public partial class MenuAdmin : Window
    {
        CoffeEntities db = new CoffeEntities();
        private int currentID;
        public MenuAdmin(int userID)
        {
            InitializeComponent();
            currentID = userID;
            LoadKategory();
            LoadDish();
        }
        private void LoadKategory()
        {
            var Kategorys = db.Kategory.ToList();
            listKategory.ItemsSource = Kategorys.ToList();
            listKategory.DisplayMemberPath = "Name";
        }
        private void LoadDish()
        {
            var Dis = db.Dish.ToList();
            listDish.ItemsSource = Dis.ToList();
            listDish.DisplayMemberPath = "Name";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Admin admin = new Admin(currentID);
            admin.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string newKategory = kategory.Text;


            if (string.IsNullOrEmpty(newKategory))
            {
                MessageBox.Show("Введите название категории!");
                return;
            }
                Kategory kategorys = new Kategory()
                {
                    Name = newKategory
                };

                db.Kategory.Add(kategorys);
                db.SaveChanges();
                MessageBox.Show("Новая категория добавлена!");
                LoadKategory();
                
           
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) //удаление категории
        {
            var selectedKategory = listKategory.SelectedItem as Kategory;
            if (selectedKategory != null)
            {
                var kar  = db.Kategory.FirstOrDefault(c => c.id_kategory == selectedKategory.id_kategory);

                if (kar != null)
                {
                    if (MessageBox.Show("Вы уверены, что хотите удалить категорию?",
                "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        db.Kategory.Remove(kar);
                        db.SaveChanges();
                    MessageBox.Show("Категория удалена!");
                    LoadKategory();
                }
            }

            if (selectedKategory == null)
            {
                MessageBox.Show("Выберите категорию для удаления!");
            }

        }

        private void Button_Click_3(object sender, RoutedEventArgs e) //изменить категрию
        {
            string kater = kategory.Text;
            var selectKategor = listKategory.SelectedItem as Kategory;

            if (selectKategor != null)
            {
                var kategoria = db.Kategory.FirstOrDefault(c => c.id_kategory ==  selectKategor.id_kategory);
                if(kategoria != null)
                {
                    kategoria.Name = kater;
                }
                db.SaveChanges();
                LoadKategory();
                MessageBox.Show("Название категории обновлено!");
            }

            if (selectKategor == null)
            {
                MessageBox.Show("Выберите категорию из списка!");
                return;
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e) // сохранить блюдо
        {
            string dishs = dish.Text;

            if (dishs == null)
            {
                MessageBox.Show("Введите название блюда");
                return;
            }

            Dish dishss = new Dish()
            {
                Name = dishs
            };
            db.Dish.Add(dishss);
            db.SaveChanges();
            LoadDish();
            MessageBox.Show("Блюдо успешно добавлено!");

        }

        private void Button_Click_5(object sender, RoutedEventArgs e) // изменить блюдо
        {
            string disshes = dish.Text;
            var selectDish = listDish.SelectedItem as Dish;

            if (selectDish != null)
            {
                var dihhh = db.Dish.FirstOrDefault(c => c.id_dish == selectDish.id_dish);
                if (dihhh != null)
                {
                    dihhh.Name = disshes;
                }
                db.SaveChanges();
                LoadDish();
                MessageBox.Show("Название блюда обновлено!");
            }

            if (selectDish   == null)
            {
                MessageBox.Show("Выберите блюдо из списка!");
                return;
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e) // удалить блюдо
        {
            var selectedDish = listDish.SelectedItem as Dish;
            if (selectedDish != null)
            {
                var diS  = db.Dish.FirstOrDefault(c => c.id_dish == selectedDish.id_dish);

                if (diS != null)
                {
                    if (MessageBox.Show("Вы уверены, что хотите удалить блюдо?",
                "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    db.Dish.Remove(diS);
                    db.SaveChanges();
                    MessageBox.Show("Блюдо удалено!");
                    LoadDish();
                }
            }

            if (selectedDish == null)
            {
                MessageBox.Show("Выберите категорию для удаления!");
            }
        }
    }
}
