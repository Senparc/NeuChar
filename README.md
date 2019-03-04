<img src="https://sdk.weixin.senparc.com/images/senparc-logo-500.jpg" />

# NeuChar

Senparc.NeuChar 跨平台信息交互标准。使用 NeuChar 标准可以跨平台兼容不同平台的交互信息设置，一次设置，多平台共享。

[![Build status](https://mysenparc.visualstudio.com/Senparc%20SDK/_apis/build/status/NeuChar/Senparc.NeuCHar-%20CI)](https://mysenparc.visualstudio.com/Senparc%20SDK/_build/latest?definitionId=14)
[![Build status](https://ci.appveyor.com/api/projects/status/pwat2w0c5cykesi5/branch/master?svg=true)](https://ci.appveyor.com/project/JeffreySu/neuchar/branch/master)
[![NuGet](https://img.shields.io/nuget/dt/Senparc.NeuChar.svg)](https://www.nuget.org/packages/Senparc.NeuChar)
[![license](https://img.shields.io/github/license/Senparc/NeuChar.svg)](http://www.apache.org/licenses/LICENSE-2.0)


| 名称    |        DLL          |  Nuget                                                                                | 支持 .NET 版本 
|---------|---------------------|---------------------------------------------------------------------------------------|--------------------------------------
| NeuChar | Senparc.NeuChar.dll   | [![Senparc.NeuChar][1.1]][1.2]    [![Senparc.NeuChar][nuget-img-base]][nuget-url-base]  |  ![.NET 3.5][net35Y]    ![.NET 4.0][net40Y]   ![.NET 4.5][net45Y]    ![.NET Core 2.0][core20Y]
| NeuChar.App | Senparc.NeuChar.App.dll   | [![Senparc.NeuChar.App][2.1]][2.2]    [![Senparc.NeuChar.App][nuget-img-base-app]][nuget-url-base-app]  |  ![.NET 3.5][net35Y]    ![.NET 4.0][net40Y]   ![.NET 4.5][net45Y]    ![.NET Core 2.0][core20Y]


[1.1]: https://img.shields.io/nuget/v/Senparc.NeuChar.svg?style=flat
[1.2]: https://www.nuget.org/packages/Senparc.NeuChar
[2.1]: https://img.shields.io/nuget/v/Senparc.NeuChar.App.svg?style=flat
[2.2]: https://www.nuget.org/packages/Senparc.NeuChar.App

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

[nuget-img-base-app]: https://img.shields.io/nuget/dt/Senparc.NeuChar.App.svg
[nuget-url-base-app]: https://www.nuget.org/packages/Senparc.NeuChar.App

## 线上 SaaS 集成服务

官网：<a href="https://www.neuchar.com" target="_blank">https://www.neuchar.com</a>

> https://www.neuchar.com 提供了一整套基于 Senparc.NeuChar 标准的服务平台，同时服务于开发者（提供 App）以及运营者（订阅 App）。<br>
> 开发者提供的一个 App，即可同时被多个平台的运营人员使用；运营人员维护一次信息，即可同时同步到多个平台。



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


## QQ 技术交流群

<img src="https://sdk.weixin.senparc.com/images/QQ_Group_Avatar/NeuChar/QQ-Group.jpg" width="380" />

