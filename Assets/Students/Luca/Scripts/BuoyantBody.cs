using System;
using System.Collections.Generic;
using UnityEngine;

namespace Students.Luca.Scripts
{
    public class BuoyantBody : MonoBehaviour
    {
        public bool doDebug = false;
        
        public Transform centerOfMass;
        public Transform waterEntryLevel = null; // Can be null; Otherwise, if its a boat for example, up-facing-triangles only will apply down forces when the objects waterEntryLevel is below the water surface. TODO HACKY; Not Optimal
        
        public Water currentWater;

        public AnimationCurve areaAngleToGravityMultiplierCurve;
        
        
        public MeshFilter meshFilter;
        public Rigidbody rb;

        private Vector3[] underwaterMeshPoints;
        private float[] underwaterMeshPointDistances;
        private UnderwaterAreaData[] underwaterFacesData;
        
        Vector3[] vertices;
        int[] triangles;
        // Start is called before the first frame update
        void Start()
        {
            
            meshFilter = (meshFilter == null?GetComponent<MeshFilter>():meshFilter);
            rb = (rb == null?GetComponent<Rigidbody>():rb);
            if (rb != null && centerOfMass != null)
            {
                rb.centerOfMass = centerOfMass.localPosition;
            }

            if (meshFilter != null)
            {
                underwaterMeshPoints = new Vector3[meshFilter.mesh.vertexCount];
                underwaterMeshPointDistances = new float[meshFilter.mesh.vertexCount];
                underwaterFacesData = new UnderwaterAreaData[meshFilter.mesh.vertexCount];
                
                vertices = meshFilter.mesh.vertices;
                triangles = meshFilter.mesh.triangles;
            }
        }

        // Update is called once per frame
        void Update()
        {
            // TODO Just for testing purposes.
            if (centerOfMass != null && centerOfMass.localPosition != rb.centerOfMass)
            {
                Debug.Log("Center of Mass changed.");
                rb.centerOfMass = centerOfMass.localPosition;
            }
            
            //CalculateUnderwaterPoints();
            CalculateUnderWaterAreas();
        }

        private void FixedUpdate()
        {
            ApplyBuoyantForce();
        }

        private void ApplyBuoyantForce()
        {
            if (underwaterFacesData.Length > 0)
            {
                for (int i = 0; i < underwaterFacesData.Length; i++)
                {//underwaterMeshPointDistances
                    UnderwaterAreaData uad = underwaterFacesData[i];
                    
                    if (uad == null) // Break, supposedly iterated through all areas under water;
                        break;
                    
                    //Vector3 areaCenterWorldPos = transform.localToWorldMatrix.MultiplyPoint3x4(uad.centerPoint);
                    //TODO: @Gravity.. might need to add a multiplier reducing gravity when on an angle.
                    float faceAngleToGravity = Vector3.Angle(uad.areaNormal, Vector3.down); //Mathf.Clamp(Vector3.Angle(uad.areaNormal, Vector3.down),0,90);
                    
                    Vector3 force =  /*uad.areaNormal.normalized  * -1*/Vector3.up * (currentWater.density * uad.surfaceArea * uad.distanceToSurface * areaAngleToGravityMultiplierCurve.Evaluate(faceAngleToGravity) * Mathf.Abs(Physics.gravity.y)); // ?? * distanceToSurface to increase applied force??
                    rb.AddForceAtPosition(force, uad.centerPoint);
                    
                    if(doDebug)
                        Debug.DrawRay(uad.centerPoint,force,Color.green);
                }
            }
            /*if (underwaterMeshPoints.Length > 0)
            {
                for (int i = 0; i < underwaterMeshPoints.Length; i++)
                {//underwaterMeshPointDistances
                    //Vector3 force = currentWater.density * Physics.gravity.y;
                    rb.AddForceAtPosition(force, underwaterMeshPoints[i]);
                }
            }*/
        }

