using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Exceptions;
using Senparc.NeuChar.MessageHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.App.MessageHandlers
{
    /// <summary>
    /// NeuChar-App MessageEntityEnlightener
    /// </summary>
    public class NeuCharAppMessageEntityEnlightener : MessageEntityEnlightener
    {
        //说明：此处不需要强制使用单例模式

        /// <summary>
        /// MpMessageEntityEnlightener 全局对象
        /// </summary>
        public static NeuCharAppMessageEntityEnlightener Instance = new NeuCharAppMessageEntityEnlightener(NeuChar.PlatformType.General);

        NeuCharAppMessageEntityEnlightener(NeuChar.PlatformType platformType) : base(platformType) { }

        public override IRequestMessageEvent NewRequestMessageEvent()
        {
            throw new MessageHandlerException("NeuChar-App SDK 不支持 IRequestMessageEvent 响应类型");
        }

        public override IRequestMessageFile NewRequestMessageFile()
        {
            throw new MessageHandlerException("NeuChar-App SDK 不支持 IRequestMessageFile 响应类型");
        }

        public override IRequestMessageImage NewRequestMessageImage()
        {
            throw new MessageHandlerException("NeuChar-App SDK 不支持 IRequestMessageImage 响应类型");
        }

        public override IRequestMessageLink NewRequestMessageLink()
        {
            throw new MessageHandlerException("NeuChar-App SDK 不支持 IRequestMessageLink 响应类型");
        }

        public override IRequestMessageLocation NewRequestMessageLocation()
        {
            throw new MessageHandlerException("NeuChar-App SDK 不支持 IRequestMessageLocation 响应类型");
        }

        public override IRequestMessageShortVideo NewRequestMessageShortVideo()
        {
            throw new MessageHandlerException("NeuChar-App SDK 不支持 IRequestMessageShortVideo 响应类型");
        }

        public override IRequestMessageText NewRequestMessageText()
        {
            throw new MessageHandlerException("NeuChar-App SDK 不支持 IRequestMessageText 响应类型");
        }

        public override IRequestMessageVideo NewRequestMessageVideo()
        {
            throw new MessageHandlerException("NeuChar-App SDK 不支持 IRequestMessageVideo 响应类型");
        }

        public override IRequestMessageVoice NewRequestMessageVoice()
        {
            throw new MessageHandlerException("NeuChar-App SDK 不支持 IRequestMessageVoice 响应类型");
        }

        public override IResponseMessageImage NewResponseMessageImage()
        {
            throw new MessageHandlerException("NeuChar-App SDK 不支持 IResponseMessageImage 响应类型");
        }

        public override IResponseMessageMpNews NewResponseMessageMpNews()
        {
            throw new MessageHandlerException("NeuChar-App SDK 不支持 IResponseMessageMpNews 响应类型");
        }

        public override IResponseMessageMusic NewResponseMessageMusic()
        {
            throw new MessageHandlerException("NeuChar-App SDK 不支持 IResponseMessageMusic 响应类型");
        }

        public override IResponseMessageNews NewResponseMessageNews()
        {
            throw new MessageHandlerException("NeuChar-App SDK 不支持 IResponseMessageNews 响应类型");
        }

        public override IResponseMessageText NewResponseMessageText()
        {
            throw new MessageHandlerException("NeuChar-App SDK 不支持 IResponseMessageText 响应类型");
        }

        public override IResponseMessageTransfer_Customer_Service NewResponseMessageTransfer_Customer_Service()
        {
            throw new MessageHandlerException("NeuChar-App SDK 不支持 IResponseMessageTransfer_Customer_Service 响应类型");
        }

        public override IResponseMessageVideo NewResponseMessageVideo()
        {
            throw new MessageHandlerException("NeuChar-App SDK 不支持 IResponseMessageVideo 响应类型");
        }

        public override IResponseMessageVoice NewResponseMessageVoice()
        {
            throw new MessageHandlerException("NeuChar-App SDK 不支持 IResponseMessageVoice 响应类型");
        }
    }
}
