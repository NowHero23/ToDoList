using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;
using ToDoListWpf.Data.Classes;
using System.IO;
using Newtonsoft.Json;

namespace ToDoListWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<UserTask> Tasks { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();


            //BackEnd
            
          



            //test
            Tasks = new List<UserTask>(); 
            Tasks.Add(new UserTask()
            {
                Id = 1,
                Title = "First",
                Description = "SomeText",

                NeedTime = DateTime.Now,
                IsActual = true,
                Created = DateTime.Now,

                Updated = DateTime.Now,

            });


            
            
            //timer
            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();


            DataContext = this; //context for binding
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Title = $"ToDo App {DateTime.Now.ToString("HH:mm:ss")}";           
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Description Of Message", "Title Of Message", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                //Save new data


            }
            else
            {
                //Cancel changing
                

            }
        }

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            PropertyDescriptor propertyDescriptor = (PropertyDescriptor)e.PropertyDescriptor;
            e.Column.Header = propertyDescriptor.DisplayName;
            e.Column.Width = DataGridLength.Auto;
            if (propertyDescriptor.DisplayName == "Id")
            {
                e.Column.IsReadOnly = true;
            }
            else if (propertyDescriptor.DisplayName == "Title")
            {
                
            }
            else if (propertyDescriptor.DisplayName == "Description")
            {

            }
            else if (propertyDescriptor.DisplayName == "IsActual")
            {
                e.Column.Header = "Is Actual";
            }
            else if (propertyDescriptor.DisplayName == "NeedTime")
            {
                e.Column.Header = "Need Time";
            }
            else if (propertyDescriptor.DisplayName == "Created")
            {
                e.Column.IsReadOnly = true;
            }
            else if (propertyDescriptor.DisplayName == "Updated")
            {
                e.Column.IsReadOnly = true;
                e.Column.Header = "Last Update";
            }

            if(IsTypeOrNullableOfType(e.PropertyType, typeof(DateTime)))
            {
                DataGridTemplateColumn col = new DataGridTemplateColumn();
                col.Header = e.Column.Header;
                FrameworkElementFactory datePickerFactoryElem = new FrameworkElementFactory(typeof(Xceed.Wpf.Toolkit.DateTimePicker));
                Binding dateBind = new Binding(e.PropertyName);
                dateBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                dateBind.Mode = BindingMode.TwoWay;

                datePickerFactoryElem.SetValue(Xceed.Wpf.Toolkit.DateTimePicker.ValueProperty, dateBind);
                datePickerFactoryElem.SetValue(Xceed.Wpf.Toolkit.DateTimePicker.FormatProperty, Xceed.Wpf.Toolkit.DateTimeFormat.Custom);
                datePickerFactoryElem.SetValue(Xceed.Wpf.Toolkit.DateTimePicker.FormatStringProperty, "MM/dd/yyyy HH:mm");

                DataTemplate cellTemplate = new DataTemplate();
                cellTemplate.VisualTree = datePickerFactoryElem;
                col.CellTemplate = cellTemplate;
                

                e.Column = col;
            }
       
    }
        private static bool IsTypeOrNullableOfType(Type propertyType, Type desiredType)
        {
            return (propertyType == desiredType || Nullable.GetUnderlyingType(propertyType) == desiredType);
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            //Read json
            string fileName = "название файла.json";
            string jsonString = File.ReadAllText(fileName);
           UserTask usertask = JsonConvert.DeserializeObject<UserTask>(jsonString)!;
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            //Wrote json
            string jsonString2 = JsonSerializer.Serialize(Tasks);
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(@"D:\myJson.json", json);
        }
    }
}
