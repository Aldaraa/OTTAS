using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;

        public UnitOfWork(DataContext context)
        {
            _context = context;
        }
        public Task Save(CancellationToken cancellationToken)
        {
            _context.AuditSave();
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
