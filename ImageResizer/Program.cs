using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourcePath = Path.Combine(Environment.CurrentDirectory, "images");
            string destinationPath = Path.Combine(Environment.CurrentDirectory, "output"); ;
            long SyncAveTime=0;
            long AsyncAveTime = 0;
            int countAll = 0;

            ImageProcess imageProcess = new ImageProcess();

            Console.WriteLine("同步時間Start");
            for (var i=0;i<20;i++)
            {
                imageProcess.Clean(destinationPath);
                Stopwatch sw = new Stopwatch();
                sw.Start();
                imageProcess.ResizeImages(sourcePath, destinationPath, 2.0); 
                sw.Stop();
                Console.WriteLine($"第{i+1}次花費時間: {sw.ElapsedMilliseconds} ms");
                SyncAveTime += sw.ElapsedMilliseconds;
                countAll++;
            }
            Console.WriteLine("同步時間End\n");

            SyncAveTime = SyncAveTime / countAll;
            countAll = 0;
            Console.WriteLine($"同步平均時間:{SyncAveTime} ms\n");
            imageProcess.Clean(destinationPath);
            Console.WriteLine("非同步時間Start");
            for (var i = 0; i < 20; i++)
            {
                imageProcess.CleanAsync(destinationPath);

                Stopwatch sw = new Stopwatch();
                sw.Start();
                imageProcess.ResizeImagesAsync(sourcePath, destinationPath, 2.0);
                sw.Stop();
                Console.WriteLine($"第{i+1}次花費時間: {sw.ElapsedMilliseconds} ms");
                AsyncAveTime += sw.ElapsedMilliseconds;
                countAll++;
            }
            Console.WriteLine("非同步時間End\n");

            AsyncAveTime = AsyncAveTime / countAll;
            countAll = 0;
            Console.WriteLine($"非同步平均時間:{AsyncAveTime} ms\n");


            Console.WriteLine(string.Format("提升百分比:{0} %", ((double)(SyncAveTime - AsyncAveTime) / (double)SyncAveTime)*100).ToString());
            //_log.Trace(sw.ElapsedMilliseconds);
            Console.ReadKey();
        }
    }
}
