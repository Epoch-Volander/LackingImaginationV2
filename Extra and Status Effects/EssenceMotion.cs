using System.Collections.Generic;
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

    
    
    
    
    public class LineOfSight
    {
        public static bool LOS (Character hitChar, Vector3 origin)
        {
            bool line = false;
            if(hitChar != null)
            {
                RaycastHit hitInfo = default(RaycastHit);
                var rayDirection = hitChar.GetCenterPoint() - origin;
                if(Physics.Raycast(origin, rayDirection, out hitInfo))
                {
                    if(Collision(hitChar, hitChar.GetCollider(), hitInfo))
                    {
                        line = true;
                    }
                    else
                    {
                        for(int i = 0; i < 8; i++)
                        {
                            Vector3 boundsSize = hitChar.GetCollider().bounds.size;
                            var rayDirectionMod = (hitChar.GetCenterPoint() + new Vector3(boundsSize.x * (UnityEngine.Random.Range(-i, i) / 6f), boundsSize.y * (UnityEngine.Random.Range(-i, i) / 4f), boundsSize.z * (UnityEngine.Random.Range(-i, i) / 6f))) - origin;
                            if (Physics.Raycast(origin, rayDirectionMod, out hitInfo))
                            {
                                if (Collision(hitChar, hitChar.GetCollider(), hitInfo))
                                {
                                    line = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return line;
        }
         
        
        private static bool Collision(Character chr, Collider col, RaycastHit hit)
        {
            if (hit.collider == chr.GetCollider())
            {
                return true;
            }
            Character ch = null;
            hit.collider.gameObject.TryGetComponent<Character>(out ch);
            bool flag = ch != null;
            List<Component> components = new List<Component>();
            components.Clear();
            hit.collider.gameObject.GetComponents<Component>(components);
            if (ch == null)
            {
                ch = (Character)hit.collider.GetComponentInParent(typeof(Character));
                flag = ch != null;
                if (ch == null)
                {
                    ch = (Character)hit.collider.GetComponentInChildren<Character>();
                    flag = ch != null;
                }
            }
            if(flag && ch == chr)
            {
                return true;
            }
            return false;
        }
        
        
    }
}

