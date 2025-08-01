using System.ComponentModel.DataAnnotations;

namespace InventoryManagementLibrary.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }
         
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Email { get; set; }


        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
