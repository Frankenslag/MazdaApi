using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WingandPrayer.MazdaApi
{
    internal partial class MazdaApiController
    {
        private readonly MazdaApiConnection _connection;

        public async ValueTask<string>GetVehicleBaseInformation() => await _connection.ApiRequest(HttpMethod.Post, "/remoteServices/getVecBaseInfos/v4", new Dictionary<string, string>() { { "internaluserid", "__INTERNAL_ID__" } }, true, true);

        public MazdaApiController(string emailAddress, string password, string region)
        {
            _connection = new MazdaApiConnection(emailAddress, password, region);
        }
    }
}
