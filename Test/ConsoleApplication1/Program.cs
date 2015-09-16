using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dos.Common;
using Newtonsoft.Json;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = HttpHelper.Get("", "", 3);
            LogHelper.Debug("哈哈","测试一个");
            return;
            var aa12311 = HttpHelper.Get("http://localhost:1045/Api/Common/SendMsg");
            var aaeq = Dos.CmdHelper.Run("ipconfig");
            return;
            var aa123 = new NewsItem();
            aa123.Test = new
            {
                Test1 = "123",
                Test2 = new
                {
                    Test3 = "456"
                }
            };
            var str22111 = JsonConvert.SerializeObject(aa123);
           // var str2244222 = JsonHelper.Serialize(aa123);


            var str = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\TextFile1.txt");
            var jj = JsonConvert.DeserializeObject<List<NewsItem>>(str);
            var str22 = JsonConvert.SerializeObject(jj);

            //var jj33 = JsonHelper.Deserialize<List<NewsItem>>(str);
            //var str2244 = JsonHelper.Serialize(jj);


            //var jj44 = JsonHelper.Deserialize<List<NewsItem>>(str2244);

            var aa = HttpHelper.Post("http://localhost:1045/Api/Common/SendMsg", new HttpParam() { { "Type", "111" }, { "aa", "aa" } },null,1000);

            var bb = HttpHelper.Post("http://localhost:1045/Api/Common/SendMsg", new HttpParam() { { "Type", "222" }, { "aa", "aa" } }, new HttpParam() { { "Test", "456" } },1000);

            var cc = HttpHelper.Get("http://localhost:1045/Api/Common/GetQrCode", new HttpParam() { { "Type", "333" }, { "aa", "aa" } },1000);
        }
    }
    public class NewsItem
    {
        public const string NodeName = "item";

        public string Title { get; set; }

        public string Description { get; set; }

        public string PicUrl { get; set; }

        public string Url { get; set; }
        public DateTime Time { get; set; }
        public object Test { get; set; }
    }
}
