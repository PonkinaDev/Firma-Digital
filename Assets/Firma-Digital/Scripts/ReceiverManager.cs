// ReceiverManager.cs
// Sin UI. El servidor TCP arranca automáticamente al darle Play.
// Todo sale por consola de Unity.

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace CryptoLab
{
    public class ReceiverManager : MonoBehaviour
    {
        [Header("Red")]
        public int listenPort = 9000;

        private ICryptoHash _hasher;
        private ICryptoVerifier _verifier;

        private TcpListener _listener;
        private Thread _listenThread;

        private readonly ConcurrentQueue<Action> _mainThreadQueue = new ConcurrentQueue<Action>();

        private void Awake()
        {
            _hasher   = new CustomHashAdapter();
            _verifier = new CustomVerifierAdapter();

            _listener = new TcpListener(IPAddress.Any, listenPort);
            _listener.Start();
            Debug.Log($"[Receiver] Escuchando en puerto {listenPort}...");

            _listenThread = new Thread(ListenLoop) { IsBackground = true };
            _listenThread.Start();
        }

        private void Update()
        {
            while (_mainThreadQueue.TryDequeue(out Action action))
                action?.Invoke();
        }

        private void OnDestroy()
        {
            _listener?.Stop();
        }

        private void ListenLoop()
        {
            while (true)
            {
                try
                {
                    TcpClient client = _listener.AcceptTcpClient();
                    new Thread(() => HandleClient(client)) { IsBackground = true }.Start();
                }
                catch (SocketException) { break; }
            }
        }

        private void HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                byte[] lengthBuffer = new byte[4];
                stream.Read(lengthBuffer, 0, 4);
                int dataLength = BitConverter.ToInt32(lengthBuffer, 0);

                byte[] dataBuffer = new byte[dataLength];
                int totalRead = 0;
                while (totalRead < dataLength)
                    totalRead += stream.Read(dataBuffer, totalRead, dataLength - totalRead);

                string json = Encoding.UTF8.GetString(dataBuffer);
                ProcessPacket(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Receiver] Error: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        private void ProcessPacket(string json)
        {
            try
            {
                SignaturePacket packet = JsonUtility.FromJson<SignaturePacket>(json);
                string recomputedHash  = _hasher.ComputeHash(packet.message);
                byte[] signatureBytes  = packet.GetSignatureBytes();
                bool   isValid         = _verifier.Verify(recomputedHash, signatureBytes, packet.publicKeyXml);

                Debug.Log($"[Receiver] Mensaje : {packet.message}");
                Debug.Log($"[Receiver] Hash    : {recomputedHash}");
                Debug.Log(isValid
                    ? "[Receiver] ✔ FIRMA VÁLIDA"
                    : "[Receiver] ✘ FIRMA INVÁLIDA");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Receiver] Error verificando: {ex.Message}");
            }
        }
    }
}