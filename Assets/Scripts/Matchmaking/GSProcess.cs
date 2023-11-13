using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class GSProcess
{
    private Process process;

    public GSProcess(int port, string password)
    {
        string path = @"D:\ISAMM Formation\PongMultiplayer-ST\Build\Server\PongMultiplayer.exe";

        string args = string.Empty;
        args += " -port " + port;
        args += " -password " + password;

        process = new Process();
        process.StartInfo.FileName = path;
        process.StartInfo.Arguments = args;
        process.Start();
    }

    public void Close()
    {
        try
        {
            if (process != null && !process.HasExited)
                process.Kill();
        }
        catch (Exception)
        {
            Debug.Log("Close Unity Instance Exception");
            throw;
        }
    }
}