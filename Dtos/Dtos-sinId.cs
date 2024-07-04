namespace ChiropracticApi.Dtos
{
    public class RoleCreateDto
    {
        public string Description_Role { get; set; }
        public RoleCreateDto()
    {
        Description_Role = string.Empty;
    }
    }

    public class ServiceCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public int Duration { get; set; }
        public int active { get; set; }
        public ServiceCreateDto()
    {
        Name = string.Empty;
        Description = string.Empty;
    }
    }

    public class UserCreateDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; }= string.Empty;
        public string Name { get; set; }= string.Empty;
        public string Last_Name { get; set; }= string.Empty;
        public int? Age { get; set; }
        public int? Gender { get; set; }
        public string Phone { get; set; }= string.Empty;
        public DateTime? Last_Login { get; set; } = DateTime.Now;
    }

    public class ImageCreateDto
    {
        public string Url { get; set; }= string.Empty;
        public int Service_idservice { get; set; }
    }

    public class UserAppointmentCreateDto
    {
        public int UserIdUsuario { get; set; }
        public int Appointment_IdAppointment { get; set; }
        public int ServiceIdService { get; set; }
    }

    public class AppointmentHistoryCreateDto
    {
        public int Status { get; set; }
        public int Appointment_IdAppointment { get; set; }
    }

    public class AppointmentCreateDto
    {
        public DateTime Date { get; set; }
        public string Observation { get; set; }= string.Empty;
        public string Allergy { get; set; }= string.Empty;
        public int Status { get; set; }
        public int canceled_by_user { get; set; }
        public int canceled_at { get; set; }
    }
}
