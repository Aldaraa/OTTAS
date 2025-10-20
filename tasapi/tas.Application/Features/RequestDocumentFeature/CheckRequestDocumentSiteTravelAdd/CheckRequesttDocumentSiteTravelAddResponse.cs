using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDocumentFeature.CheckRequestDocumentSiteTravelAdd
{
    public class CheckRequestDocumentSiteTravelAddResponse
    {
        public int Id { get; set; }
        public string? Description { get; set; }

        public string? CurrentAction { get; set; }

        public string? AssignedEmployee { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? DocumentType { get; set; }
        public string? DocumentTag { get; set; }
    }
}
