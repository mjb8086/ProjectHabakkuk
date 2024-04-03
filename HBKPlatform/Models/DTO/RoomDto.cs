namespace HBKPlatform.Models.DTO;

public class RoomDto
{
    public int Id { get; set; }
    public int ClinicId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? Img { get; set; }
    public double PricePerUse { get; set; }
}
