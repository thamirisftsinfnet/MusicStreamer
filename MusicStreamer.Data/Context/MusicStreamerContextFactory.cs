using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Data.Context
{
    public class MusicStreamerContextFactory : IDesignTimeDbContextFactory<MusicStreamerContext>
    {
        public MusicStreamerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MusicStreamerContext>();

            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MusicStreamerDb;Trusted_Connection=True;");

            return new MusicStreamerContext(optionsBuilder.Options);
        }
    }
}
