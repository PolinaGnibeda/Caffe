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
using System.Text.RegularExpressions;

namespace OnlineCaffe
{
    /// <summary>
    /// Логика взаимодействия для Regestrazia.xaml
    /// </summary>
    public partial class Regestrazia : Window
    {
        CoffeEntities db = new CoffeEntities();
        public Regestrazia()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string Name = name.Text;
            string Lastname = lastName.Text;
            string MiddleName = middleName.Text;
            string Password = password.Text;
            string Login = login.Text;
            string Email = email.Text;
            string Phone = phone.Text;

            if (string.IsNullOrEmpty(Name) || (string.IsNullOrEmpty(Lastname) || (string.IsNullOrEmpty(MiddleName) ||
                (string.IsNullOrEmpty(Password) || (string.IsNullOrEmpty(Phone) || (string.IsNullOrEmpty(Email) ||
                (string.IsNullOrEmpty(Login))))))))
            {
                MessageBox.Show("Не все поля заполнены! Проверьте данные еще раз");
                return;
            }
            string check = @"^(\+7|8)\d{10}$";
            
            if(!Regex.IsMatch(Phone, check))
            {
                MessageBox.Show("Некорректный номер телефона");
                return;
            }

            string check2 = @"^[A-Za-zА-Яа-я]*\d+@.*$";

            if(!Regex.IsMatch(Email, check2))     
            {
                MessageBox.Show("Некорректный email. Пример: letter5@mail");
                return;
            }

            string check3 = @"^[a-zA-Z0-9._]{3,20}$";

            if(!Regex.IsMatch(Login, check3))
            {
                MessageBox.Show("Некорректный логин. Пример: user123");
                return;
            }

            Client client = new Client()
            {
                Name = Name,
                LastName = Lastname,
                MiddleName = MiddleName,
                Password = Password,
                Login = Login,
                Email = Email,
                Phone = Phone,
                RegistrationDate = DateTime.Now,
                id_status =  1,
                id_rol = 1
            };
            db.Client.Add(client);
            db.SaveChanges();

            MessageBox.Show($"Пользователь {Name} {Lastname} {MiddleName} успешно добавлен!");
        }
    }
}
