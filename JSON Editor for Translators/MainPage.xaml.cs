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
using Windows.UI.Popups;
using JSON_Editor_for_Translators.JEFT;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace JSON_Editor_for_Translators
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        TextViewModel viewModel;
        Windows.Storage.StorageFile GlobalFile;
        public MainPage()
        {
            viewModel = new TextViewModel();
            this.InitializeComponent();
            //DataContextChanged += (sender, args) => this.Bindings.Update();
        }

        private void AddButton(object sender, RoutedEventArgs e)
        { 
            viewModel.AddItem();
        }

        private async void DeleteButton(object sender, RoutedEventArgs e)
        {
            TextModel SelectedItem = (TextModel)MainListView.SelectedItem;

            if (SelectedItem == null)
                return;

            MessageDialog message = new MessageDialog("Are you sure you want to delete " +
                SelectedItem.Key + "?");

            message.Commands.Clear();
            message.Commands.Add(new UICommand { Label = "Yes", Id = 1 });
            message.Commands.Add(new UICommand { Label = "No", Id = 0 });

            var result = await message.ShowAsync();

            if((int)result.Id == 1)
            {
                Messages.Text = "Deleted " + SelectedItem.Key;
                viewModel.TextList.Remove(SelectedItem);
            }
                
        }

        private async void LoadFile(object sender, RoutedEventArgs e)
        {

            Windows.Storage.Pickers.FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker();

            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            picker.FileTypeFilter.Add(".json");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if(file != null)
            {
                GlobalFile = file;
                Messages.Text = "Loaded " + file.Name;
                String text = await Windows.Storage.FileIO.ReadTextAsync(file);

                if(viewModel.LoadJson(text))
                {
                    Messages.Text = "Loaded data.";
                }
                else
                {
                    Messages.Text = "Failed to load data.";
                }
            }
            else
            {
                Messages.Text = "Didn't pick a file or failed to load.";
            }
        }

        private void SaveFileAs(object sender, RoutedEventArgs e)
        {
            SaveFileAs();
        }

        private async void SaveFile(object sender, RoutedEventArgs e)
        {
            String JsonString;
            if (!viewModel.HandledExportedLast)
                JsonString = viewModel.toJsonString();
            else
                JsonString = viewModel.toExportJsonString();

            Messages.Text = JsonString;

            Windows.Storage.StorageFile File = GlobalFile;
            try
            {
                if (File != null)
                {
                    //According to the msdn documentation, this prevents updates to a remote version of
                    //a file until we are done syncing.
                    Windows.Storage.CachedFileManager.DeferUpdates(File);

                    await Windows.Storage.FileIO.WriteTextAsync(File, JsonString);

                    //Let Windows know we are done with changing the file so other files can get to it.
                    Windows.Storage.Provider.FileUpdateStatus FileStatus = await Windows.Storage
                        .CachedFileManager.CompleteUpdatesAsync(File);

                    if (FileStatus == Windows.Storage.Provider.FileUpdateStatus.Complete)
                    {
                        Messages.Text = "Saved file " + File.Name + ".";
                    }
                    else
                    {
                        Messages.Text = "Failed to save " + File.Name + ".";
                    }
                }
                else
                {
                    SaveFileAs();
                }
            }catch(Exception Ex)
            {
                SaveFileAs();
            }
        }

        private async void SaveFileAs()
        {
            String JsonString = viewModel.toJsonString();
            Messages.Text = JsonString;

            Windows.Storage.Pickers.FileSavePicker Picker = new Windows.Storage.Pickers.FileSavePicker();

            Picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            Picker.FileTypeChoices.Add("JSON", new List<String>() { ".json" });

            Picker.SuggestedFileName = "language-.json";

            Windows.Storage.StorageFile File = await Picker.PickSaveFileAsync();

            if (File != null)
            {
                GlobalFile = File; //Global file will be one we are currently working on.
                //According to the msdn documentation, this prevents updates to a remote version of
                //a file until we are done syncing.
                Windows.Storage.CachedFileManager.DeferUpdates(File);

                await Windows.Storage.FileIO.WriteTextAsync(File, JsonString);

                //Let Windows know we are done with changing the file so other files can get to it.
                Windows.Storage.Provider.FileUpdateStatus FileStatus = await Windows.Storage
                    .CachedFileManager.CompleteUpdatesAsync(File);

                if (FileStatus == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    Messages.Text = "Saved file " + File.Name + ".";
                }
                else
                {
                    Messages.Text = "Failed to save " + File.Name + ".";
                }
            }
            else
            {
                Messages.Text = "Failed to get file for saving.";
            }
        }
        //Copy pasted from above.
        private async void ExportFile(object sender, RoutedEventArgs e)
        {
            String JsonString = viewModel.toExportJsonString();

            Windows.Storage.Pickers.FileSavePicker Picker = new Windows.Storage.Pickers.FileSavePicker();

            Picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            Picker.FileTypeChoices.Add("JSON", new List<String>() { ".json" });

            Picker.SuggestedFileName = "-export.json";

            Windows.Storage.StorageFile File = await Picker.PickSaveFileAsync();

            if (File != null)
            {
                GlobalFile = File; //Global file will be one we are currently working on.
                //According to the msdn documentation, this prevents updates to a remote version of
                //a file until we are done syncing.
                Windows.Storage.CachedFileManager.DeferUpdates(File);

                await Windows.Storage.FileIO.WriteTextAsync(File, JsonString);

                //Let Windows know we are done with changing the file so other files can get to it.
                Windows.Storage.Provider.FileUpdateStatus FileStatus = await Windows.Storage
                    .CachedFileManager.CompleteUpdatesAsync(File);

                if (FileStatus == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    Messages.Text = "Exported file " + File.Name + ".";
                }
                else
                {
                    Messages.Text = "Failed to export " + File.Name + ".";
                }
            }
            else
            {
                Messages.Text = "Failed to get export file.";
            }
        }
    }
}
