using HBKPlatform.Globals;

namespace HBKPlatform.Models.DTO;

public struct ClientDetailsLite
{
    public string Name { get; set; }
    public int Id { get; set; }
    public int ClinicId { get; set; }
}

public class PracDetailsLite
{
    public string Name { get; set; }
    public int Id { get; set; }
    public int ClinicId { get; set; }
}

public class PracDetailsUac : PracDetailsLite
{
   public bool HasLockout { get; set; } 
}

public struct ClientRecordLite
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public Enums.RecordVisibility Visibility { get; set; }
    public bool IsPriority { get; set; }
}

public struct TreatmentLite
{
    public int Id { get; set; }
    public Enums.TreatmentRequestability Requestability { get; set; }
    public double Cost { get; set; }
    public string Title { get; set; }
}

public struct TimeslotLite
{
    public int Id { get; set; }
    public int WeekNum { get; set; }
    public string Description { get; set; }
}

public class PracBookingFormModel
{
    public string TimeslotWeekNum { get; set; }
    public int TreatmentId { get; set; }
    public int ClientId { get; set; }

    public int[] ParseTsWeekNum()
    {
        return TimeslotWeekNum.Split('|').Select(int.Parse).ToArray();
    }
}

public class CancelAppointmentFormModel
{
    public string Reason { get; set; }
}
