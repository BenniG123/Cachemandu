using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Cachemandu.Views
{
    public sealed partial class GenerateLogFileDialog : ContentDialog
    {
        private StorageFile executable;
        public StorageFile logFile;

        public GenerateLogFileDialog()
        {
            this.InitializeComponent();
            IsPrimaryButtonEnabled = false;
        }

        private void Generate(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string strCmdText = "sde -odebugtrace \"" + logFile.Path.ToString() + "\" -dt_syscall 0 -dt_instruction 0 -- \"" + executable.Path.ToString() + "\"";
            var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
            dataPackage.SetText(strCmdText);
            Clipboard.SetContent(dataPackage);
        }

        private async void PickExe(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".exe");

            executable = await openPicker.PickSingleFileAsync();
            if (executable != null)
            {
                // Application now has read/write access to the picked file
                txtExeName.Text = executable.Name;
                IsPrimaryButtonEnabled = logFile != null;
            }
        }

        private async void PickLogFile(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.SuggestedFileName = "log.txt";
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });

            logFile = await savePicker.PickSaveFileAsync();
            if (logFile != null)
            {
                // Application now has read/write access to the picked file
                txtLogFileName.Text = logFile.Name;
                IsPrimaryButtonEnabled = executable != null;
            }
        }

        private void Cancel(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }
    }
}
