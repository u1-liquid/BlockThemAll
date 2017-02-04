using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockThemAll
{
    public enum WorkStatus
    {
        READY,
        PREVIEW,
        STARTING,
        STARTED,
        PAUSED,
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
