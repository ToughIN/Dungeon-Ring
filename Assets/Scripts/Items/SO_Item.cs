using UnityEngine;

public class SO_Item : ScriptableObject
{
    [Header("Item Information")] 
    public string itemNme;

    public Sprite itemIcon;

    [TextArea] public string itemDescription;

    public int itemID;

    public string addressablePath;
    
    public EItemType itemType;

}