using System;
using System.Collections.Generic;
using System.Text;

namespace PhotonMMO.Common
{
    public enum ErrorCode : Byte
    {
        NoError,
        Login,
        UserExisting,
        WrongPassword,
        WrongLogin
    }
}
