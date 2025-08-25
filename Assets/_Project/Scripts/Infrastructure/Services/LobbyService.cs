using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Assets._Project.Scripts
{
    public class LobbyService
    {

        private Lobby _lobby;
        private string _relayCode;

        public LobbyService()
        {
            Init();
        }
        public async void Init()
        {
            await UnityServices.InitializeAsync();
            await SignUpWithUsernamePasswordAsync();
            _lobby = null;
        }
        async Task SignUpWithUsernamePasswordAsync()
        {
            try
            {
                var randomNum = UnityEngine.Random.Range(1000, 9999).ToString();
                var username = "Player_" + randomNum;
                var password = "Password" + randomNum + "!";
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                Debug.Log("SignUp is successful.");
            }
            catch (AuthenticationException ex)
            {
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
            }
        }
        public async Task CreateLobby(int maxPlayers)
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
                _relayCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                CreateAndStartHost(allocation);

                var options = new CreateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                {
                    { "JoinCode", new DataObject(DataObject.VisibilityOptions.Member, _relayCode) }
                }
                };

                _lobby = await Lobbies.Instance.CreateLobbyAsync("TicTacToe_Public", maxPlayers, options);

                Debug.Log("Lobby created: " + _lobby.Id);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        public async Task QuickJoinLobby()
        {
            try
            {
                Debug.Log("=== Quick Join Started ===");

                // Если мы уже в лобби — не ищем
                if (_lobby != null)
                {
                    Debug.Log("Already in lobby: " + _lobby.Id);
                    return;
                }

                // Проверяем, в каких лобби мы уже состоим
                var joinedLobbies = await Lobbies.Instance.GetJoinedLobbiesAsync();
                Debug.Log("Already in lobbies: " + joinedLobbies.Count);

                // Поиск лобби
                var queryResponse = await Lobbies.Instance.QueryLobbiesAsync(new QueryLobbiesOptions
                {
                    Filters = new List<QueryFilter>
            {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            }
                });

                Debug.Log("Found lobbies: " + queryResponse.Results.Count);

                // Сначала ищем подходящее лобби, потом подключаемся
                Unity.Services.Lobbies.Models.Lobby lobbyToJoin = null;

                // Ищем лобби, в котором мы НЕ состоим
                foreach (var lobby in queryResponse.Results)
                {
                    Debug.Log($"Lobby: {lobby.Id} | Name: '{lobby.Name}' | Players: {lobby.Players.Count}/{lobby.MaxPlayers}");

                    // Проверяем, что мы НЕ в этом лобби
                    if (!joinedLobbies.Contains(lobby.Id))
                    {
                        lobbyToJoin = lobby;
                        Debug.Log("Found lobby to join: " + lobby.Id); // ? Добавим это
                        break; // Нашли подходящее — выходим из цикла
                    }
                }
                Debug.Log("Already in lobbies: " + joinedLobbies.Count);
                foreach (var id in joinedLobbies)
                {
                    Debug.Log("Already joined lobby ID: " + id);
                }
                // Только после цикла — подключаемся или создаём
                Debug.Log("lobbyToJoin is null: " + (lobbyToJoin == null)); // ? Добавим это

                if (lobbyToJoin != null)
                {
                    Debug.Log("Joining lobby: " + lobbyToJoin.Id);
                    await JoinLobby(lobbyToJoin.Id);
                }
                else
                {
                    Debug.Log("No available lobbies found. Creating new one...");
                    await CreateLobby(2);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Quick join error: " + e.Message);
            }
        }

        public async Task JoinLobby(string lobbyId)
        {
            try
            {
                _lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId);

                string joinCode = _lobby.Data["JoinCode"].Value;
                JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                StartClient(allocation);

            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void CreateAndStartHost(Allocation allocation)
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

            NetworkManager.Singleton.StartHost();
        }

        private void StartClient(JoinAllocation allocation)
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);

            NetworkManager.Singleton.StartClient();
        }
        [ContextMenu("lobby info")]
        private async void UpdateLobbyInfo()
        {
            if (_lobby != null)
            {
                try
                {
                    var updatedLobby = await Lobbies.Instance.GetLobbyAsync(_lobby.Id);
                    Debug.Log($"Players in lobby: {updatedLobby.Players.Count}/{updatedLobby.MaxPlayers}");
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to update lobby info: " + e.Message);
                }
            }
        }
    }
}