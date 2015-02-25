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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Xml;
using System.Threading;

namespace WpfApplication1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.QuadraticFieldComboBox.ItemsSource = GetFieldInfo("../../Sources/informationAboutField.xml");
            this.QuadraticFieldComboBox.DisplayMemberPath = "Key";
            this.QuadraticFieldComboBox.SelectedValuePath = "Value";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Result.Text = String.Empty;

            List<Int64> factorList;

            FieldInfo fundamentalUnit = (FieldInfo) this.QuadraticFieldComboBox.SelectedValue;
            string classCharacterRingCoef = this.classCharacterRingCoef.Text;

            if (String.IsNullOrEmpty(classCharacterRingCoef) || fundamentalUnit==null)
            {
                MessageBox.Show("Введите информацию о классовом кольце характера и о квадратичном поле!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TryParseFactors(classCharacterRingCoef, out factorList))
            {
                MessageBox.Show("Некорректная информация о квадратичном поле!" + Environment.NewLine + "Используйте ^ и * для возведения в степень и умножения", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var periodList = factorList.AsParallel().Select(item => getPeriod(fundamentalUnit, item));

            var period = UnitInQuadraticFieldSearching.GetGeneralPeriod(periodList);

            this.Result.Text = this.Result.Text + Environment.NewLine  +
                                    String.Format("T({0}) = {1}", classCharacterRingCoef, period) + Environment.NewLine;
        }
        
        private Dictionary<int, FieldInfo> GetFieldInfo(string filePath)
        {
            Dictionary<int, FieldInfo> result = new Dictionary<int, FieldInfo>();

            FileStream fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fStream);
            var nodeList = xmlDoc.DocumentElement.SelectNodes("//Field");

            foreach (XmlNode node in nodeList)
            {
                var a = Int32.Parse(node.SelectSingleNode("a").InnerText);
                var b = Int32.Parse(node.SelectSingleNode("b").InnerText);
                var d = Int32.Parse(node.SelectSingleNode("d").InnerText);

                result.Add(d, new FieldInfo(a, b, d));
            }

            fStream.Dispose();

            return result;
        }

        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0) && e.Text!="^" && e.Text!="*") e.Handled = true;
        }

        private bool TryParseFactors(string value, out List<Int64> result)
        {
            result = new List<Int64>();
            string[] factorList = value.Split('*');

            foreach (var factor in factorList)
            {
                string[] split = factor.Split('^');
                int baseNumber, exponent=1;

                if (split.Count() == 0 || split.Count() > 2 || !Int32.TryParse(split[0], out baseNumber))
                    return false;

                if (split.Count() == 2 && !Int32.TryParse(split[1], out exponent))
                    return false;

                Int64 number = (Int64) Math.Pow(baseNumber, exponent);

                result.Add(number);
            }

            return true;
        }

        private Int64 getPeriod(FieldInfo fieldInfo, Int64 item)
        {
            var result = UnitInQuadraticFieldSearching.GetPeriod(fieldInfo, item);
            this.Result.Dispatcher.BeginInvoke(new ThreadStart(delegate { this.Result.Text = this.Result.Text + Environment.NewLine + String.Format("T({0}) = {1}", item, result); }));
            return result;
        }
    }
}
