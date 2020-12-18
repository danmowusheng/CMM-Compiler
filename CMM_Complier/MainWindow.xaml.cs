using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WordAnalyzer;
using GrammerParser;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit;

namespace CMM_GUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //string[] keywords = { "if", "else", "while", "read", "write" };//deepblue
        //string[] classkeyword = { "real", "int" };//purple

        //Regex functionRegex = new Regex("\\b[A-Za-z0-9_]+(?=\\()");
        //Regex commentRegex = new Regex("/\\*(.|\n)*\\*/");

        List<Token> b;

        public MainWindow()
        {
            InitializeComponent();
        }

        private static void LoadFile(string filename, TextEditor richTextBox)
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
                string ext = System.IO.Path.GetExtension(filename);
                if (String.Compare(ext, ".cmm", true) == 0)
                {
                    richTextBox.Document.Text = System.IO.File.ReadAllText(filename);
                }
                else
                {
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBox.Show("请选择.cmm格式的文件", "", button, icon);
                }
            }
        }

        private static void SaveFile(string filename, TextEditor richTextBox)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException();
            }
            using (FileStream stream = File.OpenWrite(filename))
            {
                string ext = System.IO.Path.GetExtension(filename);
                if (String.Compare(ext, ".cmm", true) == 0)
                {
                    richTextBox.Save(stream);
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Information;
                    MessageBox.Show("保存成功", "", button, icon);
                }
            }
        }

        private void Button_Click_OpenFile(object sender, RoutedEventArgs e)
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
            LoadFile(filename, this.editorRichTextbox);
            Console.WriteLine("调用结束");
        }

        private void Button_Click_SaveFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CMM|*.cmm";
            if (saveFileDialog.ShowDialog() == true)
            {
                var filename = saveFileDialog.FileName;
                SaveFile(filename, editorRichTextbox);
            }
        }

        private void Button_Click_SaveTemp(object sender, RoutedEventArgs e)
        {
            SaveFile("temp.cmm", editorRichTextbox);
        }

        //public void Find(string l, RichTextBox richBox, string keyword)
        //{
        //    // 设置文字指针为Document初始位置
        //    // richBox.Document.FlowDirection
        //    TextPointer position = richBox.Document.ContentStart;
        //    while (position != null)
        //    {
        //        // 向前搜索,需要内容为Text
        //        if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
        //        {
        //            // 拿出Run的Text
        //            string text = position.GetTextInRun(LogicalDirection.Forward);
        //            // 可能包含多个keyword,做遍历查找
        //            int index = 0;
        //            while (index < text.Length)
        //            {
        //                index = text.IndexOf(keyword, index);
        //                // MessageBox.Show(index.ToString());
        //                if (index == -1)
        //                {
        //                    break;
        //                }
        //                else
        //                {
        //                    // 添加为新的Range
        //                    TextPointer start = position.GetPositionAtOffset(index);
        //                    TextPointer end = start.GetPositionAtOffset(keyword.Length);
        //                    //TextPointer end1 = start.GetPositionAtOffset(index + keyword.Length);
        //                    TextRange range = new TextRange(start, end);
        //                    ChangeColor(l, range);

        //                    index += keyword.Length;
        //                }
        //            }
        //        }
        //        // 文字指针向前偏移
        //        position = position.GetNextContextPosition(LogicalDirection.Forward);
        //    }
        //}

        //private void ChangeColor(string color, TextRange range)
        //{
        //    // 高亮选择
        //    if (color == "DeepSkyBlue")
        //    {
        //        range.ApplyPropertyValue(TextElement.ForegroundProperty,
        //        new SolidColorBrush(Colors.DeepSkyBlue));
        //        range.ApplyPropertyValue(TextElement.FontWeightProperty,
        //        FontWeights.Bold);
        //    }
        //    else if (color == "Purple")
        //    {
        //        range.ApplyPropertyValue(TextElement.ForegroundProperty,
        //        new SolidColorBrush(Colors.Purple));
        //        range.ApplyPropertyValue(TextElement.FontWeightProperty,
        //        FontWeights.Bold);
        //    }
        //    else if (color == "Green")
        //    {
        //        range.ApplyPropertyValue(TextElement.ForegroundProperty,
        //        new SolidColorBrush(Colors.Green));
        //        range.ApplyPropertyValue(TextElement.FontWeightProperty,
        //        FontWeights.Bold);
        //    }
        //    else if (color == "Red")
        //    {
        //        range.ApplyPropertyValue(TextElement.ForegroundProperty,
        //        new SolidColorBrush(Colors.Red));
        //        range.ApplyPropertyValue(TextElement.FontWeightProperty,
        //        FontWeights.Bold);
        //    }

        //    TextPointer position = editorRichTextbox.Document.ContentStart;
        //    TextPointer start = position.GetPositionAtOffset(2);
        //    TextPointer end = editorRichTextbox.Document.ContentEnd;
        //    TextRange range1 = editorRichTextbox.Selection;
        //    range1.Select(end, end);
        //    range1.ApplyPropertyValue(TextElement.ForegroundProperty,
        //           new SolidColorBrush(Colors.Black));
        //    range1.ApplyPropertyValue(TextElement.FontWeightProperty,
        //        FontWeights.Normal);
        //}

        //private void EditorRichTextbox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    foreach (string s in keywords)
        //    {
        //        Find("DeepSkyBlue", editorRichTextbox, s);
        //    }
        //    foreach (string s in classkeyword)
        //    {
        //        Find("Purple", editorRichTextbox, s);
        //    }

        //    TextPointer start = editorRichTextbox.Document.ContentStart;
        //    TextPointer end = editorRichTextbox.Document.ContentEnd;
        //    TextRange all = new TextRange(start, end);
        //    MatchCollection mc = commentRegex.Matches(all.Text);
        //    foreach (Match m in mc)
        //    {
        //        Console.WriteLine(m.Value + "\n");
        //        start = start.GetPositionAtOffset(m.Index);
        //        end = start.GetPositionAtOffset(m.Length);
        //        TextRange range = new TextRange(start, end);
        //        range.ApplyPropertyValue(TextElement.ForegroundProperty,
        //        new SolidColorBrush(Colors.Green));
        //        range.ApplyPropertyValue(TextElement.FontWeightProperty,
        //        FontWeights.Bold);

        //    }

        //}

        private void Button_Click_Word(object sender, RoutedEventArgs e)
        {
            Analyzer a = new Analyzer(@"temp.cmm");
            b = a.toTokenList();
            string output = "";
            foreach (Token s in b)
            {
                output += s.LineNum + " " + s.Type + " " + s.Value + "\n";
            }
            outputTextBlock.Text = output;

        }

        private void Button_Click_Grammar(object sender, RoutedEventArgs e)
        {
            outputTextBlock.Text="";
            Token endToken = new Token();
            endToken.Type = TokenType.END;
            endToken.LineNum = b.FindLast((Token t) => t.LineNum >= 0).LineNum;
            b.Add(endToken);
            string output="";

            foreach (Token s in b)
            {
                output += s.LineNum + " " + s.Type + " " + s.Value+"\n";
            }
            Parser parser = new Parser();
            //获取词法分析产生的token序列
            Parser.tokens = b;
            //读入分析表
            Parser.getAnalysisTable(@"parsing_table_v1.1.csv");
            //读入产生式
            Parser.getProduction(@"production.txt");

            output = Parser.Parse();

            outputTextBlock.Text = output;
        }
    }
}
