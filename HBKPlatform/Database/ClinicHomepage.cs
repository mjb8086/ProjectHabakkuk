namespace HBKPlatform.Database;

public class ClinicHomepage : HbkBaseEntity
{
   public int ClinicId { get; set; } 
   public string HpText { get; set; }
   public int TplStyleId { get; set; }
   public int TplVariant { get; set; }
   public PublishStatus PublishStatus { get; set; }
   public int UpdateUserId { get; set; }

   public virtual Clinic Clinic { get; set; }
}

public enum PublishStatus
{
   MaintenanceMode,
   Live
}