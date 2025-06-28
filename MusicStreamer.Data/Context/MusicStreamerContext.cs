using Microsoft.EntityFrameworkCore;
using MusicStreamer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Data.Context
{
    public class MusicStreamerContext : DbContext
    {
        public MusicStreamerContext(DbContextOptions<MusicStreamerContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Band> Bands { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Music> Musics { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<PlaylistMusic> PlaylistMusics { get; set; }
        public DbSet<UserFavoriteMusic> UserFavoriteMusics { get; set; }
        public DbSet<UserFavoriteBand> UserFavoriteBands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
            });


            modelBuilder.Entity<Band>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.Name);
            });


            modelBuilder.Entity<Album>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.HasOne(e => e.Band)
                      .WithMany(b => b.Albums)
                      .HasForeignKey(e => e.BandId);
            });


            modelBuilder.Entity<Music>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.HasOne(e => e.Album)
                      .WithMany(a => a.Musics)
                      .HasForeignKey(e => e.AlbumId);
                entity.HasIndex(e => e.Title);
            });

            modelBuilder.Entity<Playlist>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.HasOne(e => e.User)
                      .WithMany(u => u.Playlists)
                      .HasForeignKey(e => e.UserId);
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                      .WithOne(u => u.Subscription)
                      .HasForeignKey<Subscription>(e => e.UserId);
            });


            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);

                modelBuilder.Entity<Transaction>(entity =>
                {
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.Amount).HasPrecision(18, 2);
                    entity.HasOne(e => e.User)
                          .WithMany(u => u.Transactions)
                          .HasForeignKey(e => e.UserId);
                });
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.HasOne(e => e.User)
                      .WithMany(u => u.Transactions)
                      .HasForeignKey(e => e.UserId);
            });

            modelBuilder.Entity<PlaylistMusic>(entity =>
            {
                entity.HasKey(e => new { e.PlaylistId, e.MusicId });
                entity.HasOne(e => e.Playlist)
                      .WithMany(p => p.PlaylistMusics)
                      .HasForeignKey(e => e.PlaylistId);
                entity.HasOne(e => e.Music)
                      .WithMany(m => m.PlaylistMusics)
                      .HasForeignKey(e => e.MusicId);
            });

            modelBuilder.Entity<UserFavoriteMusic>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.MusicId });
                entity.HasOne(e => e.User)
                      .WithMany(u => u.FavoriteMusics)
                      .HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.Music)
                      .WithMany(m => m.UserFavorites)
                      .HasForeignKey(e => e.MusicId);
            });

            modelBuilder.Entity<UserFavoriteBand>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.BandId });
                entity.HasOne(e => e.User)
                      .WithMany(u => u.FavoriteBands)
                      .HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.Band)
                      .WithMany(b => b.UserFavorites)
                      .HasForeignKey(e => e.BandId);
            });
        }
    }
}
