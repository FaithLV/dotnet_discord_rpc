using System;
using System.Runtime.InteropServices;

namespace dotnet_discord_rpc
{
    public class DiscordRPC
    {
        public readonly string ClientID;
        public readonly string SteamID;

        private RPCLibrary.RichPresence RPC_RichPresence;
        private RPCLibrary.EventHandlers RPC_EventHandlers;

        public DiscordRPC(string ClientID, string SteamID = null)
        {
            this.ClientID = ClientID;
            this.SteamID = SteamID;

            InitializeHandlers();
            InitializeRPC();
        }

        ~DiscordRPC()
        {
            Shutdown();
        }

        private void InitializeHandlers()
        {
            RPC_EventHandlers = new RPCLibrary.EventHandlers();

            RPC_EventHandlers.readyCallback += ReadyCallback;
            RPC_EventHandlers.disconnectedCallback += DisconnectCallback;
            RPC_EventHandlers.errorCallback += ErrorCallback;
        }

        private void UpdatePresence()
        {
            RPCLibrary.UpdatePresence(ref RPC_RichPresence);
        }

        private void InitializeRPC()
        {
            RPCLibrary.Initialize(ClientID, ref RPC_EventHandlers, true, SteamID);
        }

        public void RunCallbacks()
        {
            RPCLibrary.RunCallbacks();
        }

        public void Shutdown()
        {
            RPCLibrary.Shutdown();
        }

        #region RichPresenceValues

        /// <summary>
        /// The user's current party status
        /// </summary>
        public string State
        {
            get => RPC_RichPresence.state;
            set
            {
                RPC_RichPresence.state = value;
                UpdatePresence();
            }
        }

        /// <summary>
        /// What the player is currently doing
        /// </summary>
        public string Details
        {
            get => RPC_RichPresence.details;
            set
            {
                RPC_RichPresence.details = value;
                UpdatePresence();
            }
        }

        /// <summary>
        /// Epoch seconds for game start - including will show time as "elapsed"
        /// </summary>
        public long startTimestamp
        {
            get => RPC_RichPresence.startTimestamp;
            set
            {
                RPC_RichPresence.startTimestamp = value;
                UpdatePresence();
            }
        }

        /// <summary>
        /// Epoch seconds for game end - including will show time as "remaining"
        /// </summary>
        public long endTimestamp
        {
            get => RPC_RichPresence.endTimestamp;
            set
            {
                RPC_RichPresence.endTimestamp = value;
                UpdatePresence();
            }
        }

        /// <summary>
        /// Name of the uploaded image for the large profile artwork
        /// </summary>
        public string LargeImageKey
        {
            get => RPC_RichPresence.largeImageKey;
            set
            {
                RPC_RichPresence.largeImageKey = value;
                UpdatePresence();
            }
        }

        /// <summary>
        /// Tooltip for the largeImageKey
        /// </summary>
        public string LargeImageText
        {
            get => RPC_RichPresence.largeImageText;
            set
            {
                RPC_RichPresence.largeImageText = value;
                UpdatePresence();
            }
        }

        /// <summary>
        /// Name of the uploaded image for the small profile artwork
        /// </summary>
        public string SmallImageKey
        {
            get => RPC_RichPresence.smallImageKey;
            set
            {
                RPC_RichPresence.smallImageKey = value;
                UpdatePresence();
            }
        }

        /// <summary>
        /// Tootltip for the smallImageKey
        /// </summary>
        public string SmallImageText
        {
            get => RPC_RichPresence.smallImageText;
            set
            {
                RPC_RichPresence.smallImageText = value;
                UpdatePresence();
            }
        }

        /// <summary>
        /// ID of the player's party, lobby, or group
        /// </summary>
        public string PartyID
        {
            get => RPC_RichPresence.partyId;
            set
            {
                RPC_RichPresence.partyId = value;
                UpdatePresence();
            }
        }

        /// <summary>
        /// Current size of the player's party, lobby, or group
        /// </summary>
        public int PartySize
        {
            get => RPC_RichPresence.partySize;
            set
            {
                RPC_RichPresence.partySize = value;
                UpdatePresence();
            }
        }

