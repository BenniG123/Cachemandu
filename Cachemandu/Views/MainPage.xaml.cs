using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Cachemandu.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Cachemandu.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StorageFile logFile;
        Cache c;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
               AppViewBackButtonVisibility.Collapsed;

            if (logFile == null)
            {
                btnRun.IsEnabled = false;
            }
            else
            {
                btnRun.IsEnabled = true;
            }

            GC.Collect();
        }

        private async void PickLogFile(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".txt");

            logFile = await openPicker.PickSingleFileAsync();
            if (logFile != null)
            {
                // Application now has read/write access to the picked file
                txtLogFileName.Text = logFile.Name;
                btnRun.IsEnabled = true;
            }
        }

        private void RunSimulation(object sender, RoutedEventArgs e)
        { 
            int wordSize = int.Parse(((ComboBoxItem)lstWordSize.SelectedValue).Content.ToString());
            int blockSize = int.Parse(((ComboBoxItem)lstBlockSize.SelectedValue).Content.ToString());
            int numBlocks = int.Parse(((ComboBoxItem)lstNumBlocks.SelectedValue).Content.ToString());
            int mappingSize = int.Parse(((ComboBoxItem)lstMapSize.SelectedValue).Content.ToString());

            // TODO - Parse other options

            c = new Cache(wordSize, blockSize, numBlocks, mappingSize, new RandomReplacementPolicy(), false);
            Tuple<Cache, StorageFile> t = Tuple.Create<Cache, StorageFile>(c, logFile);
            Frame.Navigate(typeof(SimulationPage), t);
        }
    }
}
