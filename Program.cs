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
        private const string FILEPATH = @"D:\David\test\";

        static void Main(string[] args)
        {
            StartThread01();
            StartThread02();
            StartThread03();
            StartThread04();
            StartThread05();

            // The threads need to run for
            // a considerable amount of time
            // (10 minutes in this case).
            //
            // The longer the threads run, the more
            // accurate the results yielded.
            Thread.Sleep(60000 * 10);

            // Once the main thread has finished
            // its sleep, set m_bContinue to false
            // to allow the threads to end.
            m_bContinue = false;

            // The threads are allowed to end naturally 
            // instead of being aborted.
            //
            // Aborting each thread will affect the accuracy
            // of results, since while one thread is being
            // aborted, the other threads will continue
            // running.
        }

        static void StartThread01()
        {
            m_thread_01 = new Thread(new ParameterizedThreadStart(WorkerThreadMethod));

            // Threads 01 - 04 are given the Highest priority.
            //
            // This grants them the most CPU time to perform
            // their tasks.
            m_thread_01.Priority = ThreadPriority.Highest;
            m_thread_01.Start(FILEPATH + "test_01.txt");
        }

        static void StartThread02()
        {
            m_thread_02 = new Thread(new ParameterizedThreadStart(WorkerThreadMethod));
            m_thread_02.Priority = ThreadPriority.Highest;
            m_thread_02.Start(FILEPATH + "test_02.txt");
        }

        static void StartThread03()
        {
            m_thread_03 = new Thread(new ParameterizedThreadStart(WorkerThreadMethod));
            m_thread_03.Priority = ThreadPriority.Highest;
            m_thread_03.Start(FILEPATH + "test_03.txt");
        }

        static void StartThread04()
        {
            m_thread_04 = new Thread(new ParameterizedThreadStart(WorkerThreadMethod));            
            m_thread_04.Priority = ThreadPriority.Highest;
            m_thread_04.Start(FILEPATH + "test_04.txt");
        }

        static void StartThread05()
        {
            m_thread_05 = new Thread(new ParameterizedThreadStart(WorkerThreadMethod));

            // The priority of thread 05 is set to Lowest.
            //
            // This grants it the least CPU time to perform
            // its tasks.
            m_thread_05.Priority = ThreadPriority.Lowest;
            m_thread_05.Start(FILEPATH + "test_05.txt");
        }

        static void WorkerThreadMethod(object obj)
        {
            // The parameter passed into WorkerThreadMethod
            // is the path to the file that the current
            // thread wants to access.
            //
            // Each thread has its own file to write data
            // to.
            string str_Filepath = (string)obj;

            // b_MutexAcquired indicates whether or not
            // the current thread has acquired the Mutex.
            // Initially set to false.
            bool b_MutexAcquired = false;
            int count = 0;

            try
            {
                // This while loop will continue as long
                // as m_bContinue is true.
                //
                // In each iteration of the loop, the thread
                // attempts to acquire the Mutex. It will wait
                // until the Mutex has been acquired. Once this
                // happens, b_MutexAcquired will be set to true.
                //
                // The value of count increments. count is an
                // accumulator variable meant to represent the
                // total number of while loop iterations each
                // thread undergoes.
                //
                // Finally, the thread releases the Mutex are resets
                // the value of b_MutexAcquired before the while loop
                // reiterates.
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
                // The thread reaches this section once
                // m_bContinue is set to false.
                //
                // If b_MutexAcquired is true (i.e. the thread
                // has ownership of the Mutex), it releases the
                // Mutex.
                //
                // It then writes the value of count to a specified
                // file.
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
        private static Thread m_thread_04 = null;
        private static Thread m_thread_05 = null;

        private static Mutex m_mutex = new Mutex(false);

        // A boolean variable that dictates whether
        // or not the threads should continue
        // running.
        //
        // Initially set to true, to allow the threads
        // to run freely.
        private static bool m_bContinue = true;
    }
}
