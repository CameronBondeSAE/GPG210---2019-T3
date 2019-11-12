using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Students.Luca.Scripts
{
    
    public class Cuboid : MonoBehaviour
    {
        public bool breakIt = false;
        public List<Color> colors = new List<Color>() {Color.red, Color.blue, Color.cyan, Color.green, Color.yellow};
        private Renderer renderer;
        private Rigidbody rb;
        
        private void Start()
        {
            
            renderer = GetComponent<Renderer>();
            rb = GetComponent<Rigidbody>();

            StartCoroutine(ChangeColorAfterTime(1f));
        }

        private float cd = 0;
        private void Update()
        {
            if (cd <= 0 && rb != null)
            {
                rb.AddForce(transform.TransformDirection(Vector3.up) * 1000);
                Instantiate(gameObject);
                cd = 5;
            }

            cd -= Time.deltaTime;
        }
        
        // Edited Script;; https://answers.unity.com/questions/1214118/color-lerping-continuously-with-different-colours.html
        IEnumerator ChangeColorAfterTime( float delayTime)
        {
            Color currentcolor = (Color)colors [UnityEngine.Random.Range (0, colors.Count-1)];
            Color nextcolor;

            renderer.material.color = currentcolor ;
     
            while (!breakIt)
            {
                
                nextcolor = (Color)colors [UnityEngine.Random.Range (0, colors.Count-1)];
                //Debug.Log(currentcolor + "  " + nextcolor);
                for( float t = 0 ; t < delayTime ; t += Time.deltaTime )
                {
                    renderer.material.color = Color.Lerp (currentcolor, nextcolor, t / delayTime );
                    yield return null ;
                }
                currentcolor = nextcolor ;
            }
            yield return 0;
        }
        
    }
}