

namespace activedirectory.Models
{
     public class ActiveDirectoryUser
    {

        public string Id { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public string Mobile { get; set; }
        public bool IsEnabled { get; set; }
        public string? FN { get; set; } 
        public string? LN{ get; set; }
        public string? Created { get; set; }
        public string? Modified { get; set; }
        public string? Manager { get; set; }
    }
}
