namespace BlueNest.Core.Entities.RoomModule
{
    public class RoomImage:BaseEntity<int>
    {
        public string PictureUrl { get; set; } = null!;

        public int RoomId { get; set; }

    }
}