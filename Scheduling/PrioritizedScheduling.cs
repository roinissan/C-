using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    class PrioritizedScheduling : SchedulingPolicy
    {
        Queue<int> active_proccess = new Queue<int>();
        private int quantum;

        public PrioritizedScheduling(int iQuantum)
        {
            quantum = iQuantum;
        }

        public override int NextProcess(Dictionary<int, ProcessTableEntry> dProcessTable)
        {
            if (active_proccess.Count == 0)
                return -1;
            int id = highestPriority(dProcessTable);
            removeProcess(id);
            ProcessTableEntry proccess = dProcessTable[id];

            if (!proccess.Done && !proccess.Blocked)
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

        private int highestPriority(Dictionary<int, ProcessTableEntry> dProcessTable) {
            int max = -1;
            int max_id = -1;
            ProcessTableEntry proccess;
            foreach (int id in active_proccess) {
                proccess = dProcessTable[id];
                if (proccess.Priority > max) {
                    max_id = id;
                    max = proccess.Priority;
                }
            }

                return max_id;
        }

        private void removeProcess(int processID) {
            Queue<int> new_active_proccess = new Queue<int>();
            foreach (int id in active_proccess)
            {              
                if (id != processID)
                {
                    new_active_proccess.Enqueue(id);
                }
            }
            active_proccess = new_active_proccess;
        }
    }
}
