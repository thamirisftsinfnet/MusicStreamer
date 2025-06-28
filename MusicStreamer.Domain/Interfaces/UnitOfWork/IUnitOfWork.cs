using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Domain.Interfaces.UnitOfWork
{
    public interface IUnitOfWork
    {
        void BeginTransaction();
        Task CommitAsync();
        void Commit();
    }
}
