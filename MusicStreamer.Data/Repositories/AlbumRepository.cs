using MusicStreamer.Data.Context;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Data.Repositories
{
    public class AlbumRepository : Repository<Album>, IAlbumRepository
    {
        public AlbumRepository(MusicStreamerContext context) : base(context)
        {
        }
    }
}
