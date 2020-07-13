using MyMp3Importer.BLL;
using MyMp3Importer.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

        private FileDetailsList _fileDetails = null;
        private string _ignores = "NA,_Images";
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
                labelFailed.Content = "0";
                labelSuccess.Content = "0";

                this.Activate();

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
            ImportStart();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonTest_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonList_Click(object sender, RoutedEventArgs e)
        {
            CreateNewList();
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

        private void CheckIfSampler(List<FileDetails> files)
        {
            bool sampler = false;

            var ar = files[0].File.ToString().Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);

            string tmp = ar[0];

            foreach (var file in files)
            {
                ar = file.File.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                if (ar[0] != tmp) sampler = true;
            }

            checkboxSampler.IsChecked = sampler;
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

        private async void Scanner()
        {
            // ToDo try usebility of hashtable (s.o.) in Parser or scanner
            //Hashtable hashtable = new Hashtable() { { "Genre", 1 }, { "Catalog", 2 }, { "Media", 3 }, { "Interpret", 4 }, { "Album", 5 } };

            string startDirectory = textboxStartfolder.Text;
            string filePattern = textboxExtension.Text;

            if (startDirectory == "") return;

            labelSuccess.Content = "0";
            labelFailed.Content = "0";

            #region processing files

            var filedetails = new FileDetailsList(startDirectory, filePattern);
            _fileDetails = filedetails.Load();

            labelFolders.Content = filedetails.DirCount.ToString();
            labelFiles.Content = filedetails.FileCount.ToString();

            var size = filedetails.FileSizeAll / 1024;
            labelSize.Content = size.ToString("#,#") + " Kb";

            if (filedetails.FileSizeAll > 0)
                buttonImport.IsEnabled = true;

            CheckIfSampler(_fileDetails);

            datagridFilelist.ItemsSource = _fileDetails;

            #endregion

            #region processing parser tokens

            List<ParserToken> tokens = null;
            Parser parser = new Parser((bool)checkboxSampler.IsChecked);

            tokens = await parser.ParserTokens(startDirectory);
            Debug.Print("");

            await Task.Run(() =>
            {
                for (int i = 1; i <= tokens.Count; i++)
                {
                    if (i == 1)     // Genre
                    {
                        if (_genres.Contains(tokens[0].Token))
                        {
                            Dispatcher.Invoke(() =>
                            {
                                comboboxGenre.SelectedItem = tokens[0].Token;
                                labelGenre.Tag = true;
                            });
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                _genres.Add(tokens[0].Token);
                                comboboxGenre.SelectedItem = tokens[0].Token;
                                labelGenre.Foreground = Brushes.Red;
                                labelGenre.Tag = false;
                            });
                        }
                    }

                    if (i == 2)     // Catalog
                    {
                        if (_catalogs.Contains(tokens[1].Token))
                        {
                            Dispatcher.Invoke(() =>
                            {
                                comboboxCatalog.SelectedItem = tokens[1].Token;
                                labelCatalog.Tag = true;
                            });
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                _catalogs.Add(tokens[1].Token);
                                comboboxCatalog.SelectedItem = tokens[1].Token;
                                labelCatalog.Foreground = Brushes.Red;
                                labelCatalog.Tag = false;
                            });
                        }
                    }

                    if (i == 3)     // Media
                    {
                        if (_medias.Contains(tokens[2].Token))
                        {
                            Dispatcher.Invoke(() =>
                            {
                                comboboxMedia.SelectedItem = tokens[2].Token;
                                labelMedia.Tag = true;
                            });
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                _medias.Add(tokens[2].Token);
                                comboboxMedia.SelectedItem = tokens[2].Token;
                                labelMedia.Foreground = Brushes.Red;
                                labelMedia.Tag = false;
                            });
                        }
                    }

                    if (i == 4)     // Interpret
                    {
                        if (_interpreters.Contains(tokens[3].Token))
                        {
                            Dispatcher.Invoke(() =>
                            {
                                comboboxInterpret.SelectedItem = tokens[3].Token;
                                labelArtist.Tag = true;
                            });
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                _interpreters.Insert(1, tokens[3].Token);
                                comboboxInterpret.SelectedItem = tokens[3].Token;
                                labelArtist.Foreground = Brushes.Red;
                                labelArtist.Tag = false;
                            });
                        }
                    }

                    if (i == 5)     // Album
                    {
                        if (_albums.Contains(tokens[4].Token))
                        {
                            Dispatcher.Invoke(() =>
                            {
                                comboboxAlbum.SelectedItem = tokens[4].Token;
                                labelAlbum.Tag = true;
                            });
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                _albums.Insert(1, tokens[4].Token);
                                comboboxAlbum.SelectedItem = tokens[4].Token;
                                labelAlbum.Foreground = Brushes.Red;
                                labelAlbum.Tag = false;
                            });
                        }
                    }
                }
            });

            #endregion

            #region Fill Albums condtionaly
            var folders = System.Convert.ToInt32(labelFolders.Content);

            if (comboboxInterpret.SelectedItem.ToString() != "NA" && folders > 0)
            {
                List<string> allDirs = Helpers.GetDirectories(startDirectory, false);
                _albums.Clear();

                foreach (string dir in allDirs)
                {
                    if (_ignores.Contains(dir.Split('\\').Last()) != true)
                    {
                        System.Console.WriteLine(dir);

                        DirectoryInfo di = new DirectoryInfo(dir);

                        Console.WriteLine(di.Name);

                        _albums.Add(di.Name);
                    }
                }
                comboboxAlbum.SelectedIndex = 0;
            }

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

            if (checkboxSampler.IsChecked == true)
                ImportSampler();
            else
                ImportAlbum();

        }

        private void ImportAlbum()
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
            }

            List<FileDetails> fileDetailsList = datagridFilelist.ItemsSource as List<FileDetails>;

            for (int i = 0; i <= comboboxAlbum.Items.Count - 1; i++)
            {
                comboboxAlbum.SelectedIndex = i;

                if (_ignores.Contains(comboboxAlbum.Text))  // == "NA" || comboboxAlbum.Text == "_Images")
                    continue;

                List<MP3Record> mp3List = mp3Records(fileDetailsList, false);

                // save records
                if ((bool)checkboxTestimport.IsChecked == true)
                    recordsAffected += DataGetSet.SaveTestRecord(mp3List);
                else
                    recordsAffected += DataGetSet.SaveRecord(mp3List);

                labelSuccess.Content = $"{recordsAffected}";
                labelFailed.Content = $"{mp3List.Count - recordsAffected}";
            }

            DateTime t2 = DateTime.Now;
            statusbarProgress.Visibility = Visibility.Hidden;
            statusbarDauer.Content = (t2 - t1).Milliseconds.ToString() + " ms";

            labelSuccess.Content = $"{recordsAffected}";
            labelFailed.Content = $"{datagridFilelist.Items.Count - recordsAffected}";

            buttonImport.IsEnabled = true;
        }

        private void ImportSampler()
        {
            int importFailed = 0;
            int importSuccess = 0;
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
            }

            for (int i = 0; i <= comboboxAlbum.Items.Count - 1; i++)
            {
                comboboxAlbum.SelectedIndex = i;

                if (comboboxAlbum.Text == "NA")
                    continue;

                List<MP3Record> mp3List = mp3Records(_fileDetails, true);

                // save records
                if ((bool)checkboxTestimport.IsChecked == true)
                    recordsAffected += DataGetSet.SaveTestRecord(mp3List);
                else
                    recordsAffected += DataGetSet.SaveRecord(mp3List);

                DateTime t2 = DateTime.Now;
                statusbarProgress.Visibility = Visibility.Hidden;
                statusbarDauer.Content = (t2 - t1).Milliseconds.ToString() + " ms";

                labelSuccess.Content = $"{recordsAffected}";
                labelFailed.Content = $"{_fileDetails.Count - recordsAffected}";

                var lastID = DataGetSet.GetLastID("tSongsTest");
                Debug.Print($"Import success = {importSuccess}, failed={importFailed}, lastId={lastID}");
            }

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
                mp3.MD5 = Helpers.MD5(mp3.Path + mp3.FileName);

                var ar = item.File.Split('-');
                mp3.Titel = ar[1].Trim();

                //if (isSampler == true)
                //    mp3.Artist = ar[0].Trim();
                //else
                mp3.Artist = ar[0].Trim();

                mp3List.Add(mp3);
            }

            return mp3List;
        }

        private async Task CreateNewList()
        {
            string startFolder = @"\\win2k16dc01\FS012";
            string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\win2k16dc01_FS012.txt";

            buttonList.IsEnabled = false;

            await Task.Run(() =>
            {
                string[] allfiles = Directory.GetFileSystemEntries(startFolder, "*.mp3", SearchOption.AllDirectories);


                File.WriteAllLines(documents, allfiles, Encoding.UTF8);

            });

            buttonList.IsEnabled = true;

        }
        #endregion

    }
}
