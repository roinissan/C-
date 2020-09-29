using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    class OperatingSystem
    {
        public Disk Disk { get; private set; }
        public CPU CPU { get; private set; }
        private Dictionary<int, ProcessTableEntry> m_dProcessTable;
        private List<ReadTokenRequest> m_lReadRequests;
        private int m_cProcesses;
        private SchedulingPolicy m_spPolicy;
        private static int IDLE_PROCESS_ID = 0;

        public OperatingSystem(CPU cpu, Disk disk, SchedulingPolicy sp)
        {
            CPU = cpu;
            Disk = disk;
            m_dProcessTable = new Dictionary<int, ProcessTableEntry>();
            m_lReadRequests = new List<ReadTokenRequest>();
            cpu.OperatingSystem = this;
            disk.OperatingSystem = this;
            m_spPolicy = sp;

            IdleCode idleCode = new IdleCode();            
            m_dProcessTable[IDLE_PROCESS_ID] = new ProcessTableEntry(IDLE_PROCESS_ID, "idle", idleCode);
            m_dProcessTable[IDLE_PROCESS_ID].StartTime = CPU.TickCount;
            m_spPolicy.AddProcess(m_cProcesses);
            m_cProcesses++;
            
        }


        public void CreateProcess(string sCodeFileName)
        {
            Code code = new Code(sCodeFileName);
            m_dProcessTable[m_cProcesses] = new ProcessTableEntry(m_cProcesses, sCodeFileName, code);
            m_dProcessTable[m_cProcesses].StartTime = CPU.TickCount;
            m_spPolicy.AddProcess(m_cProcesses);
            m_cProcesses++;
        }
        public void CreateProcess(string sCodeFileName, int iPriority)
        {
            Code code = new Code(sCodeFileName);
            m_dProcessTable[m_cProcesses] = new ProcessTableEntry(m_cProcesses, sCodeFileName, code);
            m_dProcessTable[m_cProcesses].Priority = iPriority;
            m_dProcessTable[m_cProcesses].StartTime = CPU.TickCount;
            m_spPolicy.AddProcess(m_cProcesses);
            m_cProcesses++;
        }

        public void ProcessTerminated(Exception e)
        {
            if (e != null)
                Console.WriteLine("Process " + CPU.ActiveProcess + " terminated unexpectedly. " + e);
            m_dProcessTable[CPU.ActiveProcess].Done = true;
            m_dProcessTable[CPU.ActiveProcess].Console.Close();
            m_dProcessTable[CPU.ActiveProcess].EndTime = CPU.TickCount;
            ActivateScheduler();
        }

        public void TimeoutReached()
        {
            ActivateScheduler();
        }

        public void ReadToken(string sFileName, int iTokenNumber, int iProcessId, string sParameterName)
        {
            ReadTokenRequest request = new ReadTokenRequest();
            request.ProcessId = iProcessId;
            request.TokenNumber = iTokenNumber;
            request.TargetVariable = sParameterName;
            request.Token = null;
            request.FileName = sFileName;
            m_dProcessTable[iProcessId].Blocked = true;
            if (Disk.ActiveRequest == null)
                Disk.ActiveRequest = request;
            else
                m_lReadRequests.Add(request);
            CPU.ProgramCounter = CPU.ProgramCounter + 1;
            ActivateScheduler();
        }

        public void Interrupt(ReadTokenRequest rFinishedRequest)
        {
            //implement an "end read request" interrupt handler.
            //translate the returned token into a value (double). 
            //when the token is null, EOF has been reached.
            //write the value to the appropriate address space of the calling process.
            //activate the next request in queue on the disk.

            ProcessTableEntry active_process = m_dProcessTable[rFinishedRequest.ProcessId];
            String var_name = rFinishedRequest.TargetVariable;
            String var_value = rFinishedRequest.Token;
            if (var_value == null)
            {
                active_process.AddressSpace[var_name] = double.NaN;
            }

            else {
                active_process.AddressSpace[var_name] = Convert.ToDouble(var_value);
            }

            m_dProcessTable[rFinishedRequest.ProcessId].Blocked = false;
            m_spPolicy.AddProcess(rFinishedRequest.ProcessId);
            m_lReadRequests.Remove(rFinishedRequest);
            if (m_lReadRequests.Count > 0) {
                ReadTokenRequest awaiting_procees_request = m_lReadRequests.First();
                //m_dProcessTable[awaiting_procees_request.ProcessId].Blocked = true;
                Disk.ActiveRequest = awaiting_procees_request;
            }
            if (m_spPolicy.RescheduleAfterInterrupt())
                ActivateScheduler();
        }

        private ProcessTableEntry ContextSwitch(int iEnteringProcessId)
        {
            ProcessTableEntry out_process = null;
            if (CPU.ActiveProcess != -1) {

                out_process = m_dProcessTable[CPU.ActiveProcess];
                out_process.Console = CPU.ActiveConsole;
                out_process.ProgramCounter = CPU.ProgramCounter;
                out_process.LastCPUTime = CPU.TickCount;
                if (m_spPolicy is RoundRobin || m_spPolicy is PrioritizedScheduling) {
                    if (!out_process.Done) {
                        m_spPolicy.AddProcess(iEnteringProcessId);
                        CPU.RemainingTime = out_process.Quantum;

                    }

                }

            }
            ProcessTableEntry in_process = m_dProcessTable[iEnteringProcessId];
            CPU.ActiveProcess = iEnteringProcessId;
            CPU.ProgramCounter = in_process.ProgramCounter;
            CPU.ActiveConsole = in_process.Console;
            CPU.ActiveAddressSpace = in_process.AddressSpace;
            if (m_spPolicy is RoundRobin || m_spPolicy is PrioritizedScheduling)
            {
                CPU.RemainingTime = in_process.Quantum;              
            }
            return out_process;

        }

        public void ActivateScheduler()
        {
            int iNextProcessId = m_spPolicy.NextProcess(m_dProcessTable);
            if (iNextProcessId == -1)
            {
                Console.WriteLine("All processes terminated or blocked.");
                CPU.Done = true;
            }
            else
            {
                bool bOnlyIdleRemains = false;
                if (iNextProcessId == IDLE_PROCESS_ID)
                {
                    bOnlyIdleRemains = true;
                    foreach (ProcessTableEntry e in m_dProcessTable.Values)
                    {
                        if (e.Name != "idle" && e.Done != true)
                        {
                            bOnlyIdleRemains = false;
                        }
                    }
                }
                if(bOnlyIdleRemains)
                {
                    Console.WriteLine("Only idle remains.");
                    CPU.Done = true;
                }
                else
                    ContextSwitch(iNextProcessId);
            }
        }

        public double AverageTurnaround()
        {
            int sum_of_times = 0;
            int num_of_proccess = m_dProcessTable.Count;
            if (num_of_proccess == 0)
                return 0;
            for (int id = 1; id < m_dProcessTable.Count; id++)
                sum_of_times = sum_of_times + (m_dProcessTable[id].EndTime - m_dProcessTable[id].StartTime);
            return sum_of_times / num_of_proccess;
        }
        public int MaximalStarvation()
        {
            return -1;
        }
    }
}
