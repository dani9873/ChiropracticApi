namespace ChiropracticApi.Models
{
    public class Role
    {
        public int IdRole { get; set; }
        public string description_role { get; set; } = string.Empty;
        public List<User> Users { get; set; }  = new List<User>();
    
    }



    public class Service
    {
        public int IdService { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float Price { get; set; }
        public int Duration { get; set; }
        public int Active { get; set; }
        public List<Image> Images { get; set; } = new List<Image>();
        public List<UserAppointment> UserAppointments { get; set; } = new List<UserAppointment>();
    }

    public class Login
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class User
    {
        public int IdUsuario { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Last_Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public int Gender { get; set; }
        public string Phone { get; set; } = string.Empty;
        public DateTime Last_Login { get; set; } = DateTime.Now;
        public DateTime? Created_At { get; set; }
        public DateTime? Updated_At { get; set; }
        public int Role_idrole { get; set; }
        public required Role Role { get; set; } 
        public List<UserAppointment> UserAppointments { get; set; } = new List<UserAppointment>();
    }

    public class Image
    {
        public int IdImage { get; set; }
        public string Url { get; set; } = string.Empty;
        public int Service_idservice { get; set; }
        public required Service Service { get; set; }
    }

    public class Appointment
    {
        public int IdAppointment { get; set; }
        public DateTime Date { get; set; }
        public string Observation { get; set; } = string.Empty;
        public string Allergy { get; set; } = string.Empty;
        public int Status { get; set; }
        public int Canceled_By_User { get; set; }
        public DateTime? Canceled_At { get; set; }
        public DateTime? Created_At { get; set; }
        public DateTime? Updated_At { get; set; }
        public List<UserAppointment> UserAppointments { get; set; }  = new List<UserAppointment>(); 
        public List<AppointmentHistory> appointment_history { get; set; } = new List<AppointmentHistory>(); 
    }

    public class UserAppointment
    {
        public int IdUser_Appointment { get; set; }
        public int User_idusuario { get; set; }
        public required User User { get; set; } 
        public int Appointment_idappointment { get; set; }
        public required Appointment Appointment { get; set; }
        public int Service_idservice { get; set; }
        public required Service Service { get; set; }
        public DateTime? Checked_In { get; set; }
        public DateTime? Created_At { get; set; }
        public DateTime? Updated_At { get; set; }
    }

    public class AppointmentHistory
    {
        public int Idappointment_history { get; set; }
        public int Status { get; set; }
        public int Appointment_idappointment { get; set; }
        public required Appointment Appointment { get; set; }

        // Constructor que inicializa la propiedad no anulable
    public AppointmentHistory()
    {
        Appointment = new Appointment(); // Asegúrate de que esta inicialización tenga sentido en tu contexto
    }
        public DateTime? Created_At { get; set; }
        public DateTime? Updated_At { get; set; }
    }
}
