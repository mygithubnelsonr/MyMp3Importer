using MyMp3Importer.BLL;
using MyMp3Importer.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MyMp3Importer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        private ObservableCollection<string> _genres = null;
        private ObservableCollection<string> _catalogs = null;
        private ObservableCollection<string> _medias = null;
        private ObservableCollection<string> _artists = null;
        private ObservableCollection<string> _albums = null;

        private FileDetailsList _fileDetails = null;
        private string _ignores = "NA,_Images,Backup";
        #endregion

        #region CTOR
        public MainWindow()
        {
            InitializeComponent();
            statusVersion.Content = Properties.Settings.Default.Version;
            FillCombosAsync();
        }

        #endregion

        #region FormEvents
        private void Move_Window(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        #region Textbox Drag & Drop
        private void textboxStartfolder_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void textboxStartfolder_PreviewDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void textboxStartfolder_PreviewDrop(object sender, DragEventArgs e)
        {
            //if (e.Data.GetDataPresent(DataFormats.FileDrop))
            //{
            //    datagridFilelist.ItemsSource = null;

            //    string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            //    DirectoryInfo di;
            //    di = new DirectoryInfo(filenames[0]);
            //    string folder;

            //    if ((di.Attributes & FileAttributes.Directory) > 0)
            //        folder = di.FullName;
            //    else
            //        folder = di.Parent.FullName;

            //    textboxStartfolder.Text = folder;
            //    textboxStartfolder.ToolTip = folder;
            //    labelFailed.Content = "0";
            //    labelSuccess.Content = "0";

            //    this.Activate();

            //    checkboxTestimport.IsChecked = true;

            //    if (CheckStartfolder() == true)
            //        Scanner();
            //}
        }
        #endregion

        #region Datagrid Drag & Drop
        private void datagridFilelist_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void datagridFilelist_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
        }

        private void datagridFilelist_PreviewDrop(object sender, DragEventArgs e)
        {
            checkboxTestimport.IsChecked = true;
            datagridFilelist.ItemsSource = "";
            string fullpath = "";

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                fullpath = filenames[0];

                if (CheckStartfolder(fullpath) == false)
                {
                    textboxStartfolder.Background = Brushes.Salmon;
                    buttonScan.IsEnabled = false;
                    return;
                }
                else
                {
                    textboxStartfolder.Background = (Brush)new BrushConverter().ConvertFrom("#FFD1E6A7");   //.LightGreen;
                    textboxStartfolder.IsEnabled = true;
                    buttonScan.IsEnabled = true;
                }

                bool isFolder = File.GetAttributes(fullpath).HasFlag(FileAttributes.Directory);

                if (isFolder == true)
                {
                    textboxStartfolder.Text = fullpath;
                    ProcessFolder(fullpath, textboxExtension.Text);
                }
                else
                {
                    textboxStartfolder.Text = Path.GetDirectoryName(fullpath);
                    ProcessFileList(filenames, textboxStartfolder.Text, textboxExtension.Text);
                }

                labelFailed.Content = "0";
                labelSuccess.Content = "0";

                this.Activate();
            }
        }

        private void ProcessFolder(string path, string extension)
        {
            if (CheckStartfolder(path) == true)
            {
                ScannFolder(path, extension);
            }

            textboxStartfolder.Text = path;
            textboxStartfolder.ToolTip = path;
        }

        private void ProcessFileList(string[] files, string path, string extension)
        {
            if (CheckStartfolder(path) == true)
            {
                ScannFileList(files, path, extension);
            }

            //textboxStartfolder.Text = path;
            //textboxStartfolder.ToolTip = path;
        }

        #endregion

        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonScan_Click(object sender, RoutedEventArgs e)
        {
            //datagridFilelist.ItemsSource = null;
            //datagridFilelist.Items.Clear();

            //_artists.Clear();
            //_artists.Add("NA");
            //_albums.Clear();
            //_albums.Add("NA");

            //comboboxArtist.SelectedItem = "NA";
            //comboboxAlbum.SelectedItem = "NA";

            //ScannFolder(textboxStartfolder.Text, textboxExtension.Text);
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            datagridFilelist.ItemsSource = "";
        }

        private void buttonImport_Click(object sender, RoutedEventArgs e)
        {
            ImportStart();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonTest_Click(object sender, RoutedEventArgs e)
        {
            //List<ParserToken> tokens = new List<ParserToken>();
            //ParserTokenList parserlist = new ParserTokenList(@"\\win2k16dc01\FS012\Andere\Berni\M3\Norah Jones");
            //tokens = parserlist.Get();
            //Debug.Print($"{tokens.Count}");

            //foreach (var token in tokens)
            //{
            //    Debug.Print($"{token.Name}, {token.Token}, {token.State}");
            //}
        }

        private void checkboxTestimport_Click(object sender, RoutedEventArgs e)
        {
            labelSuccess.Content = "0";
            labelFailed.Content = "0";
        }

        #endregion

        #region Methods
        private bool CheckStartfolder(string fullpath)
        {
            try
            {
                string path = Path.GetDirectoryName(fullpath);
                DirectoryInfo di = new DirectoryInfo(path);

                if (di.Exists)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        private bool CheckIfSampler(List<FileDetails> files)
        {
            bool isSampler = false;

            var ar = files[0].File.ToString().Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);

            string tmp = ar[0];

            foreach (var file in files)
            {
                ar = file.File.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                if (ar[0] != tmp) isSampler = true;
            }

            return isSampler;
        }

        private async Task FillCollectionsAsync()
        {
            List<string> list = await DataGetSet.GetGenresAsync();
            list.Insert(0, "NA");
            _genres = new ObservableCollection<string>(list);

            list = await DataGetSet.GetCatalogsAsync();
            list.Insert(0, "NA");
            _catalogs = new ObservableCollection<string>(list);

            list = await DataGetSet.GetMediaAsync();
            list.Insert(0, "NA");
            _medias = new ObservableCollection<string>(list);

            list = new List<string>();      // await DataGetSet.GetInterpretersAsync();
            list.Add("NA");
            _artists = new ObservableCollection<string>(list);

            list = new List<string>();      // await DataGetSet.GetAlbumsAsync();
            list.Add("NA");
            _albums = new ObservableCollection<string>(list);
        }

        private async Task FillCombosAsync()
        {
            await FillCollectionsAsync();

            comboboxGenre.Items.Clear();
            comboboxCatalog.Items.Clear();
            comboboxMedia.Items.Clear();
            comboboxArtist.Items.Clear();
            comboboxAlbum.Items.Clear();

            comboboxGenre.ItemsSource = _genres;
            comboboxCatalog.ItemsSource = _catalogs;
            comboboxMedia.ItemsSource = _medias;
            comboboxArtist.ItemsSource = _artists;
            comboboxAlbum.ItemsSource = _albums;

            comboboxGenre.SelectedItem = "NA";
            comboboxCatalog.SelectedItem = "NA";
            comboboxMedia.SelectedItem = "NA";
            comboboxArtist.SelectedItem = "NA";
            comboboxAlbum.SelectedItem = "NA";
        }

        private void ScannFolder(string startDirectory, string filePattern)
        {
            if (startDirectory == "")
                return;

            #region processing files

            var filedetails = new FileDetailsList(startDirectory, filePattern);
            _fileDetails = filedetails.Load();

            labelFolders.Content = filedetails.DirCount.ToString();
            labelFiles.Content = filedetails.FileCount.ToString();

            var size = filedetails.FileSizeAll / 1024;
            labelSize.Content = size.ToString("#,#") + " Kb";

            if (filedetails.FileSizeAll > 0)
                buttonImport.IsEnabled = true;

            checkboxSampler.IsChecked = CheckIfSampler(_fileDetails);
            datagridFilelist.ItemsSource = _fileDetails;

            #endregion

            #region processing parser tokens

            List<ParserToken> tokens = new List<ParserToken>();
            ParserTokenList parserlist = new ParserTokenList(startDirectory);
            tokens = parserlist.Get();

            UpdateCombos(tokens);

            #endregion

            #region Fill Albums condtionaly
            var folders = System.Convert.ToInt32(labelFolders.Content);

            if (comboboxArtist.SelectedItem.ToString() != "NA" && folders > 0)
            {
                DirectoryInfo di = null;
                List<string> allDirs = Helpers.GetDirectories(startDirectory, false);
                _albums.Clear();

                foreach (string dir in allDirs)
                {
                    di = new DirectoryInfo(dir);

                    if (_ignores.Contains(dir.Split('\\').Last()) != true)
                        _albums.Add(di.Name);
                    else
                        _albums.Add(di.Parent.Name);
                }
                comboboxAlbum.SelectedIndex = 0;
            }

            #endregion
        }

        private void UpdateCombos(List<ParserToken> tokens)
        {
            comboboxGenre.SelectedItem = tokens[0].Token;
            labelGenre.Tag = tokens[0].State;

            comboboxCatalog.SelectedItem = tokens[1].Token;
            labelCatalog.Tag = tokens[1].State;

            comboboxMedia.SelectedItem = tokens[2].Token;
            labelMedia.Tag = tokens[2].State;

            _artists.Add(tokens[3].Token);
            comboboxArtist.SelectedItem = tokens[3].Token;
            labelArtist.Tag = tokens[3].State;

            _albums.Add(tokens[4].Token);
            comboboxAlbum.SelectedItem = tokens[4].Token;
            labelAlbum.Tag = tokens[4].State;
        }

        private void ScannFileList(string[] files, string path, string extension)
        {
            if (path == "")
                return;

            #region processing files
            List<string> fileList = new List<string>();

            foreach (string file in files)
            {
                fileList.Add(file);
            }

            var filedetailsList = new FileDetailsList(fileList, path, extension);
            _fileDetails = filedetailsList.LoadFiles();

            labelFolders.Content = filedetailsList.DirCount.ToString();
            labelFiles.Content = filedetailsList.FileCount.ToString();

            if (_fileDetails.Count > 0)
                buttonImport.IsEnabled = true;

            checkboxSampler.IsChecked = CheckIfSampler(_fileDetails);
            datagridFilelist.ItemsSource = _fileDetails;

            #endregion

            #region processing parser tokens

            List<ParserToken> tokens = new List<ParserToken>();
            ParserTokenList parserlist = new ParserTokenList(path);
            tokens = parserlist.Get();

            UpdateCombos(tokens);

            #endregion

        }

        private void ImportStart()
        {
            int catalogID = -1;

            labelSuccess.Content = "0";
            labelFailed.Content = "0";

            if (datagridFilelist.Items.Count == 0) return;

            if ((bool)labelGenre.Tag == false)
            {
                var result = MessageBox.Show($"Missing genre '{comboboxGenre.Text}' in table tGenre!\n\n" +
                            "You want create this genre?", "New Genre", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.No)
                    return;
                else
                    catalogID = DataGetSet.CreateGenre(comboboxGenre.Text);
            }

            if ((bool)labelCatalog.Tag == false)
            {
                var result = MessageBox.Show($"Missing catalog '{comboboxCatalog.Text}' in table tCatalogs!\n\n" +
                            "You want create this catalog?", "New Catalog", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.No)
                    return;

                catalogID = DataGetSet.CreateCatalog(comboboxCatalog.Text);

                if (catalogID == -1)
                    return;
            }

            //if (checkboxSampler.IsChecked == true)
            //    ImportSampler();
            //else
            //    ImportAlbum();

            Import((bool)checkboxSampler.IsChecked);

        }

        private void Import(bool isSampler)
        {
            int recordsAffected = 0;

            buttonImport.IsEnabled = false;

            DateTime t1 = DateTime.Now;
            statusbarStart.Content = t1.ToString("HH:mm:ss");
            statusbarDauer.Content = "";
            statusbarProgress.Visibility = Visibility.Visible;

            if (checkboxTestimport.IsChecked == true)
            {
                var result = DataGetSet.TruncateTestTables();
                Debug.Print($"TruncateTestTables result = {result}");
                if (result == false) return;
            }

            for (int i = 0; i <= comboboxAlbum.Items.Count - 1; i++)
            {
                comboboxAlbum.SelectedIndex = i;

                if (isSampler == false && _ignores.Contains(comboboxAlbum.Text))
                {
                    continue;
                }

                List<MP3Record> mp3List = mp3Records(_fileDetails, false);  // isSampler = false

                // save records
                if ((bool)checkboxTestimport.IsChecked == true)
                    recordsAffected += DataGetSet.SaveTestRecord(mp3List);
                else
                    recordsAffected += DataGetSet.SaveRecord(mp3List);

            }

            DateTime t2 = DateTime.Now;
            statusbarProgress.Visibility = Visibility.Hidden;
            statusbarDauer.Content = (t2 - t1).Milliseconds.ToString() + " ms";

            labelSuccess.Content = $"{recordsAffected}";
            labelFailed.Content = $"{_fileDetails.Count - recordsAffected}";

            buttonImport.IsEnabled = true;
        }

        private List<MP3Record> mp3Records(List<FileDetails> list, bool isSampler)
        {
            List<MP3Record> mp3List = new List<MP3Record>();

            foreach (FileDetails item in list)
            {
                if (Convert.ToBoolean(buttonCancel.Tag) == true)
                    break;

                if (!item.Path.Contains(comboboxAlbum.Text))
                    continue;

                MP3Record mp3 = new MP3Record();

                mp3.Genre = comboboxGenre.SelectedIndex;
                mp3.Catalog = comboboxCatalog.SelectedIndex;
                mp3.Media = comboboxMedia.SelectedIndex;
                mp3.Album = comboboxAlbum.Text;
                mp3.Titel = item.File;
                mp3.FileName = item.File + item.Extension;
                mp3.FileSize = Convert.ToInt32(item.Size);
                mp3.FileDate = item.LastWrite;
                mp3.Path = item.Path;
                mp3.IsSample = isSampler;
                mp3.MD5 = Helpers.MD5($"{mp3.Path}\\{mp3.FileName}");


                if (item.File.Contains('-'))
                {
                    var ar = item.File.Split('-');
                    mp3.Titel = ar[1].Trim();
                    mp3.Artist = ar[0].Trim();
                }
                else
                {
                    var artist = item.Path.Replace(mp3.Album, "");
                    DirectoryInfo di = new DirectoryInfo(artist);
                    mp3.Titel = item.File;
                    mp3.Artist = di.Name;
                }

                mp3List.Add(mp3);
            }

            return mp3List;
        }

        #endregion

    }
}
