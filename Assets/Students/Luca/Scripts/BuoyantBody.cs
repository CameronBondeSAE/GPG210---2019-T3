using System;
using System.Collections.Generic;
using UnityEngine;

namespace Students.Luca.Scripts
{
    public class BuoyantBody : MonoBehaviour
    {
        public bool doDebug = false;
        
        [Header("Components")]
        public MeshFilter meshFilter;
        public Rigidbody rb;
        public Transform centerOfMass;
        [Tooltip("(OPTIONAL) If set, up-facing triangles will only experience buoyant forces if the watersurface level is above this transforms position. (Useful for boats)")]
        public Transform waterEntryLevel = null; // Can be null; Otherwise, if its a boat for example, up-facing-triangles only will apply down forces when the objects waterEntryLevel is below the water surface. TODO HACKY; Not Optimal
        
        public Water currentWater;

        [Header("Settings")]
        public float dragCoefficient = .5f;
        [Tooltip("True: Only upward force (-Gravity) will be applied, False: Directional force will be applied depending on the Triangles normals.")]
        public bool useUpwardBuoyantForce = true;
        

        private UnderwaterAreaData[] underwaterFacesData;
        
        private Vector3[] vertices;
        private int[] triangles;

        private void Awake()
        {
            meshFilter = (meshFilter == null?GetComponent<MeshFilter>():meshFilter);
            rb = (rb == null?GetComponent<Rigidbody>():rb);
            if (rb != null && centerOfMass != null)
            {
                rb.centerOfMass = centerOfMass.localPosition;
            }

            if (meshFilter != null)
            {
                underwaterFacesData = new UnderwaterAreaData[meshFilter.mesh.triangles.Length]; // meshFilter.mesh.vertexCount
                
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
                rb.centerOfMass = centerOfMass.localPosition;
            }
            
            CalculateUnderWaterAreas();
        }

        private void FixedUpdate()
        {
            ApplyBuoyantForce();

            if (doDebug)
            {
                Debug.DrawRay(rb.transform.position, rb.velocity, Color.cyan);
                //Debug.DrawRay(rb.transform.position, rb.angularVelocity*10, Color.red);
            }
                
        }

