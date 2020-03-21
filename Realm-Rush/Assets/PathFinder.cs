using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{

    Dictionary<Vector2Int, Waypoint> grid = new Dictionary<Vector2Int, Waypoint>();
    Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    [SerializeField] Waypoint startWaypoint;
    [SerializeField] Waypoint endWaypoint;


    public List<Waypoint> GetPath()
    {
        LoadGrid();
        ColorStartAndEnd();
        return PathFind();
    }


    private void LoadGrid()
    {
        Waypoint[] waypoints = FindObjectsOfType<Waypoint>();
        foreach (Waypoint waypoint in waypoints)
        {
            Vector2Int gridPos = waypoint.GetGridPos();
            if (grid.ContainsKey(gridPos))
            {
                print("Skipping Overlapped Block: " + waypoint);
            }
            else
            {
                grid.Add(gridPos, waypoint);
            }
        }
    }

    private void ColorStartAndEnd()
    {
        startWaypoint.SetTopColor(Color.red);
        endWaypoint.SetTopColor(Color.green);
    }

    private List<Waypoint> PathFind()
    {
        Queue<Waypoint> waypoints = new Queue<Waypoint>();
        waypoints.Enqueue(startWaypoint);
        startWaypoint.isExplored = true;
        while(waypoints.Count > 0)
        {
            Waypoint waypoint = waypoints.Dequeue();
            if (waypoint == endWaypoint)
            {
                return MakePath(waypoint);
            }
            ExploreNeighbours(waypoints, waypoint);
        }
        return null;

    }

    private void ExploreNeighbours(Queue<Waypoint> waypoints, Waypoint waypoint)
    {
        foreach (Vector2Int direction in directions)
        {
            Vector2Int index = waypoint.GetGridPos() + direction;
            if (grid.ContainsKey(index))
            {
                Waypoint neighbour = grid[index];
                if (!neighbour.isExplored)
                {
                    waypoints.Enqueue(neighbour);
                    neighbour.isExplored = true;
                    neighbour.parent = waypoint;
                }
            }
        }
    }

    private List<Waypoint> MakePath(Waypoint waypoint)
    {
        EnemyMovement enemy = FindObjectOfType<EnemyMovement>();
        List<Waypoint> path = new List<Waypoint>();

        path.Add(endWaypoint);
        waypoint = waypoint.parent;
        while (waypoint != startWaypoint)
        {
            path.Add(waypoint);
            waypoint.SetTopColor(Color.magenta);
            waypoint = waypoint.parent;
        }
        path.Add(startWaypoint);
        path.Reverse();
        return path;
    }
}
