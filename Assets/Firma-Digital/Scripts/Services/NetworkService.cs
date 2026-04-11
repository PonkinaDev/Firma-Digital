using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace DigitalSignature.Services
{
    public class NetworkService
    {
        private const int    PORT        = 7777;
        private const string HOST        = "127.0.0.1";
        private const int    BUFFER_SIZE = 65536;

        private TcpListener _listener;
        private Thread      _listenThread;
        private bool        _isListening;
        public event Action<SignedPacket> OnPacketReceived;

        public void EnsureListening()
        {
            if (_isListening) return;

            _listener     = new TcpListener(IPAddress.Parse(HOST), PORT);
            _isListening  = true;
            _listenThread = new Thread(ListenLoop) { IsBackground = true };
            _listenThread.Start();

            Debug.Log($"[NetworkService] Receptor TCP iniciado en {HOST}:{PORT}");
        }

        public void StopListening()
        {
            _isListening = false;
            _listener?.Stop();
        }

        private void ListenLoop()
        {
            try
            {
                _listener.Start();
                while (_isListening)
                {
                    TcpClient client = _listener.AcceptTcpClient();
                    new Thread(() => HandleClient(client)) { IsBackground = true }.Start();
                }
            }
            catch (SocketException) when (!_isListening) { /* cierre normal */ }
            catch (Exception ex) { Debug.LogError($"[NetworkService] ListenLoop: {ex.Message}"); }
        }

        private void HandleClient(TcpClient client)
        {
            try
            {
                using (client)
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer    = new byte[BUFFER_SIZE];
                    int    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string json      = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    Debug.Log($"[NetworkService] Paquete recibido ({bytesRead} bytes).");

                    SignedPacket packet = SignedPacket.FromJson(json);
                    OnPacketReceived?.Invoke(packet);
                }
            }
            catch (Exception ex) { Debug.LogError($"[NetworkService] HandleClient: {ex.Message}"); }
        }

        public void SendPacket(SignedPacket packet)
        {
            EnsureListening();

            Thread.Sleep(50);

            string json = packet.ToJson();
            Debug.Log($"[NetworkService] Enviando paquete a {HOST}:{PORT}...");

            new Thread(() =>
            {
                try
                {
                    using (TcpClient client = new TcpClient(HOST, PORT))
                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] data = Encoding.UTF8.GetBytes(json);
                        stream.Write(data, 0, data.Length);
                        Debug.Log("[NetworkService] Paquete enviado correctamente.");
                    }
                }
                catch (Exception ex) { Debug.LogError($"[NetworkService] SendPacket: {ex.Message}"); }
            })
            { IsBackground = true }.Start();
        }
    }
}
