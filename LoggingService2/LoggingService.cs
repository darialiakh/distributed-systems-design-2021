using Hazelcast;
using LoggingService.Dto;
using System;
using System.Threading.Tasks;

namespace LoggingService
{
    public class LoggingService
    {
        private IHazelcastClient _hzClient;

        public async Task InitializeAsync()
        {
            _hzClient = await HazelcastClientFactory.StartNewClientAsync(
                new HazelcastOptions { Networking = { Addresses = { "localhost:6002" } } });
        }

        public async Task<string> GetMessagesAsync()
        {
            var data = "";

            var map = await _hzClient.GetMapAsync<Guid, string>("messages");
            var messages = await map.GetEntriesAsync();

            foreach (var msg in messages.Values)
            {
                data += msg + " ";
            }

            return data;
        }

        public async Task<string> PutMessageAsync(MessageDto model)
        {
            var map = await _hzClient.GetMapAsync<Guid, string>("messages");

            await map.PutAsync(model.Id, model.Message);

            return "OK";
        }
    }
}