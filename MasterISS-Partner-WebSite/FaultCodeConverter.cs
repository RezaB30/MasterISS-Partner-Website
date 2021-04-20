using MasterISS_Partner_WebSite_Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite
{
    public static class FaultCodeConverter
    {
        public static TaskStatusEnum GetFaultCodeTaskStatus(FaultCodeEnum faultCode)
        {
            switch (faultCode)
            {
                case FaultCodeEnum.RendezvousMade:
                case FaultCodeEnum.WaitingForNewRendezvous:
                case FaultCodeEnum.NoFault:
                    return TaskStatusEnum.InProgress;
                case FaultCodeEnum.InvalidAddress:
                case FaultCodeEnum.TelekomLineFault:
                case FaultCodeEnum.ClientCancelled:
                case FaultCodeEnum.ModemFault:
                case FaultCodeEnum.CustomerCouldNotBeReached:
                case FaultCodeEnum.TelekomGeneralFault:
                case FaultCodeEnum.BuildingInstallationFault:
                    return TaskStatusEnum.Halted;
                case FaultCodeEnum.SetupComplete:
                    return TaskStatusEnum.Completed;
                case FaultCodeEnum.CustomerDidTheSetup:
                    return TaskStatusEnum.Cancelled;
                default:
                    return TaskStatusEnum.New;
            }
        }
    }
}