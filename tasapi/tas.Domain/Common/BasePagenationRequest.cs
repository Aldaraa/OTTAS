using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Domain.Common
{

    public abstract record BasePagenationRequest
    {

        public int pageIndex { get; set; } = 1;

        public int pageSize { get; set; } = 100;


    }
}
