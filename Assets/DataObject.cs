using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataObject : MonoBehaviour
{
    public ScriptableObject dataObject;

    public enum Scale
    {
        micro,
        small,
        medium,
        large,
        huge
    }
    
    public enum Temperature
    {
        cold,
        cool,
        warm,
        hot,
        burning
    }
    
    public Scale CurrentScale = Scale.medium;

    public Temperature CurrentTemperature = Temperature.cool;

    public Quaternion GetCurrentRotation()
    {
        return ((CustomObject)dataObject).defaultRotation;
    }
    
    public Vector3 GetCurrentScale()
    {
        switch(CurrentScale)
        {
            case Scale.micro:
                return ((CustomObject)dataObject).microScale;
            case Scale.small:
                return ((CustomObject)dataObject).smallScale;
            case Scale.medium:
                return ((CustomObject)dataObject).mediumScale;
            case Scale.large:
                return ((CustomObject)dataObject).largeScale;
            case Scale.huge:
                return ((CustomObject)dataObject).hugeScale;
            default:
                return Vector3.zero;
        }
    }
    
    public Vector3 GetCurrentOffset()
    {
        switch(CurrentScale)
        {
            case Scale.micro:
                return ((CustomObject)dataObject).microOffsetPosition;
            case Scale.small:
                return ((CustomObject)dataObject).smallOffsetPosition;
            case Scale.medium:
                return ((CustomObject)dataObject).mediumOffsetPosition;
            case Scale.large:
                return ((CustomObject)dataObject).largeOffsetPosition;
            case Scale.huge:
                return ((CustomObject)dataObject).hugeOffsetPosition;
            default:
                return Vector3.zero;
        }
    }
    
    public Mesh GetCurrentMesh()
    {
        switch(CurrentTemperature)
        {
            case Temperature.cold:
                return ((CustomObject)dataObject).coldMesh;
            case Temperature.cool:
                return ((CustomObject)dataObject).coolMesh;
            case Temperature.warm:
                return ((CustomObject)dataObject).warmMesh;
            case Temperature.hot:
                return ((CustomObject)dataObject).hotMesh;
            case Temperature.burning:
                return ((CustomObject)dataObject).burningMesh;
            default:
                return null;
        }
    }
    
}
