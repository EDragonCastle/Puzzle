using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class TestMainThread : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            //NoneSharedMember();
            //SharedMember();
            //TestingNativeMember();
        }
    }

    private void NoneSharedMember()
    {
        JobA jobA = new JobA();
        JobHandle jobHandle = jobA.Schedule();

        UnityEngine.Debug.Log("Main thread Done");

        jobHandle.Complete();
    }
    private void SharedMember()
    {
        // Allocator type�� ���� �� �ֳ�?
        NativeArray<float> jobResult = new NativeArray<float>(1, Allocator.TempJob);

        JobB jobB = new JobB() 
        { 
            result = jobResult, 
            value = 0, 
            addNumber = 2, 
            mulNumber = 2 
        };

        // Thread �и� ����
        JobHandle jobHandle = jobB.Schedule();

        // ���� ������!
        UnityEngine.Debug.Log("Main thread Done");
        float mainNumber = 0;
        mainNumber += 5;
        mainNumber *= 2;

        // Thread ��ü
        jobHandle.Complete();

        // ��ģ ������� �̰� ����
        mainNumber += jobResult[0];
        Debug.Log($"{mainNumber}");

        // �� ������ �޸� �ݳ�
        jobResult.Dispose();
    }
    private void TestingNativeMember()
    {
        // Allocator�� job�� ������ �����ȴ�.
        var nativeInt = new NativeArray<int>(1, Allocator.TempJob);
        var nativeBool = new NativeArray<bool>(1, Allocator.TempJob);
        var nativeJobInfo = new NativeArray<JobInfo>(1, Allocator.TempJob);
        var nativeVector2 = new NativeArray<Vector2>(1, Allocator.TempJob);
        var nativeQuaternion = new NativeArray<Quaternion>(1, Allocator.TempJob);
        var nativeMatrix = new NativeArray<Matrix4x4>(1, Allocator.TempJob);
        var nativeColor = new NativeArray<Color>(1, Allocator.TempJob);

        var jobC = new JobC()
        {
            boolValue = nativeBool,
            colorValue = nativeColor,
            intValue = nativeInt,
            maxtrixValue = nativeMatrix,
            vec2Value = nativeVector2,
            quaternionValue = nativeQuaternion,
            structValue = nativeJobInfo,
        };

        JobHandle jobHandle = jobC.Schedule();

        for(int i = 0; i < 10; i++)
        {
            Debug.Log($"working... {i}");
        }

        jobHandle.Complete();


        // ����ϸ� �ݵ�� �ݳ��ؾ� �Ѵ�.
        nativeInt.Dispose();
        nativeBool.Dispose();
        nativeJobInfo.Dispose();
        nativeVector2.Dispose();
        nativeQuaternion.Dispose();
        nativeMatrix.Dispose();
        nativeColor.Dispose();
    }


}
