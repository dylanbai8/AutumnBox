﻿/********************************************************************************
** auth： zsh2401@163.com
** date： 2017/12/28 1:34:05
** filename: NetDeviceAdder.cs
** compiler: Visual Studio 2017
** desc： ...
*********************************************************************************/
using AutumnBox.Basic.Executer;
using AutumnBox.Basic.FlowFramework;
using AutumnBox.Basic.Flows.Result;
using AutumnBox.Basic.Util;
using System.Net;

namespace AutumnBox.Basic.Flows
{
    public class NetDeviceAdderArgs : FlowArgs
    {
        public IPEndPoint IPEndPoint { get; set; }
    }
    public class NetDeviceAdder : FunctionFlow<NetDeviceAdderArgs, AdvanceResult>
    {
        CommandExecuterResult _result;
        protected override OutputData MainMethod(ToolKit<NetDeviceAdderArgs> toolKit)
        {
            _result = toolKit.Executer.Execute(Command.MakeForAdb($"connect {toolKit.Args.IPEndPoint.ToString()}"));
            return _result.Output;
        }
        protected override void AnalyzeResult(AdvanceResult result)
        {
            base.AnalyzeResult(result);
            result.ExitCode = _result.ExitCode;
            result.ResultType = _result.IsSuccessful ? ResultType.Successful : ResultType.Unsuccessful;
        }
    }
}