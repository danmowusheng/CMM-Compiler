using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using ICSharpCode.AvalonEdit.CodeCompletion;
using CMM_Complier.Word;
using CMM_Complier.Grammar;
using Newtonsoft.Json;


namespace CMM_Complier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] keywords = { "if", "else", "while", "read", "write" };//deepBlue
        string[] classKeyword = { "real", "int" };//purple
        string filePath = "";
        List<Token> b;
        Analyzer analyzer;
        Parser parser;
        CompletionWindow _completionWindow;
        public MainWindow()
        {
            InitializeComponent();
            TextEditor.TextArea.TextEntered += TextAreaOnTextEntered;

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
            this.filePath = openFileDialog.FileName;
            //MessageBox.Show(this.filePath);
            //加载文件
            Load(this.filePath, TextEditor);

        }

        //另存
        private void SaveFile_Clicked(object sender, RoutedEventArgs e)
        {
            Save();
        }
        //保存
        private void Save_Clicked(object sender, RoutedEventArgs e)
        {
            if (filePath == "")
            {
                Save();
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(TextEditor.Text);
                }
                MessageBox.Show("保存成功！");
            }
        }

        private void Create_Clicked(object sender, RoutedEventArgs e)
        {
            this.filePath = "";
            TextEditor.Text = "";
        }

        private static void Load(string filePath, ICSharpCode.AvalonEdit.TextEditor textEditor)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException();
            }
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }
            using (FileStream stream = File.OpenRead(filePath))
            {
                string dataFormat = textEditor.Text;
                string ext = System.IO.Path.GetExtension(filePath);
                if (String.Compare(ext, ".cmm", true) == 0)
                {
                    dataFormat = DataFormats.Text;
                    //StreamReader str = new StreamReader(stream, Encoding.GetEncoding("gb2312"));
                    //Encoding.GetEncoding("gb2312")防止读取文字时出现乱码
                    textEditor.Text = System.IO.File.ReadAllText(filePath);
                }
            }
        }

        //保存
        private void Save()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CMM|*.cmm";
            if (saveFileDialog.ShowDialog() == true)
            {
                var Path = saveFileDialog.FileName;
                this.filePath = saveFileDialog.InitialDirectory + saveFileDialog.FileName;
                using (StreamWriter writer = new StreamWriter(this.filePath))
                {
                    writer.Write(TextEditor.Text);
                }
                MessageBox.Show("保存成功！");
            }
        }


        private void Analy_Clicked(object sender, RoutedEventArgs e)
        {
            if (filePath == "")
            {
                MessageBox.Show("请先保存文件再尝试");
            }
            else
            {
                analyzer = new Analyzer(filePath);
                analyzer.ListToFile();
                List<Token> b = analyzer.toTokenList();
                Token endToken = new Token();
                endToken.Type = Word.TokenType.END;
                endToken.LineNum = b.FindLast((Token t) => t.LineNum >= 0).LineNum;
                b.Add(endToken);

                //parser = new Parser(filePath);
                ////获取词法分析产生的token序列
                //Parser.tokens = b;
                ////读入分析表
                //Parser.getAnalysisTable(@"parsing_table_v1.1.csv");
                ////读入产生式
                //Parser.getProduction(@"production.txt");

                //bool fig = Parser.Parse();
            }

        }


        private void TextAreaOnTextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "w")
            {
                _completionWindow = new CompletionWindow(TextEditor.TextArea);

                var completionData = _completionWindow.CompletionList.CompletionData;
                completionData.Add(new CompletionData("write"));
                completionData.Add(new CompletionData("while"));
                _completionWindow.Show();

                _completionWindow.Closed += (o, args) => _completionWindow = null;
            }
            if (e.Text == "r")
            {
                _completionWindow = new CompletionWindow(TextEditor.TextArea);

                var completionData = _completionWindow.CompletionList.CompletionData;
                completionData.Add(new CompletionData("real"));
                completionData.Add(new CompletionData("read"));
                _completionWindow.Show();

                _completionWindow.Closed += (o, args) => _completionWindow = null;
            }
            if (e.Text == "i")
            {
                _completionWindow = new CompletionWindow(TextEditor.TextArea);

                var completionData = _completionWindow.CompletionList.CompletionData;
                completionData.Add(new CompletionData("int"));
                completionData.Add(new CompletionData("if"));
                _completionWindow.Show();

                _completionWindow.Closed += (o, args) => _completionWindow = null;
            }
            if (e.Text == "e")
            {
                _completionWindow = new CompletionWindow(TextEditor.TextArea);

                var completionData = _completionWindow.CompletionList.CompletionData;
                completionData.Add(new CompletionData("else"));
                _completionWindow.Show();

                _completionWindow.Closed += (o, args) => _completionWindow = null;
            }
        }

        private void GrammarAnaly_Clicked(object sender, RoutedEventArgs e)
        {
            //string output = "";  
            parser = new Parser(filePath.Replace("cmm","list"));
            Node s = parser.Analyze();           
            parser.get(s);

            if (parser.errowStr != null)
            {
                outputTextBlock.Text = parser.errowStr;
            }
            //else
            //{
            //    outputTextBlock.Text = "语法分析判断正确";
            //}
            
        }

        private void WordAnaly_Clicked(object sender, RoutedEventArgs e)
        {
            if (filePath == "")
            {
                MessageBox.Show("请先保存文件再尝试");
            }
            else
            {
                try
                {
                    analyzer = new Analyzer(filePath);
                    b = analyzer.toTokenList();
                    analyzer.ListToFile();
                    string output = "";
                    foreach (Token s in b)
                    {
                        output += s.LineNum + " " + s.Type + " " + s.Value + "\n";
                    }
                    outputTextBlock.Text = output;
                }
                catch (Exception exception)
                {
                    outputTextBlock.Text ="词法分析错误，请勿输入非法符号";
                }
                
            }
        }
    

        private void GenerateGrammarTree_Clicked(object sender, RoutedEventArgs e)
        {
            if (filePath == "")
            {
                MessageBox.Show("请先保存文件再尝试");
            }
            else
            {
                try
                {
                    string path = filePath.Replace("cmm", "list");
                    parser = new Parser(path);

                    Node s = parser.Analyze();

                    JsonNode root = JsonNode.changeNodeFormat(s);

                    string jsonData = JsonConvert.SerializeObject(root);
                    jsonData = jsonData.Replace("null", "[]");

                    string syntaxTreeHtml = File.ReadAllText("SyntaxTree.html");
                    string contentToInsert = "var data = " + jsonData + ";";
                    int start = syntaxTreeHtml.IndexOf("myChart.showLoading();");
                    int end = syntaxTreeHtml.IndexOf("myChart.hideLoading();");
                    string head = syntaxTreeHtml.Substring(0, start + 22);
                    string tail = syntaxTreeHtml.Substring(end);
                    string htmlToUpdate = head + contentToInsert + tail;
                    File.WriteAllText("SyntaxTree.html", htmlToUpdate);
                    SyntaxTree syntaxTree = new SyntaxTree();
                    string SyntaxTreeHtmlAddress = System.Environment.CurrentDirectory + @"\SyntaxTree.html";
                    syntaxTree.SyntaxTreeViewer.Address = SyntaxTreeHtmlAddress;
                    syntaxTree.Show();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                    outputTextBlock.Text = "生成语法树失败";
                }

            }
        }
    }
}
