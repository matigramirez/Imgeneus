using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Zone;
using Imgeneus.World.Game.Zone.MapConfig;

namespace Imgeneus.World.Serialization
{
    public class MapWeather : BaseSerializable
    {
        [FieldOrder(0)]
        public bool SetType = true; // ?

        [FieldOrder(1)]
        public WeatherState WeatherState;

        [FieldOrder(2)]
        public WeatherPower WeatherPower;

        public MapWeather(Map map)
        {
            WeatherState = map.WeatherState;
            WeatherPower = map.WeatherPower;
        }
    }
}
