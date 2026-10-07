using System;
using System.Collections.Generic;
using System.IO;
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
using Word = Microsoft.Office.Interop.Word;

namespace OnlineCaffe
{
    public partial class report : Window
    {
        CoffeEntities db = new CoffeEntities();
        private int currentID;

        public report(int userID)
        {
            InitializeComponent();
            currentID = userID;
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            DateTime? startDate = dpStart.SelectedDate;
            DateTime? endDate = dpEnd.SelectedDate;

            if (startDate == null || endDate == null)
            {
                MessageBox.Show("Выберите даты!");
                return;
            }

            DateTime start = startDate.Value.Date;
            DateTime end = endDate.Value.Date.AddDays(1).AddSeconds(-1);

            var orders = db.Orders
                .Where(o => o.Registration_Date >= start && o.Registration_Date <= end)
                .OrderByDescending(o => o.Registration_Date)
                .ToList();

            if (orders.Count == 0)
            {
                MessageBox.Show("Заказов не найдено!");
                listOrders.ItemsSource = null;
                return;
            }

            var reportData = orders.Select(o => new
            {
                o.id_order,
                o.Registration_Date,
                o.Cost,
                Status = o.Status_delivery != null ? o.Status_delivery.Name : "Неизвестно",
                Client = o.Client != null ? o.Client.Login : "Неизвестно"
            }).ToList();

            listOrders.ItemsSource = reportData;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var items = listOrders.ItemsSource;
            if (items == null)
            {
                MessageBox.Show("Нет данных для сохранения!");
                return;
            }

            try
            {
                string fileName = $"Отчет_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
                string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

                // Создаем Word приложение
                var wordApp = new Word.Application();
                var doc = wordApp.Documents.Add();
                var selection = wordApp.Selection;

                // Заголовок
                selection.Font.Bold = 1;
                selection.Font.Size = 16;
                selection.TypeText("ОТЧЕТ ПО ЗАКАЗАМ");
                selection.TypeParagraph();
                selection.TypeParagraph();

                // Дата периода
                DateTime? startDate = dpStart.SelectedDate;
                DateTime? endDate = dpEnd.SelectedDate;
                selection.Font.Size = 12;
                selection.Font.Bold = 0;
                selection.TypeText($"Период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");
                selection.TypeParagraph();
                selection.TypeParagraph();

                // Таблица
                var reportData = (System.Collections.IEnumerable)items;
                var dataList = reportData.Cast<object>().ToList();

                if (dataList.Count > 0)
                {
                    // Создаем таблицу
                    var table = doc.Tables.Add(selection.Range, dataList.Count + 1, 6);
                    table.Borders.Enable = 1;
                    table.Rows.Height = 20;

                    // Заголовки таблицы
                    table.Cell(1, 1).Range.Text = "№ заказа";
                    table.Cell(1, 2).Range.Text = "Дата";
                    table.Cell(1, 3).Range.Text = "Сумма";
                    table.Cell(1, 5).Range.Text = "Статус";
                    table.Cell(1, 6).Range.Text = "Клиент";

                    // Жирный шрифт для заголовков
                    for (int i = 1; i <= 6; i++)
                    {
                        table.Cell(1, i).Range.Font.Bold = 1;
                    }

                    // Заполняем данные
                    int row = 2;
                    decimal totalSum = 0;
                    int totalCount = 0;

                    foreach (var item in dataList)
                    {
                        var props = item.GetType().GetProperties();

                        var id_order = props.First(p => p.Name == "id_order").GetValue(item);
                        var date = props.First(p => p.Name == "Registration_Date").GetValue(item);
                        var cost = props.First(p => p.Name == "Cost").GetValue(item);
                        var status = props.First(p => p.Name == "Status").GetValue(item);
                        var client = props.First(p => p.Name == "Client").GetValue(item);

                        table.Cell(row, 1).Range.Text = id_order.ToString();
                        table.Cell(row, 2).Range.Text = ((DateTime)date).ToString("dd.MM.yyyy HH:mm");
                        table.Cell(row, 3).Range.Text = $"{Convert.ToDecimal(cost):F2} ₽";
                        table.Cell(row, 5).Range.Text = status?.ToString() ?? "";
                        table.Cell(row, 6).Range.Text = client?.ToString() ?? "";

                        totalSum += Convert.ToDecimal(cost);
                        totalCount++;
                        row++;
                    }

                    selection.TypeParagraph();

                    // Итоги
                    selection.Font.Bold = 1;
                    selection.TypeText("ИТОГО:");
                    selection.TypeParagraph();
                    selection.Font.Bold = 0;
                    selection.TypeText($"Количество заказов: {totalCount}");
                    selection.TypeParagraph();
                    selection.TypeText($"Общая сумма: {totalSum:F2} ₽");
                }

                // Сохраняем и закрываем
                doc.SaveAs2(filePath);
                doc.Close();
                wordApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);

                MessageBox.Show($"Отчет сохранен на рабочий стол:\n{fileName}", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}\n\nУбедитесь, что установлен Microsoft Word");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager manager = new Manager(currentID);
            manager.Show();
            this.Close();
        }
    }
}