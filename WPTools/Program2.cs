using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace WPTools
{
    class Program
    {
        static void Main(string[] args)
        {
            //定义定时器  
            System.Timers.Timer myTimer = new System.Timers.Timer();

            myTimer.Elapsed += new ElapsedEventHandler(myTimer_Elapsed);
            myTimer.Interval = 1800000;//每个半个小时执行一次
            myTimer.Enabled = true;

            //myTimer.AutoReset = true;

            Console.WriteLine("Press the Enter key to exit the program.");
            Console.ReadLine();

            // Keep the timer alive until the end of Main.
            GC.KeepAlive(myTimer);
        }

        static void myTimer_Elapsed(object source, ElapsedEventArgs e)
        {
            try
            {
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":AutoTask is Working!");

                YourTask();
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }
        }

        static void YourTask()
        {
            //在这里写你需要执行的任务  
            Console.WriteLine("开始执行任务了：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}