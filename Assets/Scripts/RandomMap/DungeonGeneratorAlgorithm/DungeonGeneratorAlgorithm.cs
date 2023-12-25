

using System.Collections.Generic;
using UnityEngine;


public interface IDungeonGeneratorAlgorithmBase
{
    public abstract void GenerateDungeon(int width, int length,ref int[,] dungeonMap,ref List<RoomInfoInMatrix> roomInfo);
}

public class DungeonGeneratorDefaultAlgorithm : IDungeonGeneratorAlgorithmBase
{
    List<RoomInfoInMatrix> directions = new List<RoomInfoInMatrix>()
    {
        new RoomInfoInMatrix(0, -1),//West
        new RoomInfoInMatrix(1, 0), //South
        new RoomInfoInMatrix(0, 1), //East
        new RoomInfoInMatrix(-1, 0) //North
    };
    public void GenerateDungeon(int width, int length, ref int[,] dungeonMap, ref List<RoomInfoInMatrix> roomInfo)
    {
        
        RoomInfoInMatrix start = new RoomInfoInMatrix();
        RoomInfoInMatrix end = new RoomInfoInMatrix();
        start.x=Random.Range(0,width);
        start.y=Random.Range(0,length);
        end.x=Random.Range(0,width);
        end.y=Random.Range(0,length);
        while(start.Equals(end))
        {
            end.x=Random.Range(0,width);
            end.y=Random.Range(0,length);
        }
        dungeonMap[start.x,start.y]=(int)ERoomType.StartRoom;
        dungeonMap[end.x,end.y]=(int)ERoomType.EndRoom;
        
        ConnectRooms(start,end,width,length,ref roomInfo,dungeonMap);
    }

    //BFS
    private void ConnectRooms(RoomInfoInMatrix start, RoomInfoInMatrix end, int width, int length, ref List<RoomInfoInMatrix> roomInfo, int[,] dungeonMap)
    {
        var queue = new Queue<RoomInfoInMatrix>();
        var visited = new HashSet<RoomInfoInMatrix>();
        var parent = new Dictionary<RoomInfoInMatrix, RoomInfoInMatrix>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            RoomInfoInMatrix current = queue.Dequeue();

            if (current.Equals(end))
            {
                end = current;
                break;
            }

            foreach (var neighbour in GetNeighbors(current, width, length, visited))
            {
                parent[neighbour] = current;
                visited.Add(neighbour);
                queue.Enqueue(neighbour);
            }
        }

        if (!parent.ContainsKey(end))
        {
            Debug.Log("No path found");
            return;
        }

        // 构建路径并设置房间关系
        for (RoomInfoInMatrix at = end; at != null && !at.Equals(start); at = parent[at])
        {
            dungeonMap[at.x, at.y] = (int)ERoomType.NormalRoom;
            at.roomType = (ERoomType)dungeonMap[at.x, at.y];
            at.rootRoom = parent.GetValueOrDefault(at);
            parent.GetValueOrDefault(at)?.childRooms.Add(at);
            roomInfo.Add(at);
        }
        dungeonMap[end.x, end.y] = (int)ERoomType.EndRoom;
        end.roomType = ERoomType.EndRoom;
        start.roomType = ERoomType.StartRoom;
        roomInfo.Add(start);
    }

    private IEnumerable<RoomInfoInMatrix> GetNeighbors(RoomInfoInMatrix pos, int width, int length, HashSet<RoomInfoInMatrix> visited)
    {
        foreach (var direction in directions)
        {
            int newX = pos.x + direction.x;
            int newY = pos.y + direction.y;
            var newRoom = new RoomInfoInMatrix(newX, newY);

            if (newX >= 0 && newX < width && newY >= 0 && newY < length && !visited.Contains(newRoom))
            {
                yield return newRoom;
            }
        }
    }

}