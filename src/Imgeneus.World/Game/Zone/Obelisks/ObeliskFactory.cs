using Imgeneus.World.Game.Monster;

namespace Imgeneus.World.Game.Zone.Obelisks
{
    public class ObeliskFactory : IObeliskFactory
    {
        private readonly IMobFactory _mobFactory;

        public ObeliskFactory(IMobFactory mobFactory)
        {
            _mobFactory = mobFactory;
        }

        public Obelisk CreateObelisk(ObeliskConfiguration config, Map map)
        {
            return new Obelisk(config, map, _mobFactory);
        }
    }
}
