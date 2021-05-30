using System.Collections.Generic;
using FacadeService.Dto;
using Newtonsoft.Json;

namespace FacadeService
{
    public class ConsulService
    {
        public ConsulService()
        {
            var data = new ServiceRegisterDto
            {
                Id = "facade1",
                Name = "facade service",
                Checks = new List<CheckDto> {new CheckDto
                    {
                        Name = "ping check",
                        Args = new List<string>
                        {
                            "ping", "-c1", "localhost"
                        },
                        Interval = "30s",
                        Status = "passing"
                    }
                }

            };

            var response = WebUtilities.Put($"http://127.0.0.1:8500/v1/agent/service/register", JsonConvert.SerializeObject(data, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
        }
    }
}
