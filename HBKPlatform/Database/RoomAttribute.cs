namespace HBKPlatform.Database;

// Join table for rooms and attributes (many to many)
public class RoomAttribute : HbkBaseEntity
{
        public int RoomId { get; set; }
        public int AttributeId { get; set; }
        public int TenancyId { get; set; }
    
        // EF Navigations
        public virtual Room Room { get; set; }
        public virtual Attribute Attribute { get; set; }
        public virtual Tenancy Tenancy { get; set; }
}