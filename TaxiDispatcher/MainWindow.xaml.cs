using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaxiDispatcher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_ShowOrderStatus_Click(object sender, RoutedEventArgs e)
        {
            //OrderTrackForm orderTrackForm = new OrderTrackForm();
            //orderTrackForm.Show();
            int result;
            if (int.TryParse(TextBox_OrderStatus.Text, out result))
            {
                var orderData = DataBaseHelper.GetCurrentDataBase().TryGetOrderDataByValidTrackingNumber(result);
                if (orderData != null)
                {
                    this.Close();
                    var th = new Thread(x => {
                        OrderTrackForm orderTrackForm = new OrderTrackForm();
                        orderTrackForm.Show();
                    });
                    th.SetApartmentState(ApartmentState.STA);
                    th.Start();
                }
                else
                {
                    MessageBox.Show("Неправильный номер");
                }
            }
            else
            {
                MessageBox.Show("Недопустимые символы!");
            }
        }

        private void TextBox_OrderStatus_TextChanged(object sender, TextChangedEventArgs e)
        {
            Button_ShowOrderStatus.IsEnabled = true;
        }
    }
}
