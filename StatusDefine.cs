using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockThemAll
{
    public enum ApplicationStatus
    {
        Ready,
        Precessing
    }

    public enum WorkStatus
    {
        READY,
        STARTING,
        STARTED,
        PAUSED,
        CONTINUING,
        STOPPING,
        STOPPED,
        DONE,
        ERROR
    }

    public enum UserStatus
    {
        NO_APIKEY,
        NO_CREDITIONAL,
        LOGIN_REQUESTED,
        LOGIN_SUCCESS,
        INVALID_CREDITIONAL
    }
}
