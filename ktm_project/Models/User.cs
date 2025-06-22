using System.ComponentModel.DataAnnotations;


namespace ktm_project.Models
{
    public class User
    {

        public string UserId
        {
            get
            {
                string hexTicks = DateTime.Now.Ticks.ToString("X");
                return hexTicks.Substring(hexTicks.Length - 15, 9);
            }
            set { }
        }

        [Display(Name = "User Id")]
        public string ViewId { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [StringLength(100)]
        public string Email { get; set; }
    }
}
