using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



    
    
    

public class DungeonGenerator 
{
    
    public int DungeonWidth
    {
        get => dungeonWidth;
        set => dungeonWidth = value;
    }
    public int DungeonLength
    {
        get => dungeonLength;
        set => dungeonLength = value;
    }
    public IDungeonGeneratorAlgorithmBase Algorithm
    {
        get=>algorithm;
        set=>algorithm=value;
    }
    
    private int[,] dungeonMap;
    private IDungeonGeneratorAlgorithmBase algorithm;
    private int dungeonLength=5; 
    private int dungeonWidth=5;
    
    public DungeonGenerator(int width, int length,IDungeonGeneratorAlgorithmBase algorithm)
    {
        dungeonWidth = width;
        dungeonLength = length;
        this.algorithm = algorithm;
        Reset();
        PrintDungeon();
    }
    
    public bool GenerateDungeon(ref int[,] dungeonMap,ref List<RoomInfoInMatrix> roomInfo)
    {
        Reset();
        algorithm.GenerateDungeon(dungeonWidth,dungeonLength,ref dungeonMap,ref roomInfo);
        this.dungeonMap = dungeonMap;
        PrintDungeon();
        return true;
    }
    

    
    
    public void Reset()
    {
        dungeonMap=new int[dungeonWidth,dungeonLength];
        for( int i = 0; i < dungeonWidth; i++ )
        {
            for( int j = 0; j < dungeonLength; j++ )
            {
                dungeonMap[i,j] = 0;
            }
        }
    }
    
    public void PrintDungeon()
    {
        Debug.Log("DungeonWidth:"+dungeonWidth+" DungeonLength:"+dungeonLength+"\n");
        string dungeonString = "";
        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonLength; y++)
            {
                dungeonString += dungeonMap[x, y];
            }
            dungeonString += "\n";
        }
        Debug.Log(dungeonString);
    }
    
   
    
}