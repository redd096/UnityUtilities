namespace redd096
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("redd096/Procedural Map/Controller Map Managers")]
    public class ControllerMapManagers : MonoBehaviour
    {
        [SerializeField] MapManager[] mapManagers = default;

        public List<Room> roomsEveryMapManager { get; private set; } = new List<Room>();

        void Start()
        {
            StartCoroutine(StartManagers());
        }

        IEnumerator StartManagers()
        {
            //destroy every map by default
            foreach (MapManager mapManager in mapManagers)
            {
                mapManager.DestroyMap();
            }

            //foreach map manager, create map
            for (int i = 0; i < mapManagers.Length; i++)
            {
                yield return mapManagers[i].CreateMap(roomsEveryMapManager);

                //add created rooms to controller
                roomsEveryMapManager.AddRange(mapManagers[i].Rooms);
            }
        }
    }
}