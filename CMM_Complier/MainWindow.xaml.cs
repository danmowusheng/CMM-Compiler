using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CMM_Complier
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] keywords = { "if", "else", "while", "read", "write" };//deepBlue
        string[] classKeyword = { "real", "int" };//purple
        public MainWindow()
        {
            InitializeComponent();
            
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

        public void Find(string l, RichTextBox richBox, string keyword)
        {
            // 设置文字指针为Document初始位置
            // richBox.Document.FlowDirection
            TextPointer position = richBox.Document.ContentStart;
            while (position != null)
            {
                // 向前搜索,需要内容为Text
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    // 拿出Run的Text
                    string text = position.GetTextInRun(LogicalDirection.Forward);
                    // 可能包含多个keyword,做遍历查找
                    int index = 0;
                    while (index < text.Length)
                    {
                        index = text.IndexOf(keyword, index);
                        // MessageBox.Show(index.ToString());
                        if (index == -1)
                        {
                            break;
                        }
                        else
                        {
                            // 添加为新的Range
                            TextPointer start = position.GetPositionAtOffset(index);
                            TextPointer end = start.GetPositionAtOffset(keyword.Length);
                            TextPointer end1 = start.GetPositionAtOffset(index + keyword.Length);
                            ChangeColor(l, richBox, keyword.Length, start, end);

                            index += keyword.Length;
                        }
                    }
                }
                // 文字指针向前偏移
                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }
        }

        //改变颜色
        public void ChangeColor(string color, RichTextBox richTextBox1, int selectLength, TextPointer tpStart, TextPointer tpOffset)
        {
            TextRange range = richTextBox1.Selection;
            range.Select(tpStart, tpOffset);

            // 高亮选择
            if (color == "DeepSkyBlue")
            {
                range.ApplyPropertyValue(TextElement.ForegroundProperty,
                new SolidColorBrush(Colors.DeepSkyBlue));
                range.ApplyPropertyValue(TextElement.FontWeightProperty,
                FontWeights.Bold);
            }
            else if(color == "Purple")
            {
                range.ApplyPropertyValue(TextElement.ForegroundProperty,
                new SolidColorBrush(Colors.Purple));
                range.ApplyPropertyValue(TextElement.FontWeightProperty,
                FontWeights.Bold);
            }
            else if(color == "")
            {

            }
            TextPointer position = richTextBox1.Document.ContentStart;
            TextPointer start = position.GetPositionAtOffset(2);
            TextPointer end = richTextBox1.Document.ContentEnd;
            TextRange range1 = richTextBox1.Selection;
            range1.Select(end, end);
            range1.ApplyPropertyValue(TextElement.ForegroundProperty,
            new SolidColorBrush(Colors.Black));
        }

        private void editorRichTextbox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            foreach (string s in keywords)
            {
                Find("DeepSkyBlue", editorRichTextbox, s); // tx是RichTextBox控件名字
            }
            foreach (string s in classKeyword)
            {
                Find("Purple", editorRichTextbox, s); // tx是RichTextBox控件名字
            }
            
        }
    }
}
