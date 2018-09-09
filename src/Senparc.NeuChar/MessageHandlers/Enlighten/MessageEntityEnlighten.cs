using Senparc.NeuChar.Enlighten;
using Senparc.NeuChar.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.MessageHandlers
{
    /// <summary>
    /// MessageHandler初始化请求和响应消息的定义类
    /// </summary>
    public class MessageEntityEnlighten : IEnlighten
    {

        /// <summary>
        /// 支持平台类型
        /// </summary>
        public PlatformType PlatformType { get; set; } //= NeuChar.PlatformType.General;

        #region 不同消息类型明确示例类型的委托

        #region 请求消息

        public Func<IRequestMessageText> NewRequestMessageText { get; set; } = () => null;
        public Func<IRequestMessageLocation> NewRequestMessageLocation { get; set; } = () => null;
        public Func<IRequestMessageImage> NewRequestMessageImage { get; set; } = () => null;
        public Func<IRequestMessageVoice> NewRequestMessageVoice { get; set; } = () => null;
        public Func<IRequestMessageVideo> NewRequestMessageVideo { get; set; } = () => null;
        public Func<IRequestMessageLink> NewRequestMessageLink { get; set; } = () => null;
        public Func<IRequestMessageShortVideo> NewRequestMessageShortVideo { get; set; } = () => null;
        public Func<IRequestMessageEvent> NewRequestMessageEvent { get; set; } = () => null;
        public Func<IRequestMessageFile> NewRequestMessageFile { get; set; } = () => null;



        #endregion

        #region 响应消息

        public Func<ResponseMessageNoResponse> NewResponseMessageNoResponse { get; set; } = () => new ResponseMessageNoResponse();

        public Func<IResponseMessageText> NewResponseMessageText { get; set; } = () => null;
        public Func<IResponseMessageNews> NewResponseMessageNews { get; set; } = () => null;
        public Func<IResponseMessageMusic> NewResponseMessageMusic { get; set; } = () => null;
        public Func<IResponseMessageImage> NewResponseMessageImage { get; set; } = () => null;
        public Func<IResponseMessageVoice> NewResponseMessageVoice { get; set; } = () => null;
        public Func<IResponseMessageVideo> NewResponseMessageVideo { get; set; } = () => null;
        public Func<IResponseMessageTransfer_Customer_Service> NewResponseMessageTransfer_Customer_Service { get; set; } = () => null;


        /// <summary>
        /// 素材多图文
        /// </summary>
        public Func<IResponseMessageMpNews> NewResponseMessageMpNews { get; set; } = () => null;


        /// <summary>
        /// 默认为 SuccessResponseMessage 类型，返回字符串 "success"
        /// </summary>
        public Func<SuccessResponseMessageBase> NewSuccessResponseMessage { get; set; } = () => new SuccessResponseMessage();

        #endregion

        #region 生成

        #endregion

        #endregion

        /// <summary>
        /// MessageEntityEnlighten 构造函数
        /// </summary>
        /// <param name="platformType"></param>
        public MessageEntityEnlighten(PlatformType platformType = PlatformType.General)
        {
            PlatformType = platformType;
        }
    }
}