        private void CalculateUnderWaterAreas()
        {
            Array.Clear(underwaterFacesData, 0, underwaterFacesData.Length);
            if (meshFilter.mesh.triangles.Length <= 0 || currentWater == null)
            {
                return;
            }

            
            
            int uwAreaCounter = 0;
            for (int i = 0; i < triangles.Length; i+=3)
            {
                //Vector3 areaNormal = normals[triangles[i+0]] + normals[triangles[i+1]] + normals[triangles[i+2]] / 3;
                Vector3 areaCenter = CalculateTriangleCentroid(vertices[triangles[i]],vertices[triangles[i+1]],vertices[triangles[i+2]]);
                //Vector3 areaCenterWorldPos = transform.localToWorldMatrix.MultiplyPoint3x4(areaCenter);
                Vector3 areaCenterWorldPos = transform.TransformPoint(areaCenter);

                // Get the current y-position of the water surface. TODO HACKY and inperformant.
                float waterHeight = currentWater.transform.position.y;
                /*RaycastHit hit;
                Ray ray = new Ray(new Vector3(areaCenterWorldPos.x,waterHeight+10,areaCenterWorldPos.z),Vector3.down);
                MeshCollider waterMeshCollider = currentWater.GetComponent<MeshCollider>();
                if (waterMeshCollider.Raycast(ray, out hit, 2.0f * 10))
                {
                    waterHeight = hit.point.y - waterMeshCollider.bounds.extents.y; // HACKY
                }*/
                //Debug.Log(waterHeight);
                
                float distanceUnderWater = /*currentWater.transform.position.y*/waterHeight - areaCenterWorldPos.y; //TODO Not considering waves / actual y pos of water

                if (distanceUnderWater > 0)
                {
                    Vector3 areaNormal = Vector3.Cross (vertices[triangles[i+1]]-vertices[triangles[i]], vertices[triangles[i+2]]-vertices[triangles[i]]);
                    //Vector3 areaNormalWorld = transform.localToWorldMatrix.MultiplyPoint3x4(areaNormal);
                    Vector3 areaNormalWorld = transform.TransformDirection(areaNormal);

                    if (waterEntryLevel == null || (waterEntryLevel.position.y < currentWater.transform.position.y || areaNormalWorld.y <= 0)) // TODO Hack: This ignores inside-faces of a "regular shaped" boat. Super Hacky.
                    {
                        UnderwaterAreaData uad = new UnderwaterAreaData((areaNormal.magnitude/2),distanceUnderWater, areaNormalWorld, areaCenterWorldPos);
                        underwaterFacesData[uwAreaCounter] = uad;
                        uwAreaCounter++;

                        if (doDebug)
                        {
                            Debug.DrawRay(areaCenterWorldPos,areaNormalWorld*10,Color.red);
                            //Debug.Log("AreaNormal: "+areaNormal+" AreaNormalWorld: "+areaNormalWorld+" AreaCenter: "+areaCenter+" AreaCenterWorldPos: "+areaCenterWorldPos + " Distance under water: "+distanceUnderWater);
                        }
                        
                    }
                    
                }
                
                
            }
            //Debug.Log("#Faces under water: "+uwAreaCounter+".");
        }

        private void OnDrawGizmos()
        {
            if (triangles != null && triangles.Length > 0)
            {
                for (int i = 0; i < triangles.Length; i+=3)
                {
                    Gizmos.color = Color.magenta;
                    Vector3 center = transform.TransformPoint(CalculateTriangleCentroid(vertices[triangles[i]],vertices[triangles[i+1]],vertices[triangles[i+2]]));
                    Gizmos.DrawSphere(center, 0.05f);
                }
            }
        }

        private Vector3 CalculateTriangleCentroid(Vector3 corner1, Vector3 corner2, Vector3 corner3)
        {
            Vector3 centroid = Vector3.zero;

            centroid.x = (corner1.x + corner2.x + corner3.x) / 3;
            centroid.y = (corner1.y + corner2.y + corner3.y) / 3;
            centroid.z = (corner1.z + corner2.z + corner3.z) / 3;

            return centroid;
        }
        
        private class UnderwaterAreaData
        {
            public float surfaceArea = 0;
            public float distanceToSurface = 0f;
            public Vector3 areaNormal = Vector3.zero;
            public Vector3 centerPoint = Vector3.zero;
            
            public UnderwaterAreaData(float pSurfaceArea, float pDistanceToSurface, Vector3 pAreaNormal, Vector3 pCenterPoint)
            {
                surfaceArea = pSurfaceArea;
                distanceToSurface = pDistanceToSurface;
                areaNormal = pAreaNormal;
                centerPoint = pCenterPoint;
            }
        }
/*

        private void CalculateUnderwaterPoints()
        {
            Array.Clear(underwaterMeshPoints, 0, underwaterMeshPoints.Length);
            if (meshFilter.mesh.vertexCount <= 0)
            {
                return;
            }

            int uwPointsCounter = 0;
            for (int i = 0; i < meshFilter.mesh.vertexCount; i++)
            {
                float distanceUnderWater = currentWater.transform.position.y - meshFilter.mesh.vertices[i].y; //TODO Not considering waves / actual y pos of water
                if (distanceUnderWater > 0)
                {
                    underwaterMeshPoints[uwPointsCounter] = meshFilter.mesh.vertices[i];
                    underwaterMeshPointDistances[uwPointsCounter] = distanceUnderWater;
                    uwPointsCounter++;
                }
            }
        }*/

        private void OnCollisionEnter(Collision other)
        {
            Water w = other.gameObject.GetComponent<Water>();
            if (w != null)
            {
                currentWater = w;
            }
        }

        private void OnTriggerEnter(Collider col)
        {
            Water w = col.GetComponent<Water>();
            if (w != null)
            {
                currentWater = w;
            }
        }

        private void OnCollisionStay(Collision other)
        {
            /*ContactPoint[] contactPoints = new ContactPoint[other.contactCount];
            int count = other.GetContacts(contactPoints);

            Debug.Log(count);

            if (contactPoints.Length > 0)
            {
                for (int i = 0; i < contactPoints.Length; i++)
                {
                    float distanceUnderWater = Mathf.Clamp((other.transform.position.y - contactPoints[i].point.y),0,10000); // Hacky; TODO Not considering waves / actual y pos of water
                    Vector3 upforce = 
                }
            }*/
        }
    }
}