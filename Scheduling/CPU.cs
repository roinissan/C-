﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduling
{
    class CPU
    {
        private int m_iActiveProcess;
        public int ActiveProcess {
            get
            {
                return m_iActiveProcess;
            }
            set
            {
                TickCount += 2;
                m_iActiveProcess = value;
            }
        }
        public AddressSpace ActiveAddressSpace { get; set; }
        public int ProgramCounter { get; set; }
        public OperatingSystem OperatingSystem { get; set; }
        public Disk Disk { get; private set; }
        public int RemainingTime { get; set; }
        public bool Done { get; set; }
        public ProcessConsole ActiveConsole { get; set; }
        public int TickCount { get; private set; }
        public bool Debug{ get; set; }

        public CPU(Disk disk)
        {
            Disk = disk;
            RemainingTime = -1;
            m_iActiveProcess = -1;
            ProgramCounter = -1;
            Done = false;
            TickCount = 0;
        }

        public void Execute()
        {
            while (!Done)
            {
                if (RemainingTime == 0)
                    OperatingSystem.TimeoutReached();
                TickCount++;
                RemainingTime--;
                ExecuteLine();
                Disk.ProcessRequest();
            }
        }

        public void ExecuteLine()
        {
            try
            {
                if (ProgramCounter == ActiveAddressSpace.Code.LineCount)
                    OperatingSystem.ProcessTerminated(null);
                else
                {
                    string sLine = ActiveAddressSpace.Code[ProgramCounter];
                    if(Debug)
                        Console.WriteLine("pid " + ActiveProcess + "," + ProgramCounter + ": " + sLine);
                    ProgramCounter = ExecuteLine(sLine);
                }
            }
            catch (Exception e)
            {
                OperatingSystem.ProcessTerminated(e);
            }
        }

        private int ExecuteLine(string sLine)
        {
            string[] asLine = sLine.Split(' ');
            if (asLine[0] == "variable")
                DefineVariable(asLine);
            else if (asLine[0] == "if")
                return ProgramCounter + ExecuteIf(asLine);
            else if (asLine[0] == "write")
                WriteLine(sLine);
            else if (asLine[0] == "read")
            {
                ReadToken(asLine);
                return ProgramCounter;
            }
            else if (asLine[0] == "goto")
            {
                return int.Parse(asLine[1]);
            }
            else if (asLine.Length > 1 && asLine[1] == "=")
                AssignValue(asLine);
            else if (asLine[0] == "yield")
            {
                ProgramCounter = ProgramCounter + 1;
                OperatingSystem.ActivateScheduler();
                return ProgramCounter;

            }
            else
                throw new NotImplementedException("Unsupported command " + sLine);
                
            return ProgramCounter + 1;
        }

        private void ReadToken(string[] asLine)
        {
            string sFileName = asLine[1];
            int iTokenNumber = 0;
            if (!int.TryParse(asLine[2], out iTokenNumber))
                iTokenNumber = (int)ActiveAddressSpace[asLine[2]];
            string sParameterName = asLine[3];
            OperatingSystem.ReadToken(sFileName, iTokenNumber, ActiveProcess, sParameterName);
        }

        private void AssignValue(string[] asLine)
        {
            string sVar = asLine[0];
            double dValue = EvaluateNumeric(asLine, 2);
            ActiveAddressSpace[sVar] = dValue;
        }

        private double EvaluateNumeric(string[] asLine, int iCurrent)
        {
            double dCurrent = 0.0;
            if(!double.TryParse(asLine[iCurrent], out dCurrent))
                dCurrent = ActiveAddressSpace[asLine[iCurrent]];
            if (iCurrent == asLine.Length - 1)
                return dCurrent;
            string sOperator = asLine[iCurrent + 1];
            double dRest = EvaluateNumeric(asLine, iCurrent + 2);
            if (sOperator == "+")
                return dCurrent + dRest;
            if (sOperator == "*")
                return dCurrent * dRest;
            if (sOperator == "-")
                return dCurrent - dRest;
            if (sOperator == "/")
                return dCurrent / dRest;
            if (sOperator == "%")
                return ((int)dCurrent) % ((int)dRest);
            throw new NotImplementedException();
        }

        private void WriteLine(string sLine)
        {
            sLine = sLine.Substring(6);//remove the "write"
            string[] asChunks = sLine.Split('+');
            string sOutput = "";
            double dValue = 0.0;
            string sCurrent = "";
            for (int iCurrent = 0; iCurrent < asChunks.Length; iCurrent++)
            {
                sCurrent = asChunks[iCurrent].Trim();
                if (sCurrent.StartsWith("\"") && sCurrent.EndsWith("\""))
                    sOutput += sCurrent.Replace("\"", "");
                else if (double.TryParse(sCurrent, out dValue))
                    sOutput += dValue;
                else
                    sOutput += ActiveAddressSpace[sCurrent];
            }
            ActiveConsole.Write(sOutput);
        }

        private int ExecuteIf(string[] asLine)
        {
            string sParam1 = asLine[1];
            string sOperator = asLine[2];
            string sParam2 = asLine[3];
            double dParam1 = 0.0, dParam2 = 0.0;
            bool bTrue = true;
            if (!double.TryParse(sParam1, out dParam1))
                dParam1 = ActiveAddressSpace[sParam1];
            if (!double.TryParse(sParam2, out dParam2))
                dParam2 = ActiveAddressSpace[sParam2];
            if (sOperator == "=")
            {
                if (double.IsNaN(dParam1))
                    bTrue = double.IsNaN(dParam2);
                else
                    bTrue = dParam1 == dParam2;
            }
            if (sOperator == "<")
                bTrue = dParam1 < dParam2;
            if (sOperator == ">")
                bTrue = dParam1 > dParam2;
            if (sOperator == "<=")
                bTrue = dParam1 <= dParam2;
            if (sOperator == ">=")
                bTrue = dParam1 >= dParam2;
            if (sOperator == "!=")
            {
                if (double.IsNaN(dParam1))
                    bTrue = !double.IsNaN(dParam2);
                else
                    bTrue = dParam1 != dParam2;
            }
            if (bTrue)
                return 1;
            else
                return 2;
        }

        private void DefineVariable(string[] asLine)
        {
            ActiveAddressSpace[asLine[1]] = 0.0;
        }
    }
}
