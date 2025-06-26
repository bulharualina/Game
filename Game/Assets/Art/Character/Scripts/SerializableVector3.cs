using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public SerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public static implicit operator SerializableVector3(Vector3 v3)
    {
        return new SerializableVector3(v3);
    }

    public static implicit operator Vector3(SerializableVector3 sv3)
    {
        return new Vector3(sv3.x, sv3.y, sv3.z);
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}





[System.Serializable]
public struct SerializableQuaternion
{
    public float x;
    public float y;
    public float z;
    public float w; 

    public SerializableQuaternion(Quaternion q)
    {
        x = q.x;
        y = q.y;
        z = q.z;
        w = q.w;
    }

  
    public static implicit operator Quaternion(SerializableQuaternion sq)
    {
        return new Quaternion(sq.x, sq.y, sq.z, sq.w);
    }

    
    public static implicit operator SerializableQuaternion(Quaternion q)
    {
        return new SerializableQuaternion(q);
    }

}
