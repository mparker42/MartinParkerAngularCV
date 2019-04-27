using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinParkerAngularCV.SharedUtils.Models.Configuration
{
    public class BlobStoreConfiguration
    {
        public string InternalStoreSecretURL { get; set; }
        public string PublicStoreSecretURL { get; set; }
    }
}
