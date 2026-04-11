using System.Collections.Generic;
using DigitalSignature.Adapter;
using DigitalSignature.Custom;
using DigitalSignature.Interfaces;
using DigitalSignature.Library;
using DigitalSignature.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DigitalSignature.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Elementos de la escena")]
        [SerializeField] private TMP_InputField _messageInput;
        [SerializeField] private Button         _sendButton;
        [SerializeField] private Toggle         _customAlgoToggle;
        [SerializeField] private TMP_Text       _modeLabel;

        private ICryptoAdapter          _adapter;
        private DigitalSignatureService _signatureService;
        private NetworkService          _networkService;
        private readonly Queue<System.Action> _mainThreadQueue = new Queue<System.Action>();
        private readonly object               _queueLock       = new object();

        // ── Lifecycle ─────────────────────────────────────────────────

        private void Awake()
        {
            BuildAdapter(useCustom: false);

            _networkService = new NetworkService();
            _networkService.OnPacketReceived += OnPacketReceivedFromNetwork;
        }

        private void Start()
        {
            _sendButton       .onClick.AddListener(OnSendClicked);
            _customAlgoToggle .onValueChanged.AddListener(OnToggleChanged);

            UpdateModeLabel();
            Debug.Log("[UIManager] Sistema de Firma Digital listo. Escribe un mensaje y pulsa el botón.");
        }

        private void Update()
        {
            lock (_queueLock)
            {
                while (_mainThreadQueue.Count > 0)
                    _mainThreadQueue.Dequeue()?.Invoke();
            }
        }

        private void OnDestroy() => _networkService?.StopListening();

        private void OnToggleChanged(bool useCustom)
        {
            BuildAdapter(useCustom);
            UpdateModeLabel();
            Debug.Log($"\n[UIManager] Modo → {(useCustom ? "Algoritmos propios" : "Librerías C#")}");
            Debug.Log($"  Hasher:    {_adapter.HasherName}");
            Debug.Log($"  Encryptor: {_adapter.EncryptorName}");
        }

        private void OnSendClicked()
        {
            string message = _messageInput != null ? _messageInput.text.Trim() : "";

            if (string.IsNullOrEmpty(message))
            {
                Debug.LogWarning("[UIManager] El mensaje está vacío.");
                return;
            }
            SignedPacket packet = _signatureService.CreateSignedPacket(message);
            _networkService.SendPacket(packet);
        }

        private void OnPacketReceivedFromNetwork(SignedPacket packet)
        {
            lock (_queueLock)
            {
                _mainThreadQueue.Enqueue(() =>
                {
                    Debug.Log("\n[UIManager] Paquete recibido — verificando...");
                    bool ok = _signatureService.VerifyPacket(packet);
                    Debug.Log(ok
                        ? "[UIManager] ✔ FIRMA VÁLIDA"
                        : "[UIManager] ✘ FIRMA INVÁLIDA");
                });
            }
        }

        private void BuildAdapter(bool useCustom)
        {
            ICryptoHasher    hasher    = useCustom ? (ICryptoHasher)   new CustomCryptoHasher()    : new LibraryCryptoHasher();
            ICryptoEncryptor encryptor = useCustom ? (ICryptoEncryptor) new CustomCryptoEncryptor() : new LibraryCryptoEncryptor();

            _adapter          = new CryptoAdapter(hasher, encryptor);
            _signatureService = new DigitalSignatureService(_adapter);
            _signatureService.Setup();
        }

        private void UpdateModeLabel()
        {
            if (_modeLabel == null) return;
            bool custom = _customAlgoToggle != null && _customAlgoToggle.isOn;
            _modeLabel.text = custom
                ? "Modo: Algoritmos propios\n(FoldHash + KeyDance)"
                : "Modo: Librerías C#\n(SHA-256 + RSA-512)";
        }
    }
}
