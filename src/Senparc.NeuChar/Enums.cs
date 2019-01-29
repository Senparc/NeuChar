#region Apache License Version 2.0
/*----------------------------------------------------------------

Copyright 2018 Suzhou Senparc Network Technology Co.,Ltd.

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
    Copyright (C) 2018 Senparc
    
    文件名：Enums.cs
    文件功能描述：定义枚举文件
    
    
    创建标识：Senparc - 20180901
    
    修改标识：Senparc - 20190105
    修改描述：v0.6.0 添加 PushNeuCharAppConfig 和 PullNeuCharAppConfig 消息类型

----------------------------------------------------------------*/



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar
{
    /// <summary>
    /// 数据类型
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// 列表
        /// </summary>
        List,
        /// <summary>
        /// 单项（唯一键）
        /// </summary>
        Unique
    }

    /// <summary>
    /// API 类型
    /// </summary>
    public enum ApiType
    {
        /// <summary>
        /// 用于获取 AccessToken 凭证
        /// </summary>
        AccessToken,
        /// <summary>
        /// 普通接口
        /// </summary>
        Normal
    }

    /// <summary>
    /// NeuChar 消息的乐行
    /// </summary>
    public enum NeuCharActionType
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        GetConfig = 0,
        /// <summary>
        /// 储存配置
        /// </summary>
        SaveConfig = 1,
        /// <summary>
        /// 检查NeuChar服务是否可用，同时拉取 APM 统计数据
        /// </summary>
        CheckNeuChar = 2,

        /// <summary>
        /// 推送 NeuChar App 的设置
        /// </summary>
        PushNeuCharAppConfig = 3,
        /// <summary>
        /// 拉取 NeuChar App 的设置
        /// </summary>
        PullNeuCharAppConfig = 4
    }

    /// <summary>
    /// AppStore状态
    /// </summary>
    public enum AppStoreState
    {
        /// <summary>
        /// 无状态
        /// </summary>
        None = 1,
        /// <summary>
        /// 已进入应用状态
        /// </summary>
        Enter = 2,
        /// <summary>
        /// 退出App状态（临时传输状态，退出后即为None）
        /// </summary>
        Exit = 4
    }


    /// <summary>
    /// 消息请求类型
    /// </summary>
    public enum RequestMsgType
    {
        Unknown = -1,//未知类型
        Text = 0, //文本
        Location = 1, //地理位置
        Image = 2, //图片
        Voice = 3, //语音
        Video = 4, //视频
        Link = 5, //连接信息
        ShortVideo = 6,//小视频
        Event = 7, //事件推送
        File = 8,//文件类型


        NeuChar = 999,//NeuChar请求
    }

    /// <summary>
    /// 消息响应类型
    /// </summary>
    public enum ResponseMsgType
    {
        [Description("其他")]
        Other = -2,
        [Description("未知")]
        Unknown = -1,//未知类型
        [Description("文本")]
        Text = 0,
        [Description("单图文")]
        News = 1,
        [Description("音乐")]
        Music = 2,
        [Description("图片")]
        Image = 3,
        [Description("语音")]
        Voice = 4,
        [Description("视频")]
        Video = 5,
        [Description("多客服")]
        Transfer_Customer_Service = 6,
        //transfer_customer_service
        [Description("素材多图文")]
        MpNews = 7,//素材多图文



        //以下为延伸类型，微信官方并未提供具体的回复类型
        [Description("多图文")]
        MultipleNews = 106,
        [Description("位置")]
        LocationMessage = 107,//
        [Description("无回复")]
        NoResponse = 110,
        [Description("success")]
        SuccessResponse = 200,


        [Description("使用API回复")]
        UseApi = 998,//使用接口访问
    }

    /// <summary>
    /// 平台类型
    /// </summary>
    public enum PlatformType
    {
        /// <summary>
        /// 通用
        /// </summary>
        General = 0,
        /// <summary>
        /// 微信公众号
        /// </summary>
        WeChat_OfficialAccount = 1,
        /// <summary>
        /// 微信小程序
        /// </summary>
        WeChat_MiniProgram = 2,
        /// <summary>
        /// 微信企业号
        /// </summary>
        WeChat_Work = 4,
        /// <summary>
        /// 微信开放平台
        /// </summary>
        WeChat_Open = 8,

        //空余：16
        //空余：32
        //空余：64

        /// <summary>
        /// QQ公众号
        /// </summary>
        QQ_OfficialAccount = 128,

        //空余：256
        //空余：512
        //空余：1024

        /// <summary>
        /// 钉钉
        /// </summary>
        DingDing = 2048,

        //空余：4096
        //空余：8192
        //空余：16384

        //待定：32768
        //待定：65536
        //待定：131072
        //待定：262144
        //待定：524288
        //待定：1048576

    }

    /// <summary>
    /// 菜单按钮类型
    /// </summary>
    public enum MenuButtonType
    {
        /// <summary>
        /// 点击
        /// </summary>
        click = 101,
        /// <summary>
        /// Url
        /// </summary>
        view = 102,
        /// <summary>
        /// 小程序
        /// </summary>
        miniprogram = 103,
        /// <summary>
        /// 扫码推事件
        /// </summary>
        scancode_push = 104,
        /// <summary>
        /// 扫码推事件且弹出“消息接收中”提示框
        /// </summary>
        scancode_waitmsg = 105,
        /// <summary>
        /// 弹出系统拍照发图
        /// </summary>
        pic_sysphoto = 106,
        /// <summary>
        /// 弹出拍照或者相册发图
        /// </summary>
        pic_photo_or_album = 107,
        /// <summary>
        /// 弹出微信相册发图器
        /// </summary>
        pic_weixin = 108,
        /// <summary>
        /// 弹出地理位置选择器
        /// </summary>
        location_select = 109,
        /// <summary>
        /// 下发消息（除文本消息）
        /// </summary>
        media_id = 110,
        /// <summary>
        /// 跳转图文消息URL
        /// </summary>
        view_limited = 111
    }

    public enum NeuCharApmKind
    {
        /// <summary>
        /// 消息请求
        /// </summary>
        Message_Request,//TODO: 可进一步统计不同的消息类型
        /// <summary>
        /// 成功返回
        /// </summary>
        Message_SuccessResponse,
        /// <summary>
        /// 响应时间（毫秒）
        /// </summary>
        Message_ResponseMillisecond,
        /// <summary>
        /// 消息处理异常
        /// </summary>
        Message_Exception

    }
}
