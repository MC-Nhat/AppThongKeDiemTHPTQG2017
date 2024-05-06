using Microsoft.VisualBasic.FileIO;
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

namespace AppThongKeDiemTHPTQG2017
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class StudentData()
        {
            public string studentId;
            public string province;
            public double mathemathematics;
            public string provincecode;
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void DisplayResult(Dictionary<string, double> result)
        {
            var sortedResult = result.OrderByDescending(r => r.Value);

            ListViewScore.ItemsSource = sortedResult.Select(r => new { provincecode = r.Id.Substring(0,2) ,Key = r.Key, Value = r.Value });
        }

        public List<StudentData> ReadCsvFile(string filePath)
        {
            List<StudentData> studentDataList = new List<StudentData>();

            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.ReadLine();
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    if (fields != null)
                    {
                        string provinceCode = fields[1].Length >= 2 ? fields[1].Substring(0, 2) : fields[1];

                        if (fields.Length > 2 && (fields[2] == null || fields[2] == ""))
                        {
                            fields[2] = "0";
                        }
                        string studentId = fields[0];
                        string provincecode = studentId.Length >= 2 ? studentId.Substring(0, 2) : studentId;
                        // Assuming you have a StudentData class to store the data
                        StudentData student = new StudentData
                        {
                            studentId = fields[0],
                            province = fields[1],
                            provincecode = provinceCode,
                            mathemathematics = Convert.ToDouble(fields[2])
                        };

                        studentDataList.Add(student);
                    }
                }
            }

            return studentDataList;
        }

        public Dictionary<string, double> CalculateAverageMathByProvince(List<StudentData> studentDataList)
        {

            var averageMathByProvince = studentDataList
                .GroupBy(s => s.province)
                .ToDictionary(g => g.Key, g => g.Average(s => s.mathemathematics));

            return averageMathByProvince;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                string filePath = ofd.FileName;
                txtBrowse.Text = filePath;

                List<StudentData> studentDataList = ReadCsvFile(filePath);
                Dictionary<string, double> averageMathByProvince = CalculateAverageMathByProvince(studentDataList);
                DisplayResult(averageMathByProvince);
            }
        }
        public Dictionary<string, double> CalculateAverageMath1ByProvince(List<StudentData> studentDataList, string searchProvince)
        {
            var filteredData = studentDataList.Where(s => string.Equals(s.province, searchProvince, StringComparison.OrdinalIgnoreCase));

            var averageMathByProvince = filteredData
                .GroupBy(s => s.province)
                .ToDictionary(g => g.Key, g => g.Average(s => s.mathemathematics));

            return averageMathByProvince;
        }
        private void btnBrowse_Search(object sender, RoutedEventArgs e)
        {
            string searchProvince = txtSearchProvince.Text.Trim();

            if (!string.IsNullOrEmpty(searchProvince))
            {
                List<StudentData> studentDataList = ReadCsvFile(txtBrowse.Text);
                Dictionary<string, double> averageMathByProvince = CalculateAverageMath1ByProvince(studentDataList, searchProvince);
                DisplayResult(averageMathByProvince);
            }
            else
            {
                
            }
        }
    }
}
