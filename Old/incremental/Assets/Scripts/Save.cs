using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;
using TMPro;

public class SimpleAES
{
    private const string initVector = "14rukl893fhjl9r5";
    private const int keysize = 256;

    public string EncryptString(string plainText, string passPhrase)
    {
        byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
        byte[] plaintTextBytes = Encoding.UTF8.GetBytes(plainText);
        PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
        byte[] keyBytes = password.GetBytes(keysize / 8);
        RijndaelManaged symmetricKey = new RijndaelManaged();
        symmetricKey.Mode = CipherMode.CBC();
        ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
        MemoryStream memoryStream = new MemoryStream();
        CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        cryptoStream.Write(plaintTextBytes, 0, plaintTextBytes.Length);
        cryptoStream.FlushFinalBlock();
        byte[] cipherTextBytes = memoryStream.ToArray();
        memoryStream.Close();
        cryptoStream.Close();
        return Convert.ToBase64String(cipherTextBytes);
    }
    public string DecryptString(string cipherText, string passPhrase)
    {
        byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
        byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
        PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
        byte[] keyBytes = password.GetBytes(keysize / 8);
        RijndaelManaged symmetricKey = new RijndaelManaged();
        symmetricKey.Mode = CipherMode.CBC;
        ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
        MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
        CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        byte[] plainTextBytes = new byte[cipherTextBytes.Length];
        int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
        memoryStream.Close();
        cryptoStream.Close();
        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
    }
}

public class Save : MonoBehaviour
{
    public TMP_InputField importValue;
    public TMP_InputField exportValue;

    public InputField encryptValue;
    public InputField decryptValue;

    public static string json = "";

    protected static string encryptKey = "3xhhr44dj1sc5g6h6f0pvq4g";
    protected static string savePath = "/playerdataV1.save";
    protected static string savePathBackup = "/playerdataV1Backup.save";

    public static int backupCount = 0;

    public static void SavePlayer(Data data)
    {
        var saveTo = backupCount == 4 ? savePathBackup : savePath;
        using (StringWriter writer = new StringWriter(Application.persistantDataPath + saveTo))
        {
            json = JsonUtility.ToJson(data);
            ConvertStringToBase64(writer, json);
            writer.Close();
            PlayerPrefs.SetString("OfflineTime", System.DateTime.Now.ToBinary().ToString());
            data.OfflineProgressCheck = true;
        }
        backupCount - (backupCount + 1) % 5;
    }

    public static string ConvertStringToBase64(StreamWriter writer, string s)
    {
        SimpleAES aes = new SimpleAES();
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes();
        string stringTemp = Convert.ToBase64String(plainTextBytes);
        return stringTemp;
    }
    public static bool LoadSaveFile(ref Data data, string path)
    {
        var success = false;
        try
        {
            using (StreamReader reader = new StreamReader(path))
            {
                json = ConvertBase64ToString(reader);
                data = JsonUtility.FromJson<data>(json);
                reader.Close();
                success = true;
            }
        }
        catch(Exception ex)
        {
            Debug.Log("Load save failed");
            CreateFile();
        }
        return success;
    }

    public static void CreateFile()
    {
        if (!File.Exists(Application.persistentDataPath + savePathBackup))
        {
            File.CreateText(Application.persistentDataPath + savePathBackup);
        }
        if (!File.Exists(Application.persistentDataPath + savePath))
        {
            File.CreateText(Application.persistentDataPath + savePath);
        }
    }
    
    public static void LoadPlayer(ref Data data)
    {
        CreateFile();
        if (!LoadSaveFile(ref data, Application.persistentDataPath + savePath))
        {
            LoadSaveFile(ref data, Application.persistentDataPath + savePathBackup);
        }
    }

    public static string ConvertBase64ToString(StreamReader reader)
    {
        SimpleAES aes = new SimpleAES();
        string stringConvert = reader.ReadLine();
        var base64EncodedBytes = Convert.FromBase64String(aes.DecryptString(stringConvert, encryptKey));
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
