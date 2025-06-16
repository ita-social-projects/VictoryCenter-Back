namespace VictoryCenter.DAL.Entities;

public class Admin
{
    public long Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
