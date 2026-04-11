using System;
using DigitalSignature.Interfaces;
using UnityEngine;

namespace DigitalSignature.Services
{

    public class DigitalSignatureService
    {
        private readonly ICryptoAdapter _adapter;

        public DigitalSignatureService(ICryptoAdapter adapter)
        {
            _adapter = adapter;
        }

        public SignedPacket CreateSignedPacket(string message)
        {
            Debug.Log("\n══════════════════════════════════════════════════");
            Debug.Log($"[SignatureService] Firmando mensaje: \"{message}\"");
            Debug.Log($"[SignatureService] Hasher:     {_adapter.HasherName}");
            Debug.Log($"[SignatureService] Encryptor:  {_adapter.EncryptorName}");

            string signature = _adapter.Sign(message);
            string publicKey = _adapter.PublicKey;

            var packet = new SignedPacket(message, signature, publicKey,
                                          _adapter.HasherName, _adapter.EncryptorName);

            Debug.Log($"[SignatureService] Paquete creado y listo para transmitir.");
            Debug.Log("══════════════════════════════════════════════════\n");

            return packet;
        }

        public bool VerifyPacket(SignedPacket packet)
        {
            Debug.Log("\n══════════════════════════════════════════════════");
            Debug.Log($"[SignatureService] Verificando paquete recibido...");
            Debug.Log($"  Algoritmos reportados por el emisor:");
            Debug.Log($"    Hasher:    {packet.HasherName}");
            Debug.Log($"    Encryptor: {packet.EncryptorName}");

            bool result = _adapter.Verify(packet.Message, packet.Signature, packet.PublicKey);

            Debug.Log("══════════════════════════════════════════════════\n");
            return result;
        }

public void Setup() => _adapter.Initialize();
    }

    [System.Serializable]
    public class SignedPacket
    {
        public string Message      { get; }
        public string Signature    { get; } 
        public string PublicKey    { get; }   
        public string HasherName   { get; }   
        public string EncryptorName{ get; }

        public SignedPacket(string message, string signature, string publicKey,
                            string hasherName, string encryptorName)
        {
            Message       = message;
            Signature     = signature;
            PublicKey     = publicKey;
            HasherName    = hasherName;
            EncryptorName = encryptorName;
        }

        public string ToJson()
        {
            string safeMsg = Message.Replace("\"", "\\\"").Replace("\n", "\\n");
            return $"{{" +
                   $"\"message\":\"{safeMsg}\"," +
                   $"\"signature\":\"{Signature}\"," +
                   $"\"publicKey\":\"{EscapeForJson(PublicKey)}\"," +
                   $"\"hasherName\":\"{HasherName}\"," +
                   $"\"encryptorName\":\"{EncryptorName}\"" +
                   $"}}";
        }

        public static SignedPacket FromJson(string json)
        {
            string msg  = ExtractJsonField(json, "message");
            string sig  = ExtractJsonField(json, "signature");
            string pk   = ExtractJsonField(json, "publicKey");
            string hn   = ExtractJsonField(json, "hasherName");
            string en   = ExtractJsonField(json, "encryptorName");
            return new SignedPacket(msg, sig, pk, hn, en);
        }

        private static string ExtractJsonField(string json, string field)
        {
            string key   = $"\"{field}\":\"";
            int start    = json.IndexOf(key);
            if (start < 0) return string.Empty;
            start += key.Length;
            int end = json.IndexOf("\"", start);
            while (end > 0 && json[end - 1] == '\\') 
                end = json.IndexOf("\"", end + 1);
            return end < 0 ? string.Empty
                           : json.Substring(start, end - start)
                                 .Replace("\\\"", "\"")
                                 .Replace("\\n", "\n");
        }

        private static string EscapeForJson(string s) =>
            s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
    }
}
