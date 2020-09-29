using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    class FirstComeFirstServedPolicy : SchedulingPolicy
    {

        Queue<int> active_proccess = new Queue<int>();
        public override int NextProcess(Dictionary<int, ProcessTableEntry> dProcessTable)
        {
            if (active_proccess.Count == 0)
                return -1;

            foreach (int id in active_proccess)
            {
                ProcessTableEntry proccess = dProcessTable[id];
                if (proccess.ProcessId != 0 && !proccess.Done && !proccess.Blocked)
                {
                    return id;
                }
            }
            return -1;
        }

        public override void AddProcess(int iProcessId)
        {
            active_proccess.Enqueue(iProcessId);
        }

        public override bool RescheduleAfterInterrupt()
        {
            return false;
        }
    }
}
