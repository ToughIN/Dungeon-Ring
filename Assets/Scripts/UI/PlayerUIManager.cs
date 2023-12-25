// using UnityEngine;
// using Unity.Netcode;
// using UnityEngine.Serialization;
//
// public class PlayerUIManager : MonoBehaviour
// {
//     private static PlayerUIManager _instance;
//
//     public static PlayerUIManager Instance
//     {
//         get
//         {
//             if (_instance == null)
//             {
//                 _instance = FindObjectOfType<PlayerUIManager>();
//                 if (_instance == null)
//                 {
//                     GameObject singleton = new GameObject();
//                     _instance = singleton.AddComponent<PlayerUIManager>();
//                     singleton.name = typeof(PlayerUIManager).ToString();
//                 }
//                 
//             }
//
//             return _instance;
//         }
//     }
//
//     [Header("NET JOIN")]
//     [SerializeField] private bool startGameAsClient;
//
//     [HideInInspector] public UI_Panel_HUD_Basic uiPanelHudBasic;
//     [HideInInspector] public PlayerUIPopupManager playerUIPopupManager;
//
//     private void Awake()
//     {
//         _instance = this;
//         DontDestroyOnLoad(gameObject);
//
//         uiPanelHudBasic = GetComponentInChildren<UI_Panel_HUD_Basic>();
//         playerUIPopupManager = GetComponentInChildren<PlayerUIPopupManager>();
//     }
//
//     private void Update()
//     {
//         if (startGameAsClient)
//         {
//             startGameAsClient = false;
//
//             //restart as a client
//             NetworkManager.Singleton.Shutdown();
//             NetworkManager.Singleton.StartClient();
//         }
//     }
// }