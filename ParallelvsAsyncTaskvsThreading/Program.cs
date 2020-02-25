using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelvsAsyncTaskvsThreading
{
    class Program
    {
        public static string RequestDataFromServer(string url)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string result = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();
            return result;
        }
        public static async Task<List<Student>> InitStudentAsync(int sizeMax)
        {
            List<Student> listStudent = new List<Student>();
            for (int i = 0; i < sizeMax; i++)
            {
                listStudent.Add(new Student { Id = i, Name = "HoangDat", Age = i });
            }
            return listStudent;
        }
        public static void BrowserListStudent(List<Student> listStudent)
        {
            foreach (Student temp in listStudent)
            {
                temp.Age += 1;
            }
        }
        public static long CheckTimeBrowserAsyncTask(int maxSize, List<Student> listStudent)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Task[] arrayTask = new Task[maxSize];
            for(int i = 0; i < maxSize; i++)
            {
                arrayTask[i] = Task.Run(() => BrowserListStudent(listStudent));
            }
            Task.WaitAll(arrayTask);
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
        public static long CheckTimeBrowserParallel(int maxSize, List<Student> listStudent)
        {
            Stopwatch sw = new Stopwatch();
            
            List<Action> listAction = new List<Action>();
            for(int i=0;i<maxSize;i++)
            {
                listAction.Add(new Action(() => BrowserListStudent(listStudent)));
            }
            sw.Start();
            //Parallel.For(0, maxSize, index => { BrowserListStudent(listStudent); });
            Parallel.Invoke(listAction.ToArray());
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
        public static long CheckTimeBrowserMuityThread(int maxSize, List<Student> listStudent)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Thread[] arrayThread = new Thread[maxSize];
            for (int i = 0; i < maxSize; i++)
            {
                arrayThread[i] = new Thread(()=>BrowserListStudent(listStudent));
                arrayThread[i].Start();
            }
            for(int i=0;i<maxSize;i++)
            {
                arrayThread[i].Join();
            }
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
        public static long checkTimeForLoop(List<Student> listStudent)
        {
            Stopwatch swFor = new Stopwatch();
            swFor.Start();
            for(int i=0;i<listStudent.Count;i++)
            {
                //listStudent[i].Age++;
                EncryptString(listStudent[i].Name);
            }
            swFor.Stop();
            return swFor.ElapsedMilliseconds;
        }
        public static long checkTimeForEachLoop(List<Student> listStudent)
        {
            Stopwatch swForEach = new Stopwatch();
            swForEach.Start();
            foreach(Student temp in listStudent)
            {
                EncryptString(temp.Name);
            }
            swForEach.Stop();
            return swForEach.ElapsedMilliseconds;
        }
        public static long checkTimeListForEach(List<Student> listStudent)
        {
            Stopwatch swListForEach = new Stopwatch();
            swListForEach.Start();
            listStudent.ForEach(element => EncryptString(element.Name));
            swListForEach.Stop();
            return swListForEach.ElapsedMilliseconds;
        }
        public static string EncryptString(string plainInput = "plain test ")
        {
            string key = "b14ca5898a4e4133bbce2ea2315a1916";
            byte[] iv = new byte[16];
            byte[] array;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainInput);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }
        public static async Task<string> DoWorkWiteStudentAsync(Student student)
        {
            return student.ToString();
        }
        public static string DoWorkWiteStudent(Student student)
        {
            return student.ToString();
        }
        static async Task Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();

            // Standard for loop iteration
            //sw.Start();
            //for (int n = 0; n < 100000000; n++)
            //{
            //    string converted = n.ToString();
            //}
            //Console.WriteLine($"Standard for loop takes {sw.ElapsedMilliseconds} milliseconds");

            //// Parallel For loop iteration
            //sw.Restart();
            //Parallel.For(0, 100000000, n => {
            //    string converted = n.ToString();
            //});
            //Console.WriteLine($"Parallel For loop takes {sw.ElapsedMilliseconds} milliseconds");
            //string[] urls = { "https://www.google.com", "https://www.microsoft.com", "https://www.visualstudio.com", "https://www.amazon.com", "https://www.youtube.com", "https://www.fb.com", "https://www.twitter.com" };
            //sw.Restart();
            //foreach (string url in urls)
            //{
            //    string resultRequest = RequestDataFromServer(url);
            //    //Console.WriteLine($" First 10 characters of '{url}' are \n {resultRequest.Substring(0, 10)}");
            //}
            //Console.WriteLine($"Standard foreach loop takes {sw.ElapsedMilliseconds} milliseconds");
            //sw.Restart();
            //Parallel.ForEach(urls, url =>
            // {
            //     string resultRequest = RequestDataFromServer(url);
            //     //Console.WriteLine($" First 10 characters of '{url}' are \n {resultRequest.Substring(0, 10)}");
            // });
            //Console.WriteLine($"Standard Parallel loop takes {sw.ElapsedMilliseconds} milliseconds");
            List<Student> listStudent = await InitStudentAsync(100000000);
            //sw.Start();
            //for(int i=0;i<listStudent.Count;i++)
            //{
            //    string temp =  DoWorkWiteStudent(listStudent[i]);
            //    temp += " stt :" + i.ToString();
            //}
            //Console.WriteLine($"Browser Student async for loop takes {sw.ElapsedMilliseconds} milliseconds");
            //sw.Restart();
            //Parallel.For(0, listStudent.Count, index =>
            // {
            //     string temp = DoWorkWiteStudent(listStudent[index]);
            //     temp += " stt :" + index.ToString();
            // });
            //Console.WriteLine($"Browser Student paralel foreach loop takes {sw.ElapsedMilliseconds} milliseconds");


            int maxSize = 35;
            Console.WriteLine($"Browser Student  use multyThreding  takes {CheckTimeBrowserMuityThread(maxSize, listStudent).ToString()} milliseconds");

            Console.WriteLine($"Browser Student  use multyTask  takes {CheckTimeBrowserAsyncTask(maxSize, listStudent).ToString()} milliseconds");

            Console.WriteLine($"Browser Student  use Parallel  takes {CheckTimeBrowserParallel(maxSize, listStudent).ToString()} milliseconds");


            //Console.WriteLine($"Browser Student  use FORLOOP  takes {checkTimeForLoop(listStudent).ToString()} milliseconds");

            //Console.WriteLine($"Browser Student  use FOREACHLOOP  takes {checkTimeForEachLoop(listStudent).ToString()} milliseconds");

            //Console.WriteLine($"Browser Student  use LISTFOREACHLOOP  takes {checkTimeListForEach(listStudent).ToString()} milliseconds");

            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}
