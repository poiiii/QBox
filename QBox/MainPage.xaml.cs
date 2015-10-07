//进度条君未上线
//仅支持下载= =
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Storage;
using System.Threading.Tasks;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace QBox
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            textBlock4.Text = "History\n";
        }

        public StorageFile newfile { get; private set; }
        //uri
        public static string downcommond = "https://box.zjuqsc.com/item/get/";
        public static string downcheck = "https://api.zjuqsc.com/box/get_api/";

        public static char exchange(string str)
        {
            //Change into Chinese
            return (char)int.Parse(str, System.Globalization.NumberStyles.HexNumber);
        }

        private async void Click_Here(object sender, RoutedEventArgs e)
        {
            //BoxClient_api
            var token = textBox.Text;
            var boxclient = new HttpClient();
            var response = await boxclient.GetAsync(new Uri(downcheck + token));
            var content = response.Content;
            //Judge Err
            var info = content.ToString();
            int errpos = info.IndexOf("err");
            var err = info.Substring(errpos + 5, 1);
            switch (err)
            {
                case "2":
                    {
                        textBlock1.Text = "提取码不存在";
                        break;
                    }
                case "1":
                    {
                        if (token.Length <= 15) textBlock1.Text = "提取码格式错误。必须仅包含字母、数字和下划线。";
                        else textBlock1.Text = "提取码过长，最多15个字符。";
                        break;
                    }
                case "0":
                    {
                        //get filename
                        int filepos = info.IndexOf("filename");
                        var filename = info.Substring(filepos + 11);
                        int end = filename.IndexOf(",");
                        filename = filename.Substring(0, end - 1);
                        var extraname = filename.Substring(filename.LastIndexOf('.'));
                        var headname = filename.Substring(0, filename.LastIndexOf('.'));

                        //Change Filename
                        int pos;
                        var tmpstr = headname;
                        headname = "";
                        while (tmpstr.IndexOf('\\') != -1)
                        {
                            pos = tmpstr.IndexOf('\\');
                            headname += tmpstr.Substring(0, pos) + exchange(tmpstr.Substring(pos + 2, 4));
                            tmpstr = tmpstr.Substring(pos + 6);
                        }
                        headname += tmpstr;
                        filename = headname + extraname;

                        //BoxClient_getfile
                        textBlock1.Text = "Your Downloading File Name is " + filename + "\nPlease Waiting...";
                        response = await boxclient.GetAsync(new Uri(downcommond + token));
                        content = response.Content;
                        var filebuffer = await content.ReadAsBufferAsync();
                        
                        //Save File
                        textBlock1.Text = "Your File is Downloaded\nChoose a Path to Save It";
                        var savefile = new Windows.Storage.Pickers.FileSavePicker();
                        savefile.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                        savefile.FileTypeChoices.Add(new KeyValuePair<string, IList<string>>("file", new List<string> { extraname }));
                        savefile.SuggestedFileName = filename;
                        newfile = await savefile.PickSaveFileAsync();
                        if (newfile == null)
                        {
                            textBlock1.Text = "You Did Not Save the File";
                            textBlock4.Text += DateTime.Now.ToString() + " : Download " + filename + "   Unsaved\n";
                            break;
                        }
                        var writefile = await newfile.OpenTransactedWriteAsync();
                        await writefile.Stream.WriteAsync(filebuffer);
                        await writefile.CommitAsync();

                        //Fin.
                        textBlock1.Text = "Your File is Saved";
                        textBlock4.Text += DateTime.Now.ToString() + " : Download " + filename + "   Saved\n";
                        break;
                    }
            }
            boxclient.Dispose();
        }
    }
}
