using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DungeonRoomBase : MonoBehaviour 
{
    protected EDirection rootRoomToThisDirection=EDirection.None;
    protected EDirection thisToRootRoomDirection=EDirection.None;
    [SerializeField]protected DungeonCorridorBase corridorToRootRoom;
    
    public List<DungeonDoorBase> doors;
    
    public RoomInfoInMatrix matrixInfo;
    protected List<DungeonRoomBase> childRooms;
    
    public virtual ERoomType RoomType
    {
        get => matrixInfo.roomType;
        set => matrixInfo.roomType = value;
    }

    public virtual EDirection ThisToRootRoomDirection
    {
        get
        {
            if(RootRoom!=null&&thisToRootRoomDirection==EDirection.None)
            {
                thisToRootRoomDirection =
                    DungeonMgr.Instance.DecideDirection(transform.position, RootRoom.transform.position);
            }
            return thisToRootRoomDirection;
        }
        set=>thisToRootRoomDirection=value;
    }
    public virtual EDirection RootRoomToThisDirection
    {
        get
        {
            if(RootRoom!=null&&rootRoomToThisDirection==EDirection.None)
            {
                rootRoomToThisDirection =
                    DungeonMgr.Instance.DecideDirection(RootRoom.transform.position, transform.position);
            }
            return rootRoomToThisDirection;
        }
        set=>rootRoomToThisDirection=value;
    }
    public virtual DungeonCorridorBase CorridorToRootRoom
    {
        get
        {
            if (corridorToRootRoom == null)
            {
                return null;
            }
            return corridorToRootRoom;
        }
        set => corridorToRootRoom = value;
    }
    
    public virtual DungeonRoomBase RootRoom
    {
        get
        {
            if (matrixInfo.rootRoom == null) return null;
            return matrixInfo.rootRoom.Room;
        }
        set => matrixInfo.rootRoom.Room = value;
    }

    public virtual List<DungeonRoomBase> ChildRooms
    {
        get
        {
            if (childRooms == null)
            {
                childRooms = new List<DungeonRoomBase>();
                foreach (var roomInfo in matrixInfo.childRooms)
                {
                    childRooms.Add(roomInfo.Room);
                }
            }
            return childRooms;
        }
        
    }
    
    
    public virtual bool OpenDoor(EDirection direction)
    {
        foreach (DungeonDoorBase door in doors)
        {
            if (door.direction == direction)
            {
                door.Open();
                return true;
            }
        }
        return false;
    }
    
    public virtual void PrintRelations()
    {
        string info = this.name;
        if (RootRoom != null)
        {
            info += " <- " + RootRoom.name;
        }
        if (ChildRooms.Count > 0)
        {
            info += " \n-> ";
            foreach (var childRoom in ChildRooms)
            {
                info += childRoom.name + " ";
            }
        }
        Debug.Log(info);
    }
    
    
    public virtual void RefreshChildRooms()
    {
        childRooms.Clear();
        foreach (var roomInfo in matrixInfo.childRooms)
        {
            childRooms.Add(roomInfo.Room);
        }
    }

    protected void OnDestroy()
    {
    }
}