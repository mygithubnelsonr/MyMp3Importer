﻿using MyMp3Importer.Common;
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
        private static void WriteLog(string entry)
        {
            var log = LogList.Instance;
            log.Write(entry);
        }

        #region FileImporter

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

        public static List<string> GetGenres()
        {
            List<string> genres = null;
            using (var context = new MyJukeboxEntities())
            {
                genres = context.tGenres.Select(g => g.Name).ToList();
                return genres;
            }
        }

        public static List<string> GetCatalogs()
        {
            List<string> catalogues = null;

            using (var context = new MyJukeboxEntities())
            {
                catalogues = context.tCatalogs.Select(c => c.Name).ToList();
                return catalogues;
            }
        }

        public static int GetCatalogId(string catalog)
        {
            using (var context = new MyJukeboxEntities())
            {
                var result = context.tCatalogs
                            .Select(c => c)
                            .Where(c => c.Name == catalog);

                int id = (from pair in result
                          where pair.Name == catalog
                          select pair.ID).FirstOrDefault();

                return id;
            }
        }

        public static string GetCatalog(int catalog)
        {
            using (var context = new MyJukeboxEntities())
            {
                var result = context.tCatalogs
                            .Select(c => c)
                            .Where(c => c.ID == catalog);

                string name = (from pair in result
                               where pair.ID == catalog
                               select pair.Name).FirstOrDefault();

                return name;
            }
        }

        public static int GetGenreId(string genre)
        {
            using (var context = new MyJukeboxEntities())
            {
                var result = context.tGenres
                            .Select(c => c)
                            .Where(c => c.Name == genre);

                int id = (from pair in result
                          where pair.Name == genre
                          select pair.ID).FirstOrDefault();

                return id;
            }
        }

        public static string GetGenre(int genre)
        {
            using (var context = new MyJukeboxEntities())
            {
                var result = context.tGenres
                            .Select(c => c)
                            .Where(c => c.ID == genre);

                string name = (from pair in result
                               where pair.ID == genre
                               select pair.Name).FirstOrDefault();

                return name;
            }
        }

        public static int GetMediaId(string media)
        {
            using (var context = new MyJukeboxEntities())
            {
                var result = context.tMedias
                            .Select(c => c)
                            .Where(c => c.Type == media);

                int id = (from pair in result
                          where pair.Type == media
                          select pair.ID).FirstOrDefault();

                return id;
            }
        }

        public static string GetMedia(int media)
        {
            using (var context = new MyJukeboxEntities())
            {
                var result = context.tMedias
                            .Select(c => c)
                            .Where(c => c.ID == media);

                string name = (from pair in result
                               where pair.ID == media
                               select pair.Type).FirstOrDefault();

                return name;
            }
        }

        public static List<string> GetMedias()
        {
            List<string> medias = null;

            using (var context = new MyJukeboxEntities())
            {
                medias = context.tMedias.Select(m => m.Type).ToList();
                return medias;
            }
        }

        #endregion

        public static int CreateCatalog(string catalog)
        {
            int idDbo = -1;
            int idTst = -1;

            var context = new MyJukeboxEntities();

            #region create new catalog on [dbo]
            var catalogDboExist = context.tCatalogs
                                    .Where(c => c.Name == catalog)
                                    .FirstOrDefault();

            if (catalogDboExist == null)
            {
                context.tCatalogs
                    .Add(new tCatalog { Name = catalog });
                context.SaveChanges();

                idDbo = GetLastID("tCatalogs");
            }
            #endregion

            #region create new catalog on [tst]
            var catalogTstExist = context.tCatalogs
                                    .Where(c => c.Name == catalog)
                                    .FirstOrDefault();

            if (catalogTstExist == null)
            {
                context.tCatalogs
                    .Add(new tCatalog { Name = catalog });
                context.SaveChanges();

                idTst = GetLastID("tCatalogs");
            }
            #endregion

            if (idDbo == idTst)
                return idDbo;
            else
                return -1;
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

            foreach (MP3Record record in mP3Records)
            {
                recordsImporteds += SetRecord(record);
            }

            var logs = LogList.Instance;
            var entries = logs.Get();

            return recordsImporteds;
        }

        public static int SaveTestRecord(List<MP3Record> mP3Records)
        {
            int recordsImporteds = 0;

            foreach (MP3Record record in mP3Records)
            {
                recordsImporteds += SetTestRecord(record);
            }

            return recordsImporteds;
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

                var md5 = (tMD5)result;

                WriteLog($"title allready exist! SongID={md5.ID_Song}, MD5={md5.MD5})");
                Debug.Print($"title allready exist! (MD5={result})");
                return true;
            }
            else
            {
                return false;
            }
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

                    return 1;
                }
                catch (Exception ex)
                {
                    Debug.Print($"SetNewRecord_Error: {ex.Message}");
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
                    // tsongs_tst data
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
                    //var md5 = new tMD5_tst();
                    //md5.MD5 = mp3Record.MD5;
                    //md5.ID_Song = lastSongID;
                    //context.tMD5_tst.Add(md5);
                    //context.SaveChanges();

                    // tfileinfo data
                    //var file = new tFileInfos_tst();
                    //file.FileDate = mp3Record.FileDate;
                    //file.FileSize = mp3Record.FileSize;
                    //file.ImportDate = DateTime.Now;
                    //file.ID_Song = lastSongID;
                    //context.tFileInfos_tst.Add(file);
                    //context.SaveChanges();

                    // tinfos data
                    //var info = new tInfos_tst();
                    //info.Sampler = mp3Record.IsSample;
                    //info.ID_Song = lastSongID;
                    //context.tInfos_tst.Add(info);
                    //context.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    Debug.Print($"SetNewTestRecord_Error: {ex.Message}");
                    return 0;
                }
            }
            else
            {
                return 0;
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

    }
}
