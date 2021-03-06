﻿using AutumnBox.Basic.Device;
using AutumnBox.Basic.Device.ManagementV2;
using AutumnBox.OpenFramework.Extension;
using AutumnBox.OpenFramework.LeafExtension;
using AutumnBox.OpenFramework.LeafExtension.Attributes;
using AutumnBox.OpenFramework.LeafExtension.Fast;
using AutumnBox.OpenFramework.LeafExtension.Kit;

namespace AutumnBox.CoreModules.Extensions.Poweron
{
    [ExtName("StopApp MaxWell Mode", "zh-cn:激活小黑屋麦克斯韦妖模式")]
    [ExtAuth("zsh2401")]
    [ExtAppProperty(PKG_NAME)]
    [ExtPriority(ExtPriority.HIGH - 1)]
    [ExtIcon("Icons.stopapp.png")]
    [ExtRequiredDeviceStates(DeviceState.Poweron)]
    class EStopAppMaxWellActivator : LeafExtensionBase
    {
        private const string PKG_NAME = "web1n.stopapp";
        [LMain]
        private void Main(ILeafUI ui, IDevice device)
        {
            //            # Script to start "web1n's demon" on the device, which has a very rudimentary
            //# shell.
            //#
            //base=/system

            //path=`pm path web1n.stopapp`
            //path=${path:8}

            //export CLASSPATH=$path
            //exec app_process $base/bin com.web1n.stopapp.app_process.DemonStart "$@"
            using (ui)
            {
                var info = this.GetInformations();
                ui.Icon = info.Icon;
                ui.Title = info.Name;
                ui.Show();
                using (var shell = new AndroidShell(device))
                {
                    shell.To(e => ui.WriteOutput(e.Text));
                    shell.Open();
                    shell.WriteLine("base=/system");
                    shell.WriteLine("path=`pm path web1n.stopapp`");
                    shell.WriteLine("path=${path:8}");
                    shell.WriteLine("export CLASSPATH=$path");
                    shell.WriteLine("exec app_process $base/bin com.web1n.stopapp.app_process.DemonStart \"$@\"");
                    shell.WriteLine("exit $?");
                    shell.WaitForExit();
                    ui.WriteOutput("exit code:" + shell.ExitCode);
                    ui.Finish(shell.ExitCode == 0 ? AutumnBoxExtension.OK : AutumnBoxExtension.ERR);
                }
            }
        }
    }
}
