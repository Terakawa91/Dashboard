using OpenHardwareMonitor.Hardware;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.IO.Ports;

namespace OHM
{
    class Program
    {
        static Computer _thisComputer;
        static FileStream _fs;

        static SerialPort arduSerialPort = new SerialPort();

        static void Main(string[] args)
        {
            arduSerialPort.PortName = "COM4";
            arduSerialPort.BaudRate = 9600;
            try
            {
            arduSerialPort.Open();
            arduSerialPort.Close();
            } catch
            {

            }

            _thisComputer = new Computer() { CPUEnabled = true, GPUEnabled = true, MainboardEnabled = true, RAMEnabled = true, HDDEnabled = false };

            _thisComputer.Open();

            Console.CancelKeyPress += Console_CancelKeyPress;

            DateTime now = DateTime.Now;
            string fileStart = now.ToString("yyyyMMdd_HHmmss") + ".log";
            _fs = new FileStream(fileStart, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

            while (true)
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder log = new StringBuilder();

                foreach (var hardwareItem in _thisComputer.Hardware)
                {
                    switch (hardwareItem.HardwareType)
                    {
                        case HardwareType.CPU:
                        case HardwareType.GpuNvidia:
                        case HardwareType.HDD:
                        case HardwareType.Mainboard:
                        case HardwareType.RAM:
                            AddCpuInfo(sb, log, hardwareItem);
                            break;
                    }
                }

                log.AppendLine();
                Console.WriteLine(sb.ToString());

                byte[] buf = Encoding.ASCII.GetBytes(log.ToString());
                _fs.Write(buf, 0, buf.Length);
                _fs.Flush();
                Thread.Sleep(1000);
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _fs.Close();
            _thisComputer.Close();

            Console.WriteLine("End-of-profile");
        }

        private static void AddCpuInfo(StringBuilder sb, StringBuilder log, IHardware hardwareItem)
        {
            hardwareItem.Update();
            foreach (IHardware subHardware in hardwareItem.SubHardware)
                subHardware.Update();

            string text;

            int i = 0;

            foreach (var sensor in hardwareItem.Sensors)
            {
                string name = sensor.Name;
                string value = sensor.Value.HasValue ? sensor.Value.Value.ToString() : "-1";


                //Console.WriteLine(name + sensor.SensorType);

                switch (name)
                {
                    //CPU
                    case "CPU Total" when sensor.SensorType == SensorType.Load:
                        text = $"{name} {SensorType.Load} = {value}";
                        sb.AppendLine(text);
                        break;

                    case "CPU Package" when sensor.SensorType == SensorType.Temperature:
                        text = $"{name} {SensorType.Temperature} = {value}";
                        sb.AppendLine(text);
                        break;

                    case "CPU Package" when sensor.SensorType == SensorType.Power:
                        text = $"{name} {SensorType.Power} = {value}";
                        sb.AppendLine(text);
                        break;

                    //GPU
                    case "GPU Core" when sensor.SensorType == SensorType.Load:
                        text = $"{name} {SensorType.Load} = {value}";
                        sb.AppendLine(text);
                        break;

                    case "GPU Core" when sensor.SensorType == SensorType.Temperature:
                        text = $"{name} {SensorType.Temperature} = {value}";
                        sb.AppendLine(text);
                        break;

                    //MEMORY
                    case "Memory" when sensor.SensorType == SensorType.Load:
                        text = $"{name} {SensorType.Load} = {value}";
                        sb.AppendLine(text);
                        break;

                    case "Used Memory" when sensor.SensorType == SensorType.Data:
                        text = $"{name} {SensorType.Data} = {value}";
                        sb.AppendLine(text);
                        break;


                }
            }
        }
    }
}

//CPU Core #1
//CPU Core #2
//CPU Core #3
//CPU Core #4
//CPU Core #5
//CPU Core #6
//CPU Total
//CPU Package
//Bus Speed
//CPU Core #1
//CPU Core #2
//CPU Core #3
//CPU Core #4
//CPU Core #5
//CPU Core #6
//CPU Package
//CPU CCD #1
//CPU Core #1
//CPU Core #2
//CPU Core #3
//CPU Core #4
//CPU Core #5
//CPU Core #6
//CPU Cores
//Memory
//Used Memory
//Available Memory
//GPU Core
//GPU Core
//GPU Memory
//GPU Shader
//GPU Core
//GPU Frame Buffer
//GPU Video Engine
//GPU Bus Interface
//GPU Fan
//GPU
//GPU Memory Total
//GPU Memory Used
//GPU Memory Free
//GPU Memory
