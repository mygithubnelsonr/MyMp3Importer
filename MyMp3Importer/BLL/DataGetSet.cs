using MyMp3Importer.Common;
using MyMp3Importer.DAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyMp3Importer.BLL
{
    public class DataGetSet
    {
        public static void SetRating(int id, int rating)
        {
            var context = new MyJukeboxEntities();
            var result = context.tInfos.SingleOrDefault(s => s.ID_Song == id);

            if (result != null)
            {
                result.Rating = rating;
                context.SaveChanges();
            };
        }

        public static void SetColumnWidth(string name, int width)
        {
            var context = new MyJukeboxEntities();
            var result = context.tColumns.SingleOrDefault(n => n.Name == name);

            if (result != null)
                result.Width = width;
            else
                context.tColumns.Add(new tColumn { Name = name, Width = width });

            context.SaveChanges();
        }

        #region FileImporter

        //public static List<string> GetGenres()
        //{
        //    List<string> genres = null;
        //    using (var context = new MyJukeboxEntities())
        //    {
        //        genres = context.tGenres.Select(g => g.Name).ToList();
        //        return genres;
        //    }
        //}

        //public static List<string> GetCatalogs()
        //{
        //    List<string> catalogues = null;

        //    using (var context = new MyJukeboxEntities())
        //    {
        //        catalogues = context.tCatalogs.Select(c => c.Name).ToList();
        //        return catalogues;
        //    }
        //}

        //public static List<string> GetMedia()
        //{
        //    List<string> medias = null;

        //    using (var context = new MyJukeboxEntities())
        //    {
        //        medias = context.tMedias.Select(m => m.Type).ToList();
        //        return medias;
        //    }
        //}

        //public static List<string> GetInterpreters()
        //{
        //    List<string> interpreters = null;

        //    using (var context = new MyJukeboxEntities())
        //    {
        //        interpreters = context.vSongs
        //                            .Select(i => i.Interpret)
        //                            .Distinct().OrderBy(i => i).ToList();

        //        return interpreters;
        //    }
        //}

        //public static List<string> GetAlbums()
        //{
        //    List<string> albums = null;

        //    using (var context = new MyJukeboxEntities())
        //    {
        //        albums = context.tAlbums.Select(a => a.Name).ToList();
        //        return albums;
        //    }
        //}

        public static async Task<List<string>> GetGenresAsync()
        {
            List<string> genres = null;
            using (var context = new MyJukeboxEntities())
            {
                await Task.Run(() =>
                {
                    genres = context.tGenres.Select(g => g.Name).ToList();

                });
                return genres;
            }
        }

        public static async Task<List<string>> GetCatalogsAsync()
        {
            List<string> catalogues = null;

            using (var context = new MyJukeboxEntities())
            {
                await Task.Run(() =>
                {
                    catalogues = context.tCatalogs.Select(c => c.Name).ToList();
                });
                return catalogues;
            }
        }

        public static async Task<List<string>> GetMediaAsync()
        {
            List<string> medias = null;

            using (var context = new MyJukeboxEntities())
            {
                await Task.Run(() =>
                {
                    medias = context.tMedias.Select(m => m.Type).ToList();
                });
                return medias;
            }
        }

        public static async Task<List<string>> GetInterpretersAsync()
        {
            List<string> interpreter = null;

            using (var context = new MyJukeboxEntities())
            {
                await Task.Run(() =>
                {
                    interpreter = context.vSongs
                                    .Select(i => i.Interpret)
                                    .Distinct().OrderBy(i => i).ToList();
                });

                return interpreter;
            }
        }

        #endregion

        public static int CreateCatalog(string catalog)
        {
            int id = -1;

            var context = new MyJukeboxEntities();

            var catalogExist = context.tCatalogs
                                    .Where(c => c.Name == catalog)
                                    .FirstOrDefault();

            if (catalogExist == null)
            {
                context.tCatalogs
                    .Add(new tCatalog { Name = catalog });
                context.SaveChanges();

                id = GetLastID("tCatalogs");
            }

            return id;
        }

        public static int CreateGenre(string genre)
        {
            int id = -1;

            var context = new MyJukeboxEntities();

            var result = context.tGenres
                                    .Where(g => g.Name == genre)
                                    .FirstOrDefault();

            if (result == null)
            {
                context.tGenres
                    .Add(new tGenre { Name = genre });
                context.SaveChanges();

                id = GetLastID("tGenres");
            }

            return id;
        }

        public static int SaveRecord(List<MP3Record> mP3Records)
        {
            int recordsImporteds = 0;

            Logging.Flush();

            foreach (MP3Record record in mP3Records)
            {
                recordsImporteds += SetRecord(record);
            }

            return recordsImporteds;
        }

        public static int SaveTestRecord(List<MP3Record> mP3Records)
        {
            int recordsImporteds = 0;

            Logging.Flush();

            foreach (MP3Record record in mP3Records)
            {
                recordsImporteds += SetTestRecord(record);
            }

            return recordsImporteds;
        }

        public static bool TruncateTableQueries()
        {
            try
            {
                var context = new MyJukeboxEntities();
                var result = context.Database.ExecuteSqlCommand("truncate table [tQueries]");

                return true;
            }
            catch (Exception ex)
            {
                Debug.Print($"TruncateTableImportTest_Error: {ex.Message}");
                return false;
            }

        }

        public static bool TruncateTestTables()
        {
            try
            {
                int result = -1;
                var context = new MyJukeboxEntities();
                result = context.Database.ExecuteSqlCommand("truncate table [tst].[tSongs]");
                result = context.Database.ExecuteSqlCommand("truncate table [tst].[tFileInfos]");
                result = context.Database.ExecuteSqlCommand("truncate table [tst].[tInfos]");
                result = context.Database.ExecuteSqlCommand("truncate table [tst].[tMD5]");

                return true;
            }
            catch (Exception ex)
            {
                Debug.Print($"TruncateTableImportTest_Error: {ex.Message}");
                return false;
            }
        }

        private static bool MD5Exist(string MD5, bool testmode = false)
        {
            object result = null;

            var context = new MyJukeboxEntities();

            if (testmode == false)
            {
                result = context.tMD5
                                .Where(m => m.MD5 == MD5).FirstOrDefault();

            }
            else
            {
                result = context.tMD5_tst
                                .Where(m => m.MD5 == MD5).FirstOrDefault();
            }

            if (result != null)
            {
                Debug.Print($"title allready exist! (MD5={result})");
                return true;
            }
            else
            {
                return false;
            }
        }

        private static int SaveTestRecord(MP3Record record)
        {
            int recordsImported = 0;

            recordsImported += SetTestRecord(record);

            return recordsImported;
        }

        private static int SaveRecord(MP3Record record)
        {
            int recordsImported = 0;


            var exist = MD5Exist(record.MD5);

            if (MD5Exist(record.MD5) == false)
            {
                recordsImported += SetRecord(record);
            }
            return recordsImported;
        }

        private static int SetRecord(MP3Record mp3Record)
        {
            int lastSongID = -1;

            if (MD5Exist(mp3Record.MD5, false) == false)
            {
                try
                {
                    var context = new MyJukeboxEntities();
                    // tsongs data
                    var songs = new tSong();
                    songs.Album = mp3Record.Album;
                    songs.Artist = mp3Record.Artist;
                    songs.Titel = mp3Record.Titel;
                    songs.Pfad = mp3Record.Path;
                    songs.FileName = mp3Record.FileName;
                    songs.ID_Genre = mp3Record.Genre;
                    songs.ID_Catalog = mp3Record.Catalog;
                    songs.ID_Media = mp3Record.Media;
                    context.tSongs.Add(songs);
                    context.SaveChanges();

                    lastSongID = GetLastID("tSongs");

                    // tmd5 data
                    var md5 = new tMD5();
                    md5.MD5 = mp3Record.MD5;
                    md5.ID_Song = lastSongID;
                    context.tMD5.Add(md5);
                    context.SaveChanges();

                    // tfileinfo data
                    var file = new tFileInfo();
                    file.FileDate = mp3Record.FileDate;
                    file.FileSize = mp3Record.FileSize;
                    file.ImportDate = DateTime.Now;
                    file.ID_Song = lastSongID;
                    context.tFileInfos.Add(file);
                    context.SaveChanges();

                    // tinfos data
                    var info = new tInfo();
                    info.Sampler = mp3Record.IsSample;
                    info.ID_Song = lastSongID;
                    context.tInfos.Add(info);
                    context.SaveChanges();

                    Logging.Log("1 record added");
                    return 1;
                }
                catch (Exception ex)
                {
                    Debug.Print($"SetNewTestRecord_Error: {ex.Message}");
                    Logging.Log(ex.Message);
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        private static int SetTestRecord(MP3Record mp3Record)
        {
            int lastSongID = -1;

            if (MD5Exist(mp3Record.MD5, true) == false)
            {
                try
                {
                    var context = new MyJukeboxEntities();
                    // tsongs data
                    var songs = new tSongs_tst();
                    songs.Album = mp3Record.Album;
                    songs.Artist = mp3Record.Artist;
                    songs.Titel = mp3Record.Titel;
                    songs.Pfad = mp3Record.Path;
                    songs.FileName = mp3Record.FileName;
                    songs.ID_Genre = mp3Record.Genre;
                    songs.ID_Catalog = mp3Record.Catalog;
                    songs.ID_Media = mp3Record.Media;
                    context.tSongs_tst.Add(songs);
                    context.SaveChanges();

                    lastSongID = GetLastID("tSongs_tst");

                    // tmd5 data
                    var md5 = new tMD5_tst();
                    md5.MD5 = mp3Record.MD5;
                    md5.ID_Song = lastSongID;
                    context.tMD5_tst.Add(md5);
                    context.SaveChanges();

                    // tfileinfo data
                    var file = new tFileInfos_tst();
                    file.FileDate = mp3Record.FileDate;
                    file.FileSize = mp3Record.FileSize;
                    file.ImportDate = DateTime.Now;
                    file.ID_Song = lastSongID;
                    context.tFileInfos_tst.Add(file);
                    context.SaveChanges();

                    // tinfos data
                    var info = new tInfos_tst();
                    info.Sampler = mp3Record.IsSample;
                    info.ID_Song = lastSongID;
                    context.tInfos_tst.Add(info);
                    context.SaveChanges();

                    Logging.Log("1 record added");
                    return 1;
                }
                catch (Exception ex)
                {
                    Debug.Print($"SetNewTestRecord_Error: {ex.Message}");
                    Logging.Log(ex.Message);
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        #region Parameters
        public static List<Setting> GetParameters()
        {
            List<Setting> settings = new List<Setting>();

            try
            {
                using (var context = new MyJukeboxEntities())
                {
                    var result = context.tSettings.Select(s => s).ToList();
                    foreach (var s in result)
                    {
                        settings.Add(new Setting { ID = s.ID, Name = s.Name, Value = s.Value, Editable = s.Editable }); ;
                    }
                }

                return settings;
            }
            catch
            {
                return null;
            }
        }

        public static void SaveParameters(List<Setting> settings)
        {
            var context = new MyJukeboxEntities();

            foreach (Setting setting in settings)
            {
                var result = context.tSettings.SingleOrDefault(p => p.Name == setting.Name);
                result.Value = setting.Value;
            }

            context.SaveChanges();

        }
        #endregion

        #region Generell
        public static int GetCatalogFromString(string catalog)
        {
            List<int> catalogs;
            try
            {
                var context = new MyJukeboxEntities();
                catalogs = context.tCatalogs
                            .Where(c => c.Name == catalog)
                            .Select(c => c.ID).ToList();

                return catalogs[0];
            }
            catch
            {
                return -1;
            }
        }

        public static int GetGenreFromString(string genre)
        {
            List<int> genres;
            var context = new MyJukeboxEntities();
            genres = context.tGenres
                        .Where(g => g.Name == genre)
                        .Select(g => g.ID).ToList();

            return genres[0];
        }

        internal static int GetMediaIDByType(string type)
        {
            try
            {
                var context = new MyJukeboxEntities();

                var media = context.tMedias
                                .Where(m => m.Type == type).FirstOrDefault();
                return media.ID;
            }
            catch
            {
                return -1;
            }
        }

        public static int GetLastID(string tableName)
        {
            int lastId = -1;

            try
            {
                var context = new MyJukeboxEntities();

                if (tableName == "tGenres")
                {
                    var result = context.tGenres
                                    .Select(n => n.ID).Count();

                    if (result != 0)
                        lastId = context.tGenres.Max(n => n.ID);
                }

                if (tableName == "tGenres_tst")
                {
                    var result = context.tGenres_tst
                                    .Select(n => n.ID).Count();

                    if (result != 0)
                        lastId = context.tGenres_tst.Max(n => n.ID);
                }

                if (tableName == "tCatalogs")
                {
                    var result = context.tCatalogs
                                    .Select(n => n.ID).Count();

                    if (result != 0)
                        lastId = context.tCatalogs.Max(n => n.ID);
                }

                if (tableName == "tSongs")
                {
                    var result = context.tSongs
                                    .Select(n => n.ID).Count();

                    if (result != 0)
                        lastId = context.tSongs.Max(n => n.ID);
                }

                if (tableName == "tSongs_tst")
                {
                    var result = context.tSongs_tst
                                    .Select(n => n.ID).Count();

                    if (result != 0)
                        lastId = context.tSongs_tst.Max(n => n.ID);
                }

                return lastId;
            }
            catch
            {
                return -1;
            }
        }

        public static async Task RefillMD5Table()
        {
            List<tMD5> rec = new List<tMD5>();
            using (var context = new MyJukeboxEntities())
            {
                await Task.Run(() =>
                {
                    var result = context.tSongs
                                    .OrderBy(s => s.ID)
                                    .Select(s => new { s.ID, s.Pfad, s.FileName });

                    foreach (var s in result)
                    {
                        string hash = Helpers.MD5($"{s.Pfad}{s.FileName}");
                        Debug.Print($"ID_Song={s.ID}, md5={hash}");


                        rec.Add(new tMD5 { ID_Song = s.ID, MD5 = hash });

                    }

                    context.tMD5.AddRange(rec);
                    context.SaveChanges();
                });
            }
        }

        //public static MP3Record GetRecordInfo(string startDirectory)
        //{
        //    MP3Record record = null;

        //    // no special import
        //    string[] arTmp = startDirectory.Split('\\');

        //    if (arTmp.Length < 5)
        //        return record;

        //    List<int> media;
        //    string type = arTmp[arTmp.Length - 3];
        //    var context = new MyJukeboxEntities();
        //    media = context.tMedias
        //                .Where(m => m.Type == type)
        //                .Select(m => m.ID).ToList();

        //    try
        //    {
        //        record = new MP3Record();
        //        record.Album = arTmp[arTmp.Length - 1];
        //        record.Artist = arTmp[arTmp.Length - 2];
        //        record.Media = media[0];
        //        //record.Genre = arTmp[arTmp.Length - 5];
        //        //record.Catalog = arTmp[arTmp.Length - 4];

        //        return record;
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageBox.Show("Media Type not found!", ex.Message);
        //        return null;
        //    }
        //}

        #endregion
    }
}
