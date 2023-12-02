using System.Text;
using Smarthome.Bulbs.interfaces;
using Smarthome.interfaces;
using Newtonsoft.Json;


namespace Smarthome.Bulbs.Services
{
    public class BulbsService : IBulbsService
    {
        private readonly HttpClient _httpClient = new();
        private readonly Device[] _httpDevices = LoadDevicesFromJson("httpDevices.json");

        private static Device[] LoadDevicesFromJson(string jsonFilePath)
        {
            var json = File.ReadAllText(jsonFilePath); // Assuming you have a JSON file
            var deviceDataList = JsonConvert.DeserializeObject<List<Device>>(json);
            if (deviceDataList == null)
            {
                return Array.Empty<Device>();
            }

            return deviceDataList.Select(device => new Device
            {
                DeviceId = device.DeviceId,
                DeviceIp = device.DeviceIp,
                DeviceName = device.DeviceName,
                DevicePort = device.DevicePort,
                DeviceType = device.DeviceType,
                Endpoints = new Endpoints
                {
                    Dimmable = device.Endpoints.Dimmable,
                    Info = device.Endpoints.Info,
                    SwitchBulb = device.Endpoints.SwitchBulb
                },
                RoomId = device.RoomId
            }).ToArray();
        }


        private static string ConstructEndpoint(string deviceIp, int devicePort, string endpoint) =>
            $"http://{deviceIp}:{devicePort}{endpoint}";

        private Device GetBulb(string bulbId)
        {
            var bulb = _httpDevices.FirstOrDefault(device =>
                device.DeviceType == DeviceType.Light && device.DeviceId == bulbId);
            if (bulb == null)
            {
                throw new Exception($"No bulb found with id {bulbId}");
            }

            return bulb;
        }

        private async Task<IBulbResponse> GetBulbInfoAsync(HttpRequestMessage request, string deviceName)
        {
            try
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(1);
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<IBulbResponse>(responseContent);
                if (responseData != null)
                {
                    responseData.Data.DeviceName = deviceName;
                }

                return responseData ?? throw new Exception("No response data");
            }
            catch (Exception ex)
            {
                return new IBulbResponse
                {
                    Error = 1,
                    Data = new IBulbInfoData
                    {
                        DeviceName = deviceName,
                        error = true
                    }
                };
            }
        }

        private List<Device> GetBulbs(int? roomId)
        {
            var bulbs = _httpDevices.Where(device => device.DeviceType == DeviceType.Light && device.RoomId == roomId)
                .ToList();
            if (!bulbs.Any())
            {
                throw new Exception("No bulbs found");
            }

            return bulbs;
        }

        public async Task<List<IBulbInfoData>> GetBulbsInfo(int? roomId)
        {
            var bulbs = GetBulbs(roomId);
            var tasks = bulbs.Select(bulb =>
                GetBulbInfoAsync(new HttpRequestMessage(HttpMethod.Post,
                    ConstructEndpoint(bulb.DeviceIp, bulb.DevicePort, bulb.Endpoints.Info))
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new { deviceid = bulb.DeviceId, data = new { } }), Encoding.UTF8,
                        "application/json")
                }, bulb.DeviceName));
            var result = await Task.WhenAll(tasks);

            return result.ToList().Select(bulb => bulb.Data).ToList();
        }

        public async Task<SwitchResponse> SwitchBulb(string bulbId, string switchState)
        {
            var bulb = GetBulb(bulbId);
            try
            {
                using (var client = new HttpClient())
                {
                    var requestContent = new StringContent(
                        JsonConvert.SerializeObject(new { deviceid = bulbId, data = new { @switch = switchState } }),
                        Encoding.UTF8,
                        "application/json");

                    var response = await client.PostAsync(
                        ConstructEndpoint(bulb.DeviceIp, bulb.DevicePort, bulb.Endpoints.SwitchBulb), requestContent);

                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<ResponseFromBulb>(responseContent);


                    return new SwitchResponse
                    {
                        Seq = responseData.Seq,
                        Error = responseData.Error,
                        @switch = switchState
                    };
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error making API call for {bulb.DeviceName}: {ex}");
                throw new Exception($"Error making API call for {bulb.DeviceName}: {ex}");
            }
        }

        public async Task<ResponseFromBulb> DimBulb(string bulbId, IBulbRequest dimBulbDto)
        {
            var bulb = GetBulb(bulbId);
            try
            {
                using (var client = new HttpClient())
                {
                    var dto = new
                    {
                        deviceid = bulbId,
                        data = new Dictionary<string, object>
                        {
                            { "ltype", dimBulbDto.ltype },
                            {
                                dimBulbDto.ltype, new
                                {
                                    br = dimBulbDto.colorTypes.Br,
                                    ct = dimBulbDto.colorTypes.Ct,
                                    R = dimBulbDto.colorTypes.R,
                                    G = dimBulbDto.colorTypes.G,
                                    B = dimBulbDto.colorTypes.B
                                }
                            }
                        }
                    };

                    var requestContent = new StringContent(
                        JsonConvert.SerializeObject(dto),
                        Encoding.UTF8,
                        "application/json");
                    var endpoint = ConstructEndpoint(bulb.DeviceIp, bulb.DevicePort, bulb.Endpoints.Dimmable);

                    var response = await client.PostAsync(
                        endpoint, requestContent);

                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<ResponseFromBulb>(responseContent);


                    return responseData;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error making API call for {bulb.DeviceName}: {ex}");
                throw new Exception($"Error making API call for {bulb.DeviceName}: {ex}");
            }
        }
    }
}