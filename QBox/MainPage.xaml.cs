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
        }

        public StorageFile newfile { get; private set; }

        private async void Click_Here(object sender, RoutedEventArgs e)
        {
            var downcommond = "https://box.zjuqsc.com/item/get/";
            var token = textBox.Text;
            var boxclient = new HttpClient();
            var response = await boxclient.GetAsync(new Uri(downcommond + token));
            var content = response.Content;
            var filename = content.Headers.ContentDisposition.FileName;
            var filebuffer = await content.ReadAsBufferAsync();
            int dotpos = filename.LastIndexOf('.');
            var extraname = filename.Substring(dotpos);
            var savepicker = new Windows.Storage.Pickers.FileSavePicker();
            savepicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            savepicker.FileTypeChoices.Add(new KeyValuePair<string, IList<string>>("file", new List<string> { extraname }));
            savepicker.SuggestedFileName = filename;
            newfile = await savepicker.PickSaveFileAsync();
            var writestream = await newfile.OpenTransactedWriteAsync();
            await writestream.Stream.WriteAsync(filebuffer);
            await writestream.CommitAsync();
            textBlock1.Text = "Your Download File Name is " + filename;

        }
    }
}
