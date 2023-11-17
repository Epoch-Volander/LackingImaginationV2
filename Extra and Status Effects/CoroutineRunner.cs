using System.Collections;
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


    // public class AnimationDelay
    // {
    //     public enum EssenceAnim
    //     {
    //         Brenna,
    //         RancidRemains,
    //     }
    //     
    //     
    //     public static void ScheduleAnimation(AnimationDelay.EssenceAnim controller, float timer)
    //     {
    //         CoroutineRunner.Instance.StartCoroutine(ScheduleAnimationCoroutine(controller, timer));
    //     }
    //     // ReSharper disable Unity.PerformanceAnalysis
    //     private static IEnumerator ScheduleAnimationCoroutine(AnimationDelay.EssenceAnim controller, float timer)
    //     {
    //         yield return new WaitForSeconds(timer); // Wait for the specified time
    //
    //         switch (controller)
    //         {
    //             case EssenceAnim.Brenna:
    //                 xSkeletonSynergy.SkeletonSynergyBrennaController = false;
    //                 break;
    //             case EssenceAnim.RancidRemains:
    //                 xSkeletonSynergy.SkeletonSynergyRancidController = false;
    //                 break;
    //
    //             
    //             
    //             
    //             
    //         }
    //         
    //     }
    // }
}