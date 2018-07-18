using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Collections.Generic;

namespace WPTools
{
    class Program
    {
        //定义日志列表
        static string logPath = AppDomain.CurrentDomain.BaseDirectory + "log.log";
        //要加载的站点列表
        static string siteUrls = AppDomain.CurrentDomain.BaseDirectory + "site.log";
        static List<string> lsSiteUrls = new List<string>();
        
        static void Main(string[] args)
        {
            lsSiteUrls = LoadTextToList(siteUrls, true);

            #region 定时核心代码

            System.Threading.Timer timer = null;

            //计算现在到目标时间要过的时间段。 规定在每天凌晨一点执行任务
            DateTime LuckTime = DateTime.Now.Date.Add(new TimeSpan(1, 0, 0));

            TimeSpan span = LuckTime - DateTime.Now;

            //如果小Zero 表示时间已经执行过了,就在当前时间上加上一天
            if (span < TimeSpan.Zero)
            {
                span = LuckTime.AddDays(1d) - DateTime.Now;
            }

            //按需传递的状态或者对象。 
            object state = new object();

            //定义计时器 
            timer = new System.Threading.Timer(new System.Threading.TimerCallback(CertainTask),
                state, span, TimeSpan.FromTicks(TimeSpan.TicksPerDay));

            Console.WriteLine("Press the Enter key to exit the program.");

            Console.ReadLine();

            // Keep the timer alive until the end of Main.
            GC.KeepAlive(timer);

            #endregion

            //string code = RequestGet("http://127.0.0.1/wp/wztools.php?action=update_post_status", null, 1);
            //Console.WriteLine(code);
        }

        private static void CertainTask(object state)
        {
            //这里写你的任务逻辑 
            //Console.WriteLine("定时器执行了：");
            //TraceLog("定时器确实执行了：", logPath);
            RunMain();
        }

        private static void RunMain()
        {
            try
            {
                //在此处循环的对站点列表发起post请求
                foreach (string item in lsSiteUrls)
                {
                    string Code = RequestGet(item, null, 1);
                    Console.WriteLine(item + " is " + Code);
                    TraceLog(item + " is " + Code, logPath);
                }
            }
            catch (Exception ex)
            {
                TraceLog(ex.Message, logPath);
            }
        }

        #region HTTP GET方式提交数据

        /// <summary>
        /// HTTP GET方式提交数据
        /// </summary>
        /// <param name="TheURL">提交的URL</param>
        /// <param name="cookies">cookie</param>
        /// <param name="type">0 表示获取源代码 1表示获取状态码</param>
        /// <returns></returns>
        public static string RequestGet(string TheURL, CookieCollection cookies, int type)
        {
            System.Net.ServicePointManager.Expect100Continue = false;

            //1.定义URL地址
            Uri uri = new Uri(TheURL);

            //2.建立HttpWebRequest对象
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            string page = string.Empty;
            try
            {
                request.KeepAlive = false;
                //设置Htpp协议的版本
                request.ProtocolVersion = HttpVersion.Version11;
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                //allow auto redirects from redirect headers 允许请求跟随重定向相应
                request.AllowAutoRedirect = true;
                //maximum of 10 auto redirects 
                request.MaximumAutomaticRedirections = 10;
                //30 second timeout for request定义到服务器的超时时间 默认是100秒
                request.Timeout = (int)new TimeSpan(0, 0, 60).TotalMilliseconds;
                //give the crawler a name. 
                //request.UserAgent = "Mozilla/3.0 (compatible; My Browser/1.0)";
                //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
                request.ServicePoint.Expect100Continue = false;
                if (cookies != null)
                {
                    request.CookieContainer = new CookieContainer();
                    request.CookieContainer.Add(cookies);
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                switch (type)
                {
                    case 0:
                        Stream responseStream = response.GetResponseStream();
                        StreamReader readStream = new StreamReader(responseStream, System.Text.Encoding.UTF8);

                        page = readStream.ReadToEnd();
                        readStream.Close();
                        responseStream.Close();

                        break;
                    case 1:
                        page = response.StatusCode.ToString();
                        response.Close();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ee)
            {
                page = "Fail message : " + ee.Message;
            }
            return page;
        }

        #endregion

        #region 追踪日志查看结果
        /// <summary>
        /// 追踪日志查看结果
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        public static void TraceLog(string data, string filePath)
        {
            try
            {
                File.AppendAllText(filePath, string.Concat(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), " ", data, Environment.NewLine));
            }
            catch (Exception ex)
            {
                File.AppendAllText(filePath, string.Concat(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), " ", ex.Message, Environment.NewLine));
            }
        }
        #endregion

        #region LoadTextToList
        /// <summary>
        /// txt之将txt中的数据变为个List
        /// </summary>
        /// <param name="filePath">文本的路径</param>
        /// <param name="bl">是否去除掉文本中的空格，txt，js html文件</param>
        /// <returns></returns>
        public static List<string> LoadTextToList(string filePath, bool bl)
        {
            List<string> ls = new List<string>();
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (bl)
                        {
                            ls.Add(line.Trim());
                        }
                        else
                        {
                            ls.Add(line);
                        }
                    }
                }
                return ls;
            }
        }
        #endregion
    }
}