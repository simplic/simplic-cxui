using Simplic.DynamicUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xaml;

namespace SXUI.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            codeTextBox.Text = @"<UserControl x:Class=""Testing.TestUserControl""
             xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
             xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""> 
     <Grid>  
         <TextBox Name=""hello"" />    
        </Grid>
    </UserControl>";
        }

        private void generateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                contentGrid.Children.Clear();

                SXUIBuilder builder = new SXUIBuilder();
                builder.AddXamlSource(new XamlSource() { Name = "TestUserControl", XamlCode = codeTextBox.Text });
                builder.RootNamespace = "Testing";

                // Set references
                builder.References = new[]
                {
                typeof(object).Assembly,
                typeof(Enumerable).Assembly,
                typeof(Uri).Assembly,
                typeof(System.Xml.XmlAttribute).Assembly,
                typeof(XamlLanguage).Assembly,
                typeof(System.Windows.Point).Assembly,
                typeof(System.Windows.Application).Assembly,
                typeof(ApplicationGesture).Assembly,
                typeof(System.Xaml.XamlSchemaContext).Assembly
            };

                builder.Build();

                Type generatedType = null;

                foreach (var type in builder.GeneratedTypes)
                {
                    if (type.Namespace == "Testing" && type.Name == "TestUserControl")
                    {
                        generatedType = type;
                        break;
                    }
                }

                if (generatedType != null)
                {
                    var userControl = (UserControl)Activator.CreateInstance(generatedType);
                    contentGrid.Children.Add(userControl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
