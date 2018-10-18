using Senparc.NeuChar.Enlightener;
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
    public abstract class MessageEntityEnlightener : IEnlightener
    {

        /// <summary>
        /// 支持平台类型
        /// </summary>
        public PlatformType PlatformType { get; set; }//= NeuChar.PlatformType.General;

        #region 不同消息类型明确示例类型的委托

        #region 请求消息

        public abstract IRequestMessageText NewRequestMessageText();
        public abstract IRequestMessageLocation NewRequestMessageLocation();
        public abstract IRequestMessageImage NewRequestMessageImage();
        public abstract IRequestMessageVoice NewRequestMessageVoice();
        public abstract IRequestMessageVideo NewRequestMessageVideo();
        public abstract IRequestMessageLink NewRequestMessageLink();
        public abstract IRequestMessageShortVideo NewRequestMessageShortVideo();
        public abstract IRequestMessageEvent NewRequestMessageEvent();
        public abstract IRequestMessageFile NewRequestMessageFile();



        #endregion

        #region 响应消息

        public ResponseMessageNoResponse NewResponseMessageNoResponse()
        {
            return new ResponseMessageNoResponse();
        }

        public abstract IResponseMessageText NewResponseMessageText();
        public abstract IResponseMessageNews NewResponseMessageNews();
        public abstract IResponseMessageMusic NewResponseMessageMusic();
        public abstract IResponseMessageImage NewResponseMessageImage();
        public abstract IResponseMessageVoice NewResponseMessageVoice();
        public abstract IResponseMessageVideo NewResponseMessageVideo();
        public abstract IResponseMessageTransfer_Customer_Service NewResponseMessageTransfer_Customer_Service();


        /// <summary>
        /// 素材多图文
        /// </summary>
        public abstract IResponseMessageMpNews NewResponseMessageMpNews();


        /// <summary>
        /// 默认为 SuccessResponseMessage 类型，返回字符串 "success"
        /// </summary>
        public virtual SuccessResponseMessageBase SuccessResponseMessage()
        {
            return new SuccessResponseMessage();
        }


        #endregion

        #region 生成

        #endregion

        #endregion

        /// <summary>
        /// MessageEntityEnlighten 构造函数
        /// </summary>
        /// <param name="platformType"></param>
        public MessageEntityEnlightener(PlatformType platformType = PlatformType.General)
        {
            PlatformType = platformType;
        }
    }
}
