using Microsoft.AspNetCore.Mvc;

namespace NoiseDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BSPController : ControllerBase
    {
        // Post Method for using the BSP algorithm
        [HttpPost("generateBSP")]
        public IActionResult GenerateBSP([FromBody] BSPRequest request)
        {
            // Check if the request is valid
            if (request == null)
            {
                return BadRequest("Invalid request.");
            }

            // Set the variables for the BSP Tree from the incoming variables
            var initialSpace = new Rectangle(request.X, request.Y, request.Width, request.Height);
            var minRoomWidth = request.MinRoomWidth;
            var minRoomHeight = request.MinRoomHeight;

            // Create the BSP Tree
            var bspTree = new BSPTree(initialSpace, minRoomWidth, minRoomHeight);

            // Collect all rooms with doors into a list that can then be converted to JSON
            List<BSPTree.RoomWithDoors> rooms = new List<BSPTree.RoomWithDoors>();
            bspTree.CollectRoomsWithDoors(bspTree.GetRoot(), rooms);

            // Return the List of rooms as JSON
            return Ok(rooms);
        }
    }
    // BSP helper class
    public class BSPRequest
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float MinRoomWidth { get; set; }
        public float MinRoomHeight { get; set; }
    }
}
