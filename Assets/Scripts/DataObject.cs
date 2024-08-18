using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCustomObject", menuName = "CustomObject")]
public class CustomObject : ScriptableObject
{ 
    public Quaternion defaultRotation;
    
    public Vector3 microOffsetPosition;
    
    public Vector3 smallOffsetPosition;
    
    public Vector3 mediumOffsetPosition;

    public Vector3 largeOffsetPosition;
    
    public Vector3 hugeOffsetPosition;
    
    public Vector3 microScale;
    
    public Vector3 smallScale;
    
    public Vector3 mediumScale;
    
    public Vector3 largeScale;
    
    public Vector3 hugeScale;
    
    public Mesh coldMesh;
    
    public Mesh coolMesh;
    
    public Mesh warmMesh;
    
    public Mesh hotMesh;
    
    public Mesh burningMesh;
}