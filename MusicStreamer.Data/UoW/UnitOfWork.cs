using Microsoft.EntityFrameworkCore;
using MusicStreamer.Data.Context;
using MusicStreamer.Data.Repositories;
using MusicStreamer.Domain.Interfaces.Repositories;
using MusicStreamer.Domain.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Data.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MusicStreamerContext _context;
        private bool _disposed;

        public UnitOfWork(MusicStreamerContext context)
        {
            _context = context;
        }

        public void BeginTransaction()
        {
            _disposed = false;
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
