public class BSPTree
{
    // First we create the root of the BSPTree.
    // This is the starting point of the algorithm which then splits and branches out
    private BSPTreeNode root;
    private Random rnd = new Random();

    public BSPTree(Rectangle initialSpace, float minRoomWidth, float minRoomHeight)
    {
        root = BuildTree(initialSpace, minRoomWidth, minRoomHeight);
    }


    private bool IsAspectRatioValid(Rectangle room)
    {
        float aspectRatio = room.Width / room.Height;
        return aspectRatio >= 0.2 && aspectRatio <= 3.0;
    }



    // This is the main part of the BSP algorithm
    // BuildTree gets called recursively until null is returned ending the recursion loop for that branch of the tree.
    private BSPTreeNode BuildTree(Rectangle space, float minRoomWidth, float minRoomHeight, int splitCount = 0)
    {
        // This makes sure the rooms aren't split into too small chunks.
        // If they are too small to split it returns as a leaf.
        if (!space.IsLargeEnough(minRoomWidth, minRoomHeight))
        {
            return new BSPTreeNode(space); // If room is too small, it's a leaf node
        }

        // Force splits for the first two levels (splitCount <= 1)
        bool shouldSplit;

        if (splitCount < 2)
        {
            shouldSplit = true; // Ensure the first two splits always happen
        }
        else
        {
            // For subsequent splits, calculate dynamic split chance
            double splitChance = CalculateSplitChance(space, splitCount);
            shouldSplit = rnd.NextDouble() < splitChance; // Split based on calculated chance

            // Log the decision of whether to split the room or not
            Console.WriteLine($"Room size: {space.Width}x{space.Height}, Split chance: {splitChance * 100}%, Random decision: {(shouldSplit ? "Split" : "Skip")}");


        }


        if (shouldSplit)
        {
            // Randomize a number between 0 & 1
            // If the number is 0 the boolean is set to true
            bool splitHorizontally = rnd.Next(2) == 0;
            float splitLine;

            if (splitHorizontally)
            {
                // For horizontal splits, ensure the split line is a multiple of minRoomHeight.
                // Generate a random line between minRoomHeight and (Height - minRoomHeight)
                int start = (int)(space.Y + minRoomHeight);
                int end = (int)(space.Y + space.Height - minRoomHeight);

                // Make sure that there is enough space to split the room
                if (end > start)
                {
                    // Find the nearest multiple of minRoomHeight within the range
                    int range = (end - start) / (int)minRoomHeight;
                    splitLine = rnd.Next(range) * (int)minRoomHeight + start;

                    Console.WriteLine($"splitLine horizontal: {splitLine}");
                }
                else
                {
                    // If the room is too small to split, skip this split
                    Console.WriteLine("Room is too small for a horizontal split, skipping...");
                    return new BSPTreeNode(space); // Return as leaf node
                }
            }
            else
            {
                // For vertical splits, ensure the split line is a multiple of minRoomWidth.
                // Generate a random line between minRoomWidth and (Width - minRoomWidth)
                int start = (int)(space.X + minRoomWidth);
                int end = (int)(space.X + space.Width - minRoomWidth);

                // Make sure that there is enough space to split the room
                if (end > start)
                {
                    // Find the nearest multiple of minRoomWidth within the range
                    int range = (end - start) / (int)minRoomWidth;
                    splitLine = rnd.Next(range) * (int)minRoomWidth + start;

                    Console.WriteLine($"splitLine vertical: {splitLine}");
                }
                else
                {
                    // If the room is too small to split, skip this split
                    Console.WriteLine("Room is too small for a vertical split, skipping...");
                    return new BSPTreeNode(space); // Return as leaf node
                }
            }




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


            // Check if both rooms have a valid aspect ratio
            if (!IsAspectRatioValid(leftRoom) || !IsAspectRatioValid(rightRoom))
            {
                Console.WriteLine("rooms skipped due to invalid aspect ratio: leftRoom: " + leftRoom + ", rightRoom: " + rightRoom);
                return new BSPTreeNode(space); // Skip this split if aspect ratio is invalid
            }


            // Ensure both rooms are large enough before creating subtrees
            if (leftRoom.IsLargeEnough(minRoomWidth, minRoomHeight) && rightRoom.IsLargeEnough(minRoomWidth, minRoomHeight))
            {
                // Create a new node and recursively build sub-trees
                BSPTreeNode node = new BSPTreeNode(space);

                // Create new rectangles with root set to false
                node.Left = BuildTree(leftRoom, minRoomWidth, minRoomHeight, splitCount + 1);
                node.Right = BuildTree(rightRoom, minRoomWidth, minRoomHeight, splitCount + 1);

                return node;
            }
        }

        if (splitCount > 2) // Only allow merging after 2 splits
        {
            MergeSmallRooms(root, minRoomWidth, minRoomHeight);
        }

        // If the rooms are not large enough, return null
        return new BSPTreeNode(space); // Return the current room as a leaf node
    }