        private void ApplyBuoyantForce()
        {
            if (underwaterFacesData.Length > 0)
            {
                for (int i = 0; i < underwaterFacesData.Length; i++)
                {
                    UnderwaterAreaData uad = underwaterFacesData[i];
                    
                    if (uad == null) // Break, iterated through all areas under water; (Rest of the array should be empty)
                        break;
                    
                    float faceAngleToGravity = Vector3.Angle(uad.areaNormal, Vector3.down);
                    float angleToGravityMultiplier = Mathf.Cos(faceAngleToGravity*Mathf.Deg2Rad);
                    
                    // Buoyant Force
                    Vector3 buoyantForce;

                    // Buoyancy Formula: F = P * G * A * h | P = Density of Liquid; Gravity; A Surface Area of Attack; h Height / Distance below water
                    if (useUpwardBuoyantForce)
                    {
                        buoyantForce =  Vector3.up * (currentWater.density * uad.surfaceArea * uad.distanceToSurface * angleToGravityMultiplier * Mathf.Abs(Physics.gravity.y));
                    }
                    else
                    {
                        buoyantForce =  (currentWater.density * uad.surfaceArea * uad.distanceToSurface * angleToGravityMultiplier * Physics.gravity.y)  * uad.areaNormal.normalized;
                    }
                    
                    /*
                     ///// OLD DRAG FORCE CALC
                    // Drag Force
                    float angleOfAttack = Vector3.Angle(rb.velocity, uad.areaNormal);
                    float dragForceMultiplier = Mathf.Clamp(Mathf.Cos(angleOfAttack*Mathf.Deg2Rad),0,1); // For simplicity: Angles more than 90° will result in zero-drag.
                    Vector3 localVelocity = dragForceMultiplier * Mathf.Cos(angleOfAttack*Mathf.Deg2Rad) * rb.velocity;
                    Vector3 dragForce = -.5f * currentWater.density * localVelocity.sqrMagnitude * uad.surfaceArea * dragCoefficient * localVelocity.normalized; // (0.5f) = Drag Coefficient?
                    
                    // Angular Drag Forcefloat angularAngleOfAttack = Vector3.Angle(rb.angularVelocity, uad.areaNormal);
                    float angularDragAnglForceMultiplier = 1;//Mathf.Cos(angularAngleOfAttack * Mathf.Deg2Rad);//Mathf.Clamp(Mathf.Cos(angularAngleOfAttack*Mathf.Deg2Rad),0,1); // For simplicity: Angles more than 90° will result in zero-drag.
                    float distanceToCenterOfMass = 1;//Vector3.Distance(rb.centerOfMass, uad.centerPoint);
                    Vector3 localAngularVelocity = angularDragAnglForceMultiplier * Mathf.Cos(angularAngleOfAttack*Mathf.Deg2Rad) * rb.angularVelocity * distanceToCenterOfMass;
                    Vector3 angularDragForce = -.5f * currentWater.density * localAngularVelocity.sqrMagnitude * uad.surfaceArea * (0.5f) * localAngularVelocity.normalized; // (0.5f) = Drag Coefficient?
                    */

                    // Drag Force
                    Vector3 localVelocity = rb.GetPointVelocity(uad.centerPoint);
                    float angleOfAttack = Vector3.Angle(localVelocity, uad.areaNormal);
                    float dragForceMultiplier = Mathf.Clamp(Mathf.Cos(angleOfAttack*Mathf.Deg2Rad),0,1); // For simplicity: Angles more than 90° will result in zero-drag.
                    Vector3 dragForce = -.5f * currentWater.density * localVelocity.sqrMagnitude * uad.surfaceArea * dragCoefficient * dragForceMultiplier * localVelocity.normalized;
                    //Vector3 dragForce = Vector3.zero;
                    
                    // Add Final Force
                    Vector3 composedForce = buoyantForce + dragForce;
                    composedForce.x = float.IsNaN(composedForce.x) ? 0 : Mathf.Clamp(composedForce.x, -10000, 10000);//(float.IsInfinity(composedForce.x)?0:composedForce.x);
                    composedForce.y = float.IsNaN(composedForce.y) ? 0 : Mathf.Clamp(composedForce.y, -10000, 10000);//(float.IsInfinity(composedForce.y)?0:composedForce.y);
                    composedForce.z = float.IsNaN(composedForce.z) ? 0 : Mathf.Clamp(composedForce.z, -10000, 10000);//(float.IsInfinity(composedForce.z)?0:composedForce.z);
                    
                    rb.AddForceAtPosition(composedForce, uad.centerPoint);

                    if (doDebug)
                    {
                        Debug.DrawRay(uad.centerPoint,buoyantForce,Color.green);
                        Debug.DrawRay(uad.centerPoint, dragForce, Color.magenta);
                        Debug.DrawRay(uad.centerPoint, localVelocity, Color.cyan);
                        //Debug.DrawRay(uad.centerPoint,uad.areaNormal*10,Color.red);
                        //Debug.DrawRay(uad.centerPoint, composedForce, Color.yellow);
                    }
                }
            }
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
                Vector3 areaCenter = CalculateTriangleCentroid(vertices[triangles[i]],vertices[triangles[i+1]],vertices[triangles[i+2]]);
                Vector3 areaCenterWorldPos = transform.TransformPoint(areaCenter);

                float waterHeight = currentWater.transform.position.y;
                
                float distanceUnderWater = waterHeight - areaCenterWorldPos.y; //TODO Not considering waves / actual y pos of water

                if (distanceUnderWater > 0)
                {
                    Vector3 areaNormal = Vector3.Cross (vertices[triangles[i+1]]-vertices[triangles[i]], vertices[triangles[i+2]]-vertices[triangles[i]]);
                    
                    Vector3 areaNormalWorld = transform.TransformDirection(areaNormal);

                    if (waterEntryLevel == null || (waterEntryLevel.position.y < currentWater.transform.position.y || areaNormalWorld.y <= 0)) // TODO Hack: This ignores inside-faces of a "regular shaped" boat. Super Hacky.
                    {
                        UnderwaterAreaData uad = new UnderwaterAreaData((areaNormal.magnitude/2),distanceUnderWater, areaNormalWorld, areaCenterWorldPos);
                        underwaterFacesData[uwAreaCounter] = uad;
                        uwAreaCounter++;
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (triangles != null && triangles.Length > 0)
            {
                for (int i = 0; i < triangles.Length; i+=3)
                {
                    Gizmos.color = Color.magenta;
                    Vector3 center = transform.TransformPoint(CalculateTriangleCentroid(vertices[triangles[i]],vertices[triangles[i+1]],vertices[triangles[i+2]]));
                    Gizmos.DrawSphere(center, 0.005f);
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

        private void OnTriggerEnter(Collider col)
        {
            Water w = col.GetComponent<Water>();
            if (w != null)
            {
                currentWater = w;
            }
        }
    }
}