using UnityEngine;
using UnityEditor;

using System.Threading.Tasks;
using AntChain.SDK.NFTC.Models;
using UnityEngine.Networking;
using System.IO;

public class JTAppInfo
{
    public int bizStatusCode;
    public string bizStatusMessage;
    public string appId;
    public string url;
    public string maasToken;
}

public class UploadReturnInfo
{
    public int code { get; set; }
    public Data data { get; set; }
}

public class Data
{
    public string id { get; set; }
}

public class JTBindFile
{
    public string ReqMsgId
    {
        get;
        set;
    }

    public string ResultCode
    {
        get;
        set;
    }


    public string ResultMsg
    {
        get;
        set;
    }


    public long? Version
    {
        get;
        set;
    }


    public string Url
    {
        get;
        set;
    }

    public string Md5
    {
        get;
        set;
    }

    public long? Size
    {
        get;
        set;
    }
}

public class JingTanNFTCUpload
{
    //生产
    //private const string endPoint = "openapi.antchain.antgroup.com";
    //private const string product_instance_id = "nftc-resource-api-prod";

    //预发
    //private const string endPoint = "openapi-pre.antchain.antgroup.com";
    //private const string product_instance_id = "nftc-resource-api-prepub";

    //private const string protocol = "HTTPS";

    //Dev
    private const string endPoint = "openapi-dev.antchain.dl.alipaydev.com";
    private const string product_instance_id = "nftc-resource-api-dev";

    //Dev
    private const string protocol = "HTTP";
    private const string tokenType = "UGC_AFTS_TOKEN";

    public static JTAppInfo jtAppInfo;
    public static AntChain.SDK.NFTC.Client client;
    public static string showURL;
    static float progress = 0;

    public static AntChain.SDK.NFTC.Client CreateClient(string accessKeyId, string accessKeySecret)
    {
        Debug.Log("创建鲸探Client");
        Config config = new Config();
        // 您的AccessKey ID
        config.AccessKeyId = accessKeyId;
        // 您的AccessKey Secret
        config.AccessKeySecret = accessKeySecret;

        config.Endpoint = endPoint;
        config.Protocol = protocol;
        return new AntChain.SDK.NFTC.Client(config);
    }

    /// <summary>
    /// 获取一个资源文件上传地址
    /// </summary>
    public static JTAppInfo GetAppMsg(string ak, string sk)
    {
        if (client == null)
        {
            client = CreateClient(ak, sk);
        }

        ApplyResourceFiletokenRequest applyResourceFiletokenRequest = new ApplyResourceFiletokenRequest();
        applyResourceFiletokenRequest.TokenType = tokenType;

        var re = client.ApplyResourceFiletoken(applyResourceFiletokenRequest);

        Debug.Log($"return code: {re.ResultCode} \n returnMsg: {re.ResultMsg} \n id: {re.AppId}  \n url: {re.Url} \n MassToken: {re.MassToken}");

        if (re.ResultMsg != "成功")
        {
            Debug.LogError("鲸探地址获取失败，请重试！！！" + re.ResultMsg + "-错误码：" + re.ResultCode + "-reMegID: " + re.ReqMsgId);
            return null;
        }
        else
        {
            jtAppInfo = new JTAppInfo();
            jtAppInfo.bizStatusCode = int.Parse(re.ResultCode);
            jtAppInfo.bizStatusMessage = re.ResultMsg;

            jtAppInfo.url = re.Url;
            jtAppInfo.appId = "appId";
            jtAppInfo.maasToken = re.MassToken;

            return jtAppInfo;
        }
    }

