using UnityEngine;

public enum Direction {

    Start,
    Right,
    Front,
    Left,
    Back,
    Goal
};

public class MazeCell {

    public bool visited = false;
    public bool isValid = false;
    public bool wallRight = false;  // towards positive axis values
    public bool wallLeft = true;    // towards negative axis values, enabled by default
    public bool wallFront = true;   // towards positive axis values, enabled by default
    public bool wallBack = false;   // towards negative axis values
    public bool isGoal = false;
    public bool isBlocked = false;
    public int horizontal; // 2D x-axis
    public int vertical; // 2D y-axis / 3D z-axis

    // Return x and y as a Vector2Int for convenience.
    public Vector2Int position {
        get { return new Vector2Int(horizontal, vertical); }
    }

    public MazeCell(int horizontal, int vertical) {
        this.horizontal = horizontal;
        this.vertical = vertical;
    }
}