using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomInfoInMatrix
{
    public int x, y;
   
    [SerializeField]public RoomInfoInMatrix rootRoom;
   
    public List<RoomInfoInMatrix> childRooms;

    public ERoomType roomType;
    
    public DungeonRoomBase Room { get; set; }
    
    public RoomInfoInMatrix(int x=-1, int y=-1)
    {
        this.x = x;
        this.y = y;
        this.childRooms = new List<RoomInfoInMatrix>();
    }
    
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (RoomInfoInMatrix)obj;
        return x == other.x && y == other.y;
    }
    
    public override int GetHashCode()
    {
        unchecked // 允许溢出
        {
            int hash = 17;
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            return hash;
        }
    }
   
}