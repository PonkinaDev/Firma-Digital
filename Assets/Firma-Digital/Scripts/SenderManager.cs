// SenderManager.cs
// UI mínima: un TMP_InputField para el mensaje y un Button para firmar y enviar.
// Todo el resultado se imprime en la consola de Unity (Debug.Log).

using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CryptoLab
{
    public class SenderManager : MonoBehaviour
    {
        [Header("Red")]
        public string receiverIP = "127.0.0.1";
        public int receiverPort = 9000;

        [Header("UI")]
        public TMP_InputField messageInputField;
        public Button sendButton;

        private ICryptoHash _hasher;
        private ICryptoSigner _signer;

        private void Awake()
        {
            // ── Punto 2.2: Intercambio de adaptadores (Principio O + L) ──────────────
            // Para volver a las librerías del Punto 2.1, basta con restaurar estas dos líneas:
            //   _hasher = new SHA256HashAdapter();
            //   _signer = new RSASignerAdapter();
            _hasher = new CustomHashAdapter();
            _signer = new CustomSignerAdapter(new CustomKeyGenerator());
            sendButton.onClick.AddListener(OnSendClicked);
        }

        private void OnSendClicked()
        {
            string message = messageInputField.text.Trim();

            if (string.IsNullOrEmpty(message))
            {
                Debug.LogWarning("[Sender] El mensaje está vacío.");
                return;
            }

            try
            {
                // 1. Hash
                string hash = _hasher.ComputeHash(message);
                Debug.Log($"[Sender] Mensaje   : {message}");
                Debug.Log($"[Sender] Hash      : {hash}");

                // 2. Firma
                byte[] signature = _signer.Sign(hash);
                string publicKeyXml = _signer.GetPublicKeyXml();
                Debug.Log($"[Sender] Firma     : {Convert.ToBase64String(signature)}");
                Debug.Log($"[Sender] Llave pub : {publicKeyXml}");

                // 3. Empaquetado
                SignaturePacket packet = new SignaturePacket
                {
                    message = message,
                    publicKeyXml = publicKeyXml
                };
                packet.SetSignatureBytes(signature);

                // 4. Envío TCP
                string json = JsonUtility.ToJson(packet);
                SendViaTCP(json);
                Debug.Log($"[Sender] Paquete enviado a {receiverIP}:{receiverPort}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Sender] Error: {ex.Message}");
            }
        }

        private void SendViaTCP(string json)
        {
            using (TcpClient client = new TcpClient())
            {
                client.Connect(receiverIP, receiverPort);
                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(json);
                byte[] lengthPrefix = BitConverter.GetBytes(data.Length);
                stream.Write(lengthPrefix, 0, 4);
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }
        }
    }
}