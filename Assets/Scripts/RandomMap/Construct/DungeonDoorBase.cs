using System;
using UnityEngine;

public class DungeonDoorBase : MonoBehaviour
{
    [SerializeField]private DungeonRoomBase room;
    public EDirection direction=EDirection.None;
    
    Renderer renderer;
    Collider collider;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        collider = GetComponent<Collider>();
    }

    public virtual void Open()
    {
        renderer.enabled = false;
        collider.enabled = false;
    }

    public virtual void Close()
    {
        renderer.enabled = true;
        collider.enabled = true;
    }
}