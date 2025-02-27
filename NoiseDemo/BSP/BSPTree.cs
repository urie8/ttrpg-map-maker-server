public class BSPTree
{
    // First we create the root of the BSPTree.
    // This is the starting point of the algorithm which then splits and branches out
    private BSPTreeNode root;

    public BSPTree(Rectangle initialSpace, float minRoomWidth, float minRoomHeight)
    {
        root = BuildTree(initialSpace, minRoomWidth, minRoomHeight);
    }

    // This is the main part of the BSP algorithm
    // BuildTree gets called recursively until null is returned ending the recursion loop for that branch of the tree.
    private BSPTreeNode BuildTree(Rectangle space, float minRoomWidth, float minRoomHeight)
    {
        // This makes sure the rooms aren't split into too small chunks.
        // If they are too small to split it returns null ending the recursion.
        if (!space.IsLargeEnough(minRoomWidth, minRoomHeight))
        {
            return null;
        }

        // Randomize a number between 0 & 1
        // If the number is 0 the boolean is set to true
        Random rnd = new Random();
        bool splitHorizontally = rnd.Next(2) == 0;

        float splitLine;
        // Split room horizontally or vertically depending on the boolean
        if (splitHorizontally)
        {
            splitLine = space.Y + (space.Height / 2); // Horizontal split
        }
        else
        {
            splitLine = space.X + (space.Width / 2); // Vertical split
        };

        // Create a Left and Right room following the split
        Rectangle leftRoom, rightRoom;

        // Set the Left and Right rooms variables according to where they are split from
        if (splitHorizontally)
        {
            leftRoom = new Rectangle(space.X, space.Y, space.Width, splitLine - space.Y);
            rightRoom = new Rectangle(space.X, splitLine, space.Width, space.Y + space.Height - splitLine);
        }
        else
        {
            leftRoom = new Rectangle(space.X, space.Y, splitLine - space.X, space.Height);
            rightRoom = new Rectangle(splitLine, space.Y, space.X + space.Width - splitLine, space.Height);
        }

        // Ensure both rooms are large enough before creating subtrees
        if (leftRoom.IsLargeEnough(minRoomWidth, minRoomHeight) && rightRoom.IsLargeEnough(minRoomWidth, minRoomHeight))
        {
            // Create a new node and recursively build sub-trees
            BSPTreeNode node = new BSPTreeNode(space);

            node.Left = BuildTree(leftRoom, minRoomWidth, minRoomHeight);
            node.Right = BuildTree(rightRoom, minRoomWidth, minRoomHeight);

            return node;
        }

        // If the rooms are not large enough, return null
        return new BSPTreeNode(space); // Return the current room as a leaf node
    }

    // CHATGPT STUFF
    // Collect rooms from the BSP tree using pre-order traversal
    public void CollectRooms(BSPTreeNode node, List<Rectangle> rooms)
    {
        if (node == null) return;

        // Add the current room to the list
        rooms.Add(node.Room);

        // Recurse into left and right subtrees
        CollectRooms(node.Left, rooms);
        CollectRooms(node.Right, rooms);
    }

    // Getter for the root of the tree
    public BSPTreeNode GetRoot()
    {
        return root;
    }
}