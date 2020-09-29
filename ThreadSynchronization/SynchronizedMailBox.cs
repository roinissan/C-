using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ThreadSynchronization
{
    //this class extends the MailBox, by overriding the read and write methods.
    //the override must call the original implementation, which contain races, protecting the critical sections from races.
    //you cannot define a new message array or any other data structue for messages here.
    class SynchronizedMailBox : MailBox
    {
        private Mutex baton;
        private Semaphore wait;


        public SynchronizedMailBox() : base()
        {
            baton = new Mutex();
            wait = new Semaphore(0, 99999999);

        }


        public SynchronizedMailBox(int cMaxMessages):base (cMaxMessages)
        {
            baton = new Mutex();
            wait = new Semaphore(0, cMaxMessages);

        }


        public override void Write(Message msg)
        {
            baton.WaitOne();
            base.Write(msg);                               
            wait.Release();
            baton.ReleaseMutex();
        }

        public override Message Read()
        {
            wait.WaitOne();
            baton.WaitOne();
            
            Message income_mail = base.Read();
            baton.ReleaseMutex();
            return income_mail;
        }

    }
}
