using Microsoft.EntityFrameworkCore;

namespace Bamboozlers.Classes.AppDbContext;

[PrimaryKey(nameof(BlockedID), nameof(BlockerID))]
public class Block
{
    public Block(int BlockedID, int BlockerID)
    {
        this.BlockedID = BlockedID;
        this.BlockerID = BlockerID;
    }

    public int BlockerID { get; set; }
    public User Blocker { get; set; }
    public int BlockedID { get; set; }
    public User Blocked { get; set; }
}