using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Domain.Common
{
    public  abstract record BasePaginationResponse<T>
    {
        public List<T> data { get; set; }

        public int? totalcount { get; set; } = 0;

        public int? currentPage { get; set; } = 0;

        public int? pageSize { get; set; }

    }



    public abstract record BasePaginationObjectResponse<T>
    {
        public T? data { get; set; }

        public int? totalcount { get; set; } = 0;

        public int? currentPage { get; set; } = 0;

        public int? pageSize { get; set; }

    }
}
