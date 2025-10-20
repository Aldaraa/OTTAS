using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace Application.Service
{
    public class PasswordService
    {

        // Hashes the password using bcrypt
        public string HashPassword(string password)
        {
            return  BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string? hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword)) {
                return false;
            }
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
