﻿/* =============================================================================*\
*
* Filename: ToolKit
* Description: 
*
* Version: 1.0
* Created: 2017/11/23 15:05:36 (UTC+8:00)
* Compiler: Visual Studio 2017
* 
* Author: zsh2401
* Company: I am free man
*
\* =============================================================================*/
using AutumnBox.Basic.Executer;
using System;

namespace AutumnBox.Basic.FlowFramework
{
    public class ToolKit<TArgs> where TArgs : FlowArgs
    {
        public readonly Func<string, OutputData> Ae;
        public readonly Func<string, OutputData> Fe;
        public readonly CExecuter Executer;
        public TArgs Args;
        public ToolKit(TArgs args, CExecuter executer = null)
        {
            Args = args;
            Executer = executer ?? new CExecuter();
            Ae = (command) =>
            { return Executer.Execute(Command.MakeForAdb(Args.DevBasicInfo.Serial, command)); };
            Fe = (command) =>
            { return Executer.Execute(Command.MakeForFastboot(Args.DevBasicInfo.Serial, command)); };
        }
    }
}