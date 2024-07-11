namespace ChiropracticApi.Dtos
{
    public class RoleDto
    {
        public int IdRole { get; set; }
        public string Description_Role { get; set; } = string.Empty;
    }

    public class ServiceDto
    {
        public int IdService { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float Price { get; set; }
        public int Duration { get; set; }
        public int active { get; set; }
    }

    public class UserDto
    {
        public int IdUsuario { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Last_Name { get; set; } = string.Empty;
        public int? Age { get; set; }
        public int? Gender { get; set; }
        public string Phone { get; set; } = string.Empty;
        public DateTime Last_Login { get; set; } = DateTime.Now;
        public int Role_idrole { get; set; }
    }
    public class CreateUserDto
    {
        public string Email { get; set; }= string.Empty;
        public string Password { get; set; }= string.Empty;
        public string Name { get; set; }= string.Empty;
        public string Last_Name { get; set; }= string.Empty;
        public int? Age { get; set; }
        public int? Gender { get; set; }
        public string Phone { get; set; } = string.Empty;
        public DateTime Last_Login { get; set; } = DateTime.Now;
        public int Role_idrole { get; set; }
    }

    public class ImageDto
    {
        public int IdImage { get; set; }
        public string Url { get; set; } = string.Empty;
        public int Service_idservice { get; set; }
    }

    public class UserAppointmentDto
    {
        public int IdUser_Appointment { get; set; }
        public int UserIdUsuario { get; set; }
        public UserDto User { get; set; } 
        public int Appointment_IdAppointment { get; set; }
        public int ServiceIdService { get; set; }
        public ServiceDto? Service { get; set; } 
    }

    public class AppointmentHistoryDto
    {
        public int IdAppointmentHistory { get; set; }
        public int Status { get; set; }
        public int Appointment_IdAppointment { get; set; }
        public AppointmentDto Appointment { get; set; }
    }

    public class AppointmentDto
    {
        public int IdAppointment { get; set; }
        public DateTime Date { get; set; }
        public string Observation { get; set; } = string.Empty;
        public string Allergy { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime? Created_At { get; set; }
        public DateTime? Updated_At { get; set; }
    }
    
}
