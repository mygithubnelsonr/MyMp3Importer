﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyMp3Importer.DAL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MyJukeboxEntities : DbContext
    {
        public MyJukeboxEntities()
            : base("name=MyJukeboxEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tActionLog> tActionLogs { get; set; }
        public virtual DbSet<tColumn> tColumns { get; set; }
        public virtual DbSet<tFileInfo> tFileInfos { get; set; }
        public virtual DbSet<tGenre> tGenres { get; set; }
        public virtual DbSet<tMD5> tMD5 { get; set; }
        public virtual DbSet<tMedia> tMedias { get; set; }
        public virtual DbSet<tPlaylist> tPlaylists { get; set; }
        public virtual DbSet<tPLentry> tPLentries { get; set; }
        public virtual DbSet<tSetting> tSettings { get; set; }
        public virtual DbSet<vPlaylistSong> vPlaylistSongs { get; set; }
        public virtual DbSet<vSong> vSongs { get; set; }
        public virtual DbSet<vSongsLong> vSongsLongs { get; set; }
        public virtual DbSet<tCatalog> tCatalogs { get; set; }
        public virtual DbSet<tFileInfos_tst> tFileInfos_tst { get; set; }
        public virtual DbSet<tInfos_tst> tInfos_tst { get; set; }
        public virtual DbSet<tMD5_tst> tMD5_tst { get; set; }
        public virtual DbSet<tCatalogs_tst> tCatalogs_tst { get; set; }
        public virtual DbSet<tGenres_tst> tGenres_tst { get; set; }
        public virtual DbSet<tMedia_tst> tMedia_tst { get; set; }
        public virtual DbSet<tSongs_tst> tSongs_tst { get; set; }
        public virtual DbSet<tInfo> tInfos { get; set; }
        public virtual DbSet<tSong> tSongs { get; set; }
    }
}
