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
            Coder = new LinearCoder();

        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the ComboBox instance
            ComboBox comboBox = sender as ComboBox;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button Button = sender as Button;
            if (MessageTextBox.Text.Length < 2 || ComboBox.SelectedValue == null)
            {
                var ErrorDialog = new MessageDialog("Message must be in a binary format, have at least 2 symbols and coding algorithm must be selected.\nMake sure all these conditions are fulfilled.");
                ErrorDialog.ShowAsync();
                return;
            }
            if(!Coder.Code(ComboBox.SelectedValue.ToString(), MessageTextBox.Text))
            {
                var ErrorDialog = new MessageDialog("Message must be in binary format, meaning that only '0' and '1' are allowed in the message.");
                ErrorDialog.ShowAsync();
                return;
            }
            PlotView.Model = Coder.model;
        }
    }
}