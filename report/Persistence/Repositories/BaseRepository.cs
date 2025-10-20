using Application.Repositories;
using Domain.Common;
using Microsoft.Extensions.Configuration;
using Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly DataContext Context;

        public BaseRepository(DataContext context, IConfiguration configuration)
        {
            Context = context;
        }
    }
}
