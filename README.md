<img src="https://sdk.weixin.senparc.com/images/senparc-logo-500.jpg" />

# NeuChar
Senparc.NeuChar 跨平台信息交互标准。使用 NeuChar 标准可以跨平台兼容不同平台的交互信息设置，一次设置，多平台共享。

[![Build status](https://ci.appveyor.com/api/projects/status/pwat2w0c5cykesi5/branch/master?svg=true)](https://ci.appveyor.com/project/JeffreySu/neuchar/branch/master)
[![NuGet](https://img.shields.io/nuget/dt/Senparc.NeuChar.svg)](https://www.nuget.org/packages/Senparc.NeuChar)

| 名称    |        DLL          |  Nuget                                                                                | 支持 .NET 版本 
|---------|---------------------|---------------------------------------------------------------------------------------|--------------------------------------
| NeuChar | Senparc.NeuChar.dll   | [![Senparc.NeuChar][1.1]][1.2]    [![Senparc.NeuChar][nuget-img-base]][nuget-url-base]  |  ![.NET 3.5][net35Y]    ![.NET 4.0][net40Y]   ![.NET 4.5][net45Y]    ![.NET Core 2.0][core20Y]


[1.1]: https://img.shields.io/nuget/v/Senparc.CO2NET.svg?style=flat
[1.2]: https://www.nuget.org/packages/Senparc.CO2NET

[net35Y]: https://img.shields.io/badge/3.5-Y-brightgreen.svg
[net35N]: https://img.shields.io/badge/3.5-N-lightgrey.svg
[net40Y]: https://img.shields.io/badge/4.0-Y-brightgreen.svg
[net40N]: https://img.shields.io/badge/4.0-N-lightgrey.svg
[net40N-]: https://img.shields.io/badge/4.0----lightgrey.svg
[net45Y]: https://img.shields.io/badge/4.5-Y-brightgreen.svg
[net45N]: https://img.shields.io/badge/4.5-N-lightgrey.svg
[net45N-]: https://img.shields.io/badge/4.5----lightgrey.svg
[net461Y]: https://img.shields.io/badge/4.6.1-Y-brightgreen.svg
[net461N]: https://img.shields.io/badge/4.6.1-N-lightgrey.svg
[coreY]: https://img.shields.io/badge/core-Y-brightgreen.svg
[coreN]: https://img.shields.io/badge/core-N-lightgrey.svg
[coreN-]: https://img.shields.io/badge/core----lightgrey.svg
[core20Y]: https://img.shields.io/badge/core2.x-Y-brightgreen.svg
[core20N]: https://img.shields.io/badge/core2.x-N-lightgrey.svg

[nuget-img-base]: https://img.shields.io/nuget/dt/Senparc.NeuChar.svg
[nuget-url-base]: https://www.nuget.org/packages/Senparc.NeuChar

## 如何使用 Nuget 安装？

* NeuChar Nuget 地址：https://www.nuget.org/packages/Senparc.NeuChar
* 命令：
```
PM> Install-Package Senparc.NeuChar
```

## RequestMsgType 支持（存在）情况

| 枚举类型        |   微信<br>公众号   |    钉钉    |    QQ公众号  |  头条第三方   |  Facebook  |   
|----------------|-------------------|------------|-------------|--------------|------------|
|  Text          |    Y              |      Y     |       Y     |       -      |      Y     |
|  Location      |    Y              |      N     |       Y     |       -      |      -     |
|  Image         |    Y              |      Y     |       -     |       -      |      Y     |
|  Voice         |    Y              |      Y     |       N     |       -      |      -     |
|  Video         |    Y              |      N     |       N     |       -      |      -     |
|  Link          |    Y              |      Y     |       N     |       -      |      -     |
|  ShortVideo    |    Y              |      N     |       N     |       -      |      -     |
|  Event         |    Y              |      N     |       Y     |       -      |      -     |
|  File          |    Y              |      Y     |       N     |       -      |      -     |
|  OA            |    N              |      Y     |       N     |       -      |      -     |

Y：支持，N：不支持，-：待确定（较为可能不支持）

> 微信消息文档：https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1421140453<br>
> 钉钉消息文档：https://open-doc.dingtalk.com/microapp/serverapi2/al5qyp<br>
