<img src="https://sdk.weixin.senparc.com/images/senparc-logo-500.jpg" />

# NeuChar
Senparc.NeuChar 跨平台信息交互标准。使用 NeuChar 标准可以跨平台兼容不同平台的交互信息设置，一次设置，多平台共享。

## 如何使用 Nuget 安装？

* NeuChar Nuget 地址：https://www.nuget.org/packages/Senparc.NeuChar
* 命令：
```
PM> Install-Package Senparc.NeuChar
```

## RequestMsgType 支持（存在）情况

| 枚举类型        |   微信<br>公众号   |    钉钉    |    QQ公众号  |  头条第三方   |  Facebook  |   
|----------------|----------|------------|-------------|--------------|------------|
|  Text          |    Y     |      Y     |       Y     |       -      |      -     |
|  Location      |    Y     |      N     |       Y     |       -      |      -     |
|  Image         |    Y     |      Y     |       -     |       -      |      -     |
|  Voice         |    Y     |      Y     |       N     |       -      |      -     |
|  Video         |    Y     |      N     |       N     |       -      |      -     |
|  Link          |    Y     |      Y     |       N     |       -      |      -     |
|  ShortVideo    |    Y     |      N     |       N     |       -      |      -     |
|  Event         |    Y     |      N     |       Y     |       -      |      -     |
|  File          |    Y     |      Y     |       N     |       -      |      -     |
|  OA            |    N     |      Y     |       N     |       -      |      -     |

Y：支持，N：不支持，-：待确定（较为可能不支持）

> 微信消息文档：https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1421140453<br>
> 钉钉消息文档：https://open-doc.dingtalk.com/microapp/serverapi2/al5qyp<br>
