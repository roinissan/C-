using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    class RoundRobin : SchedulingPolicy
    {
        Queue<int> active_proccess = new Queue<int>();
        private int quantum;
        public RoundRobin(int iQuantum)
        {
            quantum = iQuantum;
        }

        public override int NextProcess(Dictionary<int, ProcessTableEntry> dProcessTable)
        {
            if (active_proccess.Count == 0)
                return -1;
            int id = active_proccess.Dequeue();
            ProcessTableEntry proccess = dProcessTable[id];
            
            
            if ( !proccess.Done && !proccess.Blocked)
            {
                proccess.Quantum = quantum;
                return id;
            }

            return -1;
        }

        public override void AddProcess(int iProcessId)
        {
            active_proccess.Enqueue(iProcessId);
        }

        public override bool RescheduleAfterInterrupt()
        {
            return true;
        }
    }
}
