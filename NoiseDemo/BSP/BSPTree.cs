using System.Drawing;

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

    private BSPTreeNode BuildTree(Rectangle space, float minRoomWidth, float minRoomHeight, int splitCount = 0)
    {
        // Round the room's X, Y, Width, and Height to the nearest multiple of 64
        space.X = RoundToNearest64(space.X);
        space.Y = RoundToNearest64(space.Y);
        space.Width = RoundToNearest64(space.Width);
        space.Height = RoundToNearest64(space.Height);

        // Create the node for the current room
        BSPTreeNode node = new BSPTreeNode(space);

        // Initialize the room with floor tiles
        FillRoomWithFloors(node);

        if (!space.IsLargeEnough(minRoomWidth, minRoomHeight))
        {
            return node; // Room is too small, just return the current node
        }

        bool shouldSplit = ShouldSplitRoom(splitCount, space);
        if (shouldSplit)
        {
            // Split the room
            SplitRoom(node, space, minRoomWidth, minRoomHeight, splitCount);
        }

        // Add doors
        AddDoorsToRoom(node);

        return node;
    }

    private bool ShouldSplitRoom(int splitCount, Rectangle space)
    {
        bool shouldSplit;

        // Force splits for the first two levels (splitCount <= 1)
        if (splitCount < 2)
        {
            shouldSplit = true; // Ensure the first two splits always happen
        }
        else
        {
            // For subsequent splits, calculate dynamic split chance based on room size and split count
            double splitChance = CalculateSplitChance(space, splitCount);

            // Decide whether to split or not based on the calculated chance and aspect ratio
            shouldSplit = rnd.NextDouble() < splitChance && IsAspectRatioValid(space);

            // Optional: Log the decision for debugging purposes
            Console.WriteLine($"Room size: {space.Width}x{space.Height}, Split chance: {splitChance * 100}%, Aspect ratio valid: {IsAspectRatioValid(space)}, Random decision: {(shouldSplit ? "Split" : "Skip")}");
        }

        return shouldSplit;
    }

    private void FillRoomWithFloors(BSPTreeNode node)
    {
        for (int x = 0; x < node.Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < node.Tiles.GetLength(1); y++)
            {
                node.Tiles[x, y] = TileType.Floor; // Set all tiles to floor initially
            }
        }
    }

    private void SplitRoom(BSPTreeNode node, Rectangle space, float minRoomWidth, float minRoomHeight, int splitCount)
    {
        // Randomize a split direction
        bool splitHorizontally = rnd.Next(2) == 0;
        int splitLine = 0;

        if (splitHorizontally)
        {
            // Ensure the space is large enough to split horizontally
            if (space.Height < minRoomHeight * 2)
                return; // Not enough space to split

            splitLine = rnd.Next(
                (int)(space.Y + minRoomHeight),
                (int)(space.Y + space.Height - minRoomHeight)
            );

            // Define the top and bottom rooms with updated positions
            Rectangle topRoom = new Rectangle(space.X, space.Y, space.Width, splitLine - space.Y);
            Rectangle bottomRoom = new Rectangle(space.X, splitLine, space.Width, space.Bottom - splitLine);

            // Mark the wall between the top and bottom rooms
            MarkWallsBetweenRooms(node, topRoom, bottomRoom);

            // Create the left and right child nodes and assign the updated coordinates (relative to parent)
            node.Left = BuildTree(topRoom, minRoomWidth, minRoomHeight, splitCount + 1);
            node.Right = BuildTree(bottomRoom, minRoomWidth, minRoomHeight, splitCount + 1);
        }
        else
        {
            // Ensure the space is large enough to split vertically
            if (space.Width < minRoomWidth * 2)
                return; // Not enough space to split

            splitLine = rnd.Next(
                (int)(space.X + minRoomWidth),
                (int)(space.X + space.Width - minRoomWidth)
            );

            // Define the left and right rooms with updated positions
            Rectangle leftRoom = new Rectangle(space.X, space.Y, splitLine - space.X, space.Height);
            Rectangle rightRoom = new Rectangle(splitLine, space.Y, space.Right - splitLine, space.Height);

            // Mark the wall between the left and right rooms
            MarkWallsBetweenRooms(node, leftRoom, rightRoom);

            // Create the left and right child nodes and assign the updated coordinates (relative to parent)
            node.Left = BuildTree(leftRoom, minRoomWidth, minRoomHeight, splitCount + 1);
            node.Right = BuildTree(rightRoom, minRoomWidth, minRoomHeight, splitCount + 1);
        }
    }





    private void MarkWallsBetweenRooms(BSPTreeNode node, Rectangle leftRoom, Rectangle rightRoom)
    {
        if (rightRoom.X == leftRoom.X) // Horizontal split
        {
            for (int x = (int)leftRoom.X; x < leftRoom.X + leftRoom.Width; x++)
            {
                int wallY = (int)(rightRoom.Y - 1); // Wall between the left and right rooms

                // Calculate indices
                int tileX = (int)Math.Floor((x - node.Room.X) / 64.0);
                int tileY = (int)Math.Floor((wallY - node.Room.Y) / 64.0);

                // Check bounds before accessing the array
                if (tileX >= 0 && tileX < node.Tiles.GetLength(0) && tileY >= 0 && tileY < node.Tiles.GetLength(1))
                {
                    node.Tiles[tileX, tileY] = TileType.Wall;
                }
                else
                {
                    // Log out of bounds error if necessary for debugging
                    Console.WriteLine($"Out of bounds at x: {tileX}, y: {tileY}");
                }
            }
        }
        else // Vertical split
        {
            for (int y = (int)leftRoom.Y; y < leftRoom.Y + leftRoom.Height; y++)
            {
                int wallX = (int)(rightRoom.X - 1); // Wall between the left and right rooms

                // Calculate indices
                int tileX = (int)Math.Floor((wallX - node.Room.X) / 64);
                int tileY = (int)Math.Floor((y - node.Room.Y) / 64);

                // Check bounds before accessing the array
                if (tileX >= 0 && tileX < node.Tiles.GetLength(0) && tileY >= 0 && tileY < node.Tiles.GetLength(1))
                {
                    node.Tiles[tileX, tileY] = TileType.Wall;
                }
                else
                {
                    // Log out of bounds error if necessary for debugging
                    Console.WriteLine($"Out of bounds at x: {tileX}, y: {tileY}");
                }
            }
        }
    }


    private void AddDoorsToRoom(BSPTreeNode node)
    {
        if (node.Left != null && node.Right != null)
        {
            // Place a door between left and right room
            Point door = GetDoorLocation(node.Left.Room, node.Right.Room, true); // Assuming horizontal split
            node.Doors.Add(door);
        }
    }

    private Point GetDoorLocation(Rectangle left, Rectangle right, bool splitHorizontally)
    {
        int minX = (int)Math.Min(left.Left, right.Left);
        int maxX = (int)Math.Max(left.Right, right.Right);

        int minY = (int)Math.Min(left.Top, right.Top);
        int maxY = (int)Math.Max(left.Bottom, right.Bottom);

        // Check if min values are less than max values to avoid ArgumentOutOfRangeException
        if (minX >= maxX || minY >= maxY)
        {
            // Handle case where room size is invalid or too small
            throw new InvalidOperationException("Room size is too small to place a door.");
        }

        // Generate a random door location
        Random random = new Random();
        int doorX = random.Next((int)minX, (int)maxX);
        int doorY = random.Next((int)minY, (int)maxY);

        // Round doorX and doorY to the nearest multiple of 64 (or the tile size)
        doorX = (doorX / 64) * 64; // Round to nearest multiple of 64
        doorY = (doorY / 64) * 64; // Round to nearest multiple of 64

        return new Point(doorX, doorY);
    }


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

    // Helper method to round a number to the nearest multiple of 64
    private float RoundToNearest64(float value)
    {
        return (float)(Math.Round(value / 64.0) * 64);
    }


    public TileType[,] GetRoomLayout(BSPTreeNode node)
    {
        return node.Tiles;
    }

    public enum TileType
    {
        Empty,
        Floor,
        Wall,
        Door
    }

    public void CollectRoomsWithDoors(BSPTreeNode node, List<RoomWithDoors> rooms)
    {
        if (node == null) return;

        // Collect this room's information and doors
        RoomWithDoors roomWithDoors = new RoomWithDoors
        {
            Room = node.Room,
            Doors = node.Doors
        };

        rooms.Add(roomWithDoors);

        // Recursively collect rooms for left and right subtrees
        CollectRoomsWithDoors(node.Left, rooms);
        CollectRoomsWithDoors(node.Right, rooms);
    }

    public class RoomWithDoors
    {
        public Rectangle Room { get; set; }
        public List<Point> Doors { get; set; } = new();
    }

    public BSPTreeNode GetRoot()
    {
        return root;
    }
}