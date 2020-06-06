using MyMp3Importer.BLL;
using MyMp3Importer.Common;
using NRSoft.FunctionPool;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
        private ObservableCollection<string> _interpreters = null;
        private ObservableCollection<string> _albums = null;
        #endregion

        #region CTOR
        public MainWindow()
        {
            InitializeComponent();

            FillCombosAsync();
        }

        #endregion

        #region FormEvents
        private void textboxStartfolder_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void textboxStartfolder_DragEnter(object sender, DragEventArgs e)
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

        private void textboxStartfolder_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                datagridFilelist.ItemsSource = null;

                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, false);

                DirectoryInfo di;
                di = new DirectoryInfo(filenames[0]);
                string folder;

                if ((di.Attributes & FileAttributes.Directory) > 0)
                    folder = di.FullName;
                else
                    folder = di.Parent.FullName;

                textboxStartfolder.Text = folder;
                textboxStartfolder.ToolTip = folder;

                if (CheckStartfolder() == true)
                    Scanner();
            }
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonScan_Click(object sender, RoutedEventArgs e)
        {
            datagridFilelist.ItemsSource = null;
            datagridFilelist.Items.Clear();

            Scanner();
        }

        private void buttonImport_Click(object sender, RoutedEventArgs e)
        {
            Import();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Move_Window(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        #endregion

        #region Methods
        private bool CheckStartfolder()
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(textboxStartfolder.Text);

                if (di.Exists)
                {
                    textboxStartfolder.Background = Brushes.LightGreen;
                    textboxStartfolder.IsEnabled = true;
                    return true;
                }
                else
                {
                    textboxStartfolder.Background = Brushes.Salmon;
                    buttonScan.IsEnabled = false;
                    return false;
                }

            }
            catch { return false; }
        }

        //private void FillCollectionsNotUsed()
        //{
        //List<string> list = null;

        //List<Genre> genres = null;
        //genres = DataGetSet.GetGenresAsync();

        ////list.Insert(0, "NA");
        //_genres = new ObservableCollection<Genre>(genres);

        //list = DataGetSet.GetCatalogs();
        //list.Insert(0, "NA");
        //_catalogs = new ObservableCollection<string>(list);

        //list = DataGetSet.GetMedia();
        //list.Insert(0, "NA");
        //_medias = new ObservableCollection<string>(list);

        //list = new List<string>();     // DataGetSet.GetInterpreters();
        //list.Add("NA");
        //_interpreters = new ObservableCollection<string>(list);

        //list = new List<string>();      // DataGetSet.GetAlbums();
        //list.Add("NA");
        //_albums = new ObservableCollection<string>(list);
        //}

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
            _interpreters = new ObservableCollection<string>(list);

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
            comboboxInterpret.Items.Clear();
            comboboxAlbum.Items.Clear();

            comboboxGenre.ItemsSource = _genres;
            comboboxCatalog.ItemsSource = _catalogs;
            comboboxMedia.ItemsSource = _medias;
            comboboxInterpret.ItemsSource = _interpreters;
            comboboxAlbum.ItemsSource = _albums;

            comboboxGenre.SelectedItem = "NA";
            comboboxCatalog.SelectedItem = "NA";
            comboboxMedia.SelectedItem = "NA";
            comboboxInterpret.SelectedItem = "NA";
            comboboxAlbum.SelectedItem = "NA";
        }

        private void Scanner()
        {
            List<ParserToken> tokens = null;

            string startDirectory = textboxStartfolder.Text;
            string filePattern = textboxExtension.Text;

            if (startDirectory == "") return;

            #region processing files

            List<FileInfo> fileInfos = new List<FileInfo>();
            List<FileInfo> fileInfosTemp = new List<FileInfo>();
            List<FileDetails> files = new List<FileDetails>();

            long dirCount = Helpers.DirectoryCount(startDirectory, false);

            fileInfos = FileSystemUtils.GetFileinfos(startDirectory, true);

            long allFileSize = 0;

            foreach (FileInfo fi in fileInfos)
            {
                if (fi.Extension == filePattern)
                {
                    files.Add(new FileDetails() { File = fi.Name, Extension = fi.Extension, Path = fi.DirectoryName, Size = fi.Length, LastWrite = fi.LastWriteTime });
                    allFileSize += fi.Length;
                }
            }

            labelFolders.Content = dirCount.ToString();
            labelFiles.Content = fileInfos.Count.ToString();
            labelSize.Content = allFileSize.ToString();

            if (allFileSize > 0)
            {
                buttonImport.IsEnabled = true;
            }

            datagridFilelist.ItemsSource = files;

            #endregion

            #region processing parser tokens

            Parser parser = new Parser();
            tokens = parser.ParserTokens(startDirectory);

            for (int i = 1; i <= tokens.Count; i++)
            {
                if (i == 1)
                {
                    _genres.Add(tokens[0].Token);
                    comboboxGenre.SelectedItem = tokens[0].Token;
                }
                else
                {
                    _genres.Add(tokens[0].Token);
                    comboboxGenre.SelectedItem = tokens[0].Token;
                    labelGenre.Foreground = Brushes.Red;
                    labelGenre.Tag = false;
                }

                if (i == 2)
                {
                    if (_catalogs.Contains(tokens[1].Token))
                        comboboxCatalog.SelectedItem = tokens[1].Token;
                    else
                    {
                        _catalogs.Add(tokens[1].Token);
                        comboboxCatalog.SelectedItem = tokens[1].Token;
                        labelCatalog.Foreground = Brushes.Red;
                        labelCatalog.Tag = false;
                    }
                }

                if (i == 3)
                {
                    if (_medias.Contains(tokens[2].Token))
                        comboboxMedia.SelectedItem = tokens[2].Token;
                    else
                    {
                        _medias.Add(tokens[2].Token);
                        comboboxMedia.SelectedItem = tokens[2].Token;
                        labelMedia.Foreground = Brushes.Red;
                        labelMedia.Tag = false;
                    }
                }

                if (i == 4)
                {
                    if (_interpreters.Contains(tokens[3].Token))
                        comboboxInterpret.SelectedItem = tokens[3].Token;
                    else
                    {
                        _interpreters.Insert(1, tokens[3].Token);
                        comboboxInterpret.SelectedItem = tokens[3].Token;
                        labelArtist.Foreground = Brushes.Red;
                        labelArtist.Tag = false;
                    }
                }

                if (i == 5)
                {
                    if (_albums.Contains(tokens[4].Token))
                        comboboxAlbum.SelectedItem = tokens[4].Token;
                    else
                    {
                        _albums.Insert(1, tokens[4].Token);
                        comboboxAlbum.SelectedItem = tokens[4].Token;
                        labelAlbum.Foreground = Brushes.Red;
                        labelAlbum.Tag = false;
                    }
                }
            }

            #endregion

            #region Fill Albums condtionaly
            var folders = System.Convert.ToInt32(labelFolders.Content);

            if (comboboxInterpret.SelectedItem.ToString() != "NA" && folders > 1)
            {
                List<string> allDirs = Helpers.GetDirectories(startDirectory, false);
                _albums.Clear();

                foreach (string dir in allDirs)
                {
                    System.Console.WriteLine(dir);

                    DirectoryInfo di = new DirectoryInfo(dir);

                    Console.WriteLine(di.Name);

                    _albums.Add(di.Name);

                }
                comboboxAlbum.SelectedIndex = 0;
            }

            #endregion
        }

        private void Import()
        {
            string fileExtension;
            int importFailed = 0;
            int importSuccess = 0;

            if (datagridFilelist.Items.Count == 0) return;

            if ((bool)labelCatalog.Tag == false)
            {
                var result = MessageBox.Show($"catalog '{comboboxCatalog.Text}' does not exist in the databse!\n\n" +
                            "create new catalog?", "New Catalog", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.No)
                    return;
                else
                    DataGetSet.CreateCatalog(comboboxCatalog.Text);
            }

            buttonImport.IsEnabled = false;

            List<MP3Record> mp3List = null;

            #region checkBoxSpecialImport.Checked
            //if (checkBoxSpecialImport.Checked)
            //{
            //    if (comboBoxGenre.Text == "" | comboBoxGenre.Text == "Untitled"
            //        | comboBoxKatalog.Text == "" | comboBoxKatalog.Text == "Untitled"
            //        | comboBoxMedium.Text == "" | comboBoxMedium.Text == "NA"
            //        | comboBoxAlbum.Text == "" | comboBoxAlbum.Text == "Untitled")
            //    {
            //        MessageBox.Show("If option 'Spezial Import' is checked then all 'Spezial Import' Fields must be filled out");
            //        return;
            //    }
            //}
            #endregion

            DateTime t1 = DateTime.Now;
            statusbarStart.Content = t1.Hour.ToString() + ":" + t1.Minute.ToString() + ":" + t1.Second.ToString();
            statusbarDauer.Content = "";
            statusbarProgress.Visibility = Visibility.Visible;
            //toolStripProgressBar.Enabled = true;

            var list = datagridFilelist.Items;

            for (int i = 0; i <= comboboxAlbum.Items.Count - 1; i++)
            {
                comboboxAlbum.SelectedIndex = i;
                mp3List = new List<MP3Record>();

                foreach (FileDetails item in list)
                {
                    if (Convert.ToBoolean(buttonCancel.Tag) == true)
                        break;

                    if (!item.Path.Contains(comboboxAlbum.Text))
                    {
                        continue;
                    }

                    MP3Record mp3 = new MP3Record();

                    mp3.FileName = item.File;
                    fileExtension = item.Extension;
                    mp3.FileSize = Convert.ToInt32(item.Size);
                    mp3.FileDate = item.LastWrite;
                    mp3.Path = item.Path;

                    FileInfo fi = new FileInfo(item.File);
                    fileExtension = fi.Extension.ToLower();

                    if (checkboxSpezialimport.IsChecked == false)
                    {
                        //MP3Record record = DataGetSet.GetRecordInfo(filePfad);

                        //string[] arTmp = fileName.ToLower().Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                        //if (arTmp.Length == 0)
                        //{
                        //    mp3.Interpret = "NA";
                        //    mp3.Titel = GeneralH.ToProperCase(arTmp[0].Trim());
                        //}
                        //else
                        //{
                        //    mp3.Interpret = GeneralH.ToProperCase(arTmp[0].Trim());
                        //    mp3.Titel = GeneralH.ToProperCase(arTmp[1].Trim());
                        //}

                        mp3.Genre = comboboxGenre.Text;
                        mp3.Album = comboboxAlbum.Text;  //arPath[arPath.Length - 1];
                        mp3.Media = comboboxMedia.SelectedIndex;
                        mp3.Catalog = comboboxCatalog.Text;
                        //catalogue = comboboxCatalog.Text;
                    }
                    else
                    {
                        //MP3Record record = DataGetSet.GetRecordInfo(filePfad);

                        // ??????????????????????
                        //string[] arTmp = item.Path.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);

                        //if (arTmp.Length < 5)
                        //{
                        //    MessageBox.Show("Falsche Anzahl von Folderebenen!");
                        //    buttonImport.IsEnabled = true;
                        //    statusbarProgress.Visibility = Visibility.Hidden;
                        //    break;  // Stop Import
                        //}

                        //arTmp = item.File.ToLower().Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                        //if (arTmp.Length == 0)
                        //{
                        //    mp3.Interpret = "NA";
                        //    string strTitel = GeneralH.ToProperCase(arTmp[0].Trim());
                        //    mp3.Titel = strTitel.Replace(fileExtension, "");
                        //}
                        //else
                        //{
                        //    mp3.Interpret = GeneralH.ToProperCase(arTmp[0].Trim());
                        //    string strTitel = GeneralH.ToProperCase(arTmp[1].Trim());
                        //    mp3.Titel = strTitel.Replace(fileExtension, "");
                        //}

                        #region altes zeuch
                        //List<int> media;
                        //string type = arPath[arPath.Length - 3];


                        //var context = new MyJukeboxEntities();
                        //media = context.tMedias
                        //            .Where(m => m.Type == type)
                        //            .Select(m => m.ID).ToList();

                        //mp3.Media = media[0];
                        //mp3.Album = arPath[arPath.Length - 1];
                        ////mp3.Interpret = arPath[arPath.Length - 2];
                        //mp3.Catalog = arPath[arPath.Length - 4];
                        //mp3.Genre = arPath[arPath.Length - 5];
                        #endregion
                    }

                    mp3.MD5 = Helpers.MD5(mp3.Path + mp3.FileName);
                    mp3List.Add(mp3);
                }

                statusbarProgress.Visibility = Visibility.Hidden;

                // save records
                var testimport = checkboxTestimport.IsChecked ?? false;
                int recordsAffected = DataGetSet.SaveNewRecords(mp3List, testimport);
                DateTime t2 = DateTime.Now;
                statusbarDauer.Content = (t2 - t1).Milliseconds.ToString() + " ms";

                labelSuccess.Content = $"{recordsAffected}";
                labelFailed.Content = $"{mp3List.Count - recordsAffected}";

                var lastID = DataGetSet.GetLastSongID("tTestImport");
                Debug.Print($"Import success = {importSuccess}, failed={importFailed}, lastId={lastID}");

            }
        }

        #endregion

        private void buttonTest_Click(object sender, RoutedEventArgs e)
        {


        }
    }
}
