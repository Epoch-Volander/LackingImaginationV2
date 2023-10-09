using UnityEngine;




namespace LackingImaginationV2

{
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner instance;

        public static CoroutineRunner Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject container = new GameObject("CoroutineRunner");
                    instance = container.AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(container);
                }
                return instance;
            }
        }
    }
}