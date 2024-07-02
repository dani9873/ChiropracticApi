namespace ChiropracticApi.Models
{
    public class Role
    {
        public int IdRole { get; set; }
        public string description_role { get; set; }
        public List<User> Users { get; set; }
    }

    public class Service
    {
        public int IdService { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public int Duration { get; set; }
        public int Active { get; set; }
        public List<Image> Images { get; set; }
        public List<UserAppointment> UserAppointments { get; set; }
    }

    public class Login
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class User
    {
        public int IdUsuario { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Last_Name { get; set; }
        public int Age { get; set; }
        public int Gender { get; set; }
        public string Phone { get; set; }
        public DateTime Last_Login { get; set; } = DateTime.Now;
        public DateTime? Created_At { get; set; }
        public DateTime? Updated_At { get; set; }
        public int Role_idrole { get; set; }
        public Role Role { get; set; }
        public List<UserAppointment> UserAppointments { get; set; }
    }

    public class Image
    {
        public int IdImage { get; set; }
        public string Url { get; set; }
        public int Service_idservice { get; set; }
        public Service Service { get; set; }
    }

    public class Appointment
    {
        public int IdAppointment { get; set; }
        public DateTime Date { get; set; }
        public string Observation { get; set; }
        public string Allergy { get; set; }
        public int Status { get; set; }
        public int Canceled_By_User { get; set; }
        public DateTime? Canceled_At { get; set; }
        public DateTime? Created_At { get; set; }
        public DateTime? Updated_At { get; set; }
        public List<UserAppointment> UserAppointments { get; set; }
        public List<AppointmentHistory> appointment_history { get; set; }
    }

    public class UserAppointment
    {
        public int IdUser_Appointment { get; set; }
        public int User_idusuario { get; set; }
        public User User { get; set; }
        public int Appointment_idappointment { get; set; }
        public Appointment Appointment { get; set; }
        public int Service_idservice { get; set; }
        public Service Service { get; set; }
        public DateTime? Checked_In { get; set; }
        public DateTime? Created_At { get; set; }
        public DateTime? Updated_At { get; set; }
    }

    public class AppointmentHistory
    {
        public int Idappointment_history { get; set; }
        public int Status { get; set; }
        public int Appointment_idappointment { get; set; }
        public Appointment Appointment { get; set; }
        public DateTime? Created_At { get; set; }
        public DateTime? Updated_At { get; set; }
    }
}
