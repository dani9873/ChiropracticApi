namespace ChiropracticApi.Dtos
{
    public class RoleCreateDto
    {
        public string Description_Role { get; set; }
    }

    public class ServiceCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public int Duration { get; set; }
        public int active { get; set; }
    }

    public class UserCreateDto
    {
        public string IdUsuario { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Last_Name { get; set; }
        public int? Age { get; set; }
        public int? Gender { get; set; }
        public string Phone { get; set; }
        public DateTime Last_Login { get; set; } = DateTime.Now;

        public int Role_idrole { get; set; }
    }

    public class ImageCreateDto
    {
        public string Url { get; set; }
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
        public string Observation { get; set; }
        public string Allergy { get; set; }
        public int Status { get; set; }
        public int canceled_by_user { get; set; }
        public int canceled_at { get; set; }
    }
}
