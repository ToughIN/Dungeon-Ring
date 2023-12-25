using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToufFrame;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DungeonMgr : MonoSingletonBase<DungeonMgr>
{
    public int Width { get; set; } = 5;
    public int Length { get; set; } = 5;

    public int[,] DungeonMap
    {
        get => dungeonMap;
        set => dungeonMap = value;
    }

    private int[,] dungeonMap;

    public Vector3 OriginPosition { get; set; } = Vector3.zero;

    public float InterSpace { get; set; } = 50f;

    private List<DungeonRoomBase> RoomList { get; set; } = new List<DungeonRoomBase>();
    private List<RoomInfoInMatrix> roomInfo = new List<RoomInfoInMatrix>(); // 辅助存储房间位置信息



    private DungeonGenerator dungeonGenerator;
    private AsyncTaskChainProcessingHandler asyncChainHandler;
    

    public uint roomIndex;

    private float corridorLength = 33f;
    private List<DungeonCorridorBase> corridorList = new List<DungeonCorridorBase>();

    private void Awake()
    {
        asyncChainHandler = new AsyncTaskChainProcessingHandler();
        dungeonGenerator = new DungeonGenerator(5, 5, new DungeonGeneratorDefaultAlgorithm());
        roomIndex = 0;
        Reset();
    }

    public void GenerateDungeon()
    {
        Reset();
        dungeonGenerator.GenerateDungeon(ref dungeonMap, ref roomInfo);
        
        asyncChainHandler.EnqueueTasks(GenerateRoomsAsync,GenerateCorridorsAsync);
        
    }


    private void Reset()
    {
        dungeonMap = new int[Width, Length];
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Length; j++)
            {
                dungeonMap[i, j] = (int)ERoomType.None;
            }
        }
        
        if (RoomList.Count != 0)
        {
            foreach (var room in RoomList)
            {
                if (room.CorridorToRootRoom != null)
                {
                    Addressables.Release(room.CorridorToRootRoom.gameObject);
                    Destroy(room.CorridorToRootRoom.gameObject);
                }
                Addressables.Release(room.gameObject);
                Destroy(room.gameObject);
            }
            RoomList.Clear();
            roomInfo.Clear();
            corridorList.Clear();
        }
       
    }

    //从开始房间开始生成房间
    private async Task GenerateRoomsAsync()
    {

        List<Task> roomLoadTasks = new List<Task>();

        foreach (RoomInfoInMatrix info in roomInfo)
        {
            var roomLoadTask = LoadAndInstantiateRoomAsync(GameStrings.ADDRESSABLES_ROOM_Prefab, info, roomIndex++);
            roomLoadTasks.Add(roomLoadTask);
        }

        // 等待所有房间加载任务完成
        await Task.WhenAll(roomLoadTasks);
    }
    
    private async Task LoadAndInstantiateRoomAsync(string address, RoomInfoInMatrix info, uint roomIndex)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(address);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject roomPrefab = handle.Result;

            Vector3 position = OriginPosition + new Vector3(info.x * InterSpace, 0, info.y * InterSpace);
            GameObject newRoomObj = Instantiate(roomPrefab, position, Quaternion.identity);
            DungeonRoomBase room = newRoomObj.GetComponent<DungeonRoomBase>();

            RoomList.Add(room);

            info.Room = room;
            room.matrixInfo = info;
            room.gameObject.name = "Room" + roomIndex;
            
        }
        else
        {
            Debug.LogError("Failed to load room prefab.");
        }
    }
    
    // 生成走廊的方法
    private async Task GenerateCorridorsAsync()
    {
        foreach (DungeonRoomBase room in RoomList)
        {
            if (room.RootRoom != null)
            {
                await CreateCorridorBetweenRoomsAsync(room.RootRoom, room);
            }
        }
    }
    
    // 创建走廊
    private async Task CreateCorridorBetweenRoomsAsync(DungeonRoomBase rootRoom, DungeonRoomBase childRoom)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(GameStrings.ADDRESSABLES_ROOM_Corridor);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject corridorPrefab = handle.Result;

            GameObject newCorridorObj = Instantiate(corridorPrefab);
            DungeonCorridorBase corridorBase = newCorridorObj.GetComponent<DungeonCorridorBase>();

            corridorList.Add(corridorBase);

            corridorBase.rootRoom = rootRoom;
            corridorBase.childRoom = childRoom;
            childRoom.CorridorToRootRoom = corridorBase;

            PositionAndOrientCorridor(newCorridorObj, rootRoom.transform.position, childRoom.transform.position);
        }
        else
        {
            Debug.LogError("Failed to load corridor prefab.");
        }
    }

    // 调整走廊的位置和朝向
    private void PositionAndOrientCorridor(GameObject corridor, Vector3 start, Vector3 end)
    {
        EDirection direction=DecideDirection(start,end);
        // 设置走廊的位置为两个房间的中点
        corridor.transform.position = (start + end) / 2;

        if(direction==EDirection.North||direction==EDirection.South)
        {
            corridor.transform.rotation = Quaternion.Euler(0, 90, 0);
            
        }
        else if(direction==EDirection.East||direction==EDirection.West)
        {
            corridor.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            Debug.LogError("走廊方向错误");
        }
    }
    
    public EDirection DecideDirection(Vector3 cur,Vector3 next)
    {
        if (cur.x < next.x)
        {
            return EDirection.East;
        }
        else if (cur.x > next.x)
        {
            return EDirection.West;
        }
        else if (cur.z < next.z)
        {
            return EDirection.North;
        }
        else if (cur.z > next.z)
        {
            return EDirection.South;
        }
        else
        {
            return EDirection.None;
        }
    }
    
    private DungeonRoomBase FindRoomAtPosition(int x, int y)
    {
        foreach (var room in RoomList)
        {
            if (room.matrixInfo.x == x && room.matrixInfo.y == y)
            {
                return room;
            }
        }

        return null;
    }

    
}