    /// <summary>
    /// ZIP上传鲸探服务器
    /// </summary>
    public static async Task<bool> UploadToJT(string zipPath, string fileURL, string maasToken, string appID, string resID, string version)
    {
        Debug.Log("ZIP包上传鲸探服务器: " + zipPath);
        try
        {
            if (File.Exists(zipPath))
            {
                byte[] fileData = File.ReadAllBytes(zipPath);
                string zipLength = fileData.Length.ToString();
                string md5 = PrefsTool.GenerateMD5FromFile(zipPath);

                WWWForm form = new WWWForm();
                form.AddBinaryData("file", File.ReadAllBytes(zipPath), "test.zip", "application/zip"); //TODO 修改名字

                using (UnityWebRequest request = UnityWebRequest.Post(fileURL, form))
                {

                    EditorUtility.DisplayProgressBar("上传", "上传文件中。。。", progress);

                    request.SetRequestHeader("x-mass-token", maasToken);
                    request.SetRequestHeader("x-mass-biztype", "chain_myentUGC");
                    request.SetRequestHeader("x-mass-public", "true");
                    request.SetRequestHeader("x-mass-ext-name", ".zip");
                    request.SetRequestHeader("x-file-length", zipLength);
                    request.SetRequestHeader("x-mass-file-md5", md5);
                    var requestWWW = request.SendWebRequest();

                    while (!request.isDone)
                    {
                        await Task.Delay(100);
                        progress++;
                        Debug.Log("waiting....");
                    }

                    EditorUtility.ClearProgressBar();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        Debug.Log("上传完成! Server Response: " + request.downloadHandler.text);

                        string fieldID = PrefsTool.JsonToObj<UploadReturnInfo>(request.downloadHandler.text).data.id;

                        BindJTFile(fieldID, resID, appID, version);
                    }
                    else
                    {
                        Debug.Log(request.error);
                    }

                    EditorUtility.ClearProgressBar();
                }
            }
            else
            {
                Debug.Log("Zip File 不存在");
            }

        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("出现错误: " + e.Message);
        }
        return true;
    }

    /// <summary>
    /// 绑定鲸探资源文件
    /// 在使用上传地址完成文件上传后，将获取到的文件ID提交到资源管理平台，获取对应的文件全量包下载地址。
    /// 服务端在接受提交信息后，会确认文件是否已经完成上传，如果上传完成，会将文件的MD5摘要值和文件大小返回以便客户端确认文件内容。
    /// 文件上传完成后，会返回一个编辑状态的版本号。
    /// </summary>
    /// <returns></returns>
    public static void BindJTFile(string fieldID, string resID, string appID, string version)
    {
        Debug.Log("绑定鲸探资源文件");
        BindResourceGeneralresourcefileRequest bindResourceGeneralresourcefileRequest = new BindResourceGeneralresourcefileRequest();
        bindResourceGeneralresourcefileRequest.ProductInstanceId = product_instance_id;

        bindResourceGeneralresourcefileRequest.AppId = appID;
        bindResourceGeneralresourcefileRequest.ResourceId = resID;
        bindResourceGeneralresourcefileRequest.FileId = fieldID;
        bindResourceGeneralresourcefileRequest.Status = "PUBLISHING";
        bindResourceGeneralresourcefileRequest.BizVersion = version;
        //bindResourceGeneralresourcefileRequest.bizVersion = version;

        var request = client.BindResourceGeneralresourcefile(bindResourceGeneralresourcefileRequest);

        JTBindFile jTBindFile = new JTBindFile();
        jTBindFile.ResultCode = request.ResultCode;
        jTBindFile.ResultMsg = request.ResultMsg;
        jTBindFile.Version = request.Version;
        jTBindFile.Url = request.Url;
        jTBindFile.Md5 = request.Md5;
        jTBindFile.Size = request.Size;

        if (jTBindFile.ResultMsg != "成功")
        {
            Debug.LogError("绑定文件失败！！！请重试！！！" + request.ReqMsgId + request.ResultMsg);
            EditorUtility.DisplayDialog("提示", "上传失败", "好的");
        }
        else
        {
            Debug.Log(PrefsTool.ObjToJson<JTBindFile>(jTBindFile));
            EditorUtility.DisplayDialog("提示", "上传成功", "好的");
        }
    }
}
