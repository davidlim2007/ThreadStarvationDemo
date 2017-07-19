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
        /*
        
        1. Run a few threads (start with 3 threads). 

        2. Each thread is to acquire one mutex to write a line to its own text file.

        - The line is to include the thread's ID.
        - Note that each thread has its own text file.
        - Hence do not make the threads share one single thread method.
        - Remember that there is only one mutex shared by all threads.

        3. Each thread is to contain try/catch/finally blocks and each thread to handle 
        the thread abort exception.

        4. Set one thread to have the lowest priority. 

        5. The main thread to start all threads and then itself sleep for a period of time 
        (maybe 5 seconds) while the worker threads do their job. 

        - At the end of the 5 seconds, abort all threads.
        - Explain that even if a thread is waiting on a mutex, it can be aborted as well.
        - Show that there are 3 text files and that the text file written by the lowest priority 
        thread has the least number of lines.

        6. We can later evolve the program further to show how starvation can cause more undesireable 
        effects. 

        */

        private const string FILEPATH = @"D:\David\test\";

        static void Main(string[] args)
        {
            StartThread01();
            StartThread02();
            StartThread03();

            Thread.Sleep(5000);

            Thread[] threads = new Thread[] { m_thread_01, m_thread_02, m_thread_03 };

            for (int i = 0; i < threads.Length; i++)
            {
                AbortThread(ref threads[i]);
            }
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

            try
            {
                while (true)
                {
                    b_MutexAcquired = AcquireMutex();

                    File.AppendAllText(str_Filepath, "Thread "
                        + Thread.CurrentThread.ManagedThreadId + " has the Mutex."
                        + "\r\n");

                    ReleaseMutex();
                }
            }
            catch (ThreadAbortException taex)
            {
                Console.WriteLine("Thread [{0:D}] has been aborted.",
                    Thread.CurrentThread.ManagedThreadId);
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
            }
        }

        static void AbortThread(ref Thread thread)
        {
            thread.Abort();
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
    }
}
