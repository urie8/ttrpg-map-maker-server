public class BSPTreeNode
{
    // Wheen a Room is split by BSP it creates a Left & Right sub-room
    // Those Left and Right rooms can then be further split recursively.
    //
    // If the BSP algorithm cannot split the room due to minimum size,
    // the room is considered a Leaf and the recursion ends for that branch of the tree
    public Rectangle Room { get; set; }
    public BSPTreeNode Left { get; set; }
    public BSPTreeNode Right { get; set; }


    public BSPTreeNode(Rectangle room)
    {
        Room = room;
        Left = null;
        Right = null;
    }
}