    // Method to merge adjacent rooms if they're small enough and after a certain number of splits
    private void MergeSmallRooms(BSPTreeNode node, float minRoomWidth, float minRoomHeight)
    {
        if (node == null) return;

        // If the node has child rooms, attempt merging them
        if (node.Left != null && node.Right != null)
        {
            // Check if both rooms are small enough to merge
            if (node.Left.Room.Width < minRoomWidth && node.Left.Room.Height < minRoomHeight &&
                node.Right.Room.Width < minRoomWidth && node.Right.Room.Height < minRoomHeight)
            {
                // Check for adjacency (rooms must be adjacent either horizontally or vertically)
                if (AreRoomsAdjacent(node.Left.Room, node.Right.Room))
                {
                    // Use random chance to decide if we merge them
                    if (rnd.NextDouble() < 0.5f) // Change varible as needed!
                    {
                        // Merge the rooms by creating a larger room that encompasses both
                        Rectangle mergedRoom = MergeRooms(node.Left.Room, node.Right.Room);

                        // Replace the left and right rooms with the merged room
                        node.Room = mergedRoom;
                        node.Left = null;
                        node.Right = null;

                        Console.WriteLine($"Merged rooms: {node.Left.Room} and {node.Right.Room} into {mergedRoom}");
                    }
                    else
                    {
                        Console.WriteLine("Merge attempt failed due to random chance.");
                    }
                }
            }
        }

        // Recursively call MergeSmallRooms on child nodes
        MergeSmallRooms(node.Left, minRoomWidth, minRoomHeight);
        MergeSmallRooms(node.Right, minRoomWidth, minRoomHeight);
    }

    // Check if two rooms are adjacent either horizontally or vertically
    private bool AreRoomsAdjacent(Rectangle room1, Rectangle room2)
    {
        // Check horizontal adjacency
        bool horizontalAdjacent = (room1.Y == room2.Y && room1.Height == room2.Height &&
                                   (room1.X + room1.Width == room2.X || room2.X + room2.Width == room1.X));

        // Check vertical adjacency
        bool verticalAdjacent = (room1.X == room2.X && room1.Width == room2.Width &&
                                 (room1.Y + room1.Height == room2.Y || room2.Y + room2.Height == room1.Y));

        return horizontalAdjacent || verticalAdjacent;
    }

    // Merge two adjacent rooms into one larger room
    private Rectangle MergeRooms(Rectangle room1, Rectangle room2)
    {
        // Merge rooms by taking the outer bounds of the two rooms
        int x = (int)Math.Min(room1.X, room2.X);
        int y = (int)Math.Min(room1.Y, room2.Y);
        int width = (int)(Math.Max(room1.X + room1.Width, room2.X + room2.Width) - x);
        int height = (int)(Math.Max(room1.Y + room1.Height, room2.Y + room2.Height) - y);

        return new Rectangle(x, y, width, height);
    }

    // Other methods (like CollectRooms, CalculateSplitChance, etc.) remain the same...


    // Method to calculate dynamic split chance based on room size and the number of splits
    private double CalculateSplitChance(Rectangle room, int splitCount)
    {
        // Calculate the room's area
        float area = room.Width * room.Height;

        // Log the area to check if it's being calculated correctly
        Console.WriteLine($"Room size: {room.Width}x{room.Height}, Area: {area}");

        // Define the minimum area and maximum area for scaling split chance
        float minArea = 50; // Minimum area threshold for splitting (adjust as needed)
        float maxArea = 10000; // Maximum area threshold for splitting (adjust as needed)

        // If the room is larger than the maximum area, it has no effect on the split chance (keep it reasonable)
        double sizeFactor;
        if (area <= minArea)
        {
            sizeFactor = 1.0; // Fully eligible to split if it's below the minimum threshold
        }
        else if (area >= maxArea)
        {
            sizeFactor = 1; // Higher chance for large rooms 
        }
        else
        {
            // Calculate the chance based on area (smaller rooms have higher split chance)
            sizeFactor = Math.Max(0, Math.Min(1, (maxArea - area) / (maxArea - minArea)));
        }

        // Define the maximum split chance (100% at the start)
        double maxSplitChance = 1.0; // 100%

        // Exponentially decay the split chance after each split
        double splitFactor = Math.Max(0.1, Math.Pow(0.8, splitCount)); // This will sharply decay after each split


        // Combine both factors to get the final split chance
        double finalSplitChance = sizeFactor * splitFactor;

        // Log the final split chance for debugging
        Console.WriteLine($"Size Factor: {sizeFactor}, Split Factor: {splitFactor}, Final Split Chance: {finalSplitChance * 100}%");

        return finalSplitChance;
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
