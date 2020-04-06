using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

public static class EncryptUtils
{
    /// <summary>
    /// 获取一个 MD5 字符串
    /// </summary>
    /// <param name="key">字符串类型</param>
    /// <returns></returns>
    public static string GetMD5String(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return null;
        }
        MD5 md5 = MD5.Create();
        byte[] bts = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < bts.Length; i++)
        {
            sb.Append(bts[i].ToString("X2"));
            if (i < bts.Length - 1)
            {
                sb.Append(":");
            }
        }
        return sb.ToString();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key">二进制流类型</param>
    /// <returns>MD5 字符串</returns>
    public static string GetMD5String(byte[] key)
    {
        if (key == null)
        {
            return null;
        }
        MD5 md5 = MD5.Create();
        byte[] bts = md5.ComputeHash(key);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < bts.Length; i++)
        {
            sb.Append(bts[i].ToString("X2"));
            if (i < bts.Length - 1)
            {
                sb.Append(":");
            }
        }
        return sb.ToString();
    }
    /// <summary>
    /// 获取RSA的 私有、公有的秘钥
    /// </summary>
    /// <param name="savePath">秘钥保存路径</param>
    /// <param name="keyName">密钥的名字</param>
    /// <returns></returns>
    public static bool CreateRSAKey(string savePath, string keyName)
    {
        try
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                RSAParameters publicParam = rsa.ExportParameters(false);
                XmlSerializer privateXml = new XmlSerializer(publicParam.GetType());
                string pathPublic = savePath.TrimEnd('/', '\\') + "/" + keyName + "_rsa.pub";
                using (FileStream fs = new FileStream(pathPublic, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    privateXml.Serialize(fs, publicParam);
                }

                RSAParameters privateParam = rsa.ExportParameters(true);
                XmlSerializer publicXml = new XmlSerializer(privateParam.GetType());
                string pathPrivate = savePath.TrimEnd('/', '\\') + "/" + keyName + "_rsa";
                using (FileStream fs = new FileStream(pathPrivate, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    publicXml.Serialize(fs, privateParam);
                }
            }
            return true;
        }
        catch (System.Exception error)
        {
            System.Console.WriteLine("CreateRSAPublicKey Error: " + error.Message);
            return false;
        }
    }
    /// <summary>
    /// 加密数据
    /// </summary>
    /// <param name="dataToEncrypt">要加密的信息</param>
    /// <param name="rsaKeyInfo">密钥</param>
    /// <param name="do0AEPadding">只可用在微软Window xp及以后的系统中</param>
    /// <returns></returns>
    public static byte[] EncryptRSA(byte[] dataToEncrypt, RSAParameters rsaKeyInfo, bool do0AEPadding = false)
    {
        try
        {
            byte[] encryptData = null;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // 导入RSA的密钥信息， 这里是公钥
                rsa.ImportParameters(rsaKeyInfo);
                //加密传入的byte数组，并指定OAEP padding
                //OAEP padding只可用在微软Window xp及以后的系统中
                encryptData = rsa.Encrypt(dataToEncrypt, do0AEPadding);
            }
            return encryptData;
        }
        catch (System.Exception error)
        {
            System.Console.WriteLine("EncryptRSA Error: " + error.Message);
            return null;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataToEncrypt"></param>
    /// <param name="rsaKeyFile"></param>
    /// <param name="do0AEPadding"></param>
    /// <returns></returns>
    public static byte[] EncryptRSA(byte[] dataToEncrypt, string rsaKeyFile, bool do0AEPadding = false)
    {
        try
        {
            if (!File.Exists(rsaKeyFile))
            {
                return null;
            }
            RSAParameters publicParam;
            using (FileStream fs = new FileStream(rsaKeyFile, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer publicXml = new XmlSerializer(typeof(RSAParameters));
                publicParam = (RSAParameters)publicXml.Deserialize(fs);
            }
            byte[] encryptData = null;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // 导入RSA的密钥信息， 这里是公钥
                rsa.ImportParameters(publicParam);
                //加密传入的byte数组，并指定OAEP padding
                //OAEP padding只可用在微软Window xp及以后的系统中
                encryptData = rsa.Encrypt(dataToEncrypt, do0AEPadding);
            }
            return encryptData;
        }
        catch (System.Exception error)
        {
            System.Console.WriteLine("EncryptRSA Error: " + error.Message);
            return null;
        }
    }
    /// <summary>
    /// 解密数据
    /// </summary>
    /// <param name="dataToDecrypt">要解密的数据</param>
    /// <param name="rsaKeyInfo">密钥信息  RSAParameters 类型</param>
    /// <param name="do0AEPadding"></param>
    /// <returns></returns>
    public static byte[] DecryptRSA(byte[] dataToDecrypt, RSAParameters rsaKeyInfo, bool do0AEPadding = false)
    {
        try
        {
            byte[] decryptData = null;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsaKeyInfo);
                decryptData = rsa.Decrypt(dataToDecrypt, do0AEPadding);
            }
            return decryptData;
        }
        catch (System.Exception error)
        {
            System.Console.WriteLine("DecryptRSA Error: " + error.Message);
            return null;
        }
    }
    /// <summary>
    /// 解密数据
    /// </summary>
    /// <param name="dataToDecrypt">要解密的数据</param>
    /// <param name="rsaKeyFile">密钥信息  RSAParameters 类型的密钥文件</param>
    /// <param name="do0AEPadding"></param>
    /// <returns></returns>
    public static byte[] DecryptRSA(byte[] dataToDecrypt, string rsaKeyFile, bool do0AEPadding = false)
    {
        try
        {
            if (!File.Exists(rsaKeyFile))
            {
                return null;
            }
            RSAParameters privateParam;
            using (FileStream fs = new FileStream(rsaKeyFile, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer publicXml = new XmlSerializer(typeof(RSAParameters));
                privateParam = (RSAParameters)publicXml.Deserialize(fs);
            }
            byte[] decryptData = null;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(privateParam);
                decryptData = rsa.Decrypt(dataToDecrypt, do0AEPadding);
            }
            return decryptData;
        }
        catch (System.Exception error)
        {
            System.Console.WriteLine("DecryptRSA Error: " + error.Message);
            return null;
        }
    }
}