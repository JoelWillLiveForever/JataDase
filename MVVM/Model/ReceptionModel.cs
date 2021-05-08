namespace Lab8.MVVM.Model
{
    public class ReceptionModel
    {
        public int Row_Number { get; set; }
        public string DoctorShortName { get; set; }
        public string PatientShortName { get; set; }
        public string ReceptionType { get; set; }
        public Reception Reception_Object { get; set; }
    }
}
