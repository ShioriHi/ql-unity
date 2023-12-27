using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class SerialCommunication : MonoBehaviour
{
    SerialReceiver distanceSensor = new SerialReceiver();

    public string DeviceName;

    public string portName;
    public int baudRate;

    public int data;


    // Start is called before the first frame update
    void Start()
    {
        distanceSensor.Open(portName, baudRate, Parity.None,8,StopBits.One);
    }

    // Update is called once per frame
    void Update()
    {
        data = distanceSensor.data;
    }
}

public class SerialReceiver : SerialCommunicator
{
    public int data;


    private void DataReceiveFunction()
    {
        data = int.Parse(message);
    }

    public new void Open(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
    {
        base.Open(portName, baudRate, parity, dataBits, stopBits);
        DataTransformHandler += DataReceiveFunction;
    }
}

public class SerialCommunicator : System.IDisposable
{
    protected SerialPort serialPort;
    protected string message;
    Thread thread;

    public delegate void DataTransform();
    public DataTransform DataTransformHandler;

    protected bool isRunning = false;

    public void Open(string portName,int baudRate,Parity parity,int dataBits,StopBits stopBits)
    {
        serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
        serialPort.Open();

        isRunning = true;

        thread = new Thread(Read);

        thread.Start();

    }

    private void Read()
    {
        while(isRunning && serialPort != null && serialPort.IsOpen)
        {
            if (serialPort.BytesToRead > 0)
            {
                message = serialPort.ReadLine();
                DataTransformHandler?.Invoke();
            }
            Thread.Sleep(0);
        }
    }

    public void Close()
    {
        isRunning = false;

        if (thread != null && thread.IsAlive)
            thread.Join();

        if(serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            serialPort.Dispose();
        }
    }

    public void Dispose()
    {
        Close();
    }


}