        /// <summary>
        /// Maximum size of the player's party, lobby, or group
        /// </summary>
        public int PartyMax
        {
            get => RPC_RichPresence.partyMax;
            set
            {
                RPC_RichPresence.partyMax = value;
                UpdatePresence();
            }
        }

        /// <summary>
        /// [deprecated Notify Me feature, may be re-used in future]
        /// </summary>
        [Obsolete("Deprecated")]
        public string MatchSecret
        {
            get => RPC_RichPresence.matchSecret;
            set
            {
                RPC_RichPresence.matchSecret = value;
                UpdatePresence();
            }
        }

        /// <summary>
        /// Unique hashed string for chat invitations and Ask to Join          
        /// </summary>
        public string JoinSecret
        {
            get => RPC_RichPresence.joinSecret;
            set
            {
                RPC_RichPresence.joinSecret = value;
                UpdatePresence();
            }
        }

        /// <summary>
        /// Unique hashed string for Spectate button                        
        /// </summary>
        public string SpectateSecret
        {
            get => RPC_RichPresence.spectateSecret;
            set
            {
                RPC_RichPresence.spectateSecret = value;
                UpdatePresence();
            }
        }

        /// <summary>
        /// [deprecated Notify Me feature, may be re-used in future]
        /// </summary>
        [Obsolete("Deprecated")]
        public bool Instance
        {
            get => RPC_RichPresence.instance;
            set
            {
                RPC_RichPresence.instance = value;
                UpdatePresence();
            }
        }

        #endregion

        #region EventHandlers

        public delegate void DisconnectEventHandler(object sender, int errorCode, string Message);
        public event DisconnectEventHandler Disconnected;
        protected virtual void OnDisconnected(int errorCode, string message)
        {
            Disconnected?.Invoke(this, errorCode, message);
        }

        public delegate void ErrorEventHandler(object sender, int errorCode, string Message);
        public event ErrorEventHandler Error;
        protected virtual void OnError(int errorCode, string message)
        {
            Error?.Invoke(this, errorCode, message);
        }

        public delegate void ReadyEventHandler(object sender);
        public event ReadyEventHandler Ready;

        protected virtual void OnReady()
        {
            Ready?.Invoke(this);
        }

        #endregion

        #region CallbackWrappers

        private void ReadyCallback()
        {
            OnReady();
        }

        private void DisconnectCallback(int errorCode, string message)
        {
            OnDisconnected(errorCode, message);
        }

        private void ErrorCallback(int errorCode, string message)
        {
            OnError(errorCode, message);
        }

        #endregion
    }

    internal class RPCLibrary
    {
        private const string DLL = "discord-rpc-w32.dll";

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ReadyCallback();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DisconnectedCallback(int errorCode, string message);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ErrorCallback(int errorCode, string message);

        public struct EventHandlers
        {
            public ReadyCallback readyCallback;
            public DisconnectedCallback disconnectedCallback;
            public ErrorCallback errorCallback;
        }

        [System.Serializable]
        public struct RichPresence
        {
            public string state; /* max 128 bytes */
            public string details; /* max 128 bytes */
            public long startTimestamp;
            public long endTimestamp;
            public string largeImageKey; /* max 32 bytes */
            public string largeImageText; /* max 128 bytes */
            public string smallImageKey; /* max 32 bytes */
            public string smallImageText; /* max 128 bytes */
            public string partyId; /* max 128 bytes */
            public int partySize;
            public int partyMax;
            public string matchSecret; /* max 128 bytes */
            public string joinSecret; /* max 128 bytes */
            public string spectateSecret; /* max 128 bytes */
            public bool instance;
        }

        [DllImport(DLL, EntryPoint = "Discord_Initialize", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Initialize(string applicationId, ref EventHandlers handlers, bool autoRegister, string optionalSteamId);

        [DllImport(DLL, EntryPoint = "Discord_UpdatePresence", CallingConvention = CallingConvention.Cdecl)]
        public static extern void UpdatePresence(ref RichPresence presence);

        [DllImport(DLL, EntryPoint = "Discord_RunCallbacks", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunCallbacks();

        [DllImport(DLL, EntryPoint = "Discord_Shutdown", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Shutdown();
    }
}
