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
        private int numCacheLevels;

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
            int wordSize1 = int.Parse(((ComboBoxItem)lstWordSize.SelectedValue).Content.ToString());
            int blockSize1 = int.Parse(((ComboBoxItem)lstBlockSize.SelectedValue).Content.ToString());
            int numBlocks1 = int.Parse(((ComboBoxItem)lstNumBlocks.SelectedValue).Content.ToString());
            int mappingSize1 = int.Parse(((ComboBoxItem)lstMapSize.SelectedValue).Content.ToString());

            String replacementString1 = ((ComboBoxItem)lstReplacementPolicy.SelectedValue).Content.ToString();
            IReplacementPolicy replacementPolicy1 = GetReplacementPolicyFromString(replacementString1);

            if (numCacheLevels > 2)
            {
                String replacementString3 = ((ComboBoxItem)lstReplacementPolicy2.SelectedValue).Content.ToString();
                IReplacementPolicy replacementPolicy3 = GetReplacementPolicyFromString(replacementString3);

                int wordSize3 = int.Parse(((ComboBoxItem)lstWordSize3.SelectedValue).Content.ToString());
                int blockSize3 = int.Parse(((ComboBoxItem)lstBlockSize3.SelectedValue).Content.ToString());
                int numBlocks3 = int.Parse(((ComboBoxItem)lstNumBlocks3.SelectedValue).Content.ToString());
                int mappingSize3 = int.Parse(((ComboBoxItem)lstMapSize3.SelectedValue).Content.ToString());
                l3 = new Cache(wordSize3, blockSize3, numBlocks3, mappingSize3, replacementPolicy3, false, null);
            }
            else
            {
                l3 = null;
            }

            if (numCacheLevels > 1)
            {
                String replacementString2 = ((ComboBoxItem)lstReplacementPolicy2.SelectedValue).Content.ToString();
                IReplacementPolicy replacementPolicy2 = GetReplacementPolicyFromString(replacementString2);

                int wordSize2 = int.Parse(((ComboBoxItem)lstWordSize2.SelectedValue).Content.ToString());
                int blockSize2 = int.Parse(((ComboBoxItem)lstBlockSize2.SelectedValue).Content.ToString());
                int numBlocks2 = int.Parse(((ComboBoxItem)lstNumBlocks2.SelectedValue).Content.ToString());
                int mappingSize2 = int.Parse(((ComboBoxItem)lstMapSize2.SelectedValue).Content.ToString());
                l2 = new Cache(wordSize2, blockSize2, numBlocks2, mappingSize2, replacementPolicy2, false, l3);
            }
            else
            {
                l2 = null;
            }

            l1 = new Cache(wordSize1, blockSize1, numBlocks1, mappingSize1, replacementPolicy1, false, l2);
            Tuple<Cache, StorageFile, int> t = Tuple.Create<Cache, StorageFile, int>(l1, logFile, numCacheLevels);
            Frame.Navigate(typeof(SimulationPage), t);
        }

        private IReplacementPolicy GetReplacementPolicyFromString(String s)
        {
            switch (s)
            {
                case "Random":
                    return new RandomReplacementPolicy();
                case "LFU":
                    return new LFUReplacementPolicy();
                case "LRU":
                    return new LRUReplacementPolicy();
                case "FIFO":
                    return new FIFOReplacementPolicy();
                default:
                    return new RandomReplacementPolicy();
            }
        }

        private async void GenLogFile(object sender, RoutedEventArgs e)
        {
            GenerateLogFileDialog genLogDialog = new GenerateLogFileDialog();
            ContentDialogResult result = await genLogDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                if (genLogDialog.logFile != null)
                {
                    logFile = genLogDialog.logFile;
                    // Application now has read/write access to the picked file
                    txtLogFileName.Text = logFile.Name;
                    btnRun.IsEnabled = true;
                }
            }
        }

        private void lstNumCacheLevels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gridL1 == null || gridL2 == null || gridL3 == null)
            {
                numCacheLevels = 1;
                return;
            }

            numCacheLevels = int.Parse(((ComboBoxItem) lstNumCacheLevels.SelectedValue).Content.ToString());
            if (numCacheLevels == 1)
            {
                gridL2.Visibility = Visibility.Collapsed;
                gridL3.Visibility = Visibility.Collapsed;

            }
            else if (numCacheLevels == 2)
            {
                gridL2.Visibility = Visibility.Visible;
                gridL3.Visibility = Visibility.Collapsed;
            }
            else
            {
                gridL2.Visibility = Visibility.Visible;
                gridL3.Visibility = Visibility.Visible;
            }
        }
    }
}
