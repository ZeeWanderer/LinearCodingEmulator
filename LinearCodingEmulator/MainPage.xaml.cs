using OxyPlot;
using System;
using System.Collections.Generic;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LinearCodingEmulator
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            this.MyModel = new PlotModel { Title = "" };
        }

        public PlotModel MyModel { get; private set; }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public class PointData
        {
            public int Index { get; set; }
            public double Amount { get; set; }
        }

        private LinearCoder Coder;

        public MainPage()
        {
            this.InitializeComponent();
            List<String> LinearCodingAlgorythms = new List<String> { "NRZ Uni", "NRZ Bi", "AMI", "B3Z", "B6Z", "HDB3" };
            ComboBox.ItemsSource = LinearCodingAlgorythms;
            ComboBox.SelectedIndex = 0;
            Coder = new LinearCoder();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageTextBox.Text.Length < 2 || ComboBox.SelectedValue == null)
            {
                var ErrorDialog = new MessageDialog("Message must be in binary format and have at least 2 symbols.");
                ErrorDialog.ShowAsync();
                return;
            }
            if (!Coder.Code(ComboBox.SelectedValue.ToString(), MessageTextBox.Text))
            {
                var ErrorDialog = new MessageDialog("Message must be in binary format, meaning that only '0' and '1' are allowed in the message.");
                ErrorDialog.ShowAsync();
                return;
            }
            PlotView.Model = Coder.model;
        }

        private void MessageTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Button_Click(sender, e);
            }
        }
    }
}