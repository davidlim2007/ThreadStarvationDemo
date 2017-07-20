using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadStarvationDemo
{
    class Program
    {
        // TODO: Comments

        private const string FILEPATH = @"D:\David\test\";

        static void Main(string[] args)
        {
            m_bContinue = true;

            StartThread01();
            StartThread02();
            StartThread03();

            Thread.Sleep(60000 * 5);
            m_bContinue = false;
        }

        static void StartThread01()
        {
            m_thread_01 = new Thread(new ParameterizedThreadStart(WorkerThreadMethod));
            m_thread_01.Priority = ThreadPriority.Highest;
            m_thread_01.Start(FILEPATH + "test_01.txt");
        }

        static void StartThread02()
        {
            m_thread_02 = new Thread(new ParameterizedThreadStart(WorkerThreadMethod));
            m_thread_02.Priority = ThreadPriority.Normal;
            m_thread_02.Start(FILEPATH + "test_02.txt");
        }

        static void StartThread03()
        {
            m_thread_03 = new Thread(new ParameterizedThreadStart(WorkerThreadMethod));
            m_thread_03.Priority = ThreadPriority.Lowest;
            m_thread_03.Start(FILEPATH + "test_03.txt");
        }

        static void WorkerThreadMethod(object obj)
        {
            string str_Filepath = (string)obj;

            bool b_MutexAcquired = false;
            int count = 0;

            try
            {
                while (m_bContinue)
                {
                    b_MutexAcquired = AcquireMutex();
                    count++;
                    ReleaseMutex();
                    b_MutexAcquired = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred on Thread [{0:D}] : " + ex.Message,
                    Thread.CurrentThread.ManagedThreadId);
            }
            finally
            {
                if (b_MutexAcquired)
                {
                    ReleaseMutex();
                }

                File.AppendAllText(str_Filepath,
                    "Count : " + count);
            }
        }



        static bool AcquireMutex()
        {
            return m_mutex.WaitOne();
        }

        static void ReleaseMutex()
        {
            m_mutex.ReleaseMutex();
        }

        private static Thread m_thread_01 = null;
        private static Thread m_thread_02 = null;
        private static Thread m_thread_03 = null;
        private static Mutex m_mutex = new Mutex(false);
        private static bool m_bContinue = false;
    }
}
