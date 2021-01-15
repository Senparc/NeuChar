﻿#region Apache License Version 2.0
/*----------------------------------------------------------------

Copyright 2021 Suzhou Senparc Network Technology Co.,Ltd.

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.

Detail: https://github.com/JeffreySu/WeiXinMPSDK/blob/master/license.md

----------------------------------------------------------------*/
#endregion Apache License Version 2.0

/*----------------------------------------------------------------
    Copyright (C) 2021 Senparc
    
    文件名：NeuCharException.cs
    文件功能描述：NeuChar异常处理
    
    
    创建标识：Senparc - 20180916
    
----------------------------------------------------------------*/


using Senparc.CO2NET.Exceptions;
using System;

namespace Senparc.NeuChar.Exceptions
{
    /// <summary>
    /// MessageHandler异常
    /// </summary>
    public class NeuCharException : BaseException
    {
          public NeuCharException(string message)
            : this(message, null)
        {
        }

          public NeuCharException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
