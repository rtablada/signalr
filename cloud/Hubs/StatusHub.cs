using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace cloud.Hubs
{
    public class StatusHub : Hub
    {
        private IStoreStatusService _storeStatus;

        public StatusHub(IStoreStatusService storeStatus)
        {
            _storeStatus = storeStatus;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var existingStore = _storeStatus.GetStoreStatusByConnectionId(Context.ConnectionId);

            if (existingStore != null) {
                existingStore.LastConnectionTime = DateTime.UtcNow;
                existingStore.StatusState = StatusState.Disconnected;

                Task.Run(() => CheckConnection(existingStore.StoreNumber));
            } else {
                Console.WriteLine("A store disconnected but could not be found");
            }

            await base.OnDisconnectedAsync(exception);
        }

        private void CheckConnection(string storeNumber)
        {
            Console.WriteLine($"Store {storeNumber} disconnected. Waiting 10s for reconnect.");
            Thread.Sleep(10000);

            var existingStore = _storeStatus.GetStoreStatusByStoreNumber(storeNumber);

            if (existingStore == null) {
                Console.WriteLine($"Could not find store {storeNumber}");
                return;
            }

            if (existingStore.StatusState == StatusState.Connected) {
                Console.WriteLine("Store successfully reconnected");
            } else if (existingStore.StatusState == StatusState.Disconnected) {
                Console.WriteLine("Store did not reconnect");
            }
        }

        public async Task StoreConnected(string storeNumber)
        {
            Console.WriteLine($"CONNECTED {storeNumber}");
            _storeStatus.UpdateOrCreateStoreStatus(storeNumber, Context.ConnectionId);
        }
    }
}
