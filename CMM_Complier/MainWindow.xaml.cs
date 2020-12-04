using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CMM_Complier
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Name = "x";
            Value = "1";
            Language = "CMM";
            Class = "Int";
            Name_1 = ">CMM.dll!CMM.Program.main() 行2";
        }

        private void OpenFile_Clicked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("调用开始");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Multiselect = true;
            //过滤文件
            openFileDialog.Filter = "CMM|*.cmm";
            if (!(bool)openFileDialog.ShowDialog())
            {
                return;
            }
            var filename = openFileDialog.FileName;
            //加载文件
            LoadFile(filename, editorRichTextbox);
            Console.WriteLine("调用结束");

        }
        private void SaveFile_Clicked(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CMM|*.cmm";       
            if (saveFileDialog.ShowDialog() == true)
            {
                var filename = saveFileDialog.FileName;
                SaveFile(filename, editorRichTextbox);
            }
        }

        private static void LoadFile(string filename, RichTextBox richTextBox)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException();
            }
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException();
            }
            using (FileStream stream = File.OpenRead(filename))
            {
                TextRange documentTextRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                string dataFormat = DataFormats.Text;
                string ext = System.IO.Path.GetExtension(filename);
                if (String.Compare(ext, ".cmm", true) == 0)
                {
                    dataFormat = DataFormats.Text;
                }
                documentTextRange.Load(stream, dataFormat);
            }

        }

        //保存文件
        private static void SaveFile(string filename, RichTextBox richTextBox)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException();
            }
            using (FileStream stream = File.OpenWrite(filename))
            {
                TextRange documentTextRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                string dataFormat = DataFormats.Text;
                string ext = System.IO.Path.GetExtension(filename);
                if (String.Compare(ext, ".cmm", true) == 0)
                {
                    dataFormat = DataFormats.Text;
                }
                documentTextRange.Save(stream, dataFormat);
            }
        }

        private void editorRichTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        public String Value { get; set; }
        public String Name { get; set; }
        public String Class { get; set; }
        public String Name_1 { get; set; }
        public String Language { get; set; }
    }
}
