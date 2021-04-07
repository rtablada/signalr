using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace cloud.Hubs
{
    public interface IStoreStatusService
    {
        public StoreStatus GetStoreStatusByConnectionId(string connectionId);
        public StoreStatus GetStoreStatusByStoreNumber(string storeNumber);
        public void UpdateOrCreateStoreStatus(string storeNumber, string connectionId);
        void StartChecks();
    }
    public class StoreStatus
    {
        public string StoreNumber { get; set; }
        public DateTime LastConnectionTime { get; set; }
        public string ConnectionId { get; set; }
        public StatusState StatusState { get; set; }
    }
    public class StoreStatusService : IStoreStatusService
    {
        private List<StoreStatus> _stores = new List<StoreStatus>();

        public StoreStatus GetStoreStatusByConnectionId(string connectionId)
        {
            return _stores.FirstOrDefault(s => s.ConnectionId == connectionId);
        }
        public StoreStatus GetStoreStatusByStoreNumber(string storeNumber)
        {
            return _stores.FirstOrDefault(s => s.StoreNumber == storeNumber);
        }

        public void UpdateOrCreateStoreStatus(string storeNumber, string connectionId)
        {
            var existingStore = _stores.FirstOrDefault(s => s.StoreNumber == storeNumber);

            if (existingStore == null) {
                existingStore = new StoreStatus { StoreNumber = storeNumber, ConnectionId = connectionId };
                _stores.Add(existingStore);
            }

            existingStore.ConnectionId = connectionId;
            existingStore.LastConnectionTime = DateTime.UtcNow;
            existingStore.StatusState = StatusState.Connected;
        }

        public void StartChecks()
        {

        }
    }
}
