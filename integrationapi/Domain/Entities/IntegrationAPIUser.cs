using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class IntegrationAPIUser 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }
        public string? Description { get; set; }
        public string Username { get; set; }
        public string? PasswordHash { get; set; }
        public string? CurrentToken { get; set; }
        public DateTime? LastRequestDate { get; set; }
        public DateTime? TokenExpirationDate { get; set; }
        public int? Active { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public DateTime? DateDeleted { get; set; }
        public int? UserIdCreated { get; set; }
        public int? UserIdUpdated { get; set; }
        public int? UserIdDeleted { get; set; }
    }
}
