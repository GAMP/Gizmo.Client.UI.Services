namespace Gizmo.Client
{
    public class DemoUserProfile : IUserProfile
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime Registered { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        public string PostCode { get; set; }
        public int Id { get; set; }
        public Sex Sex { get; set; }
        public UserRoles Role { get; set; }

        public bool IsAdmin => false;

        public bool IsGuest => false;

        public bool IsEnabled { get; set; }
        public bool CanChangePassword { get; set; }

        public void Reset()
        {
            Id = 0;
            Address = string.Empty;
            BirthDate = DateTime.MinValue;
            City = string.Empty;
            Country = string.Empty;
            Email = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            MobilePhone = string.Empty;
            Phone = string.Empty;
            PostCode = string.Empty;
            Sex = Gizmo.Sex.Unspecified;
            Role = Gizmo.UserRoles.None;
            UserName = string.Empty;
            IsEnabled = false;
            CanChangePassword = false;
        }
    }
}
