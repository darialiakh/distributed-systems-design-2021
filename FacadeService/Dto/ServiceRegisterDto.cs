using System.Collections.Generic;

namespace FacadeService.Dto
{
    public class ServiceRegisterDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<CheckDto> Checks { get; set; }
    }

    public class CheckDto
    {
        public string Name { get; set; }
        public string Method { get; set; }
        public List<string> Args { get; set; }
        public string Interval { get; set; }
        public string Timeout { get; set; }
        public string Status { get; set; }
    }

}
