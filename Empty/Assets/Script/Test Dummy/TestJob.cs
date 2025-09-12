using Unity.Jobs;
using Unity.Collections;
using UnityEngine;

// None Shared 변수
public struct JobA : IJob
{
    public void Execute()
    {
        UnityEngine.Debug.Log("Job A Done");
    }
}

// 공통 변수 Test
public struct JobB : IJob
{
    public NativeArray<float> result;
    public float value;
    public int addNumber;
    public float mulNumber;

    public void Execute()
    {
        value += addNumber;
        value *= mulNumber;
        result[0] = value;
    }
}

// Job C에서 사용할 Struct
public struct JobInfo
{
    public int intValue;
    public float floatValue;
    public bool boolValue;
}

public struct JobC : IJob
{
    public NativeArray<int> intValue;
    public NativeArray<bool> boolValue;

    // struct에 해당한다.
    public NativeArray<JobInfo> structValue;
    public NativeArray<Vector2> vec2Value;
    public NativeArray<Quaternion> quaternionValue;
    public NativeArray<Matrix4x4> maxtrixValue;
    public NativeArray<Color> colorValue;

    public void Execute()
    {
        intValue[0] += 5;
        boolValue[0] = !boolValue[0];

        // Custom Struct
        var newJobInfo = new JobInfo() { intValue = 2, floatValue = 1.0f, boolValue = false };
        var cloneStruct = structValue[0];
        cloneStruct.intValue += newJobInfo.intValue;
        cloneStruct.floatValue += newJobInfo.floatValue;
        cloneStruct.boolValue = newJobInfo.boolValue;
        structValue[0] = cloneStruct;

        // Vector2, 3, 4
        var cloneVector2 = vec2Value[0];
        cloneVector2 += new Vector2(2, 2);
        vec2Value[0] = cloneVector2;

        // Quaternion
        var cloneQuaternion = quaternionValue[0];
        cloneQuaternion = Quaternion.identity;
        quaternionValue[0] = cloneQuaternion;

        // matrix
        var cloneMatrix = maxtrixValue[0];
        cloneMatrix = Matrix4x4.identity;
        maxtrixValue[0] = cloneMatrix;

        // Color
        var cloneColor = colorValue[0];
        cloneColor = Color.red;
        colorValue[0] = cloneColor;
    }
}