using UnityEngine;

namespace LackingImaginationV2
{
    public class EssenceMotion : MonoBehaviour
    {
        [SerializeField] public float circleRadius = 1.5f; // The radius of the horizontal circle
        [SerializeField]  public float circleSpeed = 1.0f; // The speed of the horizontal circle
        [SerializeField]   public float amplitude = 1.0f; // The amplitude of the bob motion
        [SerializeField]   public float frequency = 2.5f; // The frequency of the helix rotation

        [SerializeField]  private float circleTime = 0.0f;
        [SerializeField]   private float helixTime = 0.0f;

        private Vector3 startPoint;
        
        private void Start()
        {
                startPoint = transform.localPosition +new Vector3(0f, 1f, 0f);
        }


        void Update()
        {
            // Update the circle time variable
            circleTime += Time.deltaTime * circleSpeed;

            // Calculate the position of the game object in the horizontal circle motion
            float circleX = circleRadius * Mathf.Cos(circleTime);
            float circleZ = circleRadius * Mathf.Sin(circleTime);

            // Update the helix time variable
            helixTime += Time.deltaTime * frequency;

            // Calculate the position of the game object in the helix bob motion
           
            float helixY = amplitude * Mathf.Sin(helixTime);
            

            // Apply the combined motion to the game object's position
            transform.localPosition =startPoint + new Vector3(circleX , helixY, circleZ);
        }

    }
}

