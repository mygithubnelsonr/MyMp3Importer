using MyMp3Importer.BLL;
using MyMp3Importer.Common;
using MyMp3Importer.Views;
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

        private List<SongRecord> songRecords = new List<SongRecord>();

        #endregion

        #region CTOR
        public MainWindow()
        {
            InitializeComponent();
            statusVersion.Content = Properties.Settings.Default.Version;
            this.buttonLog.Visibility = Visibility.Hidden;
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

                _albums.Clear();
                _albums.Add("NA");

                var logs = LogList.Instance;
                logs.Clear();

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
                    //ProcessFolder(fullpath, textboxExtension.Text);
                    ScannFolder2(fullpath, textboxExtension.Text);
                }
                else
                {
                    textboxStartfolder.Text = Path.GetDirectoryName(fullpath);
                    //ProcessFileList(filenames, textboxStartfolder.Text, textboxExtension.Text);
                    ScannFileList2(filenames, textboxExtension.Text);
                }

                labelFailed.Content = "0";
                labelSuccess.Content = "0";

                this.Activate();
            }
        }

        private void datagridFilelist_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (datagridFilelist.Items.Count > 0)
            {
                int index = datagridFilelist.SelectedIndex;

                comboboxGenre.Text = DataGetSet.GetGenre(songRecords[index].Genre);
                comboboxCatalog.Text = DataGetSet.GetCatalog(songRecords[index].Catalog);
                comboboxMedia.Text = DataGetSet.GetMedia(songRecords[index].Media);

                comboboxAlbum.Text = songRecords[index].Album;
                comboboxArtist.Text = songRecords[index].Artist;
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
        }

        //private void ProcessFileList2(string[] files, string extension)
        //{
        //    ScannFileList2(files, extension);
        //}

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
            //ScannFolder2(textboxStartfolder.Text, textboxExtension.Text);
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            datagridFilelist.ItemsSource = "";
        }

        private void buttonImport_Click(object sender, RoutedEventArgs e)
        {
            ImportStart2();
            this.buttonLog.Visibility = Visibility.Visible;
        }

        private void buttonLog_Click(object sender, RoutedEventArgs e)
        {
            Log log = new Log();
            log.ShowDialog();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonTest_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GetTokens(string fullName, string extension)
        {
            string genre = "", catalog = "", media = "", artist, album = "", title, file, path, t1;
            int l;

            try
            {

                #region Path
                l = fullName.LastIndexOf("\\");
                path = fullName.Substring(0, l);
                #endregion

                #region File
                string container = Helpers.GetContainer(fullName);
                string[] ar = container.Split('\\');
                file = ar[ar.Length - 1];
                #endregion

                #region Album
                if (ar[4] == "_Various Artists")
                    album = ar[5];
                else
                    album = ar[5];
                #endregion

                #region Artist
                t1 = $" - ";
                l = l = file.IndexOf(t1);
                artist = file.Substring(0, l);
                #endregion

                #region Title
                t1 = $" - ";
                l = l = file.IndexOf(t1);
                title = file.Substring(l + t1.Length).Replace(extension, "");
                #endregion

                #region Genre
                var genreList = GenreList.Instance;
                var genres = genreList.Get();

                foreach (var g in genres)
                {
                    if (fullName.IndexOf(g) > -1)
                    {
                        genre = g;
                        break;
                    }
                }
                #endregion

                #region Catalog
                var catalogList = CatalogList.Instance;
                var catalogs = catalogList.Get();

                foreach (var c in catalogs)
                {
                    if (fullName.IndexOf(c) > -1)
                    {
                        catalog = c;
                        break;
                    }
                }
                #endregion

                #region Media
                var mediaList = MediaList.Instance;
                var medias = mediaList.Get();

                foreach (var m in medias)
                {
                    if (fullName.IndexOf($"\\{m}\\") > -1)
                    {
                        media = m;
                        break;
                    }
                }
                #endregion

                FileInfo fi = new FileInfo(fullName);

                SongRecord songRecord = new SongRecord()
                {
                    Album = album,
                    Artist = artist,
                    Catalog = DataGetSet.GetCatalogId(catalog),
                    FileName = file,
                    Genre = DataGetSet.GetGenreId(genre),
                    Media = DataGetSet.GetMediaId(media),
                    Path = path,
                    Titel = title,
                    MD5 = Helpers.MD5($"{path}\\{file}"),
                    FileDate = File.GetLastWriteTime(fullName),
                    FileSize = Convert.ToInt32(fi.Length),
                };
                songRecords.Add(songRecord);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print($"{fullName}");
            }
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

        private bool CheckIfSampler2(List<SongRecord> files)
        {
            bool isSampler = false;
            string artist = "";

            foreach (var file in files)
            {
                if (artist == "")
                    artist = file.Artist;
                else if (file.Artist != artist)
                    isSampler = true;
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
            var folders = Convert.ToInt32(labelFolders.Content);

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

        private void ScannFolder2(string fullPath, object extension)
        {
            songRecords.Clear();

            datagridFilelist.ItemsSource = null;
            datagridFilelist.Items.Clear();

            List<string> listFiles = new List<string>();
            FileInfo[] fileInfos = GetAllFileInfos(fullPath, ".mp3");

            foreach (var fileinfo in fileInfos)
            {
                listFiles.Add(fileinfo.FullName);
                GetTokens(fileinfo.FullName, textboxExtension.Text);
            };

            if (fileInfos.Length > 0)
                buttonImport.IsEnabled = true;

            checkboxSampler.IsChecked = CheckIfSampler2(songRecords);

            datagridFilelist.ItemsSource = songRecords;
            datagridFilelist.SelectedIndex = 0;
            datagridFilelist.SelectedItem = 0;
            labelFiles.Content = songRecords.Count;
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

        private void ScannFileList2(string[] files, string extension)
        {
            songRecords.Clear();

            datagridFilelist.ItemsSource = null;
            datagridFilelist.Items.Clear();

            #region processing files
            List<string> listFiles = new List<string>();
            FileInfo[] fileInfos = new FileInfo[files.Length];

            for (int n = 0; n < files.Length; n++)
            {
                FileInfo fileInfo = new FileInfo(files[n]);
                fileInfos[n] = fileInfo;
            }

            foreach (var fileinfo in fileInfos)
            {
                listFiles.Add(fileinfo.FullName);
                GetTokens(fileinfo.FullName, textboxExtension.Text);
            }

            if (fileInfos.Length > 0)
                buttonImport.IsEnabled = true;

            checkboxSampler.IsChecked = CheckIfSampler2(songRecords);

            datagridFilelist.ItemsSource = songRecords;
            datagridFilelist.SelectedIndex = 0;
            datagridFilelist.SelectedItem = 0;
            labelFiles.Content = songRecords.Count;
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

            Import((bool)checkboxSampler.IsChecked);

        }

        private void ImportStart2()
        {
            labelSuccess.Content = "0";
            labelFailed.Content = "0";

            if (datagridFilelist.Items.Count == 0) return;

            Import2((bool)checkboxSampler.IsChecked);
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
                {
                    recordsAffected += DataGetSet.SaveTestRecord(mp3List);
                }
                else
                {
                    recordsAffected += DataGetSet.SaveRecord(mp3List);
                }
            }

            DateTime t2 = DateTime.Now;
            statusbarProgress.Visibility = Visibility.Hidden;
            statusbarDauer.Content = (t2 - t1).Milliseconds.ToString() + " ms";

            labelSuccess.Content = $"{recordsAffected}";
            labelFailed.Content = $"{_fileDetails.Count - recordsAffected}";

            checkboxTestimport.IsChecked = false;
            buttonImport.IsEnabled = true;
        }

        private void Import2(bool isSampler)
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
                if (result == false)
                {
                    // Todo write log
                    return;
                }
            }

            List<MP3Record> mp3List;

            foreach (var song in songRecords)
            {
                mp3List = new List<MP3Record>();
                mp3List.Add(new MP3Record() {
                    Album = song.Album,
                    Artist = song.Artist,
                    Catalog = song.Catalog,
                    FileName = song.FileName,
                    Genre = song.Genre,
                    Media = song.Media,
                    MD5 = song.MD5,
                    Path = song.Path,
                    Titel = song.Titel,
                    FileDate = song.FileDate,
                    FileSize = song.FileSize,
                    IsSample = (bool)checkboxSampler.IsChecked}
                );

                // save records
                if ((bool)checkboxTestimport.IsChecked == true)
                {
                    recordsAffected += DataGetSet.SaveTestRecord(mp3List);
                }
                else
                {
                    recordsAffected += DataGetSet.SaveRecord(mp3List);
                }
            }

            DateTime t2 = DateTime.Now;
            statusbarProgress.Visibility = Visibility.Hidden;
            statusbarDauer.Content = (t2 - t1).Milliseconds.ToString() + " ms";

            labelSuccess.Content = $"{recordsAffected}";
            labelFailed.Content = $"{songRecords.Count - recordsAffected}";

            checkboxTestimport.IsChecked = false;
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

        #region Test
        private FileInfo[] GetAllFileInfos(string startDirectory, string filePattern)
        {
            if (startDirectory == "")
                return null;

            DirectoryInfo di = new DirectoryInfo(startDirectory);

            FileInfo[] fileInfos = di.GetFiles($"*{filePattern}", SearchOption.AllDirectories);

            return fileInfos;
        }

        #endregion

    }
}
