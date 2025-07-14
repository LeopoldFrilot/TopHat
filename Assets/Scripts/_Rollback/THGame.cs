using SharedGame;
using Unity.Collections;

public class THGame : IGame
{
    public int Framenumber { get; }
    public int Checksum { get; }
    public void Update(long[] inputs, int disconnectFlags)
    {
        throw new System.NotImplementedException();
    }

    public void FromBytes(NativeArray<byte> data)
    {
        throw new System.NotImplementedException();
    }

    public NativeArray<byte> ToBytes()
    {
        throw new System.NotImplementedException();
    }

    public long ReadInputs(int controllerId)
    {
        throw new System.NotImplementedException();
    }

    public void LogInfo(string filename)
    {
        throw new System.NotImplementedException();
    }

    public void FreeBytes(NativeArray<byte> data)
    {
        throw new System.NotImplementedException();
    }
}