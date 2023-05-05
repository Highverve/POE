using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.TileEngine
{
    public interface WeightedGraph<Point>
    {
        float Cost(Point a, Point b);
        IEnumerable<Point> Neighbors(Point id);
    }
    /* --- No longer necessary, using Dictionary<Point, float> ---
    public class WeightedTile
    {
        private Point location;
        private int cost;

        public Point Location { get { return location; } }
        public int Cost { get { return cost; } }

        public WeightedTile(Point Location, int Cost) { location = Location; cost = Cost; }

        public override bool Equals(object obj)
        {
            Point tile = (Point)obj;
            return (location.X == tile.X && location.Y == tile.Y);
        }
        public static bool operator ==(WeightedTile tile, Point b)
        {
            return tile.Location.X == b.X && tile.Location.Y == b.Y;
        }
        public static bool operator !=(WeightedTile tile, Point b)
        {
            return tile.Location.X != b.X || tile.Location.Y != b.Y;
        }
        public override int GetHashCode()
        {
            return location.GetHashCode();
        }
    }
    */
    public class SquareGrid : WeightedGraph<Point>
    {
        public static readonly Point[] Directions = new Point[]
        {
            new Point(0, -1), //N
            new Point(1, -1), //NE
            new Point(1, 0), //E
            new Point(1, 1), //SE
            new Point(0, 1), //S
            new Point(-1, 1), //SW
            new Point(-1, 0), //W
            new Point(-1, -1), //NW
        };

        public int width, height;

        public List<Point> walls = new List<Point>();
        public Dictionary<Point, int> highCost = new Dictionary<Point, int>();
        public Dictionary<Point, string> flags = new Dictionary<Point, string>();

        private int[,] grid;
        public int[,] Grid { get { return grid; } }

        public SquareGrid(int[,] Grid)
        {
            width = Grid.GetLength(0);
            height = Grid.GetLength(1);

            grid = Grid;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int index = grid[x, y];

                    // --- Walls ---
                    if (index == -1)
                        walls.Add(new Point(x, y));

                    // --- Flags ---
                    if (index == -2)
                    {
                        flags.Add(new Point(x, y), "FloorUp"); //-2 means the next floor is up.

                    }
                    if (index == -3)
                        flags.Add(new Point(x, y), "FloorDown"); //-3 means the next floor is down.

                    // --- Normall Cost Setting ---
                    if (index > 0)
                        highCost.Add(new Point(x, y), index);
                }
            }
        }

        public bool InBounds(Point location)
        {
            return (location.X >= 0 && location.X < width) &&
                   (location.Y >= 0 && location.Y < height);
        }
        public bool IsPassable(Point location) { return !walls.Contains(location); }

        public bool HasFlag(Point location) { return flags.ContainsKey(location); }
        public string RetrieveFlag(Point location) { return flags[location]; }

        public bool IsCornerMovement(Point current, Point next) { return (current.X != next.X && current.Y != next.Y); }
        /// <summary>
        /// Maximum value is 2, minumum is 0. Only used for checking for walls on diagonal movements.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public int WallCount(Point current, Point next)
        {
            int wallCount = 0;

            if (IsCornerMovement(current, next))
            {
                int dirX = next.X - current.X; //If 3x to 2x, value is 1x direction.
                int dirY = next.Y - current.Y; //If current is 3y  and next is 4y, value is -1y direction.

                if (!IsPassable(new Point(current.X + dirX, current.Y)))
                    wallCount++;
                if (!IsPassable(new Point(current.X, current.Y + dirY)))
                    wallCount++;
            }

            return wallCount;
        }
        public float Cost(Point current, Point next)
        {
            float cost = 1;

            if (IsCornerMovement(current, next))
                cost = 1.4f;

            if (highCost.ContainsKey(next))
                cost += highCost[next];

            return cost; //If the higher cost list contains the location, return index. If not, return default of 1.
        }

        public IEnumerable<Point> Neighbors(Point location)
        {
            for (int i = 0; i < Directions.Length; i++)
            {
                Point next = new Point(location.X + Directions[i].X, location.Y + Directions[i].Y);
                if (InBounds(next) && IsPassable(next))
                {
                    yield return next;
                }
            }
        }
    }
    public class PriorityQueue<T>
    {
        // I'm using an unsorted array for this example, but ideally this
        // would be a binary heap. Find a binary heap class:
        // * https://bitbucket.org/BlueRaja/high-speed-priority-queue-for-c/wiki/Home
        // * http://visualstudiomagazine.com/articles/2012/11/01/priority-queues-with-c.aspx
        // * http://xfleury.github.io/graphsearch.html
        // * http://stackoverflow.com/questions/102398/priority-queue-in-net

        private List<Tuple<T, float>> elements = new List<Tuple<T, float>>();

        public int Count
        {
            get { return elements.Count; }
        }
        public void Enqueue(T item, float priority)
        {
            elements.Add(Tuple.Create(item, priority));
        }
        public T Dequeue()
        {
            int bestIndex = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Item2 < elements[bestIndex].Item2)
                {
                    bestIndex = i;
                }
            }

            T bestItem = elements[bestIndex].Item1;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }
    public class Pathfinder
    {
        public readonly static Point TileSize = new Point(64, 64);

        private Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();
        private Dictionary<Point, float> currentCost = new Dictionary<Point, float>();
        private Dictionary<Point, string> currentFlags = new Dictionary<Point, string>();

        private Dictionary<int, Dictionary<Point, int>> connectedComponents = new Dictionary<int, Dictionary<Point, int>>();

        private PathfindMap map;
        private Dictionary<int, WeightedGraph<Point>> graph = new Dictionary<int, WeightedGraph<Point>>();

        private List<Point> currentPath = new List<Point>();
        public List<Point> CurrentPath { get { return currentPath; } }

        private float totalCost, totalHeuristic;

        public static float Euclidean(Point goal, Point current, float cost)
        {
            int dx = Math.Abs(current.X - goal.X);
            int dy = Math.Abs(current.Y - goal.Y);

            return cost * (float)Math.Sqrt(dx * dx + dy * dy);
        }
        public static float Manhattan(Point next, Point goal)
        {
            int dx = Math.Abs(next.X - goal.X);
            int dy = Math.Abs(next.Y - goal.Y);

            return 5 * (dx + dy);
        }
        public static float DiagonalHeuristic(Point next, Point goal, float minimumCost, float d2)
        {
            int dx = Math.Abs(next.X - goal.X);
            int dy = Math.Abs(next.Y - goal.Y);

            return minimumCost * (dx + dy) + (d2 - 2 * minimumCost) * Math.Min(dx, dy);
        }
        public static int Heuristic(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        public Pathfinder(PathfindMap Map)
        {
            map = Map;

            foreach (KeyValuePair<int, int[,]> map in map.Maps)
                graph.Add(map.Key, new SquareGrid(map.Value));

            RegenerateConnectedComponents();
        }
        public Pathfinder(WeightedGraph<Point> Graph)
        {
            graph[0] = Graph;

            RegenerateConnectedComponents();
        }

        /// <summary>
        /// Only regenerate this at the beginning of each map load (refactor sometime to make it faster)! It's performance heavy.
        /// </summary>
        public void RegenerateConnectedComponents()
        {
            connectedComponents.Clear();

            foreach (KeyValuePair<int, WeightedGraph<Point>> g in graph)
                GenerateConnectedComponents(g.Key, (SquareGrid)g.Value);
        }
        private void GenerateConnectedComponents(int floorNumber, SquareGrid floorGrid)
        {
            // Search through all locations that are not -1 (walls).
            int width = floorGrid.Grid.GetLength(0);
            int height = floorGrid.Grid.GetLength(1);

            int openSpaces = (width * height) - floorGrid.walls.Count; //Gets the total amount of spaces, exluding the total walls.
            int currentComponent = 1;

            Dictionary<Point, int> checkedSpaces = new Dictionary<Point, int>(); //the value here is the current component section!

            while (checkedSpaces.Count < openSpaces) //Continue searching while there are still open spaces unchecked
            {
                if (checkedSpaces.Count >= openSpaces) 
                    break; //Break if all open spaces have been checked

                //Search for
                Point start = GetNextOpenSpace(width, height, floorGrid, checkedSpaces);
                BreadthFirstSearch(floorGrid, start, checkedSpaces, currentComponent);
                currentComponent++;
            }

            connectedComponents.Add(floorNumber, checkedSpaces);
        }
        private Point GetNextOpenSpace(int width, int height, SquareGrid grid, Dictionary<Point, int> checkedSpaces)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Point current = new Point(x, y);

                    if (grid.IsPassable(current)) //Iterate until found an open tile...
                    {
                        if (!checkedSpaces.ContainsKey(current)) //And if the current key has not already been checked...
                        {
                            return current;
                        }
                    }
                }
            }

            return Point.Zero; //Rejoice! No more tiles can be checked.
        }
        public void BreadthFirstSearch(WeightedGraph<Point> graph, Point start, Dictionary<Point, int> checkedSpaces, int component)
        {
            Queue<Point> frontier = new Queue<Point>();
            frontier.Enqueue(start);

            while (frontier.Count > 0)
            {
                Point current = frontier.Dequeue();

                foreach (Point next in graph.Neighbors(current))
                {
                    if (((SquareGrid)graph).IsPassable(current)) //Iterate until found an open tile...
                    {
                        if (!checkedSpaces.ContainsKey(next)) //And if the current key has not already been checked...
                        {
                            checkedSpaces.Add(next, component);
                            frontier.Enqueue(next);
                        }
                    }
                }
            }
        }
        public void BFS_FindFlags(WeightedGraph<Point> graph, Point start)
        {

        }

        public void RegenerateMap(int currentFloor, Point start, Point goal)
        {
            cameFrom.Clear();
            currentCost.Clear();
            currentFlags.Clear();

            PriorityQueue<Point> frontier = new PriorityQueue<Point>();
            frontier.Enqueue(start, 0);

            cameFrom[start] = start;
            currentCost[start] = 0;

            if (IsSameComponent(currentFloor, start, goal))
            {
                while (frontier.Count > 0)
                {
                    Point current = frontier.Dequeue();

                    if (current.Equals(goal)) //Exit early if goal is reached.
                        break;

                    foreach (Point next in graph[currentFloor].Neighbors(current))
                    {
                        float newCost = currentCost[current] + graph[currentFloor].Cost(current, next);

                        // --- Check for and add flags. ---
                        if (!currentFlags.ContainsKey(next) && ((SquareGrid)graph[currentFloor]).HasFlag(next))
                            currentFlags.Add(next, ((SquareGrid)graph[currentFloor]).RetrieveFlag(next));

                        if (!currentCost.ContainsKey(next) || newCost < currentCost[next])
                        {
                            if (((SquareGrid)graph[currentFloor]).WallCount(current, next) == 0)
                            {
                                currentCost[next] = newCost;
                                float priority = newCost + Euclidean(next, goal, 1f);
                                frontier.Enqueue(next, priority);
                                cameFrom[next] = current;

                                totalCost = newCost;
                                totalHeuristic = priority;
                            }
                        }
                    }
                }
            }
        }

        public List<Point> GetPathToGoal(int currentFloor, Point start, Point goal)
        {
            Point current = goal;
            List<Point> path = new List<Point>();

            path.Add(current);

            // --- If the start or end points are inside of walls, return an empty path. ---
            if (!((SquareGrid)graph[currentFloor]).IsPassable(start) || !((SquareGrid)graph[currentFloor]).IsPassable(goal))
                return path; //Return empty list

            bool sameComponents = IsSameComponent(currentFloor, start, goal);

            if (sameComponents == false)
            {
                foreach (KeyValuePair<Point, string> key in currentFlags)
                {
                    if (IsSameComponent(currentFloor, start, key.Key))
                    {
                        goal = key.Key;
                    }
                }
            }

            while (current != start)
            {
                try
                {
                    current = cameFrom[current];
                    path.Add(current);

                    if (current.Equals(start))
                        break;
                }
                catch (KeyNotFoundException e)
                {
                    // --- Path has not been found, searching for alternative routes. ---

                    if (currentFlags.Count > 0) //Experimental code!
                    {
                        int floor = currentFloor;

                        /*
                        if (currentFlags.FirstOrDefault().Value.ToUpper().Equals("FLOORUP"))
                        {
                            if (graph.ContainsKey(floor + 1)) //Ensure the floor exists.
                                floor = currentFloor + 1;
                        }
                        if (currentFlags.FirstOrDefault().Value.ToUpper().Equals("FLOORDOWN"))
                        {
                            if (graph.ContainsKey(floor - 1)) //Ensure the floor exists.
                                floor = currentFloor - 1;
                        }*/


                        path = GetPathToGoal(currentFloor, current, currentFlags.FirstOrDefault().Key); //Make pathway run from current to the flag. Once the flag is hit, there should be a map trigger that changes the entity's current floor. Then, repeat the method calls.
                        //Additional note: This does not support more than one flag!
                    }

                    path.Reverse();
                    return path;
                }
            }
            path.Reverse();

            return path;
        }
        public void GeneratePath(int currentFloor, Point start, Point goal)
        {
            //New path, reset pathFailed variables
            isPathFailed = false;
            time = 0;

            currentPath = GetPathToGoal(currentFloor, start, goal);
        }

        public Point CurrentTile()
        {
            if (currentPath.Count > 0)
                return currentPath[0];
            else
                return Point.Zero;
        }

        private Vector2 currentPosition = Vector2.Zero; private Point lastTile = Point.Zero;
        public Vector2 CurrentTilePosition
        {
            get
            {
                if (lastTile != CurrentTile()) //If the current tile is not equal to the lastTile, reload values. This should prevent excessive Vector2 creation.
                {
                    currentPosition = new Vector2((CurrentTile().X * TileSize.X) + TileSize.X / 2,
                                                  (CurrentTile().Y * TileSize.Y) + TileSize.Y / 2);
                    lastTile = CurrentTile();
                }

                return currentPosition;
            }
        }

        public void RemoveCurrentTile()
        {
            if (currentPath.Count > 0)
                currentPath.RemoveAt(0);
        }
        public void RemoveLastTile()
        {
            if (currentPath.Count > 0)
                currentPath.RemoveAt(currentPath.Count - 1);
        }
        public bool IsClose(Vector2 position, float minDistance)
        {
            return Vector2.Distance(CurrentTilePosition, position) <= minDistance; //returns 
        }
        public bool IsSameComponent(int floor, Point start, Point goal)
        {
            int startComponent = 0;
            int goalComponent = -1;
            connectedComponents[floor].TryGetValue(start, out startComponent);
            connectedComponents[floor].TryGetValue(goal, out goalComponent);

            return (startComponent == goalComponent);
        }

        private bool isPathFailed;
        public bool IsPathFailed { get { return isPathFailed; } }

        private int time = 0;
        private Point failTileCheck;
        public void CheckPathFailure(GameTime gt)
        {
            if (currentPath.Count > 0)
            {
                time += gt.ElapsedGameTime.Milliseconds;

                if (failTileCheck != CurrentTile())
                    time = 0;

                if (time >= 5000) //If the current tile has been the same for more than 5 seconds, the entity has not moved. So, mark as path failure.
                {
                    isPathFailed = true;
                    //Note: this should prevent the character from infinitely getting stuck on colliders that the map designer did not mark.
                    //      If the entity is stunned/paralyzed/other from combat, their AI should change from pathfinding to defending anyway.
                }

                failTileCheck = CurrentTile();
            }
        }
    }
}
