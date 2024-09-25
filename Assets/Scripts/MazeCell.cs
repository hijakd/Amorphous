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
    public bool wallRight = false;
    public bool wallLeft = false;
    public bool wallFront = false;
    public bool wallBack = false;
    public bool isGoal = false;
    public bool isBlocked = false;
}
