using OpenHardwareMonitor.Hardware;
using System;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace OHM
{
    class Program
    {

        static Computer _thisComputer;
        static void Main(string[] args)
        {
            _thisComputer = new Computer() { CPUEnabled = true, GPUEnabled = true, MainboardEnabled = true, RAMEnabled = true, HDDEnabled = false };

            _thisComputer.Open();

            Console.CancelKeyPress += Console_CancelKeyPress;

            while (true)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var hardwareItem in _thisComputer.Hardware)
                {
                    switch (hardwareItem.HardwareType)
                    {
                        case HardwareType.CPU:
                        case HardwareType.GpuNvidia:
                        case HardwareType.HDD:
                        case HardwareType.Mainboard:
                        case HardwareType.RAM:
                            AddCpuInfo(sb, hardwareItem);
                            break;
                    }
                }

                UdpClient cli = new UdpClient();

                string msg = sb.ToString();
                byte[] datagram = Encoding.UTF8.GetBytes(msg);

                cli.Send(datagram, datagram.Length, "your arduino ip", 0000); // 0000 is your port

                Console.Write(sb.ToString());
                Console.WriteLine("[Sent] to 192.168.0.52 {0} byte", datagram.Length);
                cli.Close();

                Thread.Sleep(500);
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _thisComputer.Close();

            Console.WriteLine("End-of-profile");
        }

        private static void AddCpuInfo(StringBuilder sb, IHardware hardwareItem)
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

                switch (name)
                {
                    //CPU
                    case "CPU Total" when sensor.SensorType == SensorType.Load:
                        text = Math.Round(Double.Parse(value)) + "";
                        if (text.Length < 2)
                        {
                            sb.Append("0" + text);
                        }
                        else
                        {
                            sb.Append(text);
                        }
                        break;

                    case "CPU Package" when sensor.SensorType == SensorType.Temperature:
                        text = Math.Round(Double.Parse(value)) + "";
                        if (text.Length < 2)
                        {
                            sb.Append("0" + text + ",");
                        }
                        else
                        {
                            sb.Append(text + ",");
                        }
                        break;
                        
                    //GPU
                    case "GPU Core" when sensor.SensorType == SensorType.Load:
                        text = Math.Round(Double.Parse(value)) + "";
                        if (text.Length < 2)
                        {
                            sb.Append("0" + text);
                        }
                        else
                        {
                            sb.Append(text);
                        }
                        break;

                    case "GPU Core" when sensor.SensorType == SensorType.Temperature:
                        text = Math.Round(Double.Parse(value)) + "";
                        if (text.Length < 2)
                        {
                            sb.Append("0" + text);
                        }
                        else
                        {
                            sb.Append(text);
                        }
                        break;

                    //MEMORY
                    case "Memory" when sensor.SensorType == SensorType.Load:
                        text = Math.Round(Double.Parse(value)) + "";
                        if (text.Length < 2)
                        {
                            sb.Append("0" + text);
                        }
                        else
                        {
                            sb.Append(text);
                        }
                        break;

                    case "Used Memory" when sensor.SensorType == SensorType.Data:
                        //text = $"{name} {SensorType.Data} = {value}";
                        text = Math.Round(Double.Parse(value)) + "";
                        if (text.Length < 2)
                        {
                            sb.Append("0" + text + ",");
                        }
                        else
                        {
                            sb.Append(text + ",");
                        }
                        break;
                }
            }
        }
    }
}
// CPU Total Load, CPU Package Power, CPU Package Temperature, Memory Load, Used Memory Data, GPU Core Temperature, GPU Core Load
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
