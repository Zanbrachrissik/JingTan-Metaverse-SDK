// This file is auto-generated, don't edit it. Thanks.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Tea;
using Tea.Utils;


namespace AntChain.SDK.Sample
{
    public class Client 
    {

        /**
         * 使用AK&SK初始化账号Client
         * @param accessKeyId
         * @param accessKeySecret
         * @return Client
         * @throws Exception
         */
        public static AntChain.SDK.NFTC.Client CreateClient(string accessKeyId, string accessKeySecret)
        {
            AntChain.SDK.NFTC.Models.Config config = new AntChain.SDK.NFTC.Models.Config();
            // 您的AccessKey ID
            config.AccessKeyId = accessKeyId;
            // 您的AccessKey Secret
            config.AccessKeySecret = accessKeySecret;
            return new AntChain.SDK.NFTC.Client(config);
        }

        public static void Main(string[] args)
        {
            AntChain.SDK.NFTC.Client client = CreateClient("accessKeyId", "accessKeySecret");
            AntChain.SDK.NFTC.Models.CreateResourceGeneralresourceRequest createResourceGeneralresourceRequest = new AntChain.SDK.NFTC.Models.CreateResourceGeneralresourceRequest();
            client.CreateResourceGeneralresource(createResourceGeneralresourceRequest);
        }


    }
}