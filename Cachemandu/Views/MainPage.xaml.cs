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
        Cache l1;
        Cache l2;
        Cache l3;

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
            IReplacementPolicy replacementPolicy;
            String replacementString = ((ComboBoxItem)lstReplacementPolicy.SelectedValue).Content.ToString();
            switch (replacementString)
            {
                case "Random":
                    replacementPolicy = new RandomReplacementPolicy();
                    break;
                case "LFU":
                    replacementPolicy = new LFUReplacementPolicy();
                    break;
                case "LRU":
                    replacementPolicy = new LRUReplacementPolicy();
                    break;
                case "FIFO":
                    replacementPolicy = new FIFOReplacementPolicy();
                    break;
                default:
                    replacementPolicy = new RandomReplacementPolicy();
                    break;
            }

            l3 = new Cache(wordSize, blockSize, numBlocks, mappingSize, replacementPolicy, false, null);
            l2 = new Cache(wordSize, blockSize, numBlocks, mappingSize, replacementPolicy, false, l3);
            l1 = new Cache(wordSize, blockSize, numBlocks, mappingSize, replacementPolicy, false, l2);
            Tuple<Cache, StorageFile> t = Tuple.Create<Cache, StorageFile>(l1, logFile);
            Frame.Navigate(typeof(SimulationPage), t);
        }
    }
}
