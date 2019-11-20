using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Students.Luca.Scripts
{
    public class Water : MonoBehaviour
    {
        public MeshCollider meshCollider;
        public MeshFilter meshFilter;
        public SkinnedMeshRenderer skinnedMeshRenderer;
        public Cloth cloth;
        
        public float density = 1025;

        [ShowInInspector, SerializeField]
        private float heightIntensity = 1;
        public float HeightIntensity
        {
            get => heightIntensity;
            set
            {
                heightIntensity = value;
                HandleWaveIntensityChanged();
            }
        }
        
        [ShowInInspector, SerializeField]
        private float durationIntensity = 1;
        public float DurationIntensity
        {
            get => durationIntensity;
            set
            {
                durationIntensity = value;
                HandleWaveIntensityChanged();
            }
        }

        private Cloth waterCloth;

        private Tweener waveTween;

        private void Start()
        {
            if (meshCollider == null)
            {
                meshCollider = GetComponent<MeshCollider>();
            }
            if (meshFilter == null)
            {
                meshFilter = GetComponent<MeshFilter>();
            }

            if (cloth == null)
            {
                cloth = GetComponent<Cloth>();
            }

            if (skinnedMeshRenderer == null)
            {
                skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            }
            
            waterCloth = GetComponent<Cloth>();
            //waveTween = transform.DOPunchPosition(new Vector3(0, HeightIntensity, 0), DurationIntensity, 10, 1f, false);
            //transform.DOMoveY(heightIntensity, time);
            //waveTween = DOTween.Punch(() => transform.position, (x) => transform.position = x, new Vector3(0,heightIntensity,0), durationIntensity, 10, 1f).SetLoops(-1,LoopType.Yoyo).Play();
            waveTween = DOTween.To(() => transform.position, (x) => transform.position = x, transform.position+(new Vector3(0,heightIntensity,0)), durationIntensity).SetLoops(-1,LoopType.Yoyo);
        }

        private void Update()
        {
            if (HeightIntensity > 0 && cloth != null && meshCollider != null && skinnedMeshRenderer != null)
            {
                //meshCollider.sharedMesh = null;
                
                /*Mesh m = skinnedMeshRenderer.sharedMesh;
                m.vertices = GetComponent<Cloth>().vertices;
                m.RecalculateNormals();*/
                /*Mesh m = new Mesh();
                skinnedMeshRenderer.BakeMesh(m);*/
                /*meshCollider.sharedMesh.vertices = cloth.vertices;
                meshCollider.sharedMesh.RecalculateNormals();
                meshCollider.sharedMesh.RecalculateBounds();
                meshCollider.sharedMesh.RecalculateTangents();*/

                //meshCollider.sharedMesh = meshFilter.sharedMesh;
                Mesh mesh = new Mesh();
                skinnedMeshRenderer.BakeMesh(mesh); //could also instantiate sharedmesh for the same results
                mesh.vertices = cloth.vertices;
                mesh.normals = cloth.normals;
                //meshCollider.sharedMesh = null;
                meshCollider.sharedMesh = mesh;
                
                /*DestroyImmediate(this.GetComponent<MeshCollider>());
 var collider = this.AddComponent<MeshCollider>();
 collider.sharedMesh = myMesh;*/

            }
        }

        private void HandleWaveIntensityChanged()
        {
            Debug.Log("Values Changed "+waveTween);
            waveTween?.ChangeValues(new Vector3(0, HeightIntensity, 0), new Vector3(0, HeightIntensity, 0), DurationIntensity);
        }

        private void OnValidate()
        {
            HandleWaveIntensityChanged();
        }
    }
